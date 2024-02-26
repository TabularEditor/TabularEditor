using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor
{
    public partial class DependencyForm : Form
    {
        const int MAX_LEVELS = 50;

        public DependencyForm()
        {
            InitializeComponent();

            treeObjects.ImageList = FormMain.Singleton.tabularTreeImages;
        }

        private int currentDepth = 0;
        private IDaxObject _rootObject;
        public IDaxObject RootObject { get
            {
                return _rootObject;
            }
            set {
                _rootObject = value;
                //if (_rootObject is PartitionViewTable) _rootObject = (_rootObject as PartitionViewTable).Table;
                Text = "Object Dependencies for " + RootObject.DaxObjectFullName;
                radioButton1.Text = string.Format("Show objects on which {0} depend", RootObject.DaxObjectName);
                radioButton2.Text = string.Format("Show objects that depend on {0}", RootObject.DaxObjectName);
                radioButton3.Text = string.Format("Show relationships starting from {0}", RootObject.DaxObjectName);
                if (RootObject is Table)
                {
                    radioButton3.Enabled = true;
                    radioButton3.Checked = true;
                }
                else
                {
                    radioButton3.Enabled = false;
                    if (radioButton3.Checked) radioButton2.Checked = true;
                }
                RefreshTree();
            }
        }

        HashSet<ITabularObject> VisitedRelationships;

        private struct ObjectRel
        {
            public TabularNamedObject Object;
            public SingleColumnRelationship Relationship;
        }

        private void CreateRelationshipTree(ITabularNamedObject startFrom, TreeNode node)
        {
            var q = new Queue<TreeNode>();
            q.Enqueue(node);

            if(startFrom is Table)
            {
                while (q.Count > 0)
                {
                    node = q.Dequeue();
                    var obj = ObjFromNode(node);

                    var relevantRelationships = obj.Model.Relationships.Where(r => (r.FromTable == obj || r.ToTable == obj)
                        && (chkShowInactive.Checked || r.IsActive)).OrderBy(r => !r.IsActive).ToList();

                    foreach (var r in relevantRelationships)
                    {
                        var dstTable = r.FromTable == obj ? r.ToTable : r.FromTable;

                        if (VisitedRelationships.Contains(r)) continue;
                        VisitedRelationships.Add(r);

                        var prefix = string.Format("({0} {2} {1}) ",
                            (r.FromTable == obj ? r.FromCardinality : r.ToCardinality) == RelationshipEndCardinality.Many ? 'n' : '1',
                            (r.FromTable == obj ? r.ToCardinality : r.FromCardinality) == RelationshipEndCardinality.Many ? 'n' : '1',
                            r.CrossFilteringBehavior == CrossFilteringBehavior.BothDirections ? "\u2194" :
                                r.FromTable == obj ? "\u2190" : "\u2192");

                        var img = UI.Tree.TabularIcon.GetIconIndex(dstTable);
                        var n = new TreeNode(prefix + dstTable.DaxObjectFullName, img, img) { Tag = new ObjectRel { Object = dstTable, Relationship = r } };
                        if (!r.IsActive) n.ForeColor = Color.Silver;
                        n.ToolTipText = r.Name;
                        node.Nodes.Add(n);
                        q.Enqueue(n);
                    }
                }
            }
        }

        private void RecursiveAdd(ITabularNamedObject obj, TreeNodeCollection nodes, string toolTip = null)
        {
            var img = UI.Tree.TabularIcon.GetIconIndex(obj);
            var n = new TreeNode((obj as IDaxObject)?.DaxObjectFullName ?? obj.Name, img, img) { Tag = obj };

            n.ToolTipText = toolTip ?? (obj as IExpressionObject)?.Expression;

            nodes.Add(n);

            if(obj is IDaxDependantObject)
            {
                foreach(var d in ((IDaxDependantObject)obj).DependsOn.OrderBy(k => k.Key.ObjectType))
                {
                    if (d.Value.All(l => l.property == DAXProperty.DefaultDetailRowsExpression || l.property == DAXProperty.DetailRowsExpression)) continue;
                    currentDepth++;

                    if (d.Key == _rootObject)
                    {
                        var i = UI.Tree.TabularIcon.GetIconIndex(d.Key);
                        var node = new TreeNode(d.Key.Name + " (circular dependency)", i, i);
                        n.Nodes.Add(node);
                    }
                    else if (currentDepth < MAX_LEVELS)
                    {
                        var daxProps = d.Value.Select(v => v.property).Distinct()
                            .Select(p => p.ToString() + ": " + ((IDaxDependantObject)obj).GetDAX(p).Replace("\r\n", " ").Replace("\n", " ").Left(100)).ToArray();

                        RecursiveAdd(d.Key, n.Nodes, string.Join("\n", daxProps));
                    }
                    else n.Nodes.Add("(Infinite recursion)");
                    currentDepth--;
                }
            }
            else
            {
                if (obj is ITabularTableObject) RecursiveAdd(((ITabularTableObject)obj).Table, n.Nodes);
            }

            // Include the column used as a Sort-by column for the current column:
            if (obj is Column c && c.SortByColumn != null) RecursiveAdd(c.SortByColumn, n.Nodes);
        }
        private void InverseRecursiveAdd(IDaxObject obj, TreeNodeCollection nodes)
        {
            var img = UI.Tree.TabularIcon.GetIconIndex(obj);
            var n = new TreeNode((obj as IDaxObject)?.DaxObjectFullName ?? obj.Name, img, img) { Tag = obj };
            if (obj is IExpressionObject) n.ToolTipText = (obj as IExpressionObject).Expression;

            nodes.Add(n);

            foreach(var d in obj.ReferencedBy.OrderBy(o => o.ObjectType))
            {
                if(d.DependsOn.TryGetValue(obj, out var objectReferences))
                {
                    var i = UI.Tree.TabularIcon.GetIconIndex(d);

                    currentDepth++;
                    if (d == _rootObject)
                    {
                        if(objectReferences.Any(r => r.property == DAXProperty.DetailRowsExpression || r.property == DAXProperty.DefaultDetailRowsExpression))
                            n.Nodes.Add(new TreeNode(d.GetName() + " (detail rows)", i, i));
                        else
                            n.Nodes.Add(new TreeNode(d.GetName() + " (circular dependency)", i, i));
                    }
                    else if (currentDepth < MAX_LEVELS && d is IDaxObject) InverseRecursiveAdd(d as IDaxObject, n.Nodes);
                    else if (currentDepth < MAX_LEVELS && d is TablePermission tp)
                    {
                        var newNode = new TreeNode(tp.Role.Name + " RLS on " + tp.Table.DaxObjectFullName, i, i);
                        newNode.Tag = tp;
                        n.Nodes.Add(newNode);
                    }
                    else if (currentDepth < MAX_LEVELS && d is CalculationItem ci)
                    {
                        var newNode = new TreeNode(d.GetName(), i, i);
                        newNode.Tag = ci;
                        n.Nodes.Add(newNode);
                    }
                    else if (currentDepth < MAX_LEVELS && d is Partition p)
                    {
                        var newNode = new TreeNode(d.GetName(), i, i);
                        newNode.Tag = p;
                        n.Nodes.Add(newNode);
                    }
                    else n.Nodes.Add("(Infinite recursion)");
                    currentDepth--;
                }
            }
            if(obj is Column c)
            {
                // Include columns that use the current object as a Sort-by column:
                foreach(var col in c.UsedInSortBy)
                {
                    InverseRecursiveAdd(col, n.Nodes);
                }
            }
        }
        private void DependencyChange(object sender, EventArgs e)
        {
            RefreshTree();
        }

        public int DependencyMode
        {
            get => radioButton1.Checked ? 1 : radioButton2.Checked ? 2 : 3;
            set {
                if (value == 1)
                    radioButton1.Checked = true;
                else if (value == 2)
                    radioButton2.Checked = true;
                else if (value == 3)
                    radioButton3.Checked = true;
            }
        }

        private readonly HashSet<object> visitedObjects = new HashSet<object>();

        public void RefreshTree()
        {
            if (RootObject.IsRemoved)
            {
                Close();
                _rootObject = null;
                return;
            }
            treeObjects.Nodes.Clear();

            currentDepth = 0;
            visitedObjects.Clear();
            if (radioButton1.Checked)
            {
                RecursiveAdd(RootObject, treeObjects.Nodes);
                treeObjects.Nodes[0].Expand();
                chkShowInactive.Visible = false;
            }
            else if (radioButton2.Checked)
            {
                InverseRecursiveAdd(RootObject, treeObjects.Nodes);
                treeObjects.Nodes[0].Expand();
                chkShowInactive.Visible = false;
            }
            else
            {
                VisitedRelationships = new HashSet<ITabularObject>();

                var img = UI.Tree.TabularIcon.GetIconIndex(_rootObject);
                var n = new TreeNode(((_rootObject as IDaxObject)?.DaxObjectFullName ?? _rootObject.Name), img, img) { Tag = new ObjectRel { Object = _rootObject as TabularNamedObject, Relationship = null } };
                treeObjects.Nodes.Add(n);
                CreateRelationshipTree(RootObject, n);
                n.Expand();

                chkShowInactive.Visible = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private TabularNamedObject ObjFromNode(TreeNode node)
        {
            var res = node.Tag as TabularNamedObject;
            if (res != null) return res;

            if (node.Tag is ObjectRel)
                return ((ObjectRel)node.Tag).Object;

            return null;
        }

        private void treeObjects_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            UI.UIController.Current.Goto(ObjFromNode(e.Node));
        }

        private void treeObjects_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Clicks > 1)
            {
                treeObjects_DoubleClick = true;
            } else
            {
                var clickedNode = treeObjects.GetNodeAt(e.X, e.Y);
                if (clickedNode != null)
                    treeObjects.SelectedNode = clickedNode;
                treeObjects_DoubleClick = false;
            }
        }

        private bool treeObjects_DoubleClick = false;

        private void treeObjects_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (treeObjects_DoubleClick && e.Action == TreeViewAction.Expand) e.Cancel = true;
        }

        private void treeObjects_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (treeObjects_DoubleClick && e.Action == TreeViewAction.Collapse) e.Cancel = true;
        }

        private void treeObjects_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13 && treeObjects.SelectedNode != null)
            {
                UI.UIController.Current.Goto(ObjFromNode(treeObjects.SelectedNode));
                e.Handled = true;
            }
        }

        private void chkShowInactive_CheckedChanged(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeObjects.SelectedNode == null) e.Cancel = true;
        }

        private void goToObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeObjects.SelectedNode == null) return;
            UI.UIController.Current.Goto(ObjFromNode(treeObjects.SelectedNode));
        }

        private void copyTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeObjects.SelectedNode;
            if (node == null) return;
        
            var sb = new StringBuilder();
            WriteTree(node, sb, 0);

            Clipboard.SetText(sb.ToString());
        }

        private void WriteTree(TreeNode root, StringBuilder sb, int indentLevel)
        {
            sb.Append(new string(' ', indentLevel * 4));
            sb.AppendLine(root.Text);

            foreach (TreeNode node in root.Nodes) WriteTree(node, sb, indentLevel + 1);
        }

        private void copyTreeAsJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeObjects.SelectedNode;
            if (node == null) return;

            using (var tw = new System.IO.StringWriter())
            using (var jw = new JsonTextWriter(tw))
            {
                jw.Formatting = Formatting.Indented;

                WriteTreeJson(node, jw, true);

                jw.Flush();

                Clipboard.SetText(tw.ToString());
            }
        }

        private void WriteTreeJson(TreeNode root, JsonTextWriter jw, bool isRoot)
        {
            var tObj = ObjFromNode(root);

            jw.WriteStartObject();
            jw.WritePropertyName("ObjectType");
            jw.WriteValue(tObj.GetTypeName());
            jw.WritePropertyName("ObjectName");
            jw.WriteValue(tObj.GetName());

            if(root.Tag is ObjectRel && !isRoot)
            {
                var rel = ((ObjectRel)root.Tag).Relationship;
                if (rel != null) {
                    jw.WritePropertyName("ByRelationship");
                    jw.WriteStartObject();

                        jw.WritePropertyName("DisplayName");
                        jw.WriteValue(rel.Name);

                        jw.WritePropertyName("IsActive");
                        jw.WriteValue(rel.IsActive);

                        jw.WritePropertyName("FromColumn");
                        jw.WriteValue(rel.FromColumn.DaxObjectFullName);

                        jw.WritePropertyName("FromCardinality");
                        jw.WriteValue(rel.FromCardinality.ToString());

                        jw.WritePropertyName("ToColumn");
                        jw.WriteValue(rel.ToColumn.DaxObjectFullName);

                        jw.WritePropertyName("ToCardinality");
                        jw.WriteValue(rel.ToCardinality.ToString());

                        jw.WritePropertyName("CrossFilteringBehavior");
                        jw.WriteValue(rel.CrossFilteringBehavior.ToString());

                    jw.WriteEndObject();
                }
            }

            if (radioButton1.Checked) jw.WritePropertyName("DependsOn");
            else if (radioButton2.Checked) jw.WritePropertyName("UsedBy");
            else if (radioButton3.Checked) jw.WritePropertyName("RelatedTo");

            jw.WriteStartArray();
            foreach (TreeNode node in root.Nodes) WriteTreeJson(node, jw, false);
            jw.WriteEndArray();

            jw.WriteEndObject();
        }
    }
}

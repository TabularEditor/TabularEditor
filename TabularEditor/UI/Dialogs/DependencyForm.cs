using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    public partial class DependencyForm : Form
    {
        const int MAX_LEVELS = 50;

        public DependencyForm()
        {
            InitializeComponent();
        }

        private int currentDepth = 0;
        private IDaxObject _rootObject;
        public IDaxObject RootObject { get
            {
                return _rootObject;
            }
            set {
                _rootObject = value;
                if (_rootObject is PartitionViewTable) _rootObject = (_rootObject as PartitionViewTable).Table;
                Text = "Object Dependencies for " + RootObject.DaxObjectFullName;
                radioButton1.Text = string.Format("Show objects on which {0} depend", RootObject.DaxObjectName);
                radioButton2.Text = string.Format("Show objects that depend on {0}", RootObject.DaxObjectName);
                radioButton3.Text = string.Format("Show relationships starting from {0}", RootObject.DaxObjectName);
                if (RootObject is Table)
                {
                    radioButton3.Enabled = true;
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

        private void CreateRelationshipTree(ITabularNamedObject startFrom, TreeNode node)
        {
            var q = new Queue<TreeNode>();
            q.Enqueue(node);

            if(startFrom is Table)
            {
                while (q.Count > 0)
                {
                    node = q.Dequeue();
                    var obj = node.Tag as ITabularNamedObject;

                    var relevantRelationships = obj.Model.Relationships.Where(r => (r.FromTable == obj || r.ToTable == obj)).OrderBy(r => !r.IsActive).ToList();

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
                        var n = new TreeNode(prefix + dstTable.DaxObjectFullName, img, img) { Tag = dstTable };
                        if (!r.IsActive) n.ForeColor = Color.Silver;
                        n.ToolTipText = r.Name;
                        node.Nodes.Add(n);
                        q.Enqueue(n);
                    }
                }
            }
        }

        private void RecursiveAdd(ITabularNamedObject obj, TreeNodeCollection nodes)
        {
            var img = UI.Tree.TabularIcon.GetIconIndex(obj);
            var n = new TreeNode((obj as IDaxObject)?.DaxObjectFullName ?? obj.Name, img, img) { Tag = obj };
            if(obj is IDAXExpressionObject) n.ToolTipText = (obj as IDAXExpressionObject).Expression;

            nodes.Add(n);

            if(obj is IDAXExpressionObject)
            {
                foreach(var d in ((IDAXExpressionObject)obj).Dependencies.OrderBy(k => k.Key.ObjectType))
                {
                    currentDepth++;

                    if (d.Key == _rootObject)
                    {
                        var i = UI.Tree.TabularIcon.GetIconIndex(d.Key);
                        var node = new TreeNode(d.Key.Name + " (circular dependency)", i, i);
                        n.Nodes.Add(node);
                    }
                    else if (currentDepth < MAX_LEVELS) RecursiveAdd(d.Key, n.Nodes);
                    else n.Nodes.Add("(Infinite recursion)");
                    currentDepth--;
                }
            } else
            {
                if (obj is ITabularTableObject) RecursiveAdd(((ITabularTableObject)obj).Table, n.Nodes);
            }
        }
        private void InverseRecursiveAdd(IDaxObject obj, TreeNodeCollection nodes)
        {
            var img = UI.Tree.TabularIcon.GetIconIndex(obj);
            var n = new TreeNode((obj as IDaxObject)?.DaxObjectFullName ?? obj.Name, img, img) { Tag = obj };
            if (obj is IDAXExpressionObject) n.ToolTipText = (obj as IDAXExpressionObject).Expression;

            nodes.Add(n);

            foreach(var d in obj.Dependants.OrderBy(o => o.ObjectType))
            //foreach(var d in obj.Model.Tables.OfType<IExpressionObject>().Concat(obj.Model.Tables.SelectMany(t => t.GetChildren().OfType<IExpressionObject>())))
            {
                if(d.Dependencies.ContainsKey(obj))
                {
                    currentDepth++;
                    if (d == _rootObject)
                    {
                        var i = UI.Tree.TabularIcon.GetIconIndex(d);
                        n.Nodes.Add(new TreeNode(d.Name + " (circular dependency)", i, i));
                    }
                    else if (currentDepth < MAX_LEVELS) InverseRecursiveAdd(d, n.Nodes);
                    else n.Nodes.Add("(Infinite recursion)");
                    currentDepth--;
                }
            }
        }
        private void DependencyChange(object sender, EventArgs e)
        {
            RefreshTree();
        }

        public void RefreshTree()
        {
            treeObjects.Nodes.Clear();

            currentDepth = 0;
            if (radioButton1.Checked)
            {
                RecursiveAdd(RootObject, treeObjects.Nodes);
                treeObjects.Nodes[0].Expand();
            }
            else if (radioButton2.Checked)
            {
                InverseRecursiveAdd(RootObject, treeObjects.Nodes);
                treeObjects.Nodes[0].Expand();
            }
            else
            {
                VisitedRelationships = new HashSet<ITabularObject>();

                var img = UI.Tree.TabularIcon.GetIconIndex(_rootObject);
                var n = new TreeNode(((_rootObject as IDaxObject)?.DaxObjectFullName ?? _rootObject.Name), img, img) { Tag = _rootObject };
                treeObjects.Nodes.Add(n);
                CreateRelationshipTree(RootObject, n);
                n.Expand();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeObjects_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            UI.UIController.Current.Goto(e.Node.Tag as TabularNamedObject);
        }

        private void treeObjects_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Clicks > 1)
            {
                treeObjects_DoubleClick = true;
            } else
            {
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
                UI.UIController.Current.Goto(treeObjects.SelectedNode.Tag as TabularNamedObject);
                e.Handled = true;
            }
        }
    }
}

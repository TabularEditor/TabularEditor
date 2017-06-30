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
                Text = "Object Dependencies for " + RootObject.DaxObjectFullName;
                radioButton1.Text = string.Format("Show objects on which {0} depend", RootObject.DaxObjectName);
                radioButton2.Text = string.Format("Show objects that depend on {0}", RootObject.DaxObjectName);
                RefreshTree();
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

            if (radioButton1.Checked)
            {
                RecursiveAdd(RootObject, treeObjects.Nodes);
            } else
            {
                InverseRecursiveAdd(RootObject, treeObjects.Nodes);
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
    }
}

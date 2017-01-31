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
                radioButton1.Text = string.Format("Show objects on which {0} depend", RootObject.DaxObjectName);
                radioButton2.Text = string.Format("Show objects that depend on {0}", RootObject.DaxObjectName);
                RefreshTree();
            }
        }

        private void RecursiveAdd(ITabularNamedObject obj, TreeNodeCollection nodes)
        {
            var img = UI.Tree.TabularIcon.GetIconIndex(obj);
            var n = new TreeNode(obj.Name, img, img) { Tag = obj };

            nodes.Add(n);

            if(obj is IExpressionObject)
            {
                foreach(var d in ((IExpressionObject)obj).Dependencies.Keys.OrderBy(k => k.ObjectType))
                {
                    currentDepth++;
                    if (d == _rootObject)
                    {
                        var i = UI.Tree.TabularIcon.GetIconIndex(d);
                        n.Nodes.Add(new TreeNode(d.Name + " (circular dependency)", i, i));
                    }
                    else if (currentDepth < MAX_LEVELS) RecursiveAdd(d, n.Nodes);
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
            var n = new TreeNode(obj.Name, img, img) { Tag = obj };

            nodes.Add(n);

            foreach(var d in obj.Model.Tables.OfType<IExpressionObject>().Concat(obj.Model.Tables.SelectMany(t => t.GetChildren().OfType<IExpressionObject>())))
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
    }
}

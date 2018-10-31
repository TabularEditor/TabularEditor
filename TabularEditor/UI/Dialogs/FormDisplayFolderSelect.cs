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

namespace TabularEditor.UI.Dialogs
{
    public partial class FormDisplayFolderSelect : Form, ICustomEditor
    {
        public FormDisplayFolderSelect()
        {
            InitializeComponent();
            treeFolders.ImageList = FormMain.Singleton.tabularTreeImages;
        }

        public TreeNodeCollection FolderNodes { get { return treeFolders.Nodes; } }
        public string SelectedFolder {
            get { return string.Join("\\", treeFolders.SelectedNode?.FullPath.Split('\\').Skip(1)); }
        }

        private void treeFolders_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != rootNode)
            {
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        private void treeFolders_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node != rootNode)
            {
                e.Node.ImageIndex = 0;
                e.Node.SelectedImageIndex = 0;
            }
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            TreeNode newNode;

            if(treeFolders.SelectedNode == null)
            {
                newNode = treeFolders.Nodes.Add("New folder");
            } else
            {
                newNode = treeFolders.SelectedNode.Nodes.Add("New folder");
            }
            treeFolders.LabelEdit = true;
            treeFolders.SelectedNode = newNode;
            newNode.EnsureVisible();
            newNode.BeginEdit();
        }

        private void treeFolders_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeFolders.LabelEdit = false;
            if (e.Label == "New folder")
                e.Node.Remove();
            else
                e.Node.Tag = e.Node.FullPath;
        }

        private void treeFolders_MouseDown(object sender, MouseEventArgs e)
        {
            var n = treeFolders.GetNodeAt(e.X, e.Y);
            if (n == null)
            {
                treeFolders.SelectedNode = null;
                btnOK.Enabled = false;
                btnNewFolder.Enabled = false;
            }
        }

        private void treeFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnOK.Enabled = e.Node != null;
            btnNewFolder.Enabled = e.Node != null;
        }

        private TreeNode rootNode;

        public object Edit(object instance, string property, object value, out bool cancel)
        {
            cancel = true;

            Table table = null;

            var arr = instance as object[];
            if (arr != null && arr.Length > 0 && arr.All(obj => obj is IFolderObject))
            {
                table = (arr[0] as IFolderObject).Table;
            }
            else if (instance is IFolderObject)
            {
                table = (instance as IFolderObject).Table;
            }

            if(table != null)
            {
                FolderNodes.Clear();

                rootNode = FolderNodes.Add(table.Name, table.Name, 2, 2);

                AddChildren(rootNode.Nodes, Folder.CreateFolder(table, ""));

                treeFolders.SelectedNode = rootNode;

                cancel = ShowDialog() == DialogResult.Cancel;
                if (!cancel) value = SelectedFolder;
            }

            return value;
        }

        private void AddChildren(TreeNodeCollection parent, Folder folder)
        {
            var children = folder.GetChildrenByFolders().OfType<Folder>().OrderBy(f => f.Name).Select(f =>
            {
                var node = new TreeNode(f.Name);
                AddChildren(node.Nodes, f);
                return node;
            });

            parent.AddRange(children.ToArray());
        }

        private void treeFolders_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if(e.Node == rootNode) e.CancelEdit = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor
{
    public partial class FormDisplayFolderSelect : Form
    {
        public FormDisplayFolderSelect()
        {
            InitializeComponent();
        }

        public TreeNodeCollection FolderNodes { get { return treeFolders.Nodes; } }
        public string SelectedFolder { get { return treeFolders.SelectedNode?.Tag?.ToString(); } }

        private void treeFolders_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeFolders_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
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
            }
        }

        private void treeFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnOK.Enabled = e.Node != null;
        }
    }
}

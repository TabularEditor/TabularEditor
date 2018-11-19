using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.UIServices;
using Aga.Controls.Tree;
using System.Collections;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ImportTablesPage : UserControl
    {
        public ImportTablesPage()
        {
            InitializeComponent();
        }

        TypedDataSource Source;

        public void Init(TypedDataSource source)
        {
            var model = new SchemaModel(source);
            Source = source;
            treeViewAdv1.Model = model;
        }

        class SchemaModel : ITreeModel
        {
            TypedDataSource TypedDataSource;

            public SchemaModel(TypedDataSource dataSource)
            {
                TypedDataSource = dataSource;
            }

            public event EventHandler<TreeModelEventArgs> NodesChanged;
            public event EventHandler<TreeModelEventArgs> NodesInserted;
            public event EventHandler<TreeModelEventArgs> NodesRemoved;
            public event EventHandler<TreePathEventArgs> StructureChanged;

            public IEnumerable GetChildren(TreePath treePath)
            {
                if (treePath.IsEmpty())
                {
                    return TypedDataSource.GetDatabases().OrderBy(n => n.Name);
                }
                else
                {
                    var dbNode = treePath.FirstNode as SchemaNode;
                    var groupNode = treePath.LastNode as SchemaNode;
                    return TypedDataSource.GetTablesAndViews(dbNode.Name).OrderBy(n => n.Type).ThenBy(n => n.DisplayName);
                }
            }

            public bool IsLeaf(TreePath treePath)
            {
                if (treePath.IsEmpty()) return false;
                else if ((treePath.LastNode as SchemaNode).Type == SchemaNodeType.Database) return false;
                else
                    return true;
            }
        }

        private void nodeCheckBox1_IsVisibleValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            var schemaNode = (e.Node.Tag as SchemaNode);
            if (schemaNode == null) return;
            e.Value = schemaNode.Type == SchemaNodeType.Table || schemaNode.Type == SchemaNodeType.View;
        }

        private void nodeIcon1_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            var schemaNode = (e.Node.Tag as SchemaNode);
            if (schemaNode == null) return;
            int imageIndex = 0;
            switch(schemaNode.Type) {
                case SchemaNodeType.Database: imageIndex = 0; break;
                case SchemaNodeType.Table: imageIndex = 1; break;
                case SchemaNodeType.View: imageIndex = 2; break;
            }
            e.Value = imageList1.Images[imageIndex];
        }

        private void nodeIcon1_IsVisibleValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            var schemaNode = (e.Node.Tag as SchemaNode);
            if (schemaNode == null) return;
            switch (schemaNode.Type)
            {
                case SchemaNodeType.Database: 
                case SchemaNodeType.Table:
                case SchemaNodeType.View:
                    e.Value = true; break;
                default:
                    e.Value = false; break;
            }
        }

        private void treeViewAdv1_Expanding(object sender, TreeViewAdvEventArgs e)
        {
        }

        private void nodeSpinner_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            if (e.Node.IsExpandingNow) e.Value = imageList1.Images[3];
        }

        private void treeViewAdv1_SelectionChanged(object sender, EventArgs e)
        {
            var schemaNode = treeViewAdv1.SelectedNode?.Tag as SchemaNode;
            if(schemaNode == null || schemaNode.Type == SchemaNodeType.Database)
            {
                dataGridView1.DataSource = null;
                return;
            }
            else
            {
                Task.Run(() => this.Invoke(new MethodInvoker(() => { dataGridView1.DataSource = Source.GetSampleData(schemaNode); })));
                
            }
        }
    }
}

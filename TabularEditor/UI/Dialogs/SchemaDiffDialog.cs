using Aga.Controls.Tree;
using TabularEditor.TOMWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
    public partial class SchemaDiffDialog : Form
    {
        public SchemaDiffDialog()
        {
            InitializeComponent();

            
        }

        MetadataChangeModel Model;

        static public DialogResult Show(IEnumerable<MetadataChange> metadataChanges)
        {
            var dialog = new SchemaDiffDialog();

            dialog.Model = new MetadataChangeModel(metadataChanges);

            var res = dialog.ShowDialog();
            return res;
        }

        private void SchemaDiffDialog_Load(object sender, EventArgs e)
        {
            treeViewAdv1.Model = Model;
            treeViewAdv1.ExpandAll();
        }

        private void nodeIcon1_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            e.Value = UIController.Current.Elements.FormMain.tabularTreeImages.Images[(e.Node.Tag as Change).ChangeIcon];
        }
    }

    public class MetadataChangeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        Dictionary<Change, List<Change>> TableChanges;

        public MetadataChangeModel(IEnumerable<MetadataChange> metadataChanges)
        {
            TableChanges = metadataChanges.GroupBy(c => c.ModelTable).ToDictionary(
                g => new Change(g.Key.Name),
                g => g.Select(c => new Change(c)).ToList()
                );
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if(treePath.IsEmpty())
            {
                return TableChanges.Keys.OrderBy(k => k.ChangeText);
            }
            else
            {
                return TableChanges[(treePath.LastNode as Change)];
            }
            
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.IsEmpty()) return false;
            if (treePath.FirstNode == treePath.LastNode) return false;
            return true;
        }
    }

    public class Change
    {
        public string ChangeText { get; private set; }
        public int ChangeIcon { get; private set; }
        public bool ChangeInclude { get; set; }
        public string ModelDataType { get; private set; }
        public string SourceDataType { get; private set; }

        public Change(string tableName)
        {
            ChangeText = tableName;
            ChangeIcon = 2;
            ChangeInclude = true;
        }
        public Change(MetadataChange metadataChange)
        {
            ChangeText = metadataChange.ModelColumn?.Name ?? metadataChange.SourceColumn;
            ModelDataType = metadataChange.ModelColumn?.DataType.ToString() ?? "";
            ChangeIcon = 4;

            switch (metadataChange.ChangeType)
            {
                case MetadataChangeType.DataTypeChange:
                case MetadataChangeType.SourceColumnAdded:
                    SourceDataType = metadataChange.SourceType.ToString();
                    break;
                case MetadataChangeType.SourceColumnNotFound:
                case MetadataChangeType.SourceQueryError:
                    SourceDataType = "";
                    break;
            }
        }
    }
}

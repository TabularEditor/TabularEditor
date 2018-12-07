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
            if(res == DialogResult.OK)
            {
                var acceptedChanged = dialog.Model.TableChanges.Values.SelectMany(v => v).Where(c => c.ChangeInclude == CheckState.Checked).ToList();
                foreach(var change in acceptedChanged)
                {
                    switch(change.ChangeType)
                    {
                        case ChangeType.RemoveColumn:
                            change.MetadataChange.ModelColumn.Delete(); break;
                        case ChangeType.AddColumn:
                            change.MetadataChange.ModelTable.AddDataColumn(change.SourceColumn, change.SourceColumn, null, change.MetadataChange.SourceType); break;
                        case ChangeType.EditDataType:
                            change.MetadataChange.ModelColumn.DataType = change.MetadataChange.SourceType; break;
                    }
                }
            }
            return res;
        }

        private void SchemaDiffDialog_Load(object sender, EventArgs e)
        {
            treeViewAdv1.Model = Model;
            treeViewAdv1.ExpandAll();
        }

        private void nodeIcon1_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            var change = e.Node.Tag as Change;
            switch(change.ChangeType)
            {
                case ChangeType.Table: e.Value = UIController.Current.Elements.FormMain.tabularTreeImages.Images[2]; break; // Table icon
                case ChangeType.AddColumn: e.Value = Resources.add; break;
                case ChangeType.RemoveColumn: e.Value = Resources.remove; break;
                case ChangeType.EditDataType: e.Value = Resources.editdatatype; break;
            }
        }

        private void nodeTextBox1_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            var change = e.Node.Tag as Change;
            if(change.MetadataChange?.ChangeType == MetadataChangeType.SourceColumnAdded)
            {
                e.Font = new Font(e.Font, FontStyle.Italic);
            }
            if(change.ChangeType == ChangeType.Table && change.IsTableError)
            {
                e.TextColor = Color.Red;
            }
        }

        private void nodeCheckBox1_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            var currentItem = (e.Path.LastNode as Change);
            var currentNode = treeViewAdv1.FindNode(e.Path);
            if (currentItem.ChangeType != ChangeType.Table)
            {
                var parentNode = currentNode.Parent;
                var parentItem = parentNode.Tag as Change;
                if (parentNode.Children.Any(n => (n.Tag as Change).ChangeInclude != currentItem.ChangeInclude))
                    parentItem.ChangeInclude = CheckState.Indeterminate;
                else
                    parentItem.ChangeInclude = currentItem.ChangeInclude;
            }
            else
            {
                foreach (var child in currentNode.Children)
                {
                    (child.Tag as Change).ChangeInclude = currentItem.ChangeInclude;
                }
            }

            suspendSelectAll = true;
            var state = Model.TableChanges.Keys.First().ChangeInclude;
            if (Model.TableChanges.Keys.All(k => k.ChangeInclude == state))
            {
                chkSelectAll.CheckState = state;
            }
            else
                chkSelectAll.CheckState = CheckState.Indeterminate;
            suspendSelectAll = false;
        }

        bool suspendSelectAll = false;

        private void treeViewAdv1_RowDraw(object sender, TreeViewRowDrawEventArgs e)
        {
            if((e.Node.Tag as Change).ChangeType == ChangeType.Table)
            {
                e.Graphics.FillRectangle(Brushes.Azure, e.RowRect);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            /*var node = treeViewAdv1.SelectedNode;
            var item = node.Tag as Change;

            if (item.ChangeType == ChangeType.AddColumn)
            {
                var parentItem = node.Parent.Tag as Change;
                var table = item.MetadataChange.ModelTable;
                PopulateMapToColumnPopup(item, table, Model.TableChanges[parentItem]);
                e.Cancel = false;
            }
            else if (item.ChangeType == ChangeType.RemoveColumn)
            {
                var parentItem = node.Parent.Tag as Change;
                var table = item.MetadataChange.ModelTable;
                PopulateMapFromSourcePopup(table, Model.TableChanges[parentItem]);
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }*/

            e.Cancel = true;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (suspendSelectAll) return;
            foreach (var table in Model.TableChanges.Keys)
            {
                table.ChangeInclude = chkSelectAll.CheckState;
                foreach(var column in Model.TableChanges[table])
                {
                    column.ChangeInclude = chkSelectAll.CheckState;
                }
            }
            treeViewAdv1.Invalidate();
        }

        /*private void PopulateMapFromSourcePopup(Table table, List<Change> allChanges)
        {
            contextMenuStrip1.Items.Clear();
            var header = new ToolStripLabel("Map from Source Column:");
            header.Font = new Font(header.Font, FontStyle.Bold);
            contextMenuStrip1.Items.Add(header);
            contextMenuStrip1.Items.Add(new ToolStripSeparator());

            HashSet<string> changedColumns = new HashSet<string>();
            foreach (var change in allChanges
                .Where(c => c.ChangeType == ChangeType.AddColumn)
                .OrderBy(c => c.MetadataChange.SourceColumn))
            {
                var col = change.MetadataChange.SourceColumn;
                changedColumns.Add(col);
                contextMenuStrip1.Items.Add(col);
            }

            HashSet<string> existingSourceColumns = new HashSet<string>(table.DataColumns.Select(c => c.SourceColumn).Distinct(StringComparer.InvariantCultureIgnoreCase).Except(changedColumns));

            if (changedColumns.Count > 1 && existingSourceColumns.Count > 0) contextMenuStrip1.Items.Add(new ToolStripSeparator());

            if (existingSourceColumns.Count > 0)
            {
                var otherMenuItem = new ToolStripMenuItem("Other");
                contextMenuStrip1.Items.Add(otherMenuItem);
                foreach (var col in existingSourceColumns.OrderBy(c => c))
                {
                    otherMenuItem.DropDownItems.Add(col);
                }
            }
        }

        private void PopulateMapToColumnPopup(Change orgChange, Table table, List<Change> allChanges)
        {
            contextMenuStrip1.Items.Clear();
            var header = new ToolStripLabel("Map to Column:");
            header.Font = new Font(header.Font, FontStyle.Bold);
            contextMenuStrip1.Items.Add(header);
            contextMenuStrip1.Items.Add(new ToolStripSeparator());

            HashSet<Column> changedColumns = new HashSet<Column>();
            foreach (var change in allChanges
                .Where(c => c.ChangeType == ChangeType.RemoveColumn)
                .OrderBy(c => c.MetadataChange.ModelColumn.Name))
            {
                var col = change.MetadataChange.ModelColumn;
                changedColumns.Add(col);
                contextMenuStrip1.Items.Add(MapToColumn(orgChange, col));
            }

            if(changedColumns.Count > 1 && table.DataColumns.Count() > changedColumns.Count) contextMenuStrip1.Items.Add(new ToolStripSeparator());

            if (table.DataColumns.Count() > changedColumns.Count)
            {
                var otherMenuItem = new ToolStripMenuItem("Other");
                contextMenuStrip1.Items.Add(otherMenuItem);
                foreach (var col in table.DataColumns.Except(changedColumns).OrderBy(c => c.Name))
                {
                    otherMenuItem.DropDownItems.Add(MapToColumn(orgChange, col));
                }
            }
        }

        private ToolStripMenuItem MapToColumn(Change change, Column column)
        {
            var result = new ToolStripMenuItem(column.Name);
            result.Click += (s, e) =>
            {
                change.Description = "Map to " + column.DaxObjectName;
                Model.TableChanges.Keys.Where(k => k.)
            };
            return result;
        }*/
    }

    public class MetadataChangeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public readonly Dictionary<Change, List<Change>> TableChanges;

        public MetadataChangeModel(IEnumerable<MetadataChange> metadataChanges)
        {
            TableChanges = metadataChanges.GroupBy(c => c.ModelTable).ToDictionary(
                g => new Change(g.Key.Name, g.FirstOrDefault(c => c.ChangeType == MetadataChangeType.SourceQueryError)),
                g => g.Select(c => new Change(c)).ToList()
                );
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if(treePath.IsEmpty())
            {
                return TableChanges.Keys.OrderBy(k => k.Description);
            }
            else
            {
                return TableChanges[(treePath.LastNode as Change)];
            }
            
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.IsEmpty()) return false;
            return (treePath.LastNode as Change).IsLeaf;
        }
    }

    public enum ChangeType
    {
        Table,
        AddColumn,
        RemoveColumn,
        EditDataType
    }

    public class Change
    {
        public string ObjectName { get; private set; }
        public string SourceColumn { get; private set; }
        public string Description { get; set; }
        public ChangeType ChangeType { get; private set; }
        private CheckState _changeInclude;
        public CheckState ChangeInclude
        {
            get
            {
                return _changeInclude;
            }
            set
            {
                if (IsTableError) return;
                _changeInclude = value;
            }
        }
        public string ModelDataType { get; private set; }
        public string SourceDataType { get; private set; }
        public bool IsLeaf { get; private set; }
        public MetadataChange MetadataChange { get; private set; }
        public bool IsTableError = false;

        public Change(string tableName, MetadataChange tableError = null)
        {
            ObjectName = tableName;
            ChangeType = ChangeType.Table;
            if(tableError != null)
            {
                IsTableError = true;
                _changeInclude = CheckState.Unchecked;
                ObjectName += " (Unable to validate source query)";
                IsLeaf = true;
            }
            else
            {
                _changeInclude = CheckState.Checked;
                IsLeaf = false;
            }
        }
        public Change(MetadataChange metadataChange)
        {
            MetadataChange = metadataChange;
            IsLeaf = true;
            ChangeInclude = CheckState.Checked;
            ObjectName = metadataChange.ModelColumn?.Name ?? "(Not imported)";
            SourceColumn = metadataChange.ModelColumn?.SourceColumn ?? metadataChange.SourceColumn;
            ModelDataType = metadataChange.ModelColumn?.DataType.ToString() ?? "";
            SourceDataType = string.IsNullOrEmpty(metadataChange.SourceProviderType) ? null : metadataChange.SourceType.ToString();

            switch (metadataChange.ChangeType)
            {
                case MetadataChangeType.DataTypeChange:
                    Description = "Change Data Type"; ChangeType = ChangeType.EditDataType; break;
                case MetadataChangeType.SourceColumnAdded:
                    Description = "Import Column"; ChangeType = ChangeType.AddColumn; break;
                case MetadataChangeType.SourceColumnNotFound:
                    Description = "Remove Column"; ChangeType = ChangeType.RemoveColumn; break;
            }
        }
    }
}

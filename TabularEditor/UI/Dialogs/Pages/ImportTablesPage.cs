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
using System.Threading;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ImportTablesPage : UserControl
    {
        public ImportTablesPage()
        {
            InitializeComponent();
        }

        TypedDataSource Source;
        SchemaModel SchemaModel;

        public void Init(TypedDataSource source)
        {
            Source = source;
            SchemaModel = new SchemaModel(Source);
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
            switch (schemaNode.Type)
            {
                case SchemaNodeType.Database: imageIndex = 43; break;
                case SchemaNodeType.Table: imageIndex = 2; break;
                case SchemaNodeType.View: imageIndex = 44; break;
                case SchemaNodeType.Root: imageIndex = 36; break;
            }
            e.Value = UIController.Current.Elements.FormMain.tabularTreeImages.Images[imageIndex];

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
                case SchemaNodeType.Root:
                    e.Value = true; break;
                default:
                    e.Value = false; break;
            }
        }

        SchemaNode currentNode;

        private void treeViewAdv1_SelectionChanged(object sender, EventArgs e)
        {
            currentNode = treeViewAdv1.SelectedNode?.Tag as SchemaNode;
            if (currentNode == null || currentNode.Type == SchemaNodeType.Database || currentNode.Type == SchemaNodeType.Root)
            {
                dataGridView1.DataSource = null;
                return;
            }
            else
            {
                if (chkDisablePreview.Checked) return;
                PreviewLoaded = false;
                chkSelectAll.Visible = false;
                suspendSelectAll = true; chkSelectAll.Checked = currentNode.SelectAll; suspendSelectAll = false;

                if (fillSampleDataTask != null && !fillSampleDataTask.IsCompleted)
                {
                    cts.Cancel();
                    //fillSampleDataTask.Wait();
                    loadingPreviewSpinner.Visible = false;
                }
                dataGridView1.DataSource = null;
                lblError.Visible = false;
                loadingPreviewSpinner.Visible = true;

                txtSql.Text = currentNode.GetSql();

                cts = new CancellationTokenSource();
                var ct = cts.Token;

                // Fill the datagridview with sample data asynchronously:
                fillSampleDataTask = Task.Factory.StartNew(() =>
                {
                    if (ct.IsCancellationRequested) return;
                    var sampleData = Source.GetSampleData(currentNode, out bool isError);
                    if (ct.IsCancellationRequested) return;
                    this.Invoke(new MethodInvoker(() =>
                    {
                        loadingPreviewSpinner.Visible = false;
                        if (ct.IsCancellationRequested) return;

                        if (isError)
                        {
                            lblError.Visible = true;
                            lblError.Text = sampleData.Rows[0][0].ToString();
                        }
                        else
                        {
                            dataGridView1.SuspendDrawing();
                            dataGridView1.DataSource = sampleData;
                            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                if (sampleData.Columns[col.Index].DataType != typeof(string)) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                col.SortMode = DataGridViewColumnSortMode.Programmatic;
                                if (col.Width > 200) col.Width = 200;
                                else col.Width += 16;
                                var checkBoxHeader = new DatagridViewCheckBoxHeaderCell(currentNode.IncludedColumns.Contains(col.HeaderText));
                                col.HeaderCell = checkBoxHeader;
                                checkBoxHeader.OnCheckBoxClicked += CheckBoxHeader_OnCheckBoxClicked;
                                
                            }
                            dataGridView1.ResumeDrawing();
                            PreviewLoaded = true;
                            chkSelectAll.Visible = true;
                        }
                    }));
                }, ct);

            }
        }

        private void CheckBoxHeader_OnCheckBoxClicked(int columnIndex, bool state)
        {
            currentNode.IncludedColumns.Clear();
            currentNode.SelectAll = false;
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                if ((col.HeaderCell as DatagridViewCheckBoxHeaderCell).Checked) currentNode.IncludedColumns.Add(col.HeaderText);
            }
            txtSql.Text = currentNode.GetSql(true, false);
            suspendSelectAll = true;
            chkSelectAll.Checked = false;
            suspendSelectAll = false;
        }

        CancellationTokenSource cts;
        Task fillSampleDataTask;

        private void ImportTablesPage_Load(object sender, EventArgs e)
        {
            treeViewAdv1.Model = SchemaModel;
        }

        public void ExpandFirstNode()
        {
            if (treeViewAdv1.Root.Tag != null)
                treeViewAdv1.Root.Expand();
        }

        private void treeViewAdv1_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            if (e.Node.Tag is null)
            {
                var root = e.Node.Children[0];
                root.Expand();
                if (root.Children.Count == 1) root.Children[0].Expand();
            }
        }

        bool suspendSelectAll = false;
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (suspendSelectAll) return;
            if (!PreviewLoaded) return;

            currentNode.IncludedColumns.Clear();
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                var header = col.HeaderCell as DatagridViewCheckBoxHeaderCell;
                header.Checked = chkSelectAll.Checked;
                if (header.Checked) currentNode.IncludedColumns.Add(col.HeaderText);
            }
            dataGridView1.Invalidate();
            currentNode.SelectAll = chkSelectAll.Checked;
            txtSql.Text = currentNode.GetSql(true, false);
        }

        bool PreviewLoaded = false;

        private void chkDisablePreview_CheckedChanged(object sender, EventArgs e)
        {
            previewPane.Visible = !chkDisablePreview.Checked;
            if (previewPane.Visible) treeViewAdv1_SelectionChanged(sender, e);
        }

        private void nodeCheckBox1_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            OnSchemaObjectCheck?.Invoke(sender, e.Path.LastNode as SchemaNode, (e.Path.LastNode as SchemaNode).Selected);
        }

        public delegate void SchemaObjectCheckHandler(object sender, SchemaNode node, bool isChecked);
        public event SchemaObjectCheckHandler OnSchemaObjectCheck;

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }

    public delegate void CheckBoxClickedHandler(int columnIndex, bool state);
    public class DataGridViewCheckBoxHeaderCellEventArgs : EventArgs
    {
        bool _bChecked;
        public DataGridViewCheckBoxHeaderCellEventArgs(int columnIndex, bool bChecked)
        {
            _bChecked = bChecked;
        }
        public bool Checked
        {
            get { return _bChecked; }
        }
    }
    class DatagridViewCheckBoxHeaderCell : DataGridViewColumnHeaderCell
    {
        Point checkBoxLocation;
        Size checkBoxSize;
        bool _checked = false;
        public bool Checked { get { return _checked; } set { _checked = value; } }
        Point _cellLocation = new Point();
        System.Windows.Forms.VisualStyles.CheckBoxState _cbState =
        System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
        public event CheckBoxClickedHandler OnCheckBoxClicked;

        public DatagridViewCheckBoxHeaderCell(bool checkState)
        {
            _checked = checkState;
            Style.Padding = new Padding(Style.Padding.Left + 16, Style.Padding.Top, Style.Padding.Right, Style.Padding.Bottom);
        }

        protected override void Paint(System.Drawing.Graphics graphics,
            System.Drawing.Rectangle clipBounds,
            System.Drawing.Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates dataGridViewElementState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            Point p = new Point();
            Size s = CheckBoxRenderer.GetGlyphSize(graphics,
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            p.X = cellBounds.Location.X + 3;
            p.Y = cellBounds.Location.Y + (cellBounds.Height / 2) - (s.Height / 2);
            _cellLocation = cellBounds.Location;
            checkBoxLocation = p;
            checkBoxSize = s;
            if (_checked)
                _cbState = System.Windows.Forms.VisualStyles.
                CheckBoxState.CheckedNormal;
            else
                _cbState = System.Windows.Forms.VisualStyles.
                CheckBoxState.UncheckedNormal;
            CheckBoxRenderer.DrawCheckBox(graphics, checkBoxLocation, _cbState);
        }
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            Point p = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            if (p.X >= checkBoxLocation.X && p.X <= checkBoxLocation.X + checkBoxSize.Width && p.Y >= checkBoxLocation.Y && p.Y <= checkBoxLocation.Y + checkBoxSize.Height)
            {
                _checked = !_checked;
                if (OnCheckBoxClicked != null)
                {
                    OnCheckBoxClicked(e.ColumnIndex, _checked);
                    this.DataGridView.InvalidateCell(this);
                }
            }
            else
            {
                ListSortDirection lsd;
                switch(SortGlyphDirection)
                {
                    case SortOrder.None:
                    case SortOrder.Descending:
                        SortGlyphDirection = SortOrder.Ascending;
                        lsd = ListSortDirection.Ascending;
                        break;
                    default:
                        SortGlyphDirection = SortOrder.Descending;
                        lsd = ListSortDirection.Descending;
                        break;
                }
                DataGridView.Sort(OwningColumn, lsd);
                //base.OnMouseClick(e);
            }
        }
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
            var node = treePath.LastNode as SchemaNode;

            if (node == null)
            {
                return Enumerable.Repeat(new SchemaNode { Type = SchemaNodeType.Root, Name = TypedDataSource.TabularDsName }, 1);
            }
            else if (node.Type == SchemaNodeType.Root)
            {
                try
                {
                    return TypedDataSource.GetDatabases().OrderBy(n => n.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to read list of databases from the source:\n\n" + ex.Message, "Unable to read source metadata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Enumerable.Empty<SchemaNode>();
                }
            }
            else
            {
                try
                {
                    return TypedDataSource.GetTablesAndViews(node.Name).OrderBy(n => n.Type).ThenBy(n => n.DisplayName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to read list of tables/views from the source:\n\n" + ex.Message, "Unable to read source metadata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Enumerable.Empty<SchemaNode>();
                }
            }
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.IsEmpty()) return false;
            else if ((treePath.LastNode as SchemaNode).Type == SchemaNodeType.Root) return false;
            else if ((treePath.LastNode as SchemaNode).Type == SchemaNodeType.Database) return false;
            else
                return true;
        }
    }
}

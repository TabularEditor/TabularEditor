using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs
{
    public partial class ObjectSelectDialog<T> : Form where T: TabularNamedObject
    {
        string[] _objectNames;
        List<T> _objects;
        public T SelectedObject { get; set; }
        public object[] SelectedObjects { get; set; }
        private readonly bool _multiSelect;
        private readonly bool _allowNoSelection;

        public ObjectSelectDialog(bool multiSelect, bool allowNoSelection)
        {
            InitializeComponent();

            listView1.Resize += ListView1_Resize;
            listView1.MultiSelect = multiSelect;
            btnClear.Click += (s, e) => ClearSelection();
            _multiSelect = multiSelect;
            _allowNoSelection = allowNoSelection;
            btnClear.Visible = _allowNoSelection || multiSelect;
        }

        private void ClearSelection()
        {
            listView1.SelectedItems.Clear();
        }

        private bool suspendResize = false;

        private void ListView1_Resize(object sender, EventArgs e)
        {
            if (suspendResize) return;
            suspendResize = true;
            columnHeader1.Width = listView1.ClientRectangle.Width - 2;
            suspendResize = false;
        }

        public void Setup(IEnumerable<T> columns)
        {
            listView1.Items.Clear();
            this._objects = columns.OrderBy(c => c.Name).ToList();
            this._objectNames = this._objects.Select(c => c.Name).ToArray();
            listView1.Items.AddRange(this._objects.Select(c => new ListViewItem(c.Name)).ToArray());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_multiSelect)
            {
                SelectedObject = null;
                if (listView1.SelectedIndices.Count > 0)
                {
                    btnOK.Enabled = true;
                    SelectedObjects = listView1.SelectedIndices.OfType<int>().Select(i => _objects[i]).ToArray();
                }
                else
                {
                    if (!_allowNoSelection) btnOK.Enabled = false;
                    SelectedObjects = Array.Empty<object>();
                }

            }
            else
            {
                SelectedObjects = null;
                if (listView1.SelectedIndices.Count == 1)
                {
                    btnOK.Enabled = true;
                    SelectedObject = _objects[listView1.SelectedIndices[0]];
                }
                else
                {
                    if(!_allowNoSelection) btnOK.Enabled = false;
                    SelectedObject = null;
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
                listView1.EnsureVisible(listView1.SelectedIndices[0]);
            listView1.Focus();
            base.OnShown(e);
        }

        protected void Prep(IEnumerable<T> columns, IEnumerable<T> preselectedColumns, string label)
        {
            label1.Text = label;
            Setup(columns);
            listView1.SelectedIndices.Clear();
            btnOK.Enabled = _allowNoSelection || preselectedColumns.Any();
            foreach (var preselect in preselectedColumns)
            {
                var ix = this._objects.IndexOf(preselect);
                if (ix >= 0)
                {
                    listView1.SelectedIndices.Add(ix);
                    listView1.EnsureVisible(ix);
                }
            }
        }

        protected void Prep(IEnumerable<T> columns, T preselect, string label)
        {
            label1.Text = label;
            Setup(columns);
            listView1.SelectedIndices.Clear();
            var ix = this._objects.IndexOf(preselect);
            if (ix >= 0)
            {
                listView1.SelectedIndices.Add(ix);
                listView1.EnsureVisible(ix);
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        public static T SelectObject(IEnumerable<T> columns, T preselect = null, string label = "Select object:")
        {
            var selector = new ObjectSelectDialog<T>(multiSelect: false, allowNoSelection: true);
            selector.Text = "Select " + typeof(T).Name;
            selector.Prep(columns, preselect, label);
            if (selector.ShowDialog() == DialogResult.Cancel) return null;
            else return selector.SelectedObject;
        }
    }

    public class ColumnSelectDialog: ObjectSelectDialog<Column>, ICustomEditor
    {
        public ColumnSelectDialog(bool multiSelect, bool allowNoSelection): base(multiSelect, allowNoSelection)
        {

        }

        public object Edit(object instance, string property, object value, out bool cancel)
        {
            var instanceColumn = instance as Column ?? (instance as object[])?.First() as Column;
            var tuca = instance as TimeUnitColumnAssociation;

            switch (property)
            {
                case nameof(Column.SortByColumn):
                    Prep(instanceColumn.Table.Columns.Where(c => c != instanceColumn), value as Column, "Select column:");
                    break;
                case nameof(Column.GroupByColumns):
                    Prep(instanceColumn.Table.Columns.Where(c => c != instanceColumn), (value as IEnumerable<object>)?.Cast<Column>(), "Select column:");
                    break;
                case nameof(TimeRelatedColumnGroup.Columns):
                    var trcg = instance as TimeRelatedColumnGroup;
                    Prep(trcg.Calendar.Table.Columns, (value as IEnumerable<object>)?.Cast<Column>(), "Select columns:");
                    break;
                case nameof(TimeUnitColumnAssociation.AssociatedColumns):
                    Prep(tuca.Calendar.Table.Columns, (value as IEnumerable<object>)?.Cast<Column>(), "Select columns:");
                    break;
                case nameof(TimeUnitColumnAssociation.PrimaryColumn):
                    Prep(tuca.Calendar.Table.Columns, tuca.PrimaryColumn, "Select column:");
                    break;
            }

            if (ShowDialog() == DialogResult.Cancel)
            {
                cancel = true;
                return null;
            }

            cancel = false;
            return SelectedObject ?? (object)SelectedObjects;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs
{
    public partial class ObjectSelectDialog<T> : Form where T: TabularNamedObject
    {
        string[] columnNames;
        List<T> columns;
        public T SelectedColumn { get; set; }

        public ObjectSelectDialog()
        {
            InitializeComponent();

            listView1.Resize += ListView1_Resize;
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
            this.columns = columns.OrderBy(c => c.Name).ToList();
            this.columnNames = this.columns.Select(c => c.Name).ToArray();
            listView1.Items.AddRange(this.columns.Select(c => new ListViewItem(c.Name)).ToArray());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count == 1) {
                btnOK.Enabled = true;
                SelectedColumn = columns[listView1.SelectedIndices[0]];
            } else
            {
                btnOK.Enabled = false;
                SelectedColumn = null;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
                listView1.EnsureVisible(listView1.SelectedIndices[0]);
            listView1.Focus();
            base.OnShown(e);
        }

        protected void Prep(IEnumerable<T> columns, T preselect, string label)
        {
            label1.Text = label;
            Setup(columns);
            listView1.SelectedIndices.Clear();
            var ix = this.columns.IndexOf(preselect);
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
            var selector = new ObjectSelectDialog<T>();
            selector.Text = "Select " + typeof(T).Name;
            selector.Prep(columns, preselect, label);
            if (selector.ShowDialog() == DialogResult.Cancel) return null;
            else return selector.SelectedColumn;
        }
    }

    public class ColumnSelectDialog: ObjectSelectDialog<Column>, ICustomEditor
    {
        public object Edit(object instance, string property, object value, out bool cancel)
        {
            var instanceColumn = instance as Column ?? (instance as object[]).First() as Column;

            if (instanceColumn != null)
            {
                switch (property)
                {
                    case "Sort By Column":
                        Prep(instanceColumn.Table.Columns.Where(c => c != instanceColumn), value as Column, "Select column:");
                        break;
                    case TOMWrapper.Properties.GROUPBYCOLUMNS:
                        Prep(value == null ? instanceColumn.Table.Columns : instanceColumn.Table.Columns.Except((value as object[]).Cast<Column>()), null, "Select column:");
                        break;
                }
            }
            if (ShowDialog() == DialogResult.Cancel)
            {
                cancel = true;
                return null;
            }

            cancel = false;
            return SelectedColumn;
        }
    }
}

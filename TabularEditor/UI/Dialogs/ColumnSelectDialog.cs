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
    public partial class ColumnSelectDialog : Form, ICustomEditor
    {
        string[] columnNames;
        List<Column> columns;
        public Column SelectedColumn { get; set; }

        public ColumnSelectDialog()
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

        public void Setup(IEnumerable<Column> columns)
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

        public object Edit(object instance, string property, object value, out bool cancel)
        {
            if (instance is Column instanceColumn)
            {
                switch (property)
                {
                    case "Sort By Column":
                        Setup(instanceColumn.Table.Columns.Where(c => c != instanceColumn));
                        listView1.SelectedIndices.Clear();
                        var ix = columns.IndexOf(value as Column);
                        if (ix >= 0)
                        {
                            listView1.SelectedIndices.Add(ix);
                            listView1.EnsureVisible(ix);
                        }
                        break;
                    case TOMWrapper.Properties.GROUPBYCOLUMNS:
                        Setup(instanceColumn.Table.Columns.Except(value as IEnumerable<Column>));
                        listView1.SelectedIndices.Clear();
                        break;
                }
            }
            btnOK.Enabled = listView1.SelectedIndices.Count > 0;
            if(ShowDialog() == DialogResult.Cancel)
            {
                cancel = true;
                return null;
            }

            cancel = false;
            return SelectedColumn;
        }
    }
}

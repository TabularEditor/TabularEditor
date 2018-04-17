using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs
{
    public partial class MarkAsDateTableForm : Form
    {
        public MarkAsDateTableForm()
        {
            InitializeComponent();
        }

        private const string AnnotationKey = "TabularEditor_MarkAsDateKey";

        public void Go(Table table)
        {
            chkDateTable.Checked =
                table.DataCategory == "Time" &&
                table.Columns.Any(c => c.IsKey && c.DataType == DataType.DateTime);
            chkDateTable.Text = $"Mark '{table.Name}' as a Date Table";

            cmbKeyColumn.Items.Clear();
            cmbKeyColumn.Items.AddRange(table.Columns.Where(c => c.DataType == DataType.DateTime).Select(c => c.Name).ToArray());

            cmbKeyColumn.SelectedItem = table.Columns.FirstOrDefault(c => c.IsKey && c.DataType == DataType.DateTime)?.Name;
            if (cmbKeyColumn.SelectedItem == null && cmbKeyColumn.Items.Count > 0) cmbKeyColumn.SelectedIndex = 0;

            chkDateTable.Enabled = cmbKeyColumn.Items.Count > 0;
            cmbKeyColumn.Enabled = chkDateTable.Checked;

            if (ShowDialog() == DialogResult.OK)
            {
                table.DataCategory = chkDateTable.Checked ? "Time" : "";
                if (chkDateTable.Checked)
                {
                    var col = table.Columns[(string)cmbKeyColumn.SelectedItem];
                    if (!col.IsKey)
                    {
                        col.IsKey = true;
                        col.SetAnnotation(AnnotationKey, "1");
                    }
                } else
                {
                    // Only remove the IsKey property on the column, if it was initially set by this dialog:
                    var col = table.Columns.FirstOrDefault(c => c.IsKey);
                    if (col != null && col.GetAnnotation(AnnotationKey) == "1")
                    {
                        col.IsKey = false;
                        col.RemoveAnnotation(AnnotationKey);
                    }
                }
            }
        }

        private void chkDateTable_CheckedChanged(object sender, EventArgs e)
        {
            cmbKeyColumn.Enabled = chkDateTable.Checked;
        }
    }
}

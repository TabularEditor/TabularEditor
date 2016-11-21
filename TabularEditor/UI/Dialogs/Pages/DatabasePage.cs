using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class DatabasePage : UserControl
    {
        public event ValidationEventHandler Validation;
        public event EventHandler Accept;

        public DatabasePage()
        {
            InitializeComponent();
        }

        private Server _server;
        public Server Server
        {
            set
            {
                _server = value;
                dataGridView1.DataSource = _server?.Databases.Cast<Database>().OrderBy(db => db.Name).ToList();
            }
            get
            {
                return _server;
            }
        }
        public bool AllowNew
        {
            set {
                pnlDatabaseID.Visible = value;
            }
            get
            {
                return pnlDatabaseID.Visible;
            }
        }

        bool suspendEvent = false;

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !suspendEvent)
            {
                suspendEvent = true;

                txtDatabaseName.Text = (dataGridView1.CurrentRow.DataBoundItem as Database).ID;
                OnValidation();

                suspendEvent = false;
            }
        }

        private void txtDatabaseID_TextChanged(object sender, EventArgs e)
        {
            if (!suspendEvent)
            {
                suspendEvent = true;

                dataGridView1.ClearSelection();
                OnValidation();

                suspendEvent = false;
            }
        }

        public string DatabaseID;

        bool valid = false;

        private void OnValidation()
        {
            DatabaseID = string.IsNullOrWhiteSpace(txtDatabaseName.Text) ? null : txtDatabaseName.Text;

            var newValid = !string.IsNullOrEmpty(DatabaseID);
            if (valid != newValid)
            {
                valid = newValid;
                Validation?.Invoke(this, new ValidationEventArgs(valid));
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Accept?.Invoke(this, new EventArgs());
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Accept?.Invoke(this, new EventArgs());
            }
        }

    }
}

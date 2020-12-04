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
using System.Globalization;
using TabularEditor.UIServices;
using System.Threading;
using Microsoft.AnalysisServices;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class DatabasePage : UserControl, IValidationPage
    {
        public event ValidationEventHandler Validation;
        public event EventHandler Accept;

        public DatabasePage()
        {
            InitializeComponent();
        }

        public bool ClearSelection { get; set; } = false;
        public string PreselectDb { get; set; } = "";

        private Server _server;
        public Server Server
        {
            set
            {
                _server = value;
                if (_server == null)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.ClearSelection();
                }
                else
                {
                    dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
                    dataGridView1.DataSource = DatabaseInfo.GetDatabasesFromServer(_server);
                }
            }
            get
            {
                return _server;
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.DataBindingComplete -= dataGridView1_DataBindingComplete;
            if (ClearSelection) DoClearSelection();
            var databaseList = dataGridView1.DataSource as List<Database>;
            if (databaseList != null && !string.IsNullOrEmpty(PreselectDb))
            {
                var index = databaseList.FirstIndexOf(db => db.Name == PreselectDb);
                if (index >= 0) dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[0];
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

        public void DoClearSelection()
        {
            suspendEvent = true;
            dataGridView1.ClearSelection();
            txtDatabaseName.Text = "";
            suspendEvent = false;
            OnValidation();
        }

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1 && !suspendEvent)
            {
                suspendEvent = true;

                txtDatabaseName.Text = (dataGridView1.SelectedRows[0].DataBoundItem as DatabaseInfo).Name;
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

        public string DatabaseName { get; private set; }

        bool valid = false;
        public bool IsValid => valid;

        private void OnValidation()
        {
            DatabaseName = string.IsNullOrWhiteSpace(txtDatabaseName.Text) ? null : txtDatabaseName.Text;

            var newValid = !string.IsNullOrEmpty(DatabaseName);
            if (valid != newValid)
            {
                valid = newValid;
                Validation?.Invoke(this, new ValidationEventArgs(valid));
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
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

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                int index = 0;
                // This works only if dataGridView1's SelectionMode property is set to FullRowSelect
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    index = dataGridView1.SelectedRows[0].Index + 1;
                }
                for (int i = index; i < (dataGridView1.Rows.Count + index); i++)
                {
                    if (dataGridView1.Rows[i % dataGridView1.Rows.Count].Cells["colID"].Value.ToString().StartsWith(e.KeyChar.ToString(), true, CultureInfo.InvariantCulture))
                    {
                        foreach (var row in dataGridView1.Rows.Cast<DataGridViewRow>().Where(t => t.Selected))
                        {
                            row.Selected = false;
                        }
                        dataGridView1.Rows[i % dataGridView1.Rows.Count].Cells[0].Selected = true;
                        return; // stop looping
                    }
                }
            }

        }
    }

    public class DatabaseInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int CompatibilityLevel { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Description { get; set; }

        public static List<DatabaseInfo> GetDatabasesFromServer(Server server)
        {
            var result = new List<DatabaseInfo>();
            using (var reader = server.ExecuteReader("<Statement>SELECT * FROM $SYSTEM.DBSCHEMA_CATALOGS</Statement>", out XmlaResultCollection results))
            {
                if(results != null && results.ContainsErrors)
                {
                    throw new Exception("Unable to query databases from server: " + results.GetMessageString());
                }
                while(reader.Read())
                {
                    
                    result.Add(new DatabaseInfo
                    {
                        ID = reader.GetString(reader.GetOrdinal("DATABASE_ID")),
                        Name = reader.GetString(reader.GetOrdinal("CATALOG_NAME")),
                        CompatibilityLevel = reader.GetInt32(reader.GetOrdinal("COMPATIBILITY_LEVEL")),
                        LastUpdate = reader.GetDateTime(reader.GetOrdinal("DATE_MODIFIED")),
                        Description = reader.GetString(reader.GetOrdinal("DESCRIPTION"))
                    });
                }
            }
            return result;
        }
    }

    static class XmlaExtension
    {
        public static string GetMessageString(this XmlaResultCollection xmlaResults)
        {
            return string.Join(Environment.NewLine,
                xmlaResults.OfType<XmlaResult>().SelectMany(r => r.Messages.OfType<XmlaMessage>()).Select(msg => msg.Description).ToArray());
        }
    }
}

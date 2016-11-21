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
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ConnectPage : UserControl
    {
        public event ValidationEventHandler Validation;

        public ConnectPage()
        {
            InitializeComponent();
        }

        public Server GetServer()
        {
            var result = new Server();
            try
            {
                result.Connect(GetConnectionString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not connect to server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return result;
        }

        private void ValidateUI(object sender, EventArgs e)
        {
            txtUsername.Enabled = rdbUsernamePassword.Checked;
            txtPassword.Enabled = rdbUsernamePassword.Checked;
            OnValidation();
        }

        private void OnValidation()
        {
            var valid = !string.IsNullOrEmpty(txtServer.Text)
                && (rdbIntegrated.Checked || !string.IsNullOrEmpty(txtUsername.Text));

            Validation?.Invoke(this, new ValidationEventArgs(valid));
        }

        public string ServerName { get { return txtServer.Text; } set { txtServer.Text = value; } }
        public bool IntegratedSecurity
        {
            get { return rdbIntegrated.Checked; }
            set
            {
                if (value) rdbIntegrated.Checked = true;
                else rdbUsernamePassword.Checked = true;
            } 
        }
        public string UserName { get { return txtUsername.Text; } set { txtUsername.Text = value; } }
        public string Password { get { return txtPassword.Text; } set { txtPassword.Text = value; } }

        public string GetConnectionString()
        {
            return IntegratedSecurity ? TabularConnection.GetConnectionString(ServerName)
                : TabularConnection.GetConnectionString(ServerName, UserName, Password);
        }
    }
}

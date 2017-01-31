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
using TabularEditor.UIServices;
using System.Drawing.Imaging;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ConnectPage : UserControl
    {
        public event ValidationEventHandler Validation;

        public ConnectPage()
        {
            InitializeComponent();
        }

        bool PowerBIInstancesLoaded = false;

        public void PopulateLocalInstances()
        {
            comboBox1.Items.Clear();
            comboBox1.DisplayMember = "Name";
            PowerBIHelper.Refresh();
            comboBox1.Items.AddRange(PowerBIHelper.Instances.OrderBy(i => i.Name).ToArray());
            PowerBIInstancesLoaded = true;
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
            var valid = (!string.IsNullOrWhiteSpace(txtServer.Text) || comboBox1.SelectedIndex >= 0)
                && (rdbIntegrated.Checked || !string.IsNullOrEmpty(txtUsername.Text));

            Validation?.Invoke(this, new ValidationEventArgs(valid));
        }

        public bool AllowLocalInstanceConnect
        {
            get { return comboBox1.Visible; }
            set
            {
                comboBox1.Visible = value;
                panel1.Top = value ? 54 : 26;
                MinimumSize = new Size(MinimumSize.Width, value ? 151 : 123);
            }
        }


        public string ServerName
        {
            get
            {
                var item = comboBox1.SelectedItem as PowerBIInstance;
                if (item != null) return "localhost:" + item.Port;
                return txtServer.Text;
            }
            set
            {
                txtServer.Text = value;
            }
        }

        public string LocalInstanceName
        {
            get
            {
                return GetLocalInstanceName(comboBox1.SelectedIndex);
            }
        }
        public EmbeddedInstanceType LocalInstanceType
        {
            get
            {
                return (comboBox1.SelectedItem as PowerBIInstance)?.Icon ?? EmbeddedInstanceType.None;
            }
        }

        private string GetLocalInstanceName(int index)
        {
            if(index >= 0)
            {
                var item = comboBox1.Items[index] as PowerBIInstance;
                if (item != null) return string.Format("{0}.{1}{2}",
                    "localhost:" + item.Port,
                    item.Name,
                    item.Icon == EmbeddedInstanceType.PowerBI ? ".pbix" : "");
            }
            return null;
        }

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

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                var item = comboBox1.Items[e.Index] as PowerBIInstance;

                e.Graphics.DrawImage(imageList1.Images[(int)item.Icon - 1], e.Bounds.Left + 2, e.Bounds.Top + 2);

                e.Graphics.DrawString(GetLocalInstanceName(e.Index), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + 24, e.Bounds.Top + 3);
            }
        }

        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            comboBox1.SelectedIndex = -1;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtServer.Text = "";
            ValidateUI(sender, e);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (!PowerBIInstancesLoaded) PopulateLocalInstances();
        }
    }
}

using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Server = Microsoft.AnalysisServices.Tabular.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs.Pages
{
    public interface IValidationPage
    {
        bool IsValid { get; }
        event ValidationEventHandler Validation;
    }

    public partial class ConnectPage : UserControl, IValidationPage
    {
        public class RecentServersObject
        {
            public List<string> RecentHistory = new List<string>();
            public string Recent;
        }

        public event ValidationEventHandler Validation;

        readonly string RecentServersFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\RecentServers.json";
        RecentServersObject recentServers = new RecentServersObject();

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

        public LocalInstance LocalInstance => comboBox1.SelectedItem as LocalInstance;

        public Server GetServer()
        {
            var result = new Server();
            try
            {
                var connectionString = GetConnectionString(resetAuth: true);
                result.Connect(connectionString);

                // SharePoint mode seems to be an alias for Power BI mode starting from an update of Power BI some time in 2017:
                if(result.ServerMode != Microsoft.AnalysisServices.ServerMode.Tabular && result.ServerMode != Microsoft.AnalysisServices.ServerMode.SharePoint)
                {
                    MessageBox.Show("Tabular Editor can only connect to Analysis Services instances running in Tabular mode.", "Unsupported instance", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return null;
                }
                if (!string.IsNullOrWhiteSpace(txtServer.Text))
                {
                    var serverName = txtServer.Text;
                    if (!recentServers.RecentHistory.Contains(serverName, StringComparer.InvariantCultureIgnoreCase))
                        recentServers.RecentHistory.Add(serverName);
                    recentServers.Recent = serverName;
                    var json = JsonConvert.SerializeObject(recentServers, Formatting.Indented);
                    (new FileInfo(RecentServersFilePath)).Directory.Create();
                    File.WriteAllText(RecentServersFilePath, json);
                }                
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
            Validation?.Invoke(this, new ValidationEventArgs(IsValid));
        }

        public bool IsValid => (!string.IsNullOrWhiteSpace(txtServer.Text) || comboBox1.SelectedIndex >= 0)
                && (rdbIntegrated.Checked || rdbEntraID.Checked || !string.IsNullOrEmpty(txtUsername.Text));

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
                var item = comboBox1.SelectedItem as LocalInstance;
                if (item != null) return "localhost:" + item.Port;
                return txtServer.Text;
            }
            set
            {
                txtServer.Text = value;
            }
        }

        private string GetLocalInstanceName(int index)
        {
            if(index >= 0)
            {
                var item = comboBox1.Items[index] as LocalInstance;
                if (item != null) return string.Format("{0}.{1}{2}",
                    "localhost:" + item.Port,
                    item.Name,
                    item.Type == LocalInstanceType.PowerBI ? ".pbix" : "");
            }
            return null;
        }

        public AuthenticationMode AuthenticationMode
        {
            get => rdbIntegrated.Checked ? AuthenticationMode.Integrated
                : rdbEntraID.Checked ? AuthenticationMode.MicrosoftEntraMFA
                : AuthenticationMode.Password;
            set
            {
                if (value == AuthenticationMode.Integrated) rdbIntegrated.Checked = true;
                else if (value == AuthenticationMode.MicrosoftEntraMFA) rdbEntraID.Checked = true;
                else rdbUsernamePassword.Checked = true;
            } 
        }
        public string UserName { get { return txtUsername.Text; } set { txtUsername.Text = value; } }
        public string Password { get { return txtPassword.Text; } set { txtPassword.Text = value; } }
        private string _databaseName;
        public string DatabaseName => _databaseName;
        public string GetConnectionString(bool resetAuth = false)
        {
            return AuthenticationMode switch
            {
                AuthenticationMode.Integrated =>
                    TabularConnection.GetConnectionString(ServerName, Program.ApplicationName, ProtocolFormat.Default, InteractiveLogin.Default, IdentityMode.Default, out _databaseName),
                AuthenticationMode.MicrosoftEntraMFA =>
                    TabularConnection.GetConnectionString(ServerName, Program.ApplicationName, ProtocolFormat.Default, resetAuth ? InteractiveLogin.Always : InteractiveLogin.Default, IdentityMode.Connection, out _databaseName),
                _ =>
                    TabularConnection.GetConnectionString(ServerName, UserName, Password, Program.ApplicationName, ProtocolFormat.Default, InteractiveLogin.Default, IdentityMode.Default, out _databaseName)
            };
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                var item = comboBox1.Items[e.Index] as LocalInstance;

                e.Graphics.DrawImage(imageList1.Images[(int)item.Type - 1], e.Bounds.Left + 2, e.Bounds.Top + 2);

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

        private void ConnectPage_Load(object sender, EventArgs e)
        {
            if (File.Exists(RecentServersFilePath))
            {
                try
                {
                    recentServers = JsonConvert.DeserializeObject<RecentServersObject>(File.ReadAllText(RecentServersFilePath));
                    txtServer.Items.AddRange(recentServers.RecentHistory.OrderBy(n => n).ToArray());
                    txtServer.Text = recentServers.Recent;
                    ValidateUI(null, null);
                }
                catch
                {
                    recentServers = new RecentServersObject();
                }
            }
        }

        private void txtServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtServer.Text = txtServer.SelectedItem.ToString();
            comboBox1.SelectedIndex = -1;
            ValidateUI(sender, e);
        }
    }

    public enum AuthenticationMode
    {
        Integrated = 0,
        MicrosoftEntraMFA = 1,
        Password = 2
    }
}

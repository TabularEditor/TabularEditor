using Microsoft.AnalysisServices.Tabular;
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
    public partial class DeployForm : Form
    {
        public List<Label> Steps = new List<Label>();
        public List<Control> Pages = new List<Control>();
        public List<string> Hints = new List<string>() {
            "Enter the server name of the SQL Server 2016 Analysis Services (Tabular) instance you want to deploy to. For Azure Analysis Services, use \"asazure://<servername>\".",
            "Choose the database you want to deploy the model to, or enter a new name to create a new database on the chosen server.",
            "Choose which elements you want to deploy. Omitted elements are kept as-is in the destination database.",
            "Review your selections. Deployment will be performed when you click the \"Deploy\" button."
        };

        private int _currentPage = 0;
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                if (value < 0) throw new ArgumentException();
                if (value >= Pages.Count) throw new ArgumentException();

                Pages[_currentPage].Visible = false;
                Steps[_currentPage].Font = new Font(lblStep1.Font, FontStyle.Regular);
                _currentPage = value;
                Steps[_currentPage].Font = new Font(lblStep1.Font, FontStyle.Bold);
                Pages[_currentPage].Visible = true;
                if(_currentPage == 1) Pages[_currentPage].Focus();
                lblHint.Text = Hints[_currentPage];

                btnPrev.Enabled = _currentPage > 0;
                btnNext.Enabled = _currentPage < Pages.Count - 1;
                btnDeploy.Enabled = _currentPage == Pages.Count - 1;
            }
        }

        public DeployForm()
        {
            InitializeComponent();

            Pages.Add(page1);
            Pages.Add(page2);
            Pages.Add(page3);
            Pages.Add(page4);

            Steps.Add(lblStep1);
            Steps.Add(lblStep2);
            Steps.Add(lblStep3);
            Steps.Add(lblStep4);
        }

        private void Page_Validation(object sender, ValidationEventArgs e)
        {
            btnNext.Enabled = e.IsValid;
        }

        public Server DeployTargetServer { get { return page2.Server; } set { page2.Server = value; } }
        public string DeployTargetDatabaseID { get { return page2.DatabaseID; } set { page2.DatabaseID = value; } }

        private void DeployForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnNext.Enabled && CurrentPage == 0) btnNext.PerformClick();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(_currentPage == 0)
            {
                using (new Hourglass())
                {
                    page2.Server = page1.GetServer();
                }
                if (page2.Server == null) return;
            }
            CurrentPage++;

            if(_currentPage == 2)
            {
                DeployOptions.DeployMode = DeployTargetServer.Databases.Contains(DeployTargetDatabaseID) ? DeploymentMode.CreateOrAlter : DeploymentMode.CreateDatabase;
                if(DeployOptions.DeployMode == DeploymentMode.CreateDatabase)
                {
                    chkDeployConnections.Checked = true;
                    chkDeployPartitions.Checked = true;
                }
                chkDeployPartitions.Enabled = chkDeployConnections.Enabled = DeployOptions.DeployMode == DeploymentMode.CreateOrAlter;
            }

            if (_currentPage == 3)
            {
                DeployOptions.DeployConnections = chkDeployConnections.Checked;
                DeployOptions.DeployPartitions = chkDeployPartitions.Checked;
                DeployOptions.DeployRoles = chkDeployRoles.Checked;
                DeployOptions.DeployRoleMembers = chkDeployRoleMembers.Checked;

                tvSummary.Nodes.Clear();
                var n = tvSummary.Nodes.Add("Summary");
                var n1 = n.Nodes.Add("Destination");
                n1.Nodes.Add(string.Format("Type: {0} ({1})", DeployTargetServer.ProductName, DeployTargetServer.Version));
                n1.Nodes.Add(string.Format("Server Name: {0}", DeployTargetServer.Name, DeployTargetServer.Version));
                n1.Nodes.Add(string.Format("Database: {0}", DeployTargetDatabaseID));
                n1.Nodes.Add(string.Format("Mode: {0}", DeployOptions.DeployMode == DeploymentMode.CreateDatabase ? "Create new database" : "Deploy to existing database"));
                var n2 = n.Nodes.Add("Options");
                if (chkDeployStructure.Checked) n2.Nodes.Add("Deploy Model Structure");
                if (chkDeployConnections.Checked) n2.Nodes.Add("Deploy Connections");
                if (chkDeployPartitions.Checked) n2.Nodes.Add("Deploy Partitions");
                if (chkDeployRoles.Checked)
                {
                    var n3 = n2.Nodes.Add("Deploy Roles and Permissions");
                    if (chkDeployStructure.Checked) n3.Nodes.Add("Deploy Role Members");
                }
                tvSummary.ExpandAll();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            CurrentPage--;
        }

        private void page2_Accept(object sender, EventArgs e)
        {
            if (btnNext.Enabled) btnNext.PerformClick();
        }

        private void deployPage1_Validation(object sender, ValidationEventArgs e)
        {

        }

        private void chkDeployRoles_CheckedChanged(object sender, EventArgs e)
        {
            chkDeployRoleMembers.Enabled = chkDeployRoles.Checked;
            if (!chkDeployRoleMembers.Enabled) chkDeployRoleMembers.Checked = false;
        }

        private void chkDeployStructure_CheckedChanged(object sender, EventArgs e)
        {
            chkDeployConnections.Enabled = chkDeployStructure.Checked && DeployOptions.DeployMode == DeploymentMode.CreateOrAlter;
            chkDeployPartitions.Enabled = chkDeployStructure.Checked && DeployOptions.DeployMode == DeploymentMode.CreateOrAlter;
            chkDeployRoles.Enabled = chkDeployStructure.Checked;
            chkDeployRoleMembers.Enabled = chkDeployStructure.Checked && chkDeployRoles.Checked;

            btnNext.Enabled = chkDeployStructure.Checked;
        }

        public DeploymentOptions DeployOptions { get; private set; } = new DeploymentOptions();

        private void btnDeploy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTMSL_Click(object sender, EventArgs e)
        {
            var tmslForm = new ClipForm();
            tmslForm.Text = "TMSL Script";
            using (new Hourglass())
            {
                tmslForm.txtCode.Text = TabularDeployer.GetTMSL(UIController.Current.Handler.Database,
                    DeployTargetServer, DeployTargetDatabaseID, DeployOptions);
            }
            tmslForm.ShowDialog();
        }
    }
}

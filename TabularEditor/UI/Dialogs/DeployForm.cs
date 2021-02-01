using TOM = Microsoft.AnalysisServices.Tabular;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.UI.Dialogs.Pages;

namespace TabularEditor.UI.Dialogs
{
    public partial class DeployForm : Form
    {
        TabularModelHandler Handler { get { return UIController.Current.Handler; } }

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
                if (_currentPage == 1)
                {
                    Pages[1].Focus();
                    (Pages[1] as DatabasePage).EnableEvents();
                }
                else
                {
                    (Pages[1] as DatabasePage).DisableEvents();
                }
                lblHint.Text = Hints[_currentPage];

                btnPrev.Enabled = _currentPage > 0;
                btnNext.Enabled = (Pages[_currentPage] as IValidationPage)?.IsValid ?? _currentPage < Pages.Count - 1;
                btnDeploy.Enabled = _currentPage == Pages.Count - 1;
            }
        }

        public string PreselectDb;

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

        public TOM.Server DeployTargetServer { get { return page2.Server; } set { page2.Server = value; } }
        public string DeployTargetDatabaseName => page2.DatabaseName;

        private void DeployForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnNext.Enabled && CurrentPage == 0) btnNext.PerformClick();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage == 0)
            {
                using (new Hourglass())
                {
                    page2.ClearSelection = true;
                    page2.PreselectDb = PreselectDb;
                    page2.Server = page1.GetServer();
                }
                if (page2.Server == null) return;
            }
            CurrentPage++;
            if(_currentPage == 1)
            {
                if (PreselectDb == null) page2.DoClearSelection();
            }

            if (_currentPage == 2)
            {
                DeployOptions.DeployMode = DeployTargetServer.Databases.ContainsName(DeployTargetDatabaseName) ? DeploymentMode.CreateOrAlter : DeploymentMode.CreateDatabase;
                if (DeployOptions.DeployMode == DeploymentMode.CreateDatabase)
                {
                    chkDeployConnections.Checked = true;
                    chkDeployPartitions.Checked = true;
                }
                else if (DeployOptions.DeployMode == DeploymentMode.CreateOrAlter)
                {
                    DeployOptions = page2.DatabaseName == PreselectDb ? (DeployOptions ?? new DeploymentOptions()) : new DeploymentOptions();
                    
                    chkDeployConnections.Checked = DeployOptions.DeployConnections;
                    chkDeployPartitions.Checked = DeployOptions.DeployPartitions;
                    chkDeployRoles.Checked = DeployOptions.DeployRoles;
                    chkDeployRoleMembers.Checked = DeployOptions.DeployRoleMembers;
                    
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
                n1.Nodes.Add(string.Format("Database: {0}", DeployTargetDatabaseName));
                n1.Nodes.Add(string.Format("Mode: {0}", DeployOptions.DeployMode == DeploymentMode.CreateDatabase ? "Create new database" : "Deploy to existing database"));
                var n2 = n.Nodes.Add("Options");
                if (chkDeployStructure.Checked) n2.Nodes.Add("Deploy Model Structure");
                if (chkDeployConnections.Checked) n2.Nodes.Add("Deploy Connections");
                if (chkDeployPartitions.Checked) n2.Nodes.Add("Deploy Partitions");
                if (chkDeployRoles.Checked)
                {
                    var n3 = n2.Nodes.Add("Deploy Roles and Permissions");
                    if (chkDeployRoleMembers.Checked) n3.Nodes.Add("Deploy Role Members");
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

        public DeploymentOptions DeployOptions { get; set; } = new DeploymentOptions();

        private void btnDeploy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            PreselectDb = page2.DatabaseName;
            Close();
        }

        private void btnTMSL_Click(object sender, EventArgs e)
        {
            var tmslForm = new ClipForm();
            tmslForm.Text = "TMSL Script";
            using (new Hourglass())
            {
                tmslForm.txtCode.Text = TabularDeployer.GetTMSL(Handler.Database,
                    DeployTargetServer, DeployTargetDatabaseName, DeployOptions);
            }
            tmslForm.ShowDialog();
        }

        public const string ExportBuild_DatabaseFile = "Model.asdatabase";
        public const string ExportBuild_OptionsFile = "Model.deploymentoptions";
        public const string ExportBuild_TargetsFile = "Model.deploymenttargets";
        public const string ExportBuild_ConfigsFile = "Model.configsettings";

        private void btnExportBuild_Click(object sender, EventArgs e)
        {
            ExportBuild();
        }
        public void ExportBuild()
        {
            using (var dlg = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = Strings.ExportBuildChooseFolderCaption
            })
            {
                if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
                ExportBuild(dlg.FileName);
            }
        }

        public void ExportBuild(string folder)
        {
            Directory.CreateDirectory(folder);

            var databaseFile = folder.ConcatPath(ExportBuild_DatabaseFile);
            var optionsFile = folder.ConcatPath(ExportBuild_OptionsFile);
            var targetsFile = folder.ConcatPath(ExportBuild_TargetsFile);
            var configsFile = folder.ConcatPath(ExportBuild_ConfigsFile);

            if (File.Exists(databaseFile) || File.Exists(optionsFile) || File.Exists(targetsFile) || File.Exists(configsFile))
            {
                var dr = MessageBox.Show(
                    Strings.ExportBuildConfirmOverwrite,
                    Strings.ExportBuildConfirmOverwriteCaption,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );
                if (dr != DialogResult.OK) return;
            }

            ExportBuild_Database(databaseFile);
            ExportBuild_Options(optionsFile);
            ExportBuild_Targets(targetsFile);
            if(DeployOptions.DeployConnections) ExportBuild_Configs(configsFile);

            var allFiles = string.Format("{0}\n{1}\n{2}{3}", databaseFile, optionsFile, targetsFile, DeployOptions.DeployConnections ? "\n" + configsFile : "");

            MessageBox.Show(string.Format(Strings.ExportBuildSuccess,allFiles), Strings.ExportBuildSuccessCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        

        private void ExportBuild_Database(string databaseFile)
        {
            Handler.Save(databaseFile, SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
        }
        private void ExportBuild_Options(string optionsFile)
        {
            var transactionalDeployment = "true";
            var partitionDeployment = DeployOptions.DeployPartitions ? "DeployPartitions" : "RetainPartitions";
            var roleDeployment = DeployOptions.DeployRoles ? (DeployOptions.DeployRoleMembers ? "DeployRolesAndMembers" : "DeployRolesRetainMembers") : "RetainRoles";
            var processingOptions = "DoNotProcess";
            var configurationSettingsDeployment = DeployOptions.DeployConnections ? "Deploy" : "Retain";

            var options =
                string.Format(Strings.ExportBuildDeploymentOptions
                    , transactionalDeployment
                    , partitionDeployment
                    , roleDeployment
                    , processingOptions
                    , configurationSettingsDeployment
                );

            File.WriteAllText(optionsFile, options);
        }
        private void ExportBuild_Targets(string targetsFile)
        {
            var targets =
                string.Format(Strings.ExportBuildDeploymentTargets
                    , DeployTargetDatabaseName
                    , DeployTargetServer.Name);

            File.WriteAllText(targetsFile, targets);
        }

        private void ExportBuild_Configs(string configsFile)
        {
            string dataSources = "";
            foreach(var ds in Handler.Model.DataSources.OfType<TOMWrapper.ProviderDataSource>())
            {
                dataSources += string.Format(Strings.ExportBuildConfigDataSource
                    , ds.Name
                    , ds.ConnectionString
                    , ds.ImpersonationMode.ToString()
                    , ds.Account
                    , ds.Password);
            }

            string config = string.Format(Strings.ExportBuildConfig
                    , dataSources);

            File.WriteAllText(configsFile, config);
        }
    }
}

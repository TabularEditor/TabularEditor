using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        private const string NEW_DEPLOYMENT_TARGET = "<New deployment target...>";

        public void Database_Deploy()
        {
            ExpressionEditor_AcceptEdit();

            var f = new DeployForm();
            f.PreselectDb = LastDeploymentDb;
            if (LastDeployOptions != null) f.DeployOptions = LastDeployOptions;

            var res = f.ShowDialog();
            if (res == DialogResult.Cancel) return;

            LastDeployOptions = f.DeployOptions.Clone();
            LastDeploymentDb = f.PreselectDb;

            // Backup database metadata
            if (Preferences.Current.BackupOnSave)
            {
                var backupFilename = string.Format("{0}\\Backup_{1}_{2}.zip", Preferences.Current.BackupLocation, Handler.Database.Name, DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                TabularDeployer.SaveModelMetadataBackup(f.DeployTargetServer.ConnectionString, f.DeployTargetDatabaseID, backupFilename);
            }


            UI.StatusLabel.Text = "Deploying...";
            Application.DoEvents();
            using (new Hourglass())
            {
                bool cancelled = false;
                bool error = false;
                string message = "";

                using (var df = new DeployingForm())
                {
                    df.DeployAction = () =>
                    {
                        try
                        {
                            Program.UpdateDeploymentMetadata(Handler.Model, DeploymentModeMetadata.WizardUI);
                            TabularDeployer.Deploy(Handler, f.DeployTargetServer.ConnectionString, f.DeployTargetDatabaseID, f.DeployOptions, df.CancelToken);
                        }
                        catch (Exception ex)
                        {
                            cancelled = df.CancelToken.IsCancellationRequested;
                            error = !cancelled;
                            message = ex.Message;
                            df.ThreadClose();
                        }
                    };
                    df.ShowDialog();
                }

                if (error || cancelled)
                {
                    MessageBox.Show(message, error ? "Error occured during deployment" : "Deploy cancelled", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UI.StatusLabel.Text = string.Format(error ? "{0}: Deployment failed!" : cancelled ? "{0}: Deployment cancelled!" : "{0}: Deployment succeeded!", f.DeployTargetServer.Name);
            }
        }

        public string LocalInstanceName { get; private set; }
        public EmbeddedInstanceType LocalInstanceType { get; private set; }

        public void Database_Connect()
        {
            if (DiscardChangesCheck()) return;

            if (ConnectForm.Show() == DialogResult.Cancel) return;

            switch(ConnectForm.LocalInstanceType)
            {
                case EmbeddedInstanceType.None:
                    if (SelectDatabaseForm.Show(ConnectForm.Server) == DialogResult.Cancel) return;
                    break;
                case EmbeddedInstanceType.PowerBI:
                    MessageBox.Show("You are connecting to a Tabular model in Power BI Desktop.\n\nTabular Editor uses the TOM to make changes to the model, which is UNSUPPORTED and could corrupt your .pbix file.\n\nPower BI Desktop will not synchronize the user interface with the changes you apply to the model until you manually refresh the model in Power BI Desktop.\n\nMake sure to keep a backup of the .pbix file, and proceed at your own risk.", "Connecting to embedded model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case EmbeddedInstanceType.Devenv:
                    MessageBox.Show("You are connecting to an integrated workspace in Visual Studio.\n\nChanges made through Tabular Editor may not be properly persisted to the Tabular Project in Visual Studio and may corrupt your model file.\n\nMake sure to keep a backup of the Model.bim file and proceed at your own risk.", "Connecting to embedded model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }

            UI.StatusLabel.Text = "Opening Model from Database...";
            ClearUI();
            UpdateUIText();

            Database_Open(ConnectForm.ConnectionString,
                string.IsNullOrEmpty(ConnectForm.LocalInstanceName) ? SelectDatabaseForm.DatabaseID : null);
        }

        private System.Windows.Forms.Timer KeepAliveTimer;

        public void Database_Open(string connectionString, string databaseId)
        {
            using (new Hourglass())
            {

                var oldHandler = Handler;

                try
                {
                    Handler = new TabularModelHandler(connectionString, databaseId);
                    File_Current = null;
                    File_Directory = null;
                    File_SaveMode = ModelSourceType.Database;
                    LoadTabularModelToUI();
                    LocalInstanceName = ConnectForm.LocalInstanceName;
                    LocalInstanceType = ConnectForm.LocalInstanceType;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error connecting to database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Handler = oldHandler;
                    LoadTabularModelToUI();
                }

            }
        }

        private void Database_Save()
        {
            UI.ErrorLabel.Text = "";

            try
            {
                var conflictInfo = Handler.CheckConflicts();
                if (conflictInfo.Conflict)
                {
                    var res = MessageBox.Show(string.Format("Changes have been made to the deployed version of the model, since it was loaded into Tabular Editor.\n\nDeployed model version: {0} (changed {1})\nCurrent model version: {2}\n\nDo you want to overwrite the deployed model?",
                        conflictInfo.DatabaseVersion, conflictInfo.DatabaseLastUpdate, conflictInfo.LoadedVersion),
                        "Change conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.No) return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nSave the model as a file to make sure you do not lose any work. In case the database connection was lost, you can still apply your changes to the database by using the \"Model > Deploy\" option.", "Could not save metadata changes to database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (new Hourglass())
            {
                UI.StatusLabel.Text = "Saving changes to DB...";
                Application.DoEvents();

                if (Preferences.Current.BackupOnSave)
                {
                    var backupFilename = string.Format("{0}\\Backup_{1}_{2}.zip", Preferences.Current.BackupLocation, Handler.Database.Name, DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                    try
                    {
                        TabularDeployer.SaveModelMetadataBackup(Handler.Database.Server.ConnectionString, Handler.Database.ID, backupFilename);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Unable to save metadata backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                try
                {
                    Program.UpdateDeploymentMetadata(Handler.Model, DeploymentModeMetadata.SaveUI);
                    Handler.SaveDB();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n\nSave the model as a file to make sure you do not lose any work. In case the database connection was lost, you can still apply your changes to the database by using the \"Model > Deploy\" option.", "Could not save metadata changes to database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UI.TreeView.Refresh();

            }
        }
    }
}

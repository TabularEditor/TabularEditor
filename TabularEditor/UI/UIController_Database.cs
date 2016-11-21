using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Dialogs;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        private const string NEW_DEPLOYMENT_TARGET = "<New deployment target...>";

        public void Database_Deploy()
        {
            var f = new DeployForm();
            var res = f.ShowDialog();
            if (res == DialogResult.Cancel) return;


            UI.FormMain.Cursor = Cursors.WaitCursor;
            UI.StatusLabel.Text = "Deploying...";
            Application.DoEvents();
            var df = new DeployingForm();
            df.Cursor = Cursors.WaitCursor;
            var error = false;
            df.DeployAction = () =>
            {
                try
                {
                    TabularDeployer.Deploy(Handler.Database, f.DeployTargetServer.ConnectionString, f.DeployTargetDatabaseID, f.DeployOptions);
                }
                catch (Exception ex)
                {
                    error = true;
                    df.ThreadClose();
                    MessageBox.Show(ex.Message, "Error occured during deployment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            df.ShowDialog();
            UI.StatusLabel.Text = error ? "Deploy failed!" : "Deploy succeeded!";
            UI.FormMain.Cursor = Cursors.Default;
        }

        public void Database_Connect()
        {
            if (DiscardChangesCheck()) return;

            if (ConnectForm.Show() == DialogResult.Cancel) return;
            if (SelectDatabaseForm.Show(ConnectForm.Server) == DialogResult.Cancel) return;

            var oldHandler = Handler;

            try
            {
                Handler = new TabularModelHandler(ConnectForm.ConnectionString, SelectDatabaseForm.DatabaseName);
                LoadTabularModelToUI();
                File_Current = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error connecting to database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Handler = oldHandler;
            }
        }

        private void Database_Save()
        {
            UI.ErrorLabel.Text = "";

            ExpressionEditor_AcceptEdit();

            var conflictInfo = Handler.CheckConflicts();
            if (conflictInfo.Conflict)
            {
                var res = MessageBox.Show(string.Format("Changes have been made to the deployed version of the model, since it was loaded into Tabular Editor.\n\nDeployed model version: {0} (changed {1})\nCurrent model version: {2}\n\nDo you want to overwrite the deployed model?", 
                    conflictInfo.DatabaseVersion, conflictInfo.DatabaseLastUpdate, conflictInfo.LoadedVersion),
                    "Change conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No) return;
            }

            UI.FormMain.Cursor = Cursors.WaitCursor;
            UI.StatusLabel.Text = "Saving changes to DB...";
            Application.DoEvents();

            Handler.WriteZip(string.Format("{0}\\DeployedFiles\\deploy_{1}.zip", Application.UserAppDataPath, DateTime.Now.ToString("yyyyMMddhhmmss")));
            Handler.SaveDB();

            UI.TreeView.Refresh();

            UI.FormMain.Cursor = Cursors.Default;
        }
    }
}

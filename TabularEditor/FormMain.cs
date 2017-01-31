using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using TabularEditor.UI;
using TabularEditor.UI.Extensions;
using TabularEditor.UI.Actions;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor
{
    public partial class FormMain : Form
    {
        private UIController UI;

        public FormMain()
        {
            InitializeComponent();
            SetupUIController();
            txtFilter.Control.SetCueBanner("Filter");

            ///// Populate custom actions and samples /////
            // TODO: Do this somewhere else
            if (File.Exists(ScriptEngine.CustomActionsJsonPath))
            {
                custActions = CustomActionsJson.LoadFromJson(ScriptEngine.CustomActionsJsonPath);
                foreach (var act in custActions.Actions)
                {
                    customActionsToolStripMenuItem.DropDownItems.Add(act.Name, null, (s, e) => { txtAdvanced.Text = act.Execute; });
                }
            }
            if (custActions == null || custActions.Actions.Length == 0)
            {
                var item = customActionsToolStripMenuItem.DropDownItems.Add("(No custom actions)");
                item.Enabled = false;
            }

            samplesMenu.DropDownItems.Add("Copy display folder to active translation", null, (s, e) =>
            {
                txtAdvanced.Text =
@"Selected.Measures.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);
Selected.Columns.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);
Selected.Hierarchies.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);";
            });

            samplesMenu.DropDownItems.Add("Copy display folder to all translations", null, (s, e) =>
            {
                txtAdvanced.Text =
@"Selected.Measures.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));
Selected.Columns.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));
Selected.Hierarchies.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));";
            });
        }



        CustomActionsJson custActions = null;

        /// <summary>
        /// Instantiates the UIController, which will handle advanced UI interactions and host
        /// the TabularModelHandler from the TOMWrapper library.
        /// </summary>
        public void SetupUIController()
        {
            var elements = new UIElements()
            {
                FormMain = this,
                TreeView = tvModel,
                PropertyGrid = propertyGrid1,
                ExpressionEditor = txtExpression,
                ScriptEditor = txtAdvanced,
                ErrorLabel = lblErrors,
                StatusLabel = lblStatus,
                StatusExLabel = lblScriptStatus,
                OpenBimDialog = openBimFile,
                SaveBimDialog = saveBimFile,
                TreeImages = tabularTreeImages,
                CurrentMeasureLabel = lblCurrentMeasure,
                ModelMenu = modelToolStripMenuItem,
                PerspectiveSelector = cmbPerspective,
                TranslationSelector = cmbTranslation,
                ToolsMenu = toolsToolStripMenuItem
            };

            // The UIController class sets up all bindings and event handlers needed for UI
            // interaction, once we pass it a reference to the main UI elements. This is not
            // really a "loose coupling" between UI and model code, but it helps to keep the
            // code nicely separated.
            UI = new UIController(elements);
        }

        // TODO: Handle below as actions
        private void actOpenFile_Execute(object sender, EventArgs e)
        {
            UI.File_Open();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            UI.Database_Connect();
        }

        public const string STR_DEFAULT_TRANSLATION = "(No translation)";
        public const string STR_DEFAULT_PERSPECTIVE = "(Model)";

        private void lblErrors_Click(object sender, EventArgs e)
        {
            if (UI.Handler?.Errors == null) return;
            var c = UI.Handler.Errors.Count;
            if (c == 0) return;
            // TODO: Show error information
            MessageBox.Show((c > 25 ? c + " errors in total. Only 25 first errors shown:\n\n" : "") + string.Join("\n", UI.Handler.Errors.Select(t => t.Item1.Name + ": " + t.Item2).Take(25)), "Error messages returned from server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(UI.Handler != null && UI.Handler.HasUnsavedChanges)
            {
                if(UI.Handler.IsConnected)
                {
                    // Handle undeployed changes to connected model:
                    var res = MessageBox.Show("You have made changes to the currently connected model, which have not yet been saved. Are you sure you want to quit?", "Unsaved changes in the model", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel) e.Cancel = true;
                } else
                {
                    // Handle unsaved changes to file:
                    var res = MessageBox.Show("You have made changes to the currently loaded Model.bim file, which have not yet been saved. Are you sure you want to quit?", "Unsaved changes in the model", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel) e.Cancel = true;
                }
            }
        }

        private void actViewOptions_Execute(object sender, EventArgs e)
        {
            UI.SetDisplayOptions(
                actToggleHidden.Checked,
                actToggleDisplayFolders.Checked,
                actToggleColumns.Checked,
                actToggleMeasures.Checked,
                actToggleHierarchies.Checked,
                actToggleAllObjectTypes.Checked,
                actToggleFilter.Checked ? txtFilter.Text : null
            );
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            actToggleFilter.DoExecute();        
        }

        private void actExit_Execute(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void actCollapseExpand_Execute(object sender, EventArgs e)
        {
            if (sender == actCollapseAll) tvModel.CollapseAll();
            if (sender == actExpandAll) tvModel.ExpandAll();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Auto-load the file specified as command line arguments:
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && File.Exists(args[1]))
            {
                UI.File_Open(args[1]);
            }
        }

        private void actUndoRedo_Execute(object sender, EventArgs e)
        {
            if(!txtExpression.Focused) UI.ExpressionEditor_CancelEdit();
            propertyGrid1.Refresh();
            tvModel.Refresh();
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                actToggleFilter.Checked = true;
                actToggleFilter.DoExecute();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void actExpressionAcceptEdit_Execute(object sender, EventArgs e)
        {
            UI.ExpressionEditor_AcceptEdit();
        }

        private void actExpressionCancelEdit_Execute(object sender, EventArgs e)
        {
            UI.ExpressionEditor_CancelEdit();
        }

        private void actExpressionAcceptEdit_UpdateEx(object sender, UpdateExEventArgs e)
        {
            e.Enabled = UI.ExpressionEditor_IsDirty;
        }

        private void actExpressionCancelEdit_UpdateEx(object sender, UpdateExEventArgs e)
        {
            e.Enabled = UI.ExpressionEditor_IsEditing;
        }

        private void lblCurrentMeasure_Paint(object sender, PaintEventArgs e)
        {
            var c = Color.FromArgb(100, 100, 100);
            var s = ButtonBorderStyle.Solid;
            ControlPaint.DrawBorder(e.Graphics, lblCurrentMeasure.DisplayRectangle, c, 1, s, c, 1, s, c, 1, s, c, 0, ButtonBorderStyle.None);
        }

        private void actExpression_UpdateEx(object sender, UpdateExEventArgs e)
        {
            e.Enabled = UI.ExpressionEditor_Current != null;
        }

        private void actFind_Execute(object sender, EventArgs e)
        {
            txtExpression.ShowFindDialog();
        }

        private void actReplace_Execute(object sender, EventArgs e)
        {
            txtExpression.ShowReplaceDialog();
        }

        private void actExpressionFormatDAX_Execute(object sender, EventArgs e)
        {
            var textToFormat = "x :=" + txtExpression.Text;
            var result = TabularEditor.Dax.DaxFormatterProxy.FormatDax(textToFormat).FormattedDax;
            if (string.IsNullOrWhiteSpace(result))
            {
                lblStatus.Text = "Could not format DAX.";
                return;
            }
            lblStatus.Text = "DAX formatted succesfully";
            txtExpression.Text = result.Substring(6);
        }

        private void actToggleInfoColumns_Execute(object sender, EventArgs e)
        {
            UI.SetInfoColumns(actToggleInfoColumns.Checked);
        }

        private void actToggleFilter_UpdateEx(object sender, UpdateExEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilter.Text))
            {
                actToggleFilter.Checked = false;
                e.Enabled = false;
            }
            else
            {
                e.Enabled = true;
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            if (UI.Handler == null) return;
            var f = new ClipForm();
            f.txtCode.Text = UI.Handler.UndoManager.GetHistory();
            f.ShowDialog();
        }

        private void actExecuteScript_Execute(object sender, EventArgs e)
        {
            UI.ScriptEditor_ExecuteScript(btnUndoErrors.Checked);
        }

        public void FocusFilter()
        {
            txtFilter.Focus();
        }

        private void actSave_Execute(object sender, EventArgs e)
        {
            UI.Save();
        }

        private void actSaveAs_Execute(object sender, EventArgs e)
        {
            UI.File_SaveAs();
        }

        private void actDeploy_Execute(object sender, EventArgs e)
        {
            UI.Database_Deploy();
        }

        private void SaveCustomAction()
        {
            // TODO: Move this somewhere else
            var form = new SaveCustomActionForm();
            var res = form.ShowDialog();

            if(res == DialogResult.OK)
            {
                var act = new CustomActionJson();
                act.Name = form.txtName.Text;
                act.Tooltip = form.txtTooltip.Text;
                act.Execute = txtAdvanced.Text;
                act.Enabled = "true";
                if (custActions == null) custActions = new CustomActionsJson() { Actions = new CustomActionJson[0] };
                var list = custActions.Actions.ToList();
                list.Add(act);

                custActions.Actions = list.ToArray();
                custActions.SaveToJson(ScriptEngine.CustomActionsJsonPath);
            }
        }

        private void actSaveCustomAction_Execute(object sender, EventArgs e)
        {
            SaveCustomAction();
        }

        private void actSaveCustomAction_Update(object sender, EventArgs e)
        {
            actSaveCustomAction.Enabled = !string.IsNullOrEmpty(txtAdvanced.Text);
        }

        private void actSave_UpdateEx(object sender, UpdateExEventArgs e)
        {
            if (UI.Handler == null) return;
            actSave.Text = UI.Handler.IsConnected ? "Save DB" : "Save Model.bim";
            actSave.ToolTipText = UI.Handler.IsConnected ? "Saves the changes to the connected database" : "Saves the changes back to the Model.bim file";
        }

        private void tvModel_Click(object sender, EventArgs e)
        {

        }

        PreferencesForm PreferencesForm = new PreferencesForm();


        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesForm.ShowDialog();
            UI.Handler.AutoFixup = Preferences.Current.FormulaFixup;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            // Various checks to be performed when the main form is shown

            if(!Preferences.Current.IsLoaded)
            {
                // If IsLoaded is false, it means that this is the first time Tabular Editor is started (or that the Preferences.json file has been deleted)
                // In this case, let's ask the user if he wants to enable automatic updates.

                var res = MessageBox.Show("Do you want Tabular Editor to automatically check GitHub for updates on every startup?\n\nThis can be changed under File > Preferences.", "Enable automatic update check?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                Preferences.Current.CheckForUpdates = res == DialogResult.Yes;
                Preferences.Current.Save();
            }

            if(Preferences.Current.CheckForUpdates)
            {
                if(UpdateService.Check(false) ?? false)
                {
                    var res = MessageBox.Show("A new version of Tabular Editor is available. Would you like to open the download page now?\n\nYou can disable this check under File > Preferences.", "Tabular Editor update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes) UpdateService.OpenDownloadPage();
                }
            }
        }

        private void saveToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.File_SaveToFolder();
        }
    }
}

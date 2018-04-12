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
using System.Collections.Generic;
using System.Threading;

namespace TabularEditor
{
    public partial class FormMain : Form
    {
        private UIController UI;

        private string CurrentCustomAction;

        public FormMain()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();

            dlgOpenFile.Filter = "Tabular Model Files|*.bim;database.json;*.pbit|All files|*.*";
            dlgSaveFile.Filter = "Tabular Model Files|*.bim|All files|*.*";

            // For some reason, Visual Studio sometimes removes this from the FormMain.Designer.cs, making the
            // colors of the lines look ugly:
            propertyGrid1.LineColor = System.Drawing.SystemColors.InactiveBorder;

            // Assign our own custom Designer, to make sure we can handle property changes on multiple objects simultaneously:
            propertyGrid1.Site = new DesignerHost();

            // "Select Namespace" button should only be visible if we have loaded any plug-ins:
            toolStripButton3.Visible = ScriptEngine.PluginNamespaces.Count > 0;

            SetupUIController();
            txtFilter.Control.SetCueBanner("Filter");

            ///// Populate custom actions and samples /////
            // TODO: Do this somewhere else
            PopulateCustomActionsDropDown();

            var tutorial = (samplesMenu.DropDownItems.Add("Tutorials") as ToolStripMenuItem).DropDownItems;
            var translations = (samplesMenu.DropDownItems.Add("Translations") as ToolStripMenuItem).DropDownItems;

            tutorial.Add("Loop through all selected columns", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"foreach(var column in Selected.Columns) {
    // column.DisplayFolder = ""Test"";
}");
            });
            tutorial.Add("Loop through all selected tables", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"foreach(var table in Selected.Tables) {
    // table.IsHidden = false;
}");
            });
            tutorial.Add("Loop through columns on all selected tables", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"foreach(var column in Selected.Tables.SelectMany(t => t.Columns)) {
    // column.DisplayFolder = ""test"";
}");
            });
            tutorial.Add("Loop through columns on all tables conditionally", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"foreach(var column in Model.Tables.SelectMany(t => t.Columns)) {
    if(column.Name.EndsWith(""Key"")) {
        // column.IsHidden = true;
    }
}");
            });
            translations.Add("Copy display folder to active translation", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"Selected.Measures.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);
Selected.Columns.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);
Selected.Hierarchies.ForEach(item => item.TranslatedDisplayFolders[Selected.Culture] = item.DisplayFolder);");
            });

            translations.Add("Copy display folder to all translations", null, (s, e) =>
            {
                txtAdvanced.InsertText(
@"Selected.Measures.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));
Selected.Columns.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));
Selected.Hierarchies.ForEach(item => item.TranslatedDisplayFolders.SetAll(item.DisplayFolder));");
            });
        }


        private void PopulateCustomActionsDropDown()
        {
            customActionsToolStripMenuItem.DropDownItems.Clear();
            if (File.Exists(ScriptEngine.CustomActionsJsonPath))
            {
                custActions = CustomActionsJson.LoadFromJson(ScriptEngine.CustomActionsJsonPath);
                foreach (var act in custActions.Actions)
                {
                    customActionsToolStripMenuItem.DropDownItems.Add(act.Name, null, (s, e) =>
                    {
                        CurrentCustomAction = act.Name;
                        txtAdvanced.Text = act.Execute;
                    });
                }
            }

            if (custActions == null || custActions.Actions.Length == 0)
            {
                var item = customActionsToolStripMenuItem.DropDownItems.Add("(No custom actions)");
                item.Enabled = false;
            }
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
                OpenBimDialog = dlgOpenFile,
                SaveBimDialog = dlgSaveFile,
                TreeImages = tabularTreeImages,
                CurrentMeasureLabel = lblCurrentMeasure,
                ModelMenu = modelToolStripMenuItem,
                PerspectiveSelector = cmbPerspective,
                TranslationSelector = cmbTranslation,
                ToolsMenu = toolsToolStripMenuItem,
                DynamicMenu = dynamicToolStripMenuItem,
                ExpressionSelector = cmbExpressionSelector
            };

            // The UIController class sets up all bindings and event handlers needed for UI
            // interaction, once we pass it a reference to the main UI elements. This is not
            // really a "loose coupling" between UI and model code, but it helps to keep the
            // code nicely separated.
            UI = new UIController(elements);
            UI.ModelLoaded += UI_ModelLoaded;

            actToggleInfoColumns.Checked = Preferences.Current.View_MetadataInformation;
            UI.SetInfoColumns(actToggleInfoColumns.Checked);
            actToggleDisplayFolders.Checked = Preferences.Current.View_DisplayFolders;
            actToggleHidden.Checked = Preferences.Current.View_HiddenObjects;
            actToggleMeasures.Checked = Preferences.Current.View_Measures;
            actToggleColumns.Checked = Preferences.Current.View_Columns;
            actToggleHierarchies.Checked = Preferences.Current.View_Hierarchies;
            actToggleAllObjectTypes.Checked = Preferences.Current.View_AllObjectTypes;
            actToggleMetadataOrder.Checked = !Preferences.Current.View_SortAlphabetically;
            actViewOptions_Execute(this, null);
        }

        private void UI_ModelLoaded(object sender, EventArgs e)
        {
            if (BPAForm != null && BPAForm.Visible) BPAForm.Hide();
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
            TabularEditor.Scripting.ScriptHelper.OutputErrors(UI.Handler.Errors.OfType<TOMWrapper.TabularNamedObject>());
            // TODO: Show error information
            //MessageBox.Show((c > 25 ? c + " errors in total. Only 25 first errors shown:\n\n" : "") + string.Join("\n", UI.Handler.Errors.Select(t => t.Item1.Name + ": " + t.Item2).Take(25)), "Error messages returned from server", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            RecentFiles.Save();
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
                actToggleMetadataOrder.Checked,
                actToggleFilter.Checked ? txtFilter.Text : null
            );

            Preferences.Current.View_HiddenObjects = actToggleHidden.Checked;
            Preferences.Current.View_DisplayFolders = actToggleDisplayFolders.Checked;
            Preferences.Current.View_Columns = actToggleColumns.Checked;
            Preferences.Current.View_Measures = actToggleMeasures.Checked;
            Preferences.Current.View_Hierarchies = actToggleHierarchies.Checked;
            Preferences.Current.View_AllObjectTypes = actToggleAllObjectTypes.Checked;
            Preferences.Current.View_SortAlphabetically = !actToggleMetadataOrder.Checked;
            Preferences.Current.Save();
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
            if (args.Length == 2 && (File.Exists(args[1]) || File.Exists(args[1] + "\\database.json")))
            {
                UI.File_Open(args[1]);
            }

            // Populate list of recent files...
            PopulateRecentFilesList();
        }

        public void PopulateRecentFilesList()
        {
            recentFilesToolStripMenuItem.DropDown.Items.Clear();

            if (RecentFiles.Current.RecentHistory == null || RecentFiles.Current.RecentHistory.Count == 0)
            {
                recentFilesToolStripMenuItem.Enabled = false;
            }
            else
            {
                recentFilesToolStripMenuItem.DropDown.Items.AddRange(
                    (RecentFiles.Current.RecentHistory as IEnumerable<string>).Reverse().Take(10).Select(f =>
                        new ToolStripMenuItem(f, null, (s, ev) => {
                            if (UI.DiscardChangesCheck()) return;
                            if(File.Exists(f) || Directory.Exists(f))
                            {
                                UI.File_Open(f);
                            } else
                            {
                                MessageBox.Show("This file seems to have been moved or deleted.", "Could not open file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                var tsi = s as ToolStripMenuItem;
                                tsi.GetCurrentParent().Items.Remove(tsi);
                                RecentFiles.Current.RecentHistory.Remove(f);
                                RecentFiles.Save();
                                if (RecentFiles.Current.RecentHistory.Count == 0) recentFilesToolStripMenuItem.Enabled = false;
                            }
                        })).ToArray());
            }
        }

        private void actUndoRedo_Execute(object sender, EventArgs e)
        {
            if(!txtExpression.Focused) UI.ExpressionEditor_CancelEdit();
            //propertyGrid1.Refresh();
            //tvModel.Refresh();
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
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    txtExpression.ShowFindDialog();
                    break;
                case 1:
                    txtAdvanced.ShowFindDialog();
                    break;
            }
        }

        private void actReplace_Execute(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex) {
                case 0:
                    UI.ExpressionEditor_BeginEdit();
                    txtExpression.ShowReplaceDialog();
                    break;
                case 1:
                    txtAdvanced.ShowReplaceDialog();
                    break;
            }
        }

        private void actExpressionFormatDAX_Execute(object sender, EventArgs e)
        {
            var textToFormat = "x :=" + txtExpression.Text;
            try
            {
                var result = TabularEditor.Dax.DaxFormatterProxy.FormatDax(textToFormat).FormattedDax;
                if (string.IsNullOrWhiteSpace(result))
                {
                    lblStatus.Text = "Could not format DAX.";
                    return;
                }
                lblStatus.Text = "DAX formatted succesfully";
                txtExpression.Text = result.Substring(6);
            }
            catch
            {
                lblStatus.Text = "Could not format DAX.";
            }
        }

        private void actToggleInfoColumns_Execute(object sender, EventArgs e)
        {
            UI.SetInfoColumns(actToggleInfoColumns.Checked);
            Preferences.Current.View_MetadataInformation = actToggleInfoColumns.Checked;
            Preferences.Current.Save();
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
            form.Context = UI.Selection.Context;
            form.txtName.Text = CurrentCustomAction;

            var res = form.ShowDialog();

            if(res == DialogResult.OK)
            {
                var act = new CustomActionJson();
                act.Name = form.txtName.Text;
                act.Tooltip = form.txtTooltip.Text;
                act.Execute = txtAdvanced.Text;
                act.Enabled = "true";
                act.ValidContexts = form.Context;

                if (custActions == null) custActions = new CustomActionsJson() { Actions = new CustomActionJson[0] };

                // Remove any existing actions with the same name:
                custActions.Actions = custActions.Actions.Where(a => !a.Name.Equals(act.Name, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                var toRemove = UI.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName.Equals(act.Name, StringComparison.InvariantCultureIgnoreCase));
                if (toRemove != null) UI.Actions.Remove(toRemove);

                var list = custActions.Actions.ToList();

                list.Add(act);

                custActions.Actions = list.ToArray();
                custActions.SaveToJson(ScriptEngine.CustomActionsJsonPath);

                // Compile and add the newly created action:
                ScriptEngine.CompileCustomActions(new CustomActionsJson() { Actions = new []{ act } });
                if (!ScriptEngine.CustomActionError) ScriptEngine.AddCustomActions(UI.Actions);
                PopulateCustomActionsDropDown();
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
        

        PreferencesForm PreferencesForm = new PreferencesForm();


        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesForm.ShowDialog();
            if(UI.Handler != null) UI.Handler.Settings = Preferences.Current.GetSettings();
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

        public void UpdateTreeUIButtons()
        {
            var treeModel = (tvModel.Model as Aga.Controls.Tree.SortedTreeModel).InnerModel as TabularUITree;
            tbShowDisplayFolders.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.DisplayFolders);
            tbShowHidden.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.ShowHidden);
            tbShowAllObjectTypes.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.AllObjectTypes);
            tbShowMeasures.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.Measures);
            tbShowColumns.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.Columns);
            tbShowHierarchies.Checked = treeModel.Options.HasFlag(TOMWrapper.LogicalTreeOptions.Hierarchies);
            tbApplyFilter.Checked = !string.IsNullOrEmpty(treeModel.Filter);
        }

        private void fromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.File_Open(true);
        }

        private void bestPracticeAnalyzerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BPAForm.Model = UI.Handler?.Model;
            BPAForm.ModelTree = tvModel;
            BPAForm.FormMain = this;
            BPAForm.Show();
        }

        private BPAForm BPAForm = new BPAForm();

        private void CanComment(object sender, EventArgs e)
        {
            (sender as Crad.Windows.Forms.Actions.Action).Enabled = txtExpression.Focused && txtExpression.Text != "";
        }

        private void actComment_Execute(object sender, EventArgs e)
        {
            txtExpression.InsertLinePrefix("//");
        }

        private void actUncomment_Execute(object sender, EventArgs e)
        {
            txtExpression.RemoveLinePrefix("//");
        }

        private void actSaveToFolder_Execute(object sender, EventArgs e)
        {
            UI.File_SaveAs_ToFolder();
        }

        private void actNewModel_Execute(object sender, EventArgs e)
        {
            UI.File_New();
        }

        private void toolTreeView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnOpenScript_Click(object sender, EventArgs e)
        {
            if(ofdScript.ShowDialog() == DialogResult.OK)
            {
                txtAdvanced.Text = File.ReadAllText(ofdScript.FileName);
            }
        }

        private void btnSaveScript_Click(object sender, EventArgs e)
        {
            if(sfdScript.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfdScript.FileName, txtAdvanced.Text);
            }
        }

        private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            int zoom;
            if(int.TryParse(toolStripComboBox1.Text.Replace("%", "").Trim(), out zoom))
            {
                txtAdvanced.Zoom = zoom;
            } else
            {
                zoom = txtAdvanced.Zoom;
            }
        }

        private void txtAdvanced_ZoomChanged(object sender, EventArgs e)
        {
            int zoom = txtAdvanced.Zoom;
            toolStripComboBox1.Text = string.Format("{0} %", zoom);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var srf = new SelectReferencesForm();
            srf.lstReferences.Items.AddRange(ScriptEngine.PluginNamespaces
                .Select(pn => new ListViewItem(new[] { pn.Namespace, Path.GetFileName(pn.Assembly.Location), pn.Assembly.GetName().Version.ToString() }) {
                    Checked = Preferences.Current.Scripting_UsingNamespaces.Contains(pn.Namespace) }).ToArray());

            if (srf.ShowDialog() == DialogResult.OK)
            {
                Preferences.Current.Scripting_UsingNamespaces.Clear();
                foreach(ListViewItem item in srf.lstReferences.CheckedItems)
                {
                    Preferences.Current.Scripting_UsingNamespaces.Add(item.SubItems[0].Text);
                }
                Preferences.Current.Save();
            }
        }

        private void actBack_Update(object sender, EventArgs e)
        {
            actBack.Enabled = UI.CanNavigateBack;
        }

        private void actForward_Update(object sender, EventArgs e)
        {
            actForward.Enabled = UI.CanNavigateForward;
        }

        private void actBack_Execute(object sender, EventArgs e)
        {
            UI.Tree_NavigateBack();
        }

        private void actForward_Execute(object sender, EventArgs e)
        {
            UI.Tree_NavigateForward();
        }
    }
}

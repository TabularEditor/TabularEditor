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
using Aga.Controls.Tree;
using System.Threading.Tasks;
using TabularEditor.BestPracticeAnalyzer;

namespace TabularEditor
{
    public partial class FormMain : Form
    {
        public UIController UI;

        private CustomActionJson CurrentCustomAction;

        public static FormMain Singleton;

        public FormMain()
        {
            Singleton = this;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();

            dlgOpenFile.Filter = "Tabular Model Files|*.bim;database.json;*.pbit|All files|*.*";

            // For some reason, Visual Studio sometimes removes this from the FormMain.Designer.cs, making the
            // colors of the lines look ugly:
            propertyGrid1.LineColor = System.Drawing.SystemColors.InactiveBorder;

            // Assign our own custom Designer, to make sure we can handle property changes on multiple objects simultaneously:
            propertyGrid1.Site = new DesignerHost();

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
                foreach (var action in custActions.Actions)
                {
                    var item = customActionsToolStripMenuItem.DropDownItems.Add(action.Name);
                    item.ToolTipText = action.Tooltip;
                    item.AutoToolTip = true;
                    item.Tag = action;
                    item.Click += (s, e) =>
                    {                        
                        var clickAction = CurrentCustomAction = (CustomActionJson)(s as ToolStripItem).Tag;                        
                        txtAdvanced.Text = clickAction.Execute;
                    };
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
                BpaLabel = lblBpaRules,
                StatusExLabel = lblScriptStatus,
                OpenBimDialog = dlgOpenFile,
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
            actTogglePartitions.Checked = Preferences.Current.View_Partitions;
            actToggleAllObjectTypes.Checked = Preferences.Current.View_AllObjectTypes;
            actToggleOrderByName.Checked = !Preferences.Current.View_SortAlphabetically;

            actSearchChild.Checked = Preferences.Current.View_SearchResults == SearchResultOption.ByChild;
            actSearchParent.Checked = Preferences.Current.View_SearchResults == SearchResultOption.ByParent;
            actSearchFlat.Checked = Preferences.Current.View_SearchResults == SearchResultOption.Flat;

            // Assign column widths from preferences:
            foreach (var cp in Preferences.Current.View_ColumnPreferences)
            {
                var column = elements.TreeView.Columns.FirstOrDefault(c => c.Header == cp.Name);
                if(column != null) column.Width = cp.Width;
            } 

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
                if (e.Cancel) return;
            }

            SaveTreeViewColumnPreferences();
        }

        private void SaveTreeViewColumnPreferences()
        {
            foreach (var column in tvModel.Columns)
            {
                // Persist column widths:
                var cp = Preferences.Current.View_ColumnPreferences.FirstOrDefault(c => c.Name == column.Header);
                if (cp == null)
                {
                    cp = new ColumnPreferences { Name = column.Header, Visible = true };
                    Preferences.Current.View_ColumnPreferences.Add(cp);
                }
                cp.Width = column.Width;
            }
            Preferences.Current.Save();
        }

        private void actViewOptions_Execute(object sender, EventArgs e)
        {
            UI.SetDisplayOptions(
                actToggleHidden.Checked,
                actToggleDisplayFolders.Checked,
                actToggleColumns.Checked,
                actToggleMeasures.Checked,
                actToggleHierarchies.Checked,
                actTogglePartitions.Checked,
                actToggleAllObjectTypes.Checked,
                actToggleOrderByName.Checked,
                actToggleFilter.Checked ? txtFilter.Text : null
            );

            Preferences.Current.View_HiddenObjects = actToggleHidden.Checked;
            Preferences.Current.View_DisplayFolders = actToggleDisplayFolders.Checked;
            Preferences.Current.View_Columns = actToggleColumns.Checked;
            Preferences.Current.View_Measures = actToggleMeasures.Checked;
            Preferences.Current.View_Hierarchies = actToggleHierarchies.Checked;
            Preferences.Current.View_Partitions = actTogglePartitions.Checked;
            Preferences.Current.View_AllObjectTypes = actToggleAllObjectTypes.Checked;
            Preferences.Current.View_SortAlphabetically = !actToggleOrderByName.Checked;
            Preferences.Current.Save();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            // Only update filter results continously when we're not in LINQ mode:
            if(!LinqMode && actToggleFilter.Checked) actToggleFilter.DoExecute();        
        }

        /// <summary>
        /// Returns true only when there's an active Dynamic LINQ Filter
        /// </summary>
        private bool LinqMode => txtFilter.Text.StartsWith(":") && actToggleFilter.Checked;

        private void actExit_Execute(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void actCollapseExpand_Execute(object sender, EventArgs e)
        {
            if (sender == actCollapseAll) tvModel.CollapseAll();
            if (sender == actExpandAll) tvModel.ExpandAll();
            if (sender == actExpandFromHere) foreach (var n in tvModel.SelectedNodes) n.ExpandAll();
            if (sender == actCollapseFromHere) foreach (var n in tvModel.SelectedNodes) n.CollapseAll();
            if (tvModel.SelectedNode != null) tvModel.ScrollTo(tvModel.SelectedNode);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            actTogglePartitions.Image = tabularTreeImages.Images["partition"];

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
            e.Enabled = UI.ExpressionEditor_IsDax;
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
            if (string.IsNullOrEmpty(txtExpression.Text)) return;

            using (var hg = new Hourglass())
            {
                var textToFormat = "x :=" + txtExpression.Text;
                var newline = txtExpression.Text.StartsWith("\n") || txtExpression.Text.StartsWith("\r\n");
                try
                {
                    var result = TabularEditor.Dax.DaxFormatterProxy.Instance.FormatDax(textToFormat, Preferences.Current.UseSemicolonsAsSeparators, sender == actExpressionFormatDAXShort, Preferences.Current.DaxFormatterSkipSpaceAfterFunctionName).FormattedDax;
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        lblStatus.Text = "Could not format DAX (invalid syntax).";
                        return;
                    }
                    lblStatus.Text = "DAX formatted succesfully";
                    txtExpression.Text = (newline ? "\n" : "") + result.Substring(6).Trim();
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Could not format DAX (" + ex.Message + ").";
                }
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
            using (var form = new SaveCustomActionForm())
            {   
                form.txtName.Text = CurrentCustomAction?.Name;
                form.txtTooltip.Text = CurrentCustomAction?.Tooltip;
                form.Context = CurrentCustomAction?.ValidContexts ?? UI.Selection.Context;
             
                var res = form.ShowDialog();

                if (res == DialogResult.OK)
                {
                    var act = new CustomActionJson();
                    act.Name = form.txtName.Text;
                    act.Tooltip = form.txtTooltip.Text;
                    act.Execute = txtAdvanced.Text;
                    act.Enabled = "true";
                    act.ValidContexts = form.Context;
                    
                    ScriptEngine.CompileCustomActions(new CustomActionsJson() { Actions = new[] { act } });
                    if (ScriptEngine.CustomActionError)
                    {
                        MessageBox.Show("Compile failed, custom action contains errors and cannot be saved.", "Validation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (custActions == null) custActions = new CustomActionsJson() { Actions = new CustomActionJson[0] };

                    // Remove any existing actions with the same name:
                    custActions.Actions = custActions.Actions.Where(a => !a.Name.Equals(act.Name, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                    var toRemove = UI.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName.Equals(act.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (toRemove != null) UI.Actions.Remove(toRemove);

                    var list = custActions.Actions.ToList();
                    list.Add(act);

                    custActions.Actions = list.ToArray();
                    custActions.SaveToJson(ScriptEngine.CustomActionsJsonPath);
                    CurrentCustomAction = act;

                    ScriptEngine.AddCustomActions(UI.Actions);
                    PopulateCustomActionsDropDown();
                }
            }
        }

        private void DeleteCurrentCustomAction()
        {
            if (CurrentCustomAction == null)
                return;

            var dialogResult = MessageBox.Show($"Are you sure you want to delete the custom action [{ CurrentCustomAction.Name }] ?", "Confirm delete action", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult != DialogResult.Yes)
                return;

            if (custActions == null) 
                custActions = new CustomActionsJson() { Actions = new CustomActionJson[0] };

            custActions.Actions = custActions.Actions.Where(a => !a.Name.Equals(CurrentCustomAction.Name, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            var toRemove = UI.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName.Equals(CurrentCustomAction.Name, StringComparison.InvariantCultureIgnoreCase));
            if (toRemove != null) 
                UI.Actions.Remove(toRemove);

            custActions.SaveToJson(ScriptEngine.CustomActionsJsonPath);
            CurrentCustomAction = null;
            
            PopulateCustomActionsDropDown();
        }

        private void actSaveCustomAction_Execute(object sender, EventArgs e)
        {
            SaveCustomAction();
        }

        private void actSaveCustomAction_Update(object sender, EventArgs e)
        {
            actSaveCustomAction.Enabled = !string.IsNullOrEmpty(txtAdvanced.Text);
        }

        private void actDeleteCustomAction_Execute(object sender, EventArgs e)
        {
            DeleteCurrentCustomAction();
        }

        private void actDeleteCustomAction_Update(object sender, EventArgs e)
        {
            actDeleteCustomAction.Enabled = CurrentCustomAction != null;
        }


        PreferencesForm PreferencesForm = new PreferencesForm();


        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var usingSemicolons = Preferences.Current.UseSemicolonsAsSeparators;
            PreferencesForm.Show(UI.Handler);
            if (!Preferences.Current.BackgroundBpa)
                lblBpaRules.Text = "";
            else
                UI.InvokeBPABackground();

            if(usingSemicolons != Preferences.Current.UseSemicolonsAsSeparators)
            {
                if (Preferences.Current.UseSemicolonsAsSeparators) UI.ExpressionEditor_SwitchToSemicolons();
                else UI.ExpressionEditor_SwitchToCommas();
            }

            if (UI.Handler != null) UI.Handler.Settings = Preferences.Current.GetSettings();
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
                if(UpdateService.Check(false).UpdateAvailable(Preferences.Current.SkipPatchUpdates))
                {
                    var res = MessageBox.Show("A new version of Tabular Editor is available. Would you like to open the download page now?\n\nYou can disable this check under File > Preferences.", "Tabular Editor update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes) UpdateService.OpenDownloadPage();
                }
            }

            // Auto-load the file specified as command line arguments:
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && (File.Exists(args[1]) || File.Exists(args[1] + "\\database.json")))
            {
                UI.File_Open(args[1]);
            }
            if (args.Length == 3)
            {
                UI.Database_Open(args[1], args[2]);
            }
        }

        public void UpdateTreeUIButtons()
        {
            var treeModel = tvModel.Model as TabularUITree;
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
            BPAForm.ShowBPA();
        }

        internal BPAForm BPAForm = new BPAForm();

        private void CanComment(object sender, EventArgs e)
        {
            (sender as Crad.Windows.Forms.Actions.Action).Enabled = 
                (txtExpression.Focused && txtExpression.Text != "") ||
                (txtAdvanced.Focused && txtAdvanced.Text != "");
        }

        private void actComment_Execute(object sender, EventArgs e)
        {
            if (txtExpression.Focused) txtExpression.InsertLinePrefix("//");
            if (txtAdvanced.Focused) txtAdvanced.InsertLinePrefix("//");
        }

        private void actUncomment_Execute(object sender, EventArgs e)
        {
            if (txtExpression.Focused) txtExpression.RemoveLinePrefix("//");
            if (txtAdvanced.Focused) txtAdvanced.RemoveLinePrefix("//");
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

        private void nodeTextBox7_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            e.Value = (e.Node.Tag is TOMWrapper.Model) ? "" : (e.Node.Tag as TOMWrapper.KPI)?.MeasureName ?? 
                (e.Node.Tag as TOMWrapper.Level)?.Hierarchy?.Name ??
                (e.Node.Tag as TOMWrapper.ITabularTableObject)?.Table?.DaxTableName ??
                "Model";
        }

        private void actToggleColumns_Update(object sender, EventArgs e)
        {
            
        }

        private void DisableIfFlatResult(object sender, UpdateExEventArgs e)
        {
            e.Enabled = !UI.FilterEnabled || UI.FilterMode != FilterMode.Flat;
        }

        private void nodeTextBox5_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            var value = (e.Node.Tag as TOMWrapper.Table)?.Source ??
                (e.Node.Tag as TOMWrapper.Partition)?.Expression ??
                (e.Node.Tag as TOMWrapper.DataColumn)?.SourceColumn ??
                (e.Node.Tag as TOMWrapper.CalculatedTableColumn)?.SourceColumn ??
                (e.Node.Tag as TOMWrapper.Level)?.Column?.DaxObjectName ??
                (e.Node.Tag as TOMWrapper.IExpressionObject)?.Expression;
            e.Value = (value ?? string.Empty).Replace('\n', ' ').Trim();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Global keyboard shortcuts (if this method returns TRUE, then the key press will not be propagated to form controls)
            switch (keyData)
            {
                case (Keys.Alt | Keys.Left): UI.Tree_NavigateBack(); return true;
                case (Keys.Alt | Keys.Right): UI.Tree_NavigateForward(); return true;
                default:
                    if (UIController.Current.Actions.HandleKeyPress(keyData)) return true;
                    break;
            }

            // Shortcuts that apply only when the explorer tree has focus:
            if (tvModel.Focused)
            {
                switch (keyData) {
                    case (Keys.Control | Keys.Left):
                        if (tvModel.ContainsFocus) { actCollapseFromHere.DoExecute(); return true; }
                        break;
                    case (Keys.Control | Keys.Right):
                        if (tvModel.ContainsFocus) { actExpandFromHere.DoExecute(); return true; }
                        break;
                    case (Keys.Control | Keys.Shift | Keys.Left):
                        if (tvModel.ContainsFocus) { actCollapseAll.DoExecute(); return true; }
                        break;
                    case (Keys.Control | Keys.Shift | Keys.Right):
                        if (tvModel.ContainsFocus) { actExpandAll.DoExecute(); return true; }
                        break;
                }
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void actFind_UpdateEx(object sender, UpdateExEventArgs e)
        {
            e.Enabled = (txtExpression.ContainsFocus && UI.ExpressionEditor_Current != null) 
                || txtAdvanced.ContainsFocus 
                || propertyGrid1.ContainsFocus;
        }

        private void actSearch_Execute(object sender, EventArgs e)
        {
            actSearchChild.Checked = actSearchChild == sender;
            actSearchParent.Checked = actSearchParent == sender;
            actSearchFlat.Checked = actSearchFlat == sender;
            Preferences.Current.View_SearchResults = actSearchChild.Checked ? SearchResultOption.ByChild
                : (actSearchParent.Checked ? SearchResultOption.ByParent : SearchResultOption.Flat);
            UI.SetFilterMode((FilterMode)Preferences.Current.View_SearchResults);
        }

        private void actGotoDef_Execute(object sender, EventArgs e)
        {
            UI.ExpressionEditor_GoToDefinition();
        }

        private void actSearchResultView_UpdateEx(object sender, UpdateExEventArgs e)
        {
            e.Enabled = actToggleFilter.Checked;
        }

        private void lblBpaRules_Click(object sender, EventArgs e)
        {
            BPAForm.ShowBPA();
        }

        private void manageBPARulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BPAManager.Show(BPAForm.Analyzer);
        }

        private void actOpenBPA_Execute(object sender, EventArgs e)
        {
            BPAForm.ShowBPA();
        }

        private void btnFormatDAX_ButtonClick(object sender, EventArgs e)
        {
            actExpressionFormatDAX.DoExecute();
        }
    }
}

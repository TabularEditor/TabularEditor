using Aga.Controls.Tree;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridExtension;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Actions;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
   

    public partial class UIController
    {
        public static UIController Current;

        private UIElements UI;
        public UIElements Elements { get { return UI; } }

        public readonly MarkAsDateTableForm MarkAsDateTableDialog = new MarkAsDateTableForm();

        public event EventHandler ModelLoaded;

        public TabularModelHandler Handler { get; private set; }
        public TabularUITree TreeModel { get; private set; }

        private UITreeSelection _selection;
        public UITreeSelection Selection {
            get
            {
                if (_selection == null || SelectionInvalid)
                    _selection = new UITreeSelection(UI.TreeView.SelectedNodes);

                return _selection;
            }
        }
        private bool SelectionInvalid = true;

        public UIController(UIElements elements)
        {
            UI = elements;

            UpdateUIText();
            Tree_Init();
            ExpressionEditor_Init();
            ScriptEditor_Init();
            PropertyGrid_Init();

            ClipboardListener.ClipboardUpdate += ClipboardListener_ClipboardUpdate;

            Current = this;

            Actions = new ModelActionManager();
            Actions.CreateStandardActions();
            if(ScriptEngine.AddCustomActions != null)
            {
                Actions.Add(new Separator());
                ScriptEngine.AddCustomActions?.Invoke(Actions);
                UI.StatusExLabel.Text = string.Format("Succesfully loaded {0} custom action{1}. Compilation took {2} seconds.", 
                    ScriptEngine.CustomActionCount, ScriptEngine.CustomActionCount == 1 ? "" : "s", ScriptEngine.CustomActionCompiletime / 1000m);
            } else
            {
                if (ScriptEngine.CustomActionError) UI.StatusExLabel.Text = "Failed loading custom actions. See CustomActionsError.log for more details.";
            }

            foreach(var plugin in Program.Plugins)
            {
                plugin.RegisterActions(RegisterPluginCallback);
            }
        }

        private void ClipboardListener_ClipboardUpdate(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    if (Handler == null) return;

                    // Possible JSON serialization of Tabular objects
                    ClipboardObjects = Serializer.ParseObjectJsonContainer(Clipboard.GetText(TextDataFormat.UnicodeText));
                }
                else
                {
                    ClipboardObjects = null;
                }
            }
            catch
            {
                // General catch-all for clipboard errors (should fix issue https://github.com/otykier/TabularEditor/issues/129)
                // but not able to reproduce.
                ClipboardObjects = null;
            }
        }

        public ObjectJsonContainer ClipboardObjects { get; private set; }

        private void RegisterPluginCallback(string name, System.Action action)
        {
            var splitName = name.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var items = UI.ToolsMenu.DropDownItems;
            for (var i = 0; i < splitName.Length; i++)
            {
                var item = items.Cast<ToolStripMenuItem>().FirstOrDefault(it => it.Text == splitName[i]);
                if(item != null)
                {
                    items = item.DropDownItems;
                } else
                {
                    var newItem = items.Add(splitName[i]);
                    items = (newItem as ToolStripMenuItem).DropDownItems;
                    if (i == splitName.Length - 1)
                    {
                        newItem.Click += (s, e) =>
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception ex)
                            {
                                var st = ex.StackTrace.Split('\n');
                                if (st.Length > 1) st = st.Take(st.Length - 1).ToArray();
                                var stacktrace = string.Join("\n", st);

                                MessageBox.Show(stacktrace + "\n\n" + ex.Message, "Plug-in Error");
                            }
                        };
                    }
                }
            }
        }

        public ModelActionManager Actions { get; private set; }

        public string LastDeploymentDb;

        public void LoadTabularModelToUI()
        {
            if (Handler == null) return;

            LastDeploymentDb = null;

            UI.FormMain.actToggleFilter.Checked = false;
            DisableLinqMode();

            Handler.UndoManager.UndoStateChanged += UndoManager_UndoActionAdded;
            Handler.ObjectChanging += UIController_ObjectChanging;
            Handler.ObjectChanged += UIController_ObjectChanged;
            Handler.ObjectDeleting += UIController_ObjectDeleting;

            ClipboardListener_ClipboardUpdate(this, new EventArgs());

            ExpressionEditor_CancelEdit();
            ExpressionEditor_Current = null;

            Forward = new Stack<IExpressionObject>();
            Back = new Stack<IExpressionObject>();
            CurrentFilter = null;

            ShowSelectionStatus = false;
            TreeModel = new TabularUITree(Handler) { Options = CurrentOptions, TreeView = UI.TreeView };
            TreeModel.UpdateComplete += Tree_UpdateComplete;

            UI.TreeView.Model = TreeModel;
            UI.TreeView.FindNode(new TreePath(Handler.Model))?.Expand();

            UI.ScriptEditor.Enabled = true;


            // Takes care of simple 1:1 bindings in the UI, once a Tabular Model has been loaded.
            // "Simple" binding is for UI elements where we can use standard Windows Forms binding
            // since the underlying objects support it.
            UI.TranslationSelector.ComboBox.BindTo(Handler.Model.Cultures, "DisplayName", TreeModel, "Culture", "(No translation)");
            UI.PerspectiveSelector.ComboBox.BindTo(Handler.Model.Perspectives, "Name", TreeModel, "Perspective", "(All objects)");

            UpdateUIText();

            OnModelLoaded();
            TreeView_SelectionChanged(UI.TreeView, new EventArgs());
            UI.FormMain.modelToolStripMenuItem.Enabled = true;
            UI.ModelMenu.Enabled = true;

            InitPlugins();
        }

        private void UIController_ObjectDeleting(object sender, ObjectDeletingEventArgs e)
        {
            if(UI.PropertyGrid.SelectedObjects.Contains(e.TabularObject))
            {
                UI.PropertyGrid.SelectedObject = null;
            }
        }

        private void Tree_UpdateComplete(object sender, EventArgs e)
        {
            UI.TreeView.Invalidate();
            UI.PropertyGrid.Refresh();

            if (DependencyForm.Visible) DependencyForm.RefreshTree();
        }

        private void UIController_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.PropertyName == "Annotations" && e.TabularObject == UI.PropertyGrid.SelectedObject)
                UI.PropertyGrid.Refresh();
        }

        private void InitPlugins()
        {
            foreach(var plugin in Program.Plugins)
            {
                plugin.Init(Handler);
            }
        }

        private void UIController_ObjectChanging(object sender, ObjectChangingEventArgs e)
        {
            // This method captures all changes to object properties in the Tabular Object Model.
            // We can use this event handler to provide UI messages/warnings when specific object
            // properties are about to be changed. If we want to cancel a property change, set
            // the Cancel flag on e to true.

            // If currently executing a script, do nothing, as we assume users know what they're doing:
            if (ScriptEditor_IsExecuting) return;

            if(Handler.SourceType == ModelSourceType.Folder)
            {
                if(Handler.SerializeOptions.LocalPerspectives && e.TabularObject is Perspective && e.PropertyName == "Name")
                {
                    var r = MessageBox.Show("Model is currently loaded from a Folder Structure where perspectives are serialized on the individual objects.\n\nChanging a perspective name, will cause changes to all objects visible in that perspective, potentially causing merge conflicts in your Version Control tool. Proceed?", "Local perspective serialization", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    e.Cancel = r == DialogResult.Cancel;
                    return;
                }
                if(Handler.SerializeOptions.LocalTranslations && e.TabularObject is Culture && e.PropertyName == "Name")
                {
                    var r = MessageBox.Show("Model is currently loaded from a Folder Structure where translations are serialized on the individual objects.\n\nChanging a translation, will cause changes to all objects having that translation, potentially causing merge conflicts in your Version Control tool. Proceed?", "Local translation serialization", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    e.Cancel = r == DialogResult.Cancel;
                    return;
                }
            }
        }

        protected void OnModelLoaded()
        {
            ModelLoaded?.Invoke(this, new EventArgs());
        }

        public void BeforeAction()
        {
            UI.StatusLabel.Text = "";
            UI.StatusExLabel.Text = "";
        }

        public void ClearUI()
        {
            UI.TreeView.SelectedNode = null;
            UI.TreeView.Model = null;
            UI.TreeView.Refresh();
            TreeView_SelectionChanged(null, new EventArgs());
            UI.PropertyGrid.Refresh();
        }

        private void UpdateUIText()
        {
            var appName = Application.ProductName + " 2.7.4"; // + string.Join(".", Application.ProductVersion.Split('.').Take(2));

            if (Handler == null)
            {
                UI.FormMain.Text = appName;
                UI.StatusLabel.Text = "(No model loaded)";
                UI.StatusExLabel.Text = "";
                UI.ErrorLabel.Text = "";
            }
            else
            {
                UI.FormMain.Text = string.Format("{0}{1} - {2}",
                    Handler.IsConnected ? (
                        string.IsNullOrEmpty(LocalInstanceName) ? Handler.Source : LocalInstanceName
                        ) : (File_Current ?? "(Unsaved model)"),
                    Handler.HasUnsavedChanges ? "*" : "", appName);

                UI.StatusLabel.Text = Handler.Status;
                UI.ErrorLabel.Text = Handler?.Errors?.Count > 0 ? string.Format("{0} error{1}", Handler.Errors.Count, Handler.Errors.Count > 1 ? "s" : "") : "";
                UI.ErrorLabel.IsLink = Handler?.Errors?.Count > 0;
            }
        }

    }

    public struct UIElements
    {
        public TreeViewAdv TreeView;
        public NavigatablePropertyGrid PropertyGrid;
        public FastColoredTextBox ExpressionEditor;
        public FastColoredTextBox ScriptEditor;
        public OpenFileDialog OpenBimDialog;
        public ToolStripLabel StatusLabel;
        public ToolStripLabel StatusExLabel;
        public ToolStripLabel ErrorLabel;
        public Label CurrentMeasureLabel;
        public FormMain FormMain;
        public ToolStripDropDownItem ModelMenu;
        public ToolStripDropDownItem ToolsMenu;
        public ToolStripDropDownItem DynamicMenu;
        public ToolStripComboBox TranslationSelector;
        public ToolStripComboBox PerspectiveSelector;
        public ToolStripComboBox ExpressionSelector;
        public Crad.Windows.Forms.Actions.ActionList ActionList;
    }
}

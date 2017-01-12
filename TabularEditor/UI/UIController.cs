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
using TabularEditor.UI.Actions;

namespace TabularEditor.UI
{
   

    public partial class UIController
    {
        public static UIController Current;

        private UIElements UI;

        public event EventHandler ModelLoaded;

        public TabularModelHandler Handler { get; private set; }
        public TabularUITree Tree { get; private set; }

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
        }

        public ModelActionManager Actions { get; private set; }

        public void Refresh()
        {
            UI.TreeView.Refresh();
            UI.PropertyGrid.Refresh();
        }

        public void LoadTabularModelToUI()
        {
            Handler.UndoManager.UndoStateChanged += UndoManager_UndoActionAdded;

            ShowSelectionStatus = false;
            Tree = new TabularUITree(Handler.Model) { Options = Tree?.Options ?? LogicalTreeOptions.Default };

            var sortedModel = new SortedTreeModel(Tree);
            sortedModel.Comparer = new TabularObjectComparer(Tree);
            UI.TreeView.Model = sortedModel;
            UI.TreeView.FindNode(new TreePath(Handler.Model))?.Expand();

            UI.ScriptEditor.Enabled = true;


            // Takes care of simple 1:1 bindings in the UI, once a Tabular Model has been loaded.
            // "Simple" binding is for UI elements where we can use standard Windows Forms binding
            // since the underlying objects support it.
            UI.TranslationSelector.ComboBox.BindTo(Handler.Model.Cultures, "DisplayName", Tree, "Culture", "(No translation)");
            UI.PerspectiveSelector.ComboBox.BindTo(Handler.Model.Perspectives, "Name", Tree, "Perspective", "(All objects)");

            UpdateUIText();

            OnModelLoaded();
            TreeView_SelectionChanged(UI.TreeView, new EventArgs());
            UI.FormMain.modelToolStripMenuItem.Enabled = true;
            UI.ModelMenu.Enabled = true;
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
            var appName = Application.ProductName + " " + string.Join(".", Application.ProductVersion.Split('.').Take(2));

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
                        string.IsNullOrEmpty(LocalInstanceName) ? Handler.Database.Server.Name + "." + Handler.Database.Name : LocalInstanceName
                        ) : File_Current,
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
        public SaveFileDialog SaveBimDialog;
        public ToolStripLabel StatusLabel;
        public ToolStripLabel StatusExLabel;
        public ToolStripLabel ErrorLabel;
        public Label CurrentMeasureLabel;
        public FormMain FormMain;
        public ImageList TreeImages;
        public ToolStripDropDownItem ModelMenu;
        public ToolStripComboBox TranslationSelector;
        public ToolStripComboBox PerspectiveSelector;
    }
}

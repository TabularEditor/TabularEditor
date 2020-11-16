using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace TabularEditor.UI.Actions
{
    public interface IBaseAction
    {
        bool Enabled(object arg);
        bool HideWhenDisabled { get; }
        void Execute(object arg);
        string ToolTip { get; }
        Context ValidContexts { get; }
    }

    public interface ICustomMenuAction
    {
        void InitMenu(ToolStripItem item, Context currentContext);
    }
    public interface IModelAction: IBaseAction
    {
        string Name { get; }
        Keys Shortcut { get; }
    }

    public interface IModelMultiAction: IBaseAction
    {
        IDictionary ArgNames { get; }
        string Path { get; }
    }

    public class ModelSubAction: IBaseAction
    {
        public string ToolTip { get; }
        public bool Enabled(object arg) { return true; }
        public void Execute(object arg) { }
        public bool HideWhenDisabled { get { return true; } }
        public Context ValidContexts { get { return Context.Everywhere; } }
    }

    public class Separator: IBaseAction
    {
        public string ToolTip { get; }
        public Separator(string path = "")
        {
            Path = path;
        }
        public bool Enabled(object arg) { return true; }
        public void Execute(object arg) { }
        public bool HideWhenDisabled { get { return true; } }
        public string Path { get; set; }
        public Context ValidContexts { get { return Context.Everywhere; } }
    }

    public class Action: IModelAction
    {
        public delegate bool EnabledDelegate(UITreeSelection selected, Model model);
        public delegate void ExecuteDelegate(UITreeSelection selected, Model model);
        public delegate string NameDelegate(UITreeSelection selected, Model model);

        EnabledDelegate _enabled;
        ExecuteDelegate _execute;
        NameDelegate _name;

        UIController ui;
        private Keys _shortcut;
        public Keys Shortcut => _shortcut;

        /// <summary>
        /// If this field is set, after an action has finished executing its custom code,
        /// the object will be selected in the TreeView, and the name editor will show up.
        /// </summary>
        public ITabularNamedObject EditObjectName { get; set; } = null;
        /// <summary>
        /// If this field is set, after an action has finished executing its custom code,
        /// the object will be expanded to show child objects in the TreeView.
        /// </summary>
        public ITabularNamedObject ExpandObject { get; set; } = null;

        public Action(EnabledDelegate enabled, ExecuteDelegate execute, NameDelegate name, bool hideWhenDisabled = false, Context validContexts = Context.Everywhere)
        {
            _enabled = enabled;
            _execute = execute;
            _name = name;
            HideWhenDisabled = hideWhenDisabled;
            ValidContexts = validContexts;

            ui = UIController.Current;
        }

        public Action(EnabledDelegate enabled, ExecuteDelegate execute, NameDelegate name, bool hideWhenDisabled, Context validContexts, Keys shortcut)
            : this(enabled, execute, name, hideWhenDisabled, validContexts)
        {
            _shortcut = shortcut;

            ui.Actions.ShortcutActions.Add(shortcut, this);
        }

        public string ToolTip { get; set; }

        public virtual bool Enabled(object arg)
        {
            try
            {
                return ui.Handler != null /*&& ValidContexts.HasFlag(ui.Selection.Context)*/ && _enabled(ui.Selection, ui.Handler.Model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Custom action error: " + ex.Message);
                return false;
            }
        }
        
        private void InternalExecute(object arg, IEnumerable<ITabularNamedObject> alternateSelection = null)
        {
            TabularModelHandler handler;
            if (ui != null)
            {
                ui.Actions.SelectObjects.Clear();
                ui.Actions.LastActionExecuted = this;
                handler = ui.Handler;
            }
            else
                handler = TabularModelHandler.Singleton;

            EditObjectName = null;
            ExpandObject = null;
            var selection = alternateSelection == null ? (ui == null ? UITreeSelection.Empty : ui.Selection) : new UITreeSelection(alternateSelection);

            // Check if the context is valid before executing the action:
            if (!ValidContexts.HasX(selection.Context | Context.Model | Context.Tool)) return;

            try
            {
                handler.BeginUpdate(Name);
                _execute(selection, handler.Model);
            }
            catch (Exception e)
            {
                MessageBox.Show("The action failed with the following error: " + e.Message, "Unable to perform action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                handler.EndUpdate();
            }


            if (ui == null) return;

            // Actions can trigger certain UI actions after execution (for example, tree expansion, name editing, tree selection):
            if (ExpandObject != null)
            {
                ui.ExpandItem(ExpandObject);
            }
            if (EditObjectName != null)
            {
                ui.EditName(EditObjectName);
            }
            if (ui.Actions.SelectObjects.Count > 0)
            {
                var tree = ui.Elements.TreeView;
                tree.ClearSelection();
                TreeNodeAdv firstNode = null;
                foreach (var obj in ui.Actions.SelectObjects)
                {
                    var node = tree.FindNodeByTag(obj);
                    if (firstNode == null) firstNode = node;
                    if (node != null) node.IsSelected = true;
                }
                if(firstNode != null)
                    tree.EnsureVisible(firstNode);
            }
        }

        public virtual void Execute(object arg)
        {
            InternalExecute(arg, null);
            ui.Handler.Model.Database.CloseReader();
        }

        public void ExecuteWithSelection(object arg, IEnumerable<ITabularNamedObject> alternateSelection)
        {
            InternalExecute(arg, alternateSelection);
        }

        public Context ValidContexts { get; set; }

        public virtual string Name
        {
            get
            {
                return _name(ui == null ? UITreeSelection.Empty : ui.Selection, TabularModelHandler.Singleton?.Model);
            }
        }

        public virtual bool HideWhenDisabled { get; set; }
    }

    public class MultiAction: IModelMultiAction
    {
        /// <summary>
        /// The EnabledParamDelegate should return true/false depending on whether the action should be enabled
        /// for the item in question. As the MultiAction represents a collection of actions on related items,
        /// typically, expect arg to contain a reference to the individual items. NOTE: arg can be null!
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="model"></param>
        /// <param name="arg">Can be null (when determining if the MultiAction as a whole should be enabled.</param>
        /// <returns></returns>
        public delegate bool EnabledParamDelegate(UITreeSelection selected, Model model, object arg);
        public delegate IEnumerable ValidArgsDelegate(UITreeSelection selected, Model model);
        public delegate void ExecuteParamDelegate(UITreeSelection selected, Model model, object arg);
        public delegate string NameParamDelegate(UITreeSelection selected, Model model, object arg);

        EnabledParamDelegate _enabled;
        ExecuteParamDelegate _execute;
        NameParamDelegate _name;
        ValidArgsDelegate _args;

        UIController ui;

        public IDictionary ArgNames
        {
            get
            {
                return _args(ui.Selection, ui.Handler.Model).Cast<object>().Select(obj => new Tuple<string, object>(_name(ui.Selection, ui.Handler.Model, obj), obj))
                    .OrderBy(obj => obj.Item1).ToDictionary(obj => obj.Item1, obj => obj.Item2);
            }
        }

        public string ToolTip { get; set; }

        public bool Enabled(object arg)
        {
            return ValidContexts.HasX(ui.Selection.Context) && _enabled(ui.Selection, ui.Handler.Model, arg);
        }

        public bool HideWhenDisabled { get; set; }
        public string Path { get; set; }
        public Context ValidContexts { get; set; }

        public MultiAction(EnabledParamDelegate enabled, ExecuteParamDelegate execute, NameParamDelegate name, ValidArgsDelegate validArgs, string path, bool hideWhenDisabled = false, Context validContexts = Context.Everywhere)
        {
            _enabled = enabled;
            _execute = execute;
            _name = name;
            _args = validArgs;
            Path = path;
            HideWhenDisabled = hideWhenDisabled;
            ValidContexts = validContexts;

            ui = UIController.Current;
        }

        public void Execute(object arg)
        {
            _execute(ui.Selection, ui.Handler.Model, arg);
        }
    }
}

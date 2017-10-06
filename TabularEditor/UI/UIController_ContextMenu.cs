using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Actions;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        ToolStripItem[] StaticToolsMenuItems = null;
        ToolStripItem[] StaticModelMenuItems = null;

        private void DynamicMenu_Update()
        {
            var c = Selection.Context;
            if (c.Has1(Context.SingularObjects | (Context.Groups ^ Context.TablePartitions)) && c != Context.Model)
            {
                UI.DynamicMenu.Visible = true;
                UI.DynamicMenu.Text = c.ToString().SplitCamelCase();
                return;
            }

            UI.DynamicMenu.Visible = false;
            return;
        }

        private void DynamicMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            menu.Items.Clear();

            // Populate the Dynamic menu with all objects at the selected context, excluding
            // the Delete Action, as it can already be found under the Edit menu:
            ContextMenu_Populate(menu, Selection.Context, true, a => a != Actions.Delete);

            if (menu.Items.Count == 0)
            {
                menu.Items.Add("-");
                e.Cancel = true;
            }
        }


        private void ToolsMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            if(StaticToolsMenuItems == null) StaticToolsMenuItems = menu.Items.OfType<ToolStripItem>().ToArray();
            menu.Items.Clear();
            if (StaticToolsMenuItems.Length > 0)
            {
                menu.Items.AddRange(StaticToolsMenuItems);
                if (menu.Items.Count > 0 && !(menu.Items[menu.Items.Count - 1] is ToolStripSeparator)) menu.Items.Add(new ToolStripSeparator());
            }
            ContextMenu_Populate(menu, Context.Tool);
        }


        private void ModelMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            if (StaticModelMenuItems == null) StaticModelMenuItems = menu.Items.OfType<ToolStripItem>().ToArray();
            menu.Items.Clear();
            if (StaticModelMenuItems.Length > 0)
            {
                menu.Items.AddRange(StaticModelMenuItems);
                if (menu.Items.Count > 0 && !(menu.Items[menu.Items.Count - 1] is ToolStripSeparator)) menu.Items.Add(new ToolStripSeparator());
            }

            // Populate the Model menu with all actions at Model context, excluding custom actions:
            ContextMenu_Populate(menu, Context.Model, false);
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            var originalItems = menu.Items.OfType<ToolStripItem>().Where(item => !(item.Tag is IBaseAction || item.Tag is DynamicMenuState)).ToArray();
            menu.Items.Clear();
            if (originalItems.Length > 0)
            {
                menu.Items.AddRange(originalItems);
                if(menu.Items.Count > 0 && !(menu.Items[menu.Items.Count - 1] is ToolStripSeparator)) menu.Items.Add(new ToolStripSeparator());
            }

            ContextMenu_Populate(menu);

            e.Cancel = menu.Items.Count == 0;
        }

        private void ContextMenu_Populate(ToolStripDropDown menu, Context contextFilter = Context.Everywhere, 
            bool allowCustomActions = true, Func<IBaseAction, bool> actionFilter = null)
        {
            var availableActions = new List<IBaseAction>();
            var createNewActions = 0;

            foreach (var action in Actions)
            {
                if (actionFilter != null && !actionFilter(action)) continue;
                if (!allowCustomActions && action is CustomAction) continue;
                if (contextFilter != Context.Everywhere && !action.ValidContexts.HasX(contextFilter)) continue;
                if (contextFilter == Context.Everywhere && !action.ValidContexts.HasX(Selection.Context)) continue;

                if (action is IModelAction && (action as IModelAction).Name.StartsWith("Create New\\")) createNewActions++;
                availableActions.Add(action);
            }

            foreach (var action in availableActions)
            {
                if (action is IModelAction)
                {
                    var act = action as IModelAction;
                    bool enabled;
                    try {
                        enabled = act.Enabled(null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Custom action Enabled error: " + ex.Message);
                        enabled = false;
                    }
                    if (act.HideWhenDisabled && !enabled) continue;

                    var name = act.Name;
                    if((createNewActions <= 2 || menu == UI.ModelMenu.DropDown) && name.StartsWith("Create New\\"))
                        name = name.Replace("Create New\\", "New ");

                    var item = ContextMenu_AddFromAction(name, menu);
                    if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                    item.Tag = act;
                    item.Enabled = enabled;
                    item.Click += ContextMenuItem_Click;

                } else if (action is IModelMultiAction)
                {
                    var act = action as IModelMultiAction;
                    if (act.HideWhenDisabled && !act.Enabled(null)) continue;

                    ContextMenu_AddFromActionDynamic(act.Path, menu, act);

                    /*var dict = act.ArgNames;

                    foreach (string argName in dict.Keys)
                    {
                        var item = ContextMenu_AddFromAction(act.Path.ConcatPath(argName), menu);
                        if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                        item.Tag = act;
                        item.Name = argName;
                        item.Enabled = act.Enabled(dict[argName]);
                        item.Click += ContextMenuItem_Click;
                    }*/
                } else if (action is Separator)
                {
                    var sep = action as Separator;
                    var item = ContextMenu_AddFromAction(sep.Path.ConcatPath("---"), menu);
                    if (item != null) item.Tag = sep;
                }
            }

            // Remove unnescessary separators:
            RemoveSeparators(menu);
        }

        private void RemoveSeparators(ToolStripDropDown menu)
        {
            if (menu.Items.Count == 0) return;
            if (menu.Items[0] is ToolStripSeparator) menu.Items.RemoveAt(0);

            if (menu.Items.Count == 0) return;
            if (menu.Items[menu.Items.Count - 1] is ToolStripSeparator) menu.Items.RemoveAt(menu.Items.Count - 1);

            foreach(var item in menu.Items.OfType<ToolStripMenuItem>())
            {
                RemoveSeparators(item.DropDown);
            }
        }

        class DynamicMenuState
        {
            public int FirstItemIndex;
            public bool IsLoaded;
            public IModelMultiAction Action;
        }

        private ToolStripItem ContextMenu_AddFromActionDynamic(string name, ToolStripDropDown parent, IModelMultiAction dynamicAction)
        {
            var submenu = ContextMenu_AddFromAction(name.ConcatPath("(No actions available)"), parent);
            submenu.Enabled = false;
            var menu = submenu.OwnerItem as ToolStripMenuItem;
            var state = new DynamicMenuState()
            {
                FirstItemIndex = menu.DropDownItems.IndexOf(submenu),
                IsLoaded = false,
                Action = dynamicAction
            };
            menu.Tag = state;

            menu.DropDownOpening += ContextMenu_DynamicMenuOpening;
            return submenu;
        }

        private void ContextMenu_DynamicMenuOpening(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            Console.WriteLine("Opening submenu for {0}", menu.Text);
            var state = menu.Tag as DynamicMenuState;

            if (state.IsLoaded) return;

            var act = state.Action;

            // Remove placeholder item / previously created items:
            for (var ix = menu.DropDownItems.Count - 1; ix >= state.FirstItemIndex; ix--)
            {
                menu.DropDownItems.RemoveAt(ix);
            }
            RemoveSeparators(menu.DropDown);
            
            var dict = act.ArgNames;

            foreach (string argName in dict.Keys)
            {
                var item = ContextMenu_AddFromAction(argName, menu.DropDown);
                if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                item.Tag = act;
                item.Name = argName;
                item.Enabled = act.Enabled(dict[argName]);
                item.Click += ContextMenuItem_Click;
            }
            
            state.IsLoaded = true;
        }

        private ToolStripItem ContextMenu_AddFromAction(string name, ToolStripDropDown menu)
        {
            var parent = menu;

            var parts = name.Split('\\');
            if (parts.Length > 1)
            {
                for (var i = 0; i < parts.Length - 1; i++)
                {
                    var child = parent.Items.OfType<ToolStripDropDownItem>().FirstOrDefault(c => c.Text == parts[i]);
                    if (child == null)
                    {
                        if (parts.Last() == "---") return null;
                        child = new ToolStripMenuItem(parts[i]);
                        child.Tag = new ModelSubAction();
                        parent.Items.Add(child);
                    }
                    parent = child.DropDown;
                }
            }
            name = parts.Last();
            var item = name == "---" ? new ToolStripSeparator() : new ToolStripMenuItem(name) as ToolStripItem;

            // Below code ensures that separators never appear at the very top of a menu, or in sequence:
            var lastItem = parent.Items.Count > 0 ? parent.Items[parent.Items.Count - 1] : null;
            if (item is ToolStripSeparator && (lastItem == null || lastItem is ToolStripSeparator)) return lastItem;

            parent.Items.Add(item);

            return item;
        }

        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null) return;

            var act = item.Tag as IBaseAction;
            if (act == null) return;

            act.Execute((act as IModelMultiAction)?.ArgNames[item.Name]);
            //UI.PropertyGrid.Refresh();
        }

    }
}

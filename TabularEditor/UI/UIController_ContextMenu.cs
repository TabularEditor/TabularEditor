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
        private void ToolsMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            menu.Items.Clear();
            foreach(var act in Actions.OfType<IModelAction>())
            {
                if((act.ValidContexts & Context.Groups) > 0)
                {
                    var item = ContextMenu_AddFromAction(act.Name, menu);
                    if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                    item.Tag = act;
                    item.Enabled = act.Enabled(null);
                    item.Click += ContextMenuItem_Click;
                }
            }
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Handler == null) return;
            var menu = (sender as ToolStripDropDown);

            var originalItems = menu.Items.OfType<ToolStripItem>().Where(item => !(item.Tag is IBaseAction)).ToArray();
            menu.Items.Clear();
            if (originalItems.Length > 0)
            {
                menu.Items.AddRange(originalItems);
                if(menu.Items.Count > 0 && !(menu.Items[menu.Items.Count - 1] is ToolStripSeparator)) menu.Items.Add(new ToolStripSeparator());
            }

            ContextMenu_Populate(menu);

            e.Cancel = menu.Items.Count == 0;
        }

        private void ContextMenu_Populate(ToolStripDropDown menu)
        {
            foreach (var action in Actions)
            {
                if (!action.ValidContexts.HasFlag(Selection.Context)) continue;

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

                    var item = ContextMenu_AddFromAction(act.Name, menu);
                    if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                    item.Tag = act;
                    item.Enabled = enabled;
                    item.Click += ContextMenuItem_Click;

                } else if (action is IModelMultiAction)
                {
                    var act = action as IModelMultiAction;
                    if (act.HideWhenDisabled && !act.Enabled(null)) continue;

                    foreach (var argName in act.ArgNames)
                    {
                        var item = ContextMenu_AddFromAction(act.Path.ConcatPath(argName.Key), menu);
                        if (!string.IsNullOrEmpty(act.ToolTip)) item.ToolTipText = act.ToolTip;
                        item.Tag = act;
                        item.Enabled = act.Enabled(argName.Value);
                        item.Click += ContextMenuItem_Click;
                    }
                } else if (action is Separator)
                {
                    var sep = action as Separator;
                    var item = ContextMenu_AddFromAction(sep.Path.ConcatPath("---"), menu);
                    if (item != null) item.Tag = sep;
                }
            }

            if (menu.Items.Count > 0 && menu.Items[menu.Items.Count - 1] is ToolStripSeparator) menu.Items.RemoveAt(menu.Items.Count - 1);
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

            act.Execute((act as IModelMultiAction)?.ArgNames[item.Text.Split('\\').Last()]);
            UI.PropertyGrid.Refresh();
        }

    }
}

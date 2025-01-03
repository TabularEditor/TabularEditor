using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridUI;
using TabularEditor.UIServices;

namespace TabularEditor.PropertyGridExtension
{
    /// <summary>
    /// A PropertyGrid where the editable portion may be navigated using the keyboard.
    /// Tab moves the focus to the next property editor.
    /// Shift+Tab moves the focus to the previous property editor.
    /// </summary>
    public class NavigatablePropertyGrid: System.Windows.Forms.PropertyGrid
    {
        public NavigatablePropertyGrid()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GridItem item = this.SelectedGridItem;
            var hasItems = false;
            this.ContextMenuStrip.Items.Clear();
            if (item.PropertyDescriptor is DynamicPropertyDescriptor dpd)
            {
                if(dpd.CustomActions != null && dpd.CustomActions.Count > 0)
                {
                    var hasResetAction = false;
                    foreach (var act in dpd.CustomActions)
                    {
                        if (act.IsResetAction) hasResetAction = true;
                        if (act.Enabled())
                        {
                            hasItems = true;
                            this.ContextMenuStrip.Items.Add(act.Name).Click += (s, e2) => act.Execute();
                        }
                    }
                    if (hasResetAction)
                    {
                        //var resetIndex = this.ContextMenuStrip.Items.OfType<ToolStripItem>().FirstIndexOf(i => i.Tag is VGridStringId sId && sId == VGridStringId.MenuReset);
                        //if (resetIndex != -1) e.Menu.Items.RemoveAt(resetIndex);
                    }
                }
            }
            e.Cancel = !hasItems;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is TextBox &&
                (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
            {
                var item = SelectedGridItem;
                var list = new List<GridItem>(this.EnumerateAllItems().Where(i => i.GridItemType == GridItemType.Property));

                // Get the index of the next or previous item on the list, depending on if Shift key was pressed:
                var dir = keyData.HasFlag(Keys.Shift) ? -1 : 1;

                var newIndex = list.IndexOf(item) + dir;
                if (newIndex < 0 || newIndex >= list.Count) return false;

                SelectedGridItem = list[newIndex];

                FocusEditor();

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FocusEditor()
        {
            var edit = GetCurrentEditor();

            // If the Edit property is a TextBox, bingo!
            if (edit is TextBox)
            {
                (edit as TextBox).Focus();
                (edit as TextBox).SelectAll();
            }
        }

        public Control GetCurrentEditor()
        {
            // Uses reflection to find the GridViewEdit control of the currently selected GridItem

            // First, get the PropertyGridView object of the grid.
            MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            var gridView = methodInfo?.Invoke(this, new object[] { });

            // Next, find the Edit property of the PropertyGridView, which should be the editor control.
            PropertyInfo propInfo = gridView?.GetType().GetProperty("Edit", BindingFlags.NonPublic | BindingFlags.Instance);
            var edit = propInfo?.GetValue(gridView) as Control;

            return edit;
        }
    }
}

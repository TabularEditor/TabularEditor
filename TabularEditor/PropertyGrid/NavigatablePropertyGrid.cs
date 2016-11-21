using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.PropertyGridExtension
{
    /// <summary>
    /// A PropertyGrid where the editable portion may be navigated using the keyboard.
    /// Tab moves the focus to the next property editor.
    /// Shift+Tab moves the focus to the previous property editor.
    /// </summary>
    public class NavigatablePropertyGrid: System.Windows.Forms.PropertyGrid
    {
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

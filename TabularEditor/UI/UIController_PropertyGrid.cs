using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel.Com2Interop;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        public void PropertyGrid_Init()
        {
            CustomEditors.RegisterEditor("Display Folder", new FormDisplayFolderSelect());
        }

        public void PropertyGrid_UpdateFromSelection()
        {
            var expanded = UI.PropertyGrid.GetExpandedItemLabels();
            UI.PropertyGrid.SelectedObjects = UI.TreeView.SelectedNodes.Select(n => n.Tag).ToArray();
            UI.PropertyGrid.ExpandItemsByLabel(expanded);
        }

        public void PropertyGrid_UpdateFromObject(object obj)
        {
            var expanded = UI.PropertyGrid.GetExpandedItemLabels();
            UI.PropertyGrid.SelectedObject = obj;
            UI.PropertyGrid.ExpandItemsByLabel(expanded);
        }
    }
}

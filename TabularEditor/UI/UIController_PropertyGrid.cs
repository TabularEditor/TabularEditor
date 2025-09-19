using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel.Com2Interop;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Dialogs;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        public void PropertyGrid_Init()
        {
            CustomEditors.RegisterEditor(nameof(Measure.DisplayFolder), new FormDisplayFolderSelect());
            CustomEditors.RegisterEditor(nameof(ProviderDataSource.ConnectionString), new ConnectSqlForm());
            CustomEditors.RegisterEditor(nameof(Column.SortByColumn), new ColumnSelectDialog(multiSelect: false, allowNoSelection: true));
            CustomEditors.RegisterEditor(nameof(Column.GroupByColumns), new ColumnSelectDialog(multiSelect: true, allowNoSelection: true));
            CustomEditors.RegisterEditor(nameof(TimeUnitColumnAssociation.PrimaryColumn), new ColumnSelectDialog(multiSelect: false, allowNoSelection: false));
            CustomEditors.RegisterEditor(nameof(TimeUnitColumnAssociation.AssociatedColumns), new ColumnSelectDialog(multiSelect: true, allowNoSelection: true));
            CustomEditors.RegisterEditor(nameof(TimeRelatedColumnGroup.Columns), new ColumnSelectDialog(multiSelect: true, allowNoSelection: false));
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    internal class ColumnSetCollectionEditor: RefreshGridCollectionEditor
    {
        public ColumnSetCollectionEditor(Type type): base(type)
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            var newColumn = CustomEditors.Edit(Context.Instance, Properties.GROUPBYCOLUMNS, (Context.Instance as Column).GroupByColumns, out bool cancel) as Column;
            return newColumn;
        }
    }
}

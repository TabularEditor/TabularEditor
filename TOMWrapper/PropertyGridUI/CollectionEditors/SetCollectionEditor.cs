using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.PropertyGridUI
{
    internal class SetCollectionEditor: ClonableObjectCollectionEditor<Set>
    {
        public SetCollectionEditor(Type type) : base(type)
        {

        }

        Table table;

        protected override object[] GetItems(object editValue)
        {
            table = (editValue as SetCollection).Table;
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            return Set.CreateNew(table);
        }
    }
}

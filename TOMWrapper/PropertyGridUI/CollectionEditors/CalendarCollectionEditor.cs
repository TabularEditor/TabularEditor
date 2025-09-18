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
    internal class CalendarCollectionEditor: ClonableObjectCollectionEditor<Partition>
    {
        public CalendarCollectionEditor(Type type) : base(type)
        {

        }

        CalendarCollection Collection => Context.Instance as CalendarCollection ?? (Context.Instance as Table).Calendars;

        protected override CollectionForm CreateCollectionForm()
        {
            // HACK: By setting the private "newItemTypes" field of the CollectionEditor to null, we force the CreateNewItemTypes() method to be called upon every launch of the form:
            var newItemTypesFieldInfo = typeof(CollectionEditor).GetField("newItemTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            newItemTypesFieldInfo.SetValue(this, null);
            return base.CreateCollectionForm();
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Calendar);
        }

        Table table;

        protected override object[] GetItems(object editValue)
        {
            table = (editValue as CalendarCollection).Table;
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            return Calendar.CreateNew(table);
        }
    }
}

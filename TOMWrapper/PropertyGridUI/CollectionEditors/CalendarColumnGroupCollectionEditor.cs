using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    internal class CalendarColumnGroupCollectionEditor: RefreshGridCollectionEditor
    {
        CalendarColumnGroupCollection Collection;

        public CalendarColumnGroupCollectionEditor() : base(typeof(Collection<CalendarColumnGroup>))
        {
        }

        protected override Type[] CreateNewItemTypes()
        {
            return [typeof(TimeUnitColumnAssociation), typeof(TimeRelatedColumnGroup)];
        }

        protected override object CreateInstance(Type itemType)
        {
            if (itemType == typeof(TimeUnitColumnAssociation)) return TimeUnitColumnAssociation.CreateNew(Collection.Calendar, TimeUnit.Date);
            if (itemType == typeof(TimeRelatedColumnGroup)) return TimeRelatedColumnGroup.CreateNew(Collection.Calendar);
            throw new ArgumentException(nameof(itemType));
        }

        protected override bool CanRemoveInstance(object value)
        {
            return true;
        }

        protected override string GetDisplayText(object value)
        {
            return (value as CalendarColumnGroup).ToString();
        }

        protected override object[] GetItems(object editValue)
        {
            Collection = (editValue as CalendarColumnGroupCollection);
            return Collection.ToArray();
        }

        protected override object SetItems(object editValue, object[] value)
        {
            Collection.Clear();
            foreach (CalendarColumnGroup an in value)
            {
                if(an.IsRemoved) an.RenewMetadataObject();
                Collection.Add(an);
            }
            return Collection;
        }
    }
}

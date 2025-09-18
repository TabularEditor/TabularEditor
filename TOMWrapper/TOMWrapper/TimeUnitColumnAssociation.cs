using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    public partial class TimeUnitColumnAssociation
    {
        [Localizable(false)]
        private protected override void ResolveMissingReferences(ITabularObjectCollection parentCollection, string tomJson)
        {
            base.ResolveMissingReferences(parentCollection, tomJson);

            if (parentCollection is not CalendarColumnGroupCollection cc) return;

            // PrimaryColumn is not automatically resolved after deserializing from JSON, so we do that here:
            var jObj = JObject.Parse(tomJson);
            if (jObj["primaryColumn"] is JValue { Value: string columnName })
            {
                var column = cc.Calendar.Table.Columns.FindByName(columnName);
                if (column != null) MetadataObject.PrimaryColumn = column.MetadataObject;
            }
            if (jObj["associatedColumns"] is JArray cArray)
            {
                MetadataObject.AssociatedColumns.Clear();
                foreach(var jToken in cArray)
                {
                    if(jToken is JValue { Value: string associatedColumnName })
                    {
                        var column = cc.Calendar.Table.Columns.FindByName(associatedColumnName);
                        if (column != null) MetadataObject.AssociatedColumns.Add(column.MetadataObject);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);

            if (propertyName is nameof(PrimaryColumn) or nameof(TimeUnit))
            {
                Calendar.HandleCalendarMappingChange();
            }
        }
    }
}

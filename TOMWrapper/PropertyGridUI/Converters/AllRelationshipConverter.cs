using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    internal class AllRelationshipConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetModel(context).Relationships.OrderBy(r => r.Name).ToList());
        }

        private Model GetModel(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularNamedObject[]) return (context.Instance as ITabularNamedObject[]).First().Model;
            else if (context.Instance is ITabularObject tabularObject) return tabularObject.Model;
            else if (context.Instance is object[] objects && objects.First() is ITabularObject tabularObject2) return tabularObject2.Model;
            else return TabularModelHandler.Singleton.Model;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                var name = (string)value;
                if (string.IsNullOrEmpty(name)) return null;

                var model = GetModel(context);
                return model.Relationships.FirstOrDefault(r => r.Name == name);
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as Relationship)?.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI;

internal class QueryGroupConverter: TypeConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(GetModel(context).QueryGroups.OrderBy(r => r.Name).ToList());
    }

    private Model GetModel(ITypeDescriptorContext context)
    {
        if (context.Instance is ITabularObject[] tabularObjects) return tabularObjects.First().Model;
        if (context.Instance is ITabularNamedObject[] tabularObjects2) return tabularObjects2.First().Model;
        if (context.Instance is object[] tabularObjects3) return (tabularObjects3.First() as ITabularObject).Model;
        return (context.Instance as ITabularObject).Model;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value.GetType() == typeof(string))
        {
            var name = (string)value;
            if (string.IsNullOrEmpty(name)) return null;

            var model = GetModel(context);
            return model.QueryGroups.FirstOrDefault(r => r.Name == name);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(string))
            return true;
        return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string)) return (value as QueryGroup)?.Name;
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

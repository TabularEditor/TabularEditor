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
    internal class AllColumnConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetModel(context).Tables.SelectMany(t => t.Columns).OrderBy(c => c.DaxObjectFullName).ToList());
        }

        private Model GetModel(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularObject[]) return (context.Instance as ITabularObject[]).First().Model;
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

                var values = name.Split('[');
                if (values.Length != 2) return null;
                var tableName = values[0].StartsWith("'") ? values[0].Substring(1, values[0].Length - 2) : values[0];
                var columnName = values[1].Substring(0, values[1].Length - 1);

                var model = GetModel(context);
                if (!model.Tables.Contains(tableName)) throw new ArgumentException(string.Format("The model does not contain a table named \"{0}\"", tableName), context.PropertyDescriptor.Name);
                var table = model.Tables[tableName];
                if (!table.Columns.Contains(columnName)) throw new ArgumentException(string.Format("The table does not contain a column named \"{0}\"", columnName), context.PropertyDescriptor.Name);

                return table.Columns[columnName];
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
                return (value as Column)?.DaxObjectFullName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

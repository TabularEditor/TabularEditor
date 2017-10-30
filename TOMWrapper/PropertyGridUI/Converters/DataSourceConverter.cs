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
    internal class DataSourceConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.Instance is Partition || context.Instance is Partition[];
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Model(context).DataSources.OfType<ProviderDataSource>().ToList());
        }

        private Model Model(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularObject) return (context.Instance as ITabularObject).Model;
            if (context.Instance is ITabularObject[]) return (context.Instance as ITabularObject[]).First()?.Model;
            return null;
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

                var model = Model(context);
                if (!model.DataSources.Contains(name)) throw new ArgumentException(string.Format("The model does not contain a Data Source named \"{0}\"", name), context.PropertyDescriptor.Name);

                return model.DataSources[name];
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
                return (value as DataSource)?.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

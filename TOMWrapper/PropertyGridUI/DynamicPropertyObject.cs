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
    /// <summary>
    /// Implement this interface on objects that should be able to show/hide or change readonly
    /// state of properties at runtime. For example, an object that has a DataType and a FormatString
    /// property, might not want the FormatString property to show up whenever a certain DataType is
    /// in use.
    /// 
    /// Make sure that the implementing class uses the DynamicPropertyConverter as its TypeConverter,
    /// by decorating the class with:
    /// 
    /// [TypeConverter(typeof(DynamicPropertyConverter))]
    /// 
    /// </summary>
    internal interface IDynamicPropertyObject
    {
        bool Browsable(string propertyName);
        bool Editable(string propertyName);
    }

    /// <summary>
    /// This TypeConverter 
    /// </summary>
    internal class DynamicPropertyConverter: ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var original = base.GetProperties(context, value, attributes).Cast<PropertyDescriptor>();
            var multi = context.Instance.GetType().IsArray;

            IEnumerable<DynamicPropertyDescriptor> pds;

            if (value is IDynamicPropertyObject)
            {
                var obj = value as IDynamicPropertyObject;
                pds = original.Select(pd => new DynamicPropertyDescriptor(pd, multi, obj.Browsable(pd.Name), obj.Editable(pd.Name)));
            }
            else
            {
                pds = original.Select(pd => new DynamicPropertyDescriptor(pd, multi));
            }
            return new PropertyDescriptorCollection(pds.Where(pd => pd.IsBrowsable).ToArray());
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TabularObject) return (value as TabularObject).GetTypeName();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class DynamicPropertyNonExpandableConverter: TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var original = base.GetProperties(context, value, attributes).Cast<PropertyDescriptor>();
            var multi = context.Instance.GetType().IsArray;

            IEnumerable<DynamicPropertyDescriptor> pds;

            if (value is IDynamicPropertyObject)
            {
                var obj = value as IDynamicPropertyObject;
                pds = original.Select(pd => new DynamicPropertyDescriptor(pd, multi, obj.Browsable(pd.Name), obj.Editable(pd.Name)));
            }
            else
            {
                pds = original.Select(pd => new DynamicPropertyDescriptor(pd, multi));
            }
            return new PropertyDescriptorCollection(pds.Where(pd => pd.IsBrowsable).ToArray());
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TabularObject) return (value as TabularObject).GetTypeName();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class DynamicPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor _descriptor;
        private bool _multiselect;
        private bool _browsable;
        private bool _editable;

        public DynamicPropertyDescriptor(PropertyDescriptor descriptor, bool multiselect, bool browsable = true, bool editable = true) : base(descriptor)
        {
            _descriptor = descriptor;
            _multiselect = multiselect;
            _browsable = browsable;
            _editable = editable;
        }

        public override bool IsBrowsable
        {
            get
            {
                if(_multiselect)
                {
                    return _browsable && !base.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(NoMultiselectAttribute));
                } else
                    return _browsable && base.IsBrowsable;
            }
        }

        public override string Name
        {
            get
            {
                return string.IsNullOrEmpty(base.DisplayName) ? base.Name : base.DisplayName;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return _descriptor.ComponentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return !_editable;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _descriptor.PropertyType;
            }
        }

        public override bool CanResetValue(object component)
        {
            return _descriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return _descriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            _descriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _descriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _descriptor.ShouldSerializeValue(component);
        }
    }
}

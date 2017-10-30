using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.PropertyGridUI
{
    /// <summary>
    /// The VirtualObjectConverter provides functionality similar to the ExpandableObjectConverter.
    /// The only difference is, that the expanded properties may be inferred from a different object
    /// than the object on which the VirtualObjectConverter was assigned.
    /// 
    /// Derived classes must implement the GetObject() method, to provide the in place object.
    /// 
    /// For example, a FormatString property (of type string) can use the VirtualObjectConverter to
    /// show a set of subproperties in the property grid. These subproperties are inferred from a
    /// complex class that can be converter to and from the FormatString string.
    /// </summary>
    internal abstract class VirtualObjectConverter: TypeConverter
    {
        public abstract object GetObject(ITypeDescriptorContext context, object value);
        object baseObject = null;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if(baseObject == null) baseObject = GetObject(context, value);

            var propertyDescriptors = TypeDescriptor.GetProperties(baseObject).OfType<PropertyDescriptor>()
                .Select(pd => new VirtualObjectProperty(context, baseObject, pd))
                .Where(pd => pd.IsBrowsable)
                .ToArray();
            return new PropertyDescriptorCollection(propertyDescriptors);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private class VirtualObjectProperty : PropertyDescriptor
        {
            PropertyDescriptor baseProperty;
            object baseObject;
            PropertyGrid grid;
            public VirtualObjectProperty(ITypeDescriptorContext context, object baseObject, PropertyDescriptor baseProperty) : base(baseProperty)
            {
                this.baseObject = baseObject;
                this.baseProperty = baseProperty;

                PropertyInfo propInfo = context.GetType().GetProperty("OwnerGrid");
                grid = propInfo.GetValue(context) as PropertyGrid;
            }

            public override bool IsBrowsable
            {
                get
                {
                    var dpo = baseObject as IDynamicPropertyObject;
                    if (dpo != null) return dpo.Browsable(Name);
                    else return baseProperty.IsBrowsable;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    var dpo = baseObject as IDynamicPropertyObject;
                    if (dpo != null) return !dpo.Editable(Name);
                    else return baseProperty.IsReadOnly;
                }
            }

            protected override AttributeCollection CreateAttributeCollection()
            {
                return baseProperty.Attributes;
            }

            public override Type ComponentType
            {
                get
                {
                    return typeof(string);
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return baseProperty.PropertyType;
                }
            }

            public override bool CanResetValue(object component)
            {
                return baseProperty.CanResetValue(baseObject);
            }

            public override object GetValue(object component)
            {
                return baseProperty.GetValue(baseObject);
            }

            public override void ResetValue(object component)
            {
                baseProperty.ResetValue(baseObject);
            }

            public override void SetValue(object component, object value)
            {
                baseProperty.SetValue(baseObject, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return baseProperty.ShouldSerializeValue(baseObject);
            }
        }
    }
}

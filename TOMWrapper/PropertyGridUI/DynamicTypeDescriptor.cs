using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.DynamicProperties
{
    public class DynamicTypeDescriptor : ICustomTypeDescriptor
    {
        private ICustomTypeDescriptor td;
        private object instance;

        public DynamicTypeDescriptor(ICustomTypeDescriptor td, object instance)
        {
            this.td = td;
            this.instance = instance;
        }

        public AttributeCollection GetAttributes()
        {
            return td.GetAttributes();
        }

        public string GetClassName()
        {
            return td.GetClassName();
        }

        public string GetComponentName()
        {
            return td.GetComponentName();
        }

        public TypeConverter GetConverter()
        {
            return td.GetConverter();
        }

        public EventDescriptor GetDefaultEvent()
        {
            return td.GetDefaultEvent();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return td.GetDefaultProperty();
        }

        public object GetEditor(Type editorBaseType)
        {
            return td.GetEditor(editorBaseType);
        }

        public EventDescriptorCollection GetEvents()
        {
            return td.GetEvents();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return td.GetEvents(attributes);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[] { });
        }



        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var pdc = td.GetProperties();
            var props = pdc.Cast<PropertyDescriptor>();
            var toRemove = new HashSet<PropertyDescriptor>();

            if (instance.GetType().IsArray)
            {
                foreach (var pd in props.Where(p => p.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(NoMultiselectAttribute))))
                    toRemove.Add(pd);
            }

            foreach (var pd in props.Where(p => p.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(HidePropertyAttribute))))
            {
                var attr = pd.Attributes[typeof(HidePropertyAttribute)] as HidePropertyAttribute;
                var hide = (bool)pd.GetValue(instance);
                if (hide) toRemove.Add(props.First(p => p.Name == attr.PropertyName));
                toRemove.Add(pd);
            }

            return new PropertyDescriptorCollection(props.Except(toRemove).OrderBy(p => p.DisplayName).ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return td.GetPropertyOwner(pd);
        }
    }

    public class DynamicTypeDescriptionProvider: TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var td = TypeDescriptor.GetProvider(typeof(object)).GetTypeDescriptor(objectType, instance);
            return new DynamicTypeDescriptor(td, instance);
        }
    }
}

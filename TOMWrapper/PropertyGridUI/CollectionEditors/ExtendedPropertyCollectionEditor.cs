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
    internal class ExtendedPropertyCollectionEditor : RefreshGridCollectionEditor
    {
        ExtendedPropertyCollection Collection;

        public ExtendedPropertyCollectionEditor() : base(typeof(Collection<ExtendedProperty>))
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            if(itemType == typeof(StringExtendedProperty))
                return new StringExtendedProperty { Name = Collection.Parent.GetNewExtendedPropertyName(), Value = "" };
            if(itemType == typeof(JsonExtendedProperty))
                return new JsonExtendedProperty { Name = Collection.Parent.GetNewExtendedPropertyName(), Value = "" };

            throw new NotSupportedException();
        }

        protected override bool CanRemoveInstance(object value)
        {
            return true;
        }
        protected override Type[] CreateNewItemTypes()
        {
            return new[] { typeof(StringExtendedProperty), typeof(JsonExtendedProperty) };
        }

        protected override string GetDisplayText(object value)
        {
            return (value as ExtendedProperty).Name;
        }

        protected override object[] GetItems(object editValue)
        {
            Collection = (editValue as ExtendedPropertyCollection);
            return Collection.Keys.Select(k =>
            {
                var ep = Collection.Parent.GetExtendedPropertyType(k) == ExtendedPropertyType.Json ? new JsonExtendedProperty() : new StringExtendedProperty() as ExtendedProperty;
                ep.Name = k;
                ep.Value = Collection[k].ToString();
                return ep;
            }).ToArray();
        }

        protected override object SetItems(object editValue, object[] value)
        {
            foreach(ExtendedProperty an in value)
            {
                Collection.Parent.SetExtendedProperty(an.Name, an.Value, an.Type);
            }
            foreach(var n in Collection.Keys.ToList().Except(value.OfType<ExtendedProperty>().Select(an => an.Name)))
            {
                Collection.Parent.RemoveExtendedProperty(n);
            }

            return Collection;
        }
    }
}

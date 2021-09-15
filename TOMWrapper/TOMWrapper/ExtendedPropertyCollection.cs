using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public abstract class ExtendedProperty
    {
        public string Name { get; set; }
        [Editor(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Value { get; set; }
        public abstract ExtendedPropertyType Type { get; }
    }

    public sealed class JsonExtendedProperty: ExtendedProperty
    {
        public override ExtendedPropertyType Type => ExtendedPropertyType.Json;
    }
    public sealed class StringExtendedProperty: ExtendedProperty
    {
        public override ExtendedPropertyType Type => ExtendedPropertyType.String;
    }

    [TypeConverter(typeof(IndexerConverter))]
    public sealed class ExtendedPropertyCollection : IExpandableIndexer
    {
        public IExtendedPropertyObject Parent { get; private set; }

        internal ExtendedPropertyCollection(IExtendedPropertyObject parent)
        {
            Parent = parent;
        }

        public bool EnableMultiLine => true;

        public object this[string index]
        {
            get
            {
                return Parent.GetExtendedProperty(index);
            }

            set
            {
                if (value == null)
                {
                    Parent.RemoveExtendedProperty(index);
                    return;
                }
                var stringValue = value is string ? (string)value : JsonConvert.SerializeObject(value);
                Parent.SetExtendedProperty(index, stringValue, value is string ? ExtendedPropertyType.String : ExtendedPropertyType.Json);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return Parent.GetExtendedProperties();
            }
        }

        public string Summary
        {
            get
            {
                var n = Parent.GetExtendedPropertyCount();
                return string.Format("{0} extended propert{1}", n, n == 1 ? "y" : "ies");
            }
        }

        public string GetDisplayName(string key)
        {
            return key;
        }

        public void Refresh()
        {
            //
        }
    }
}

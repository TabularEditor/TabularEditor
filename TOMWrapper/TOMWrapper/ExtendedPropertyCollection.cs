using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

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
                Parent.SetExtendedProperty(index, value.ToString());
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

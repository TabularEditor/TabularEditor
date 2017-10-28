using System;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using TabularEditor.TOMWrapper;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace TabularEditor.PropertyGridUI
{
    /// <summary>
    /// This interface must be implemented by dictionary-type properties on a class, such as
    /// annotations, translations, etc.
    /// </summary>
    internal interface IExpandableIndexer
    {
        string Summary { get; }
        IEnumerable<string> Keys { get; }
        object this[string index] { get; set; }
        /// <summary>
        /// Gets the value to display for a given key. Note, this is not the value of the object represented by the key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetDisplayName(string key);
        //void Refresh();
    }

    /// <summary>
    /// This interface must be implemented by objects containing one or more property that
    /// should show up as a drop-down list in the PropertyGrid. The interface provides a
    /// means for the DropDown typeconverter to get the list of string items to display in
    /// the drop down.
    /// </summary>
    interface IDropDownProperties
    {
        string[] GetDropDownItems(string propertyName);
    }

    #region Handling string dictionaries in the PropertyGrid

    public class IndexerConverter: ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var pdc = new PropertyDescriptorCollection(null);
            var dict = value as IExpandableIndexer;
            //dict.Refresh();
            foreach(var key in dict.Keys.OrderBy(k => dict.GetDisplayName(k)))
            {
                PropertyDescriptor pd;

                pd = new DictionaryPropertyDescriptor(dict, key, context.PropertyDescriptor.Name, dict.GetDisplayName(key));
                pdc.Add(pd);
            }
            return pdc;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (context.Instance is object[]) return "Multiple objects selected.";
            return (context.PropertyDescriptor.GetValue(context.Instance) as IExpandableIndexer)?.Summary;
        }
    }

    class ColumnSelectConverter: StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var obj = context.Instance as IDropDownProperties;
            if (obj == null) return null;
            return new StandardValuesCollection(obj.GetDropDownItems(context.PropertyDescriptor.Name));
        }
    }

    class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        IExpandableIndexer _dictionary;
        string _key;
        string _propertyName;

        internal DictionaryPropertyDescriptor(IExpandableIndexer d, string key, string propertyName, string displayName)
            : base(displayName, null)
        {
            _propertyName = propertyName;
            _dictionary = d;
            _key = key;
        }
        public override Type PropertyType
        {
            get { return (_dictionary[_key] ?? string.Empty).GetType(); }
        }

        protected override void FillAttributes(IList attributeList)
        {
            attributeList.Add(new NotifyParentPropertyAttribute(true));

            // For annotations, make sure we enable multiline string editing:
            if(_dictionary is AnnotationCollection)
                attributeList.Add(new EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor)));

            base.FillAttributes(attributeList);
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key] ?? string.Empty;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
    #endregion
}
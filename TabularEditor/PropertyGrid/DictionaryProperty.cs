using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AnalysisServices.Tabular;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;

namespace TabularEditor
{
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

    /// <summary>
    /// This interface must be implemented by objects containing one or more string dictionaries
    /// that should be directly editable in a PropertyGrid.
    /// </summary>
    interface IStringDictionaryProperties
    {
        /// <summary>
        /// This method is called when an item of a dictionary is changed.
        /// </summary>
        void DictionaryItemChange(string dictionary, object key, object oldValue, object newValue);
        string GetDictionarySummary(string dictionary);
        object GetDefaultValue(string dictionary, object key);
    }

    public class DisplayFolderEditor: UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            TOMWrapper.TabularTableObject obj;
            if (context.Instance is TOMWrapper.TabularTableObject)
            {
                obj = context.Instance as TOMWrapper.TabularTableObject;
            }
            else if (context.Instance is object[] && ((object[])context.Instance)[0] is TOMWrapper.TabularTableObject)
            {
                obj = ((object[])context.Instance)[0] as TOMWrapper.TabularTableObject;
            }
            else
                return null;

            var tfc = new TOMWrapper.TabularFolderCollection(obj.Table, new TOMWrapper.TabularObjectCache(null), null, null);

            using (var folderSelection = new FormDisplayFolderSelect())
            {
                folderSelection.FolderNodes.Clear();

                foreach(var node in tfc.RootFolders)
                {
                    folderSelection.FolderNodes.Add(TabularFolderToTreeNode(node));

                }
                var res = svc.ShowDialog(folderSelection);
                if(res == DialogResult.OK)
                {
                    return folderSelection.SelectedFolder;
                }
            }
            return null;
        }

        private TreeNode TabularFolderToTreeNode(TOMWrapper.TabularFolder folder)
        {
            var result = new TreeNode(folder.Name);
            result.Tag = folder.Path;
            foreach(var c in folder.ChildFolders)
            {
                result.Nodes.Add(TabularFolderToTreeNode(c));
            }
            return result;
        }
    }

    #region Handling string dictionaries in the PropertyGrid
    public class NoEditor: UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }

    public class DictionaryConverter: ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var pdc = new PropertyDescriptorCollection(null);
            var dict = value as IDictionary;
            foreach(DictionaryEntry kvp in dict)
            {
                PropertyDescriptor pd;
                if (context.Instance is object[])
                    pd = new DictionaryPropertyDescriptor(dict, kvp.Key, context.PropertyDescriptor.Name, (context.Instance as object[]).Cast<IStringDictionaryProperties>().ToArray());
                else if (context.Instance is IStringDictionaryProperties)
                    pd = new DictionaryPropertyDescriptor(dict, kvp.Key, context.PropertyDescriptor.Name, context.Instance as IStringDictionaryProperties);
                else continue;
                pdc.Add(pd);
            }
            return pdc;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (context.Instance is object[]) return "Multiple objects selected.";

            return (context.Instance as IStringDictionaryProperties).GetDictionarySummary(context.PropertyDescriptor.Name);
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
        IDictionary _dictionary;
        object _key;
        IStringDictionaryProperties _parent;
        IStringDictionaryProperties[] _parents;
        string _propertyName;
        public string DictionaryName { get { return _propertyName; } }

        internal DictionaryPropertyDescriptor(IDictionary d, object key, string propertyName, IStringDictionaryProperties[] parents)
            : base(key.ToString(), null)
        {
            _parents = parents;
            _propertyName = propertyName;
            _dictionary = d;
            _key = key;
        }

        internal DictionaryPropertyDescriptor(IDictionary d, object key, string propertyName, IStringDictionaryProperties parent)
            : base(key.ToString(), null)
        {
            _parent = parent;
            _propertyName = propertyName;
            _dictionary = d;
            _key = key;
        }
        public override Type PropertyType
        {
            get { return (_dictionary[_key] ?? string.Empty).GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            var oldValue = _dictionary[_key];
            _dictionary[_key] = value;
            if (_parent != null) _parent.DictionaryItemChange(_propertyName, Name, oldValue, value);
            if (_parents != null) foreach (var p in _parents) p.DictionaryItemChange(_propertyName, Name, oldValue, value);
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
            if (_parent == null) return false;

            var val = _dictionary[_key];
            var defaultVal = _parent.GetDefaultValue(_propertyName, _key);
            return val != defaultVal;
        }
    }
    #endregion
}
using Microsoft.AnalysisServices.Tabular;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System;

namespace TabularEditor.TOMWrapper
{
    public enum DataCategory
    {
        Uncategorized,
        Address,
        City,
        Continent,
        Country,
        County,
        Image,
        ImageUrl,
        Latitude,
        Longitude,
        Organization,
        Place,
        PostalCode,
        StateOrProvince,
        WebUrl
    }

    public abstract class TabularTableObject : TabularNamedObject
    {
        [Browsable(false)]
        public abstract Table Table { get; }

        public bool ShouldSerializeDisplayFolder() { return false; }
        [NoCultureBrowsable,Category("Basic"),DisplayName("Display Folder"),MultiSelectBrowsable,Description("The Display Folder in which client tools will show the object. Use \\ (backslash) to create subfolders.")]
        [Editor(typeof(DisplayFolderEditor),typeof(UITypeEditor))]
        public virtual string DisplayFolder { get { return GetDisplayFolder(null); } set { SetDisplayFolder(value, null); } }

        public bool ShouldSerializeLocalDisplayFolder() { return false; }
        [Category("Translations"), DisplayName("Translated Display Folder"), MultiSelectBrowsable,Description("The Display Folder of the object using the current culture.")]
        [Editor(typeof(DisplayFolderEditor), typeof(UITypeEditor))]
        public virtual string LocalDisplayFolder
        {
            get { return GetDisplayFolder(TabularLogicalTree.CurrentCulture); }
            set { SetDisplayFolder(value, TabularLogicalTree.CurrentCulture); }
        }

        [NoCultureBrowsable,Category("Translations"),DisplayName("Display Folder Translations"),MultiSelectBrowsable]
        [TypeConverter(typeof(DictionaryConverter)),Editor(typeof(NoEditor), typeof(UITypeEditor))]
        public virtual IDictionary<string, string> DisplayFolderTranslations
        {
            get
            {
                return Model.Cultures.ToDictionary(c => c.Name, c => c.ObjectTranslations[MetadataObject, TranslatedProperty.DisplayFolder]?.Value ?? null);
            }
        }

        public abstract string GetDisplayFolder(Culture culture);
        public abstract void SetDisplayFolder(string folder, Culture culture);

        public override string GetDictionarySummary(string dictionary)
        {
            if(dictionary == "DisplayFolderTranslations")
            {
                var fCount = Model.Cultures.Count;
                var fEmpty = DisplayFolderTranslations.Values.Count(v => string.IsNullOrEmpty(v) && v != GetDisplayFolder(null));
                var fDefault = DisplayFolderTranslations.Values.Count(v => v == GetDisplayFolder(null));
                return string.Format("{0} empty. {1} default. {2} translated.", fEmpty, fDefault, fCount - fEmpty - fDefault);
            }
            else return base.GetDictionarySummary(dictionary);
        }
        public override object GetDefaultValue(string dictionary, object key)
        {
            if (dictionary == "DisplayFolderTranslations")
            {
                return GetDisplayFolder(null);
            }
            return base.GetDefaultValue(dictionary, key);
        }
        public override void DictionaryItemChange(string dictionary, object key, object oldValue, object newValue)
        {
            if (dictionary == "DisplayFolderTranslations")
            {
                SetDisplayFolder(newValue as string, Model.Cultures[key as string]);
            }
            else base.DictionaryItemChange(dictionary, key, oldValue, newValue);
        }
    }

    
    
    
}

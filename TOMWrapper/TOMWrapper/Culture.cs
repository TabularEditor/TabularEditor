using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper
{
    public partial class Culture: IDynamicPropertyObject
    {
        #region Translation Statistics
        [DisplayName("Translated Measure Names"),Browsable(true),Category("Translation Statistics")]
        public string StatsMeasureCaptions
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.Caption && o.Object is TOM.Measure && (o.Object as NamedMetadataObject).Name != o.Value && !string.IsNullOrEmpty(o.Value)),
                    Model.Tables.SelectMany(t => t.Measures).Count());
            }
        }

        [DisplayName("Translated Column Names"), Browsable(true), Category("Translation Statistics")]
        public string StatsColumnCaptions
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.Caption && o.Object is TOM.Column && (o.Object as NamedMetadataObject).Name != o.Value && !string.IsNullOrEmpty(o.Value)),
                    Model.Tables.SelectMany(t => t.Columns).Count());
            }
        }

        [DisplayName("Translated Hierarchy Names"), Browsable(true), Category("Translation Statistics")]
        public string StatsHierarchyCaptions
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.Caption && o.Object is TOM.Hierarchy && (o.Object as NamedMetadataObject).Name != o.Value && !string.IsNullOrEmpty(o.Value)),
                    Model.Tables.SelectMany(t => t.Hierarchies).Count());
            }
        }

        [DisplayName("Translated Level Names"), Browsable(true), Category("Translation Statistics")]
        public string StatsLevelCaptions
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.Caption && o.Object is TOM.Level && (o.Object as NamedMetadataObject).Name != o.Value && !string.IsNullOrEmpty(o.Value)),
                    Model.Tables.SelectMany(t => t.Hierarchies.SelectMany(h => h.Levels)).Count());
            }
        }

        [DisplayName("Translated Table Names"), Browsable(true), Category("Translation Statistics")]
        public string StatsTableCaptions
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.Caption && o.Object is TOM.Table && (o.Object as NamedMetadataObject).Name != o.Value && !string.IsNullOrEmpty(o.Value)),
                    Model.Tables.Count());
            }
        }

        [DisplayName("Translated Measure Folders"), Browsable(true), Category("Translation Statistics")]
        public string StatsMeasureDisplayFolders
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.DisplayFolder && o.Object is TOM.Measure && (o.Object as NamedMetadataObject).Name != o.Value),
                    Model.Tables.SelectMany(t => t.Measures).Count());
            }
        }

        [DisplayName("Translated Column Folders"), Browsable(true), Category("Translation Statistics")]
        public string StatsColumnDisplayFolders
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.DisplayFolder && o.Object is TOM.Column && (o.Object as NamedMetadataObject).Name != o.Value),
                    Model.Tables.SelectMany(t => t.Columns).Count());
            }
        }

        [DisplayName("Translated Hierarchy Folders"), Browsable(true), Category("Translation Statistics")]
        public string StatsHierarchyDisplayFolders
        {
            get
            {
                return string.Format("{0} of {1}",
                    MetadataObject.ObjectTranslations.Count(o => o.Property == TranslatedProperty.DisplayFolder && o.Object is TOM.Hierarchy && (o.Object as NamedMetadataObject).Name != o.Value),
                    Model.Tables.SelectMany(t => t.Hierarchies).Count());
            }
        }

        #endregion

        public override TabularNamedObject Clone(string newName, bool includeTranslations)
        {
            Handler.BeginUpdate("duplicate translation");
            var tom = MetadataObject.Clone();
            ////tom.IsRemoved = false;
            tom.Name = newName;
            var c = new Culture(Handler, tom);
            Model.Cultures.Add(c);

            Handler.EndUpdate();

            return c;
        }


        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Culture();
            MetadataObject.CopyTo(tom);
            ////tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        [Browsable(false)]
        public bool Unassigned { get; private set; } = false;

        public Culture() : base(TabularModelHandler.Singleton, new Microsoft.AnalysisServices.Tabular.Culture() { Name = TabularModelHandler.Singleton.Model.Cultures.MetadataObjectCollection.GetNewName("Culture") }, false)
        {
            Unassigned = true;
        }

        public Culture(string cultureId): base(TabularModelHandler.Singleton, new TOM.Culture() { Name = cultureId }, false)
        {

        }

        [Browsable(false)]
        public string DisplayName {
            get {
                if (_displayName == null) UpdateDisplayName();
                return _displayName;
            }
        }
        private string _displayName = null;

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);
            if (propertyName == "Name") base.OnPropertyChanged("DisplayName", null, newValue);
        }

        [Browsable(false)]
        public ObjectTranslationCollection ObjectTranslations { get { return MetadataObject.ObjectTranslations; } }

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Name":
                case "ObjectType":
                    return true;
                default: return propertyName.StartsWith("Stats");
            }
            
        }

        public bool Editable(string propertyName)
        {
            return propertyName == "Name";
        }

        [TypeConverter(typeof(CultureConverter)), NoMultiselect()]
        public override string Name
        {
            get
            {
                return MetadataObject.Name;
            }

            set
            {
                var oldValue = MetadataObject.Name;

                Unassigned = false;
                MetadataObject.Name = value;
                UpdateDisplayName();

                Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Name", oldValue, value));
                Handler.UpdateObject(this);
                OnPropertyChanged("Name", oldValue, value);
            }
        }

        private void UpdateDisplayName()
        {
            _displayName = Unassigned ? Name : string.Format("{0} -- ({1})", CultureInfo.GetCultureInfo(Name).DisplayName, Name);            
        }
    }

    public class CultureConverter: TypeConverter
    {
        Dictionary<string, CultureInfo> Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToDictionary(c => c.Name, c => c);

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Cultures.Values.OrderBy(c => c.DisplayName).Select(c => c.Name + " - " + c.DisplayName).ToList());
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (value as string).Split(' ').First();
            }
            else
                throw new InvalidOperationException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is string)
            {
                var cn = (value as string).Split(' ').First();
                CultureInfo c;
                if(Cultures.TryGetValue(cn, out c))
                {
                    return c.Name + " - " + c.DisplayName;
                }
                return "Unknown culture";
            }
            else
                throw new InvalidOperationException();
        }        
    }
}

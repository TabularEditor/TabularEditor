extern alias json;

using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper.Undo;
using json.Newtonsoft.Json;

namespace TabularEditor.TOMWrapper
{
    public partial class Culture
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

        [Browsable(false)]
        public bool Unassigned { get { return !CultureConverter.Cultures.ContainsKey(Name); } }

        internal Culture(string cultureId): base(new TOM.Culture() { Name = cultureId })
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
            if (propertyName == Properties.NAME) base.OnPropertyChanged("DisplayName", null, newValue);
        }

        [Browsable(false)]
        public ObjectTranslationCollection ObjectTranslations { get { return MetadataObject.ObjectTranslations; } }

        [TypeConverter(typeof(CultureConverter)), NoMultiselect(), DisplayName("Language")]
        public override string Name
        {
            get
            {
                return MetadataObject.Name;
            }

            set
            {
                var oldValue = MetadataObject.Name;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.NAME, value, ref undoable, ref cancel);
                if (cancel) return;

                MetadataObject.Name = value;
                UpdateDisplayName();

                Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.NAME, oldValue, value));
                Handler.UpdateObjectName(this);
                OnPropertyChanged(Properties.NAME, oldValue, value);
            }
        }

        private void UpdateDisplayName()
        {
            _displayName = Unassigned ? Name : string.Format("{0} -- ({1})", CultureInfo.GetCultureInfo(Name).DisplayName, Name);            
        }
    }

    public partial class CultureCollection
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.Select(c => c.Name).OrderBy(n => n).ToArray());
        }

        public void FromJson(string json)
        {
            var cultures = JsonConvert.DeserializeObject<string[]>(json);

            foreach(var c in cultures)
            {
                Handler.Model.AddTranslation(c);
            }
        }
    }
}

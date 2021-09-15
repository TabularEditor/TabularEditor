using Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using System.Text;
using System.IO;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(IndexerConverter))]
    public sealed class TranslationIndexer : IEnumerable<string>, IExpandableIndexer
    {
        public string ToJson()
        {
            // Here, we do a custom serialization, as there seems to be no way to specify the ordering when serializing a Dictionary using JsonConvert:
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using(JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                foreach(var lang in Keys.Where(k => !string.IsNullOrEmpty(this[k])).OrderBy(k => k))
                {
                    writer.WritePropertyName(lang);
                    writer.WriteValue(this[lang]);
                }
                writer.WriteEndObject();
            }
            return sb.ToString();
        }

        bool IExpandableIndexer.EnableMultiLine => false;

        public Dictionary<string, string> Copy()
        {
            return Keys.ToDictionary(k => k, k => this[k]);
        }

        [IntelliSense("Copy translations from another translation collection.")]
        public void CopyFrom(TranslationIndexer translations, Func<string,string> mutator = null)
        {
            foreach (var key in translations.Keys)
            {
                var value = translations[key];
                if (value == null) continue;
                if (mutator != null) value = mutator(value);
                this[key] = translations[key];
            }
        }

        public void CopyFrom(IDictionary<string, string> source)
        {
            foreach(var kvp in source)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        public bool IsEmpty
        {
            get { return Keys.All(k => string.IsNullOrEmpty(this[k])); }
        }

        /// <summary>
        /// Resets the translations of the object. Caption translations are removed, making the object appear with
        /// the base name in all locales. Display Folder and Description translations are set to the untranslated
        /// value of the object.
        /// </summary>
        [IntelliSense("Resets all translations.")]
        public void Reset()
        {
            _tabularObject.Handler.BeginUpdate(string.Format("reset {0} translations", _translatedProperty.ToString().ToLower()));
            switch(_translatedProperty)
            {
                case TranslatedProperty.Caption:
                    SetAll(null); break;
                case TranslatedProperty.Description:
                    SetAll((_tabularObject as IDescriptionObject).Description); break;
                case TranslatedProperty.DisplayFolder:
                    SetAll((_tabularObject as IFolderObject).DisplayFolder); break;
            }
            _tabularObject.Handler.EndUpdate();
        }

        /// <summary>
        /// Clears all translated values for the object.
        /// </summary>
        [IntelliSense("Clear all translations.")]
        public void Clear()
        {
            _tabularObject.Handler.BeginUpdate(string.Format("clear {0} translations", _translatedProperty.ToString().ToLower()));
            SetAll(null);
            _tabularObject.Handler.EndUpdate();
        }

        [IntelliSense("Sets all translations to the specified value.")]
        public void SetAll(string value)
        {
            _tabularObject.Handler.BeginUpdate("translations");
            foreach (var key in Keys) this[key] = value;
            _tabularObject.Handler.EndUpdate();
        }

        public void Refresh()
        {

        }

        public string GetDisplayName(string key)
        {
            return Cultures[key].DisplayName;
        }

        public string DefaultValue
        {
            get
            {
                switch(_translatedProperty)
                {
                    case TranslatedProperty.Caption: return (_tabularObject as TabularNamedObject).Name;
                    case TranslatedProperty.Description: return (_tabularObject as IDescriptionObject).Description;
                    case TranslatedProperty.DisplayFolder: return (_tabularObject as IFolderObject).DisplayFolder;
                    default:
                        return null;
                }
            }
        }

        private TabularObject _tabularObject;
        private TranslatedProperty _translatedProperty;

        private CultureCollection Cultures { get { return _tabularObject.Handler.Model.Cultures; } }

        public string Summary
        {
            get
            {
                var def = DefaultValue;

                return string.Format("{0} empty, {1} translated, {2} default",
                    this.Count(t => string.IsNullOrEmpty(t)),
                    this.Count(t => t != def && !string.IsNullOrEmpty(t)),
                    this.Count(t => t == def)
                    );
            }
        }

        public int TranslatedCount
        {
            get
            {
                return this.Count(t => t != DefaultValue && !string.IsNullOrEmpty(t));
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return Cultures.Select(c => c.Name);
            }
        }

        object IExpandableIndexer.this[string index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (string)value;
            }
        }

        internal TranslationIndexer(TabularObject tabularObject, TranslatedProperty translatedProperty)
        {
            _tabularObject = tabularObject;
            _translatedProperty = translatedProperty;

        }
        private string GetPropertyName()
        {
            switch (_translatedProperty)
            {
                case TranslatedProperty.Caption:
                    return "TranslatedNames";
                case TranslatedProperty.Description:
                    return "TranslatedDescriptions";
                case TranslatedProperty.DisplayFolder:
                    return "TranslatedDisplayFolders";
            }
            throw new InvalidOperationException();
        }

        public bool Contains(Culture culture)
        {
            return GetTrans(culture) != null;
        }

        private ObjectTranslation GetTrans(Culture culture)
        {
            return culture.ObjectTranslations[_tabularObject.MetadataObject, _translatedProperty];
        }

        public string this[Culture culture]
        {
            get
            {
                return GetTrans(culture)?.Value ?? string.Empty;
            }
            set
            {
                if (!(_tabularObject is ITranslatableObject)) return;

                var oldValue = GetTrans(culture)?.Value;
                if (value == oldValue) return;

                _tabularObject.Handler.UndoManager.Add(
                    new UndoPropertyChangedAction(_tabularObject, GetPropertyName(), oldValue, value, culture.Name));

                // A null value removes the translation completely (typically when the object is deleted):
                if (value == null)
                {
                    var t = GetTrans(culture);
                    if (t != null) culture.ObjectTranslations.Remove(GetTrans(culture));
                    return;
                }

                // For captions, we don't allow blank translations. In case a blank value is provided, completely remove the translation:
                if (_translatedProperty == TranslatedProperty.Caption && string.IsNullOrWhiteSpace(value))
                {
                    var t = GetTrans(culture);
                    if (t != null) culture.ObjectTranslations.Remove(t);
                }
                else
                    culture.ObjectTranslations.SetTranslation(_tabularObject.MetadataObject, _translatedProperty, value);

                if (_tabularObject.Handler.Tree.Culture == culture)
                {
                    if (_translatedProperty == TranslatedProperty.DisplayFolder)
                        _tabularObject.Handler.UpdateFolders((_tabularObject as ITabularTableObject).Table);
                    else if (_translatedProperty == TranslatedProperty.Caption)
                        _tabularObject.Handler.UpdateObjectName(_tabularObject as TabularNamedObject);
                }
            }
        }
        public string this[string cultureName]
        {
            get
            {

                return Cultures.Contains(cultureName) ? this[GetCulture(cultureName)] : null;
            }
            set
            {
                if(Cultures.Contains(cultureName)) this[GetCulture(cultureName)] = value;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Cultures.Select(c => this[c]).GetEnumerator();
        }

        private Culture GetCulture(string cultureName)
        {
            // Get the actual value from the RoleOLSIndexer:
            return Cultures[cultureName];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Get the a
            return GetEnumerator();
        }
    }

}

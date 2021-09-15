using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Linguistics;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(IndexerConverter))]
    public sealed class SynonymIndexer : IEnumerable<string>, IExpandableIndexer
    {
        bool IExpandableIndexer.EnableMultiLine => true;

        public bool IsEmpty
        {
            get { return Keys.All(k => string.IsNullOrEmpty(this[k])); }
        }

        public void Refresh()
        {

        }

        public string GetDisplayName(string key)
        {
            return Cultures[key].DisplayName;
        }

        private TabularNamedObject _tabularObject;

        private CultureCollection Cultures { get { return _tabularObject.Handler.Model.Cultures; } }

        public string Summary
        {
            get
            {
                var linguisticSchemasDefined = LinguisticCultures.Count();
                return $"{linguisticSchemasDefined} linguistic {(linguisticSchemasDefined == 1 ? "schema" : "schemas")} defined";
            }
        }

        public IEnumerable<Culture> LinguisticCultures => Cultures.Where(c => c.ContentType == ContentType.Json && !string.IsNullOrEmpty(c.Content));

        public IEnumerable<string> Keys
        {
            get
            {
                return LinguisticCultures.Select(c => c.Name);
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

        internal SynonymIndexer(TabularNamedObject tabularObject)
        {
            _tabularObject = tabularObject;

        }

        public string this[Culture culture]
        {
            get
            {
                return SynonymHelper.GetSynonyms(_tabularObject, culture);
            }
            set
            {
                // TODO: PropertyChanging call and check cancel state
                var oldValue = this[culture];
                SynonymHelper.SetSynonyms(_tabularObject, culture, value);
                _tabularObject.Handler.UndoManager.Add(
                    new UndoPropertyChangedAction(_tabularObject, nameof(ISynonymObject.Synonyms), oldValue, value, culture.Name));
                // TODO: PropertyChanged call
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

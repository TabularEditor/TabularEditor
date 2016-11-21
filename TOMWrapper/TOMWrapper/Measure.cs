using System;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Measure : ITabularPerspectiveObject, IDaxObject, IDynamicPropertyObject, IClonableObject
    {
        [Browsable(true), DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

        [IntelliSense("Deletes the measure from the table.")]
        public override void Delete()
        {
            InPerspective.None();
            base.Delete();

            if (KPI != null) Handler.WrapperLookup.Remove(MetadataObject.KPI);
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Measure();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            if (MetadataObject.KPI != null)
            {
                new KPI(Handler, MetadataObject.KPI);
            }

            base.Undelete(collection);
        }

        public TabularNamedObject CloneTo(Table table, string newName = null, bool includeTranslations = true)
        {
            Handler.BeginUpdate("duplicate measure");
            var tom = MetadataObject.Clone();
            tom.IsRemoved = false;
            tom.Name = table.Measures.MetadataObjectCollection.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);
            var m = new Measure(Handler, tom);
            table.Measures.Add(m);

            if (includeTranslations)
            {
                m.TranslatedDescriptions.CopyFrom(TranslatedDescriptions);
                m.TranslatedDisplayFolders.CopyFrom(TranslatedDisplayFolders);
                if (string.IsNullOrEmpty(newName))
                    m.TranslatedNames.CopyFrom(TranslatedNames, n => n + " copy");
                else
                    m.TranslatedNames.CopyFrom(TranslatedNames, n => n.Replace(Name, newName));
            }
            m.InPerspective.CopyFrom(InPerspective);

            Handler.EndUpdate();

            return m;
        }

        public override TabularNamedObject Clone(string newName = null, bool includeTranslations = true)
        {
            return CloneTo(Table, newName, includeTranslations);
        }

        protected override void Init()
        {
            if (MetadataObject.KPI != null) new KPI(Handler, MetadataObject.KPI);
            InPerspective = new PerspectiveMeasureIndexer(this);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Expression") NeedsValidation = true;

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;

        public bool Browsable(string propertyName)
        {
            switch (propertyName) {
                case "FormatString": return DataType != TOM.DataType.String;
                default: return true;
            }
        }

        public bool Editable(string propertyName)
        {
            if (propertyName == "DisplayFolder" && Expression == "test") return false;
            return true;
        }

        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("[{0}]", Name.Replace("]", "]]"));
            }
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return DaxObjectName;
            }
        }

        [Browsable(false)]
        public string DaxTableName
        {
            get
            {
                return Table.DaxTableName;
            }
        }
    }
}

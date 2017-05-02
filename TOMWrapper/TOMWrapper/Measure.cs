using System;
using System.Collections.Generic;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Measure : ITabularPerspectiveObject, IDaxObject, IDynamicPropertyObject
    {
        [Browsable(false)]
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; internal set; } = new Dictionary<IDaxObject, List<Dependency>>();
        [Browsable(false)]
        public HashSet<IExpressionObject> Dependants { get; private set; } = new HashSet<IExpressionObject>();


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
            //////tom.IsRemoved = false;
            MetadataObject = tom;

            if (MetadataObject.KPI != null)
            {
                new KPI(MetadataObject.KPI);
            }

            base.Undelete(collection);
        }

        public TabularNamedObject CloneTo(Table table, string newName = null, bool includeTranslations = true)
        {
            Handler.BeginUpdate("duplicate measure");
            var tom = MetadataObject.Clone();
            ////tom.IsRemoved = false;
            tom.Name = table.Measures.MetadataObjectCollection.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);
            var m = new Measure(tom);
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

        /*public override TabularNamedObject Clone(string newName = null, bool includeTranslations = true)
        {
            return CloneTo(Table, newName, includeTranslations);
        }*/

        protected override void Init()
        {
            if (MetadataObject.KPI != null) new KPI(MetadataObject.KPI);
            InPerspective = new PerspectiveMeasureIndexer(this);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Expression")
            {
                NeedsValidation = true;
                Handler.BuildDependencyTree(this);
            }
            if (propertyName == "Name" && Handler.AutoFixup)
            {
                Handler.DoFixup(this, (string)newValue);
                Handler.UndoManager.EndBatch();
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == "Name")
            {
                Handler.BuildDependencyTree();

                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.AutoFixup) Handler.UndoManager.BeginBatch("Name change");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;
        
        public bool Browsable(string propertyName)
        {
            switch (propertyName) {
                case "FormatString": return DataType != TOM.DataType.String;
                case "DetailRowsExpression":
                    return Model.Database.CompatibilityLevel >= 1400;
                default: return true;
            }
        }

        [DisplayName("Detail Rows Expression")]
        [Category("Options"), IntelliSense("A DAX expression specifying detail rows for this measure (drill-through in client tools).")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DetailRowsExpression
        {
            get
            {
                return MetadataObject.DetailRowsDefinition?.Expression;
            }
            set
            {
                var oldValue = DetailRowsExpression;

                if (oldValue == value) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging("DetailRowsExpression", value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DetailRowsDefinition == null) MetadataObject.DetailRowsDefinition = new TOM.DetailRowsDefinition();
                MetadataObject.DetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.DetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "DetailRowsExpression", oldValue, value));
                OnPropertyChanged("DetailRowsExpression", oldValue, value);
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Measure : IDaxObject, ITabularObjectContainer
    {
        [Browsable(false)]
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; internal set; } = new Dictionary<IDaxObject, List<Dependency>>();
        [Browsable(false)]
        public HashSet<IDAXExpressionObject> Dependants { get; private set; } = new HashSet<IDAXExpressionObject>();



        internal override ITabularObjectCollection GetCollectionForChild(TabularObject child)
        {
            if (child is KPI) return null;
            return base.GetCollectionForChild(child);
        }
        public KPI AddKPI()
        {
            var kpi = new TOM.KPI() { };
            KPI = KPI.CreateFromMetadata(kpi, true);
            return KPI;
        }

        internal override void RemoveReferences()
        {
            // TODO: Make sure KPIs can be deleted and undeleted - how do we even edit KPIs right now in Tabular Editor?
            //if (KPI != null) KPI.Delete();
            base.RemoveReferences();
        }


        /*public TabularNamedObject CloneTo(Table table, string newName = null, bool includeTranslations = true)
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
        }*/

        /*public override TabularNamedObject Clone(string newName = null, bool includeTranslations = true)
        {
            return CloneTo(Table, newName, includeTranslations);
        }*/

        protected override void Init()
        {
            if (MetadataObject.KPI != null) this.KPI = KPI.CreateFromMetadata(MetadataObject.KPI);
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

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case "FormatString": return DataType != TOM.DataType.String;
                case "DetailRowsExpression":
                    return Model.Database.CompatibilityLevel >= 1400;
                default: return true;
            }
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            if (KPI == null) return Enumerable.Empty<ITabularNamedObject>();
            else return Enumerable.Repeat<ITabularNamedObject>(KPI, 1);
        }

#if CL1400
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
#endif

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

    public partial class MeasureCollection
    {
        public override string GetNewName(string prefix = null)
        {
            // For measures, we must ensure that the new measure name is unique across all tables,
            // which is why we have to override the GetNewName method here.

            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Measure";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (Model.AllMeasures.Any(m => m.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }

        public static string GetNewMeasureName(string prefix)
        {
            // For measures, we must ensure that the new measure name is unique across all tables,
            // which is why we have to override the GetNewName method here.

            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Measure";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (TabularModelHandler.Singleton.Model.AllMeasures.Any(m => m.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }
    }
}

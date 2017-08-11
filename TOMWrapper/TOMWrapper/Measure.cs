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
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; } = new Dictionary<IDaxObject, List<Dependency>>();
        [Browsable(false)]
        public HashSet<IDAXExpressionObject> Dependants { get; } = new HashSet<IDAXExpressionObject>();

        public override bool CanDelete(out string message)
        {
            message = string.Empty;
            if (Dependants.Count > 0) message += Messages.ReferencedByDAX;
            if (message == string.Empty) message = null;
            return true;
        }

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
        public void RemoveKPI()
        {
            KPI = null;
        }

        private KPI KPIBackup;
        internal override void RemoveReferences()
        {
            KPIBackup = KPI;
            base.RemoveReferences();
        }

        internal override void Reinit()
        {
            if(KPIBackup != null)
            {
                Handler.WrapperLookup.Remove(KPIBackup.MetadataObject);
                KPIBackup.MetadataObject = MetadataObject.KPI;
                Handler.WrapperLookup.Add(KPIBackup.MetadataObject, KPIBackup);
            }
            base.Reinit();
        }

        /// <summary>
        /// Gets or sets the KPI of the Measure.
        /// </summary>
		[DisplayName("KPI")]
        [Category("Other"), IntelliSense("The KPI of this Measure.")]
        public KPI KPI
        {
            get
            {
                if (MetadataObject.KPI == null) return null;
                return Handler.WrapperLookup[MetadataObject.KPI] as KPI;
            }
            set
            {
                var oldValue = KPI;
                if (oldValue?.MetadataObject == value?.MetadataObject) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging("KPI", value, ref undoable, ref cancel);
                if (cancel) return;

                var newKPI = value?.MetadataObject;
                if(newKPI != null && newKPI.IsRemoved)
                {
                    Handler.WrapperLookup.Remove(newKPI);
                    newKPI = newKPI.Clone();
                    value.MetadataObject = newKPI;
                    Handler.WrapperLookup.Add(newKPI, value);
                }

                MetadataObject.KPI = newKPI;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "KPI", oldValue, value));
                OnPropertyChanged("KPI", oldValue, value);
            }
        }

        protected override void Init()
        {
            if (MetadataObject.KPI != null) this.KPI = KPI.CreateFromMetadata(MetadataObject.KPI);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == Properties.EXPRESSION)
            {
                NeedsValidation = true;
                Handler.BuildDependencyTree(this);
            }
            if (propertyName == Properties.NAME && Handler.AutoFixup)
            {
                Handler.DoFixup(this);
                Handler.UndoManager.EndBatch();
            }
            if (propertyName == Properties.KPI)
            {
                Handler.Tree.OnStructureChanged(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.NAME)
            {
                Handler.BuildDependencyTree();

                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.AutoFixup) Handler.UndoManager.BeginBatch("Set Property 'Name'");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.FORMATSTRING: return DataType != DataType.String && Description != "hej";
#if CL1400
                case Properties.DETAILROWSDEFINITION:
                    return Model.Database.CompatibilityLevel >= 1400;
#endif
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
        public string DetailRowsDefinition
        {
            get
            {
                return MetadataObject.DetailRowsDefinition?.Expression;
            }
            set
            {
                var oldValue = DetailRowsDefinition;

                if (oldValue == value) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DETAILROWSDEFINITION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DetailRowsDefinition == null) MetadataObject.DetailRowsDefinition = new TOM.DetailRowsDefinition();
                MetadataObject.DetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.DetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.DETAILROWSDEFINITION, oldValue, value));
                OnPropertyChanged(Properties.DETAILROWSDEFINITION, oldValue, value);
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

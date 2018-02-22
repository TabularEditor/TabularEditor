using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Measure : IDaxObject, ITabularObjectContainer, IDaxDependantObject
    {
        private DependsOnList _dependsOn = null;

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        [Browsable(false)]
        public ReferencedByList ReferencedBy { get; } = new ReferencedByList();

        protected override bool AllowDelete(out string message)
        {
            message = string.Empty;
            if (ReferencedBy.Count > 0) message += Messages.ReferencedByDAX;
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
            KPI = KPI.CreateFromMetadata(this, kpi);
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
            if (MetadataObject.KPI != null) this.KPI = KPI.CreateFromMetadata(this, MetadataObject.KPI);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch(propertyName)
            {
                case Properties.DETAILROWSEXPRESSION:
                case Properties.EXPRESSION:
                    NeedsValidation = true;
                    FormulaFixup.BuildDependencyTree(this);
                    break;

                case Properties.KPI:
                    Handler.Tree.OnStructureChanged(this);
                    break;

                case Properties.NAME:
                    if(Handler.Settings.AutoFixup)
                    {
                        FormulaFixup.DoFixup(this);
                        Handler.UndoManager.EndBatch(); // This batch was started in OnPropertyChanging
                    }
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            switch (propertyName)
            {
                case Properties.NAME:
                    // TODO: Important!!!
                    // - Dependency Tree will be built once for every measure that's had its name changed. This can be slow if many measures are renamed at once.
                    // - We don't need a full rebuild of the dependency tree. We can limit ourselves to those expressions that contain a token matching the new name of this measure.
                    // - Also note that we should apply the fix-up before the tree is rebuilt.
                    FormulaFixup.BuildDependencyTree();

                    // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                    // name change could result in expression changes on multiple objects:
                    if (Handler.Settings.AutoFixup)
                        Handler.UndoManager.BeginBatch("Set Property 'Name'"); // This batch will be ended in the corresponding OnPropertyChanged
                    break;
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.FORMATSTRING: return DataType != DataType.String;
                case Properties.DETAILROWSDEFINITION:
                    return Handler.CompatibilityLevel >= 1400;
                default: return true;
            }
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            if (KPI == null) return Enumerable.Empty<ITabularNamedObject>();
            else return Enumerable.Repeat<ITabularNamedObject>(KPI, 1);
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
                OnPropertyChanging(Properties.DETAILROWSEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                // TODO 01: Handle dependencies in the Detail Rows Expression

                if (MetadataObject.DetailRowsDefinition == null) MetadataObject.DetailRowsDefinition = new TOM.DetailRowsDefinition();
                MetadataObject.DetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.DetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.DETAILROWSEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.DETAILROWSEXPRESSION, oldValue, value);
            }
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

    internal static partial class Properties
    {
        public const string DETAILROWSEXPRESSION = "DetailRowsExpression";
    }

    public partial class MeasureCollection
    {
        internal override string GetNewName(string prefix = null)
        {
            // For measures, we must ensure that the new measure name is unique across all tables,
            // which is why we have to override the GetNewName method here. Also, we must make sure
            // that no columns on the same table, have the same name as the measure.

            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Measure";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (Table.Model.AllMeasures.Any(m => m.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase))
                || Table.Columns.Any(c => c.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }
    }
}

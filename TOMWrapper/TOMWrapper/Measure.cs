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

        /// <summary>
        ///             A string that explains the error state associated with the current object. It is set by the engine only when the state of the object is one of these three values: SemanticError, DependencyError, or EvaluationError. It is applicable only to columns of the type Calculated or CalculatedTableColumn. It will be empty for other column objects.
        ///             </summary>
        [DisplayName("Error Message")]
        [Category("Metadata"), Description(@"A string that explains the error state associated with the current object. It is set by the engine only when the state of the object is one of these three values: SemanticError, DependencyError, or EvaluationError. It is applicable only to columns of the type Calculated or CalculatedTableColumn. It will be empty for other column objects."), IntelliSense(@"A string that explains the error state associated with the current object. It is set by the engine only when the state of the object is one of these three values: SemanticError, DependencyError, or EvaluationError. It is applicable only to columns of the type Calculated or CalculatedTableColumn. It will be empty for other column objects.")]
        public string ErrorMessage
        {
            get
            {
                var errorMessage = "";
                if (!string.IsNullOrEmpty(MetadataObject.ErrorMessage)) errorMessage += (Handler.CompatibilityLevel >= 1400 ? "Expression: " : "") + MetadataObject.ErrorMessage;
                if (Handler.CompatibilityLevel >= 1400 && !string.IsNullOrEmpty(MetadataObject.DetailRowsDefinition?.ErrorMessage))
                {
                    if (errorMessage != "") errorMessage += "\r\n";
                    errorMessage += (Handler.CompatibilityLevel >= 1400 ? "Detail rows expression: " : "") + MetadataObject.DetailRowsDefinition.ErrorMessage;
                }
                return errorMessage;
            }
        }

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
            KPI = KPI.CreateNew();

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
        [Category("Options"), IntelliSense("The KPI of this Measure.")]
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
                        // Fixup is not performed during an undo operation. We rely on the undo stack to fixup the expressions
                        // affected by the name change (the undo stack should contain the expression changes that were made
                        // when the name was initially changed).
                        if (!Handler.UndoManager.UndoInProgress) FormulaFixup.DoFixup(this, true);
                        FormulaFixup.BuildDependencyTree();
                        Handler.EndUpdate(); // This batch was started in OnPropertyChanging
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
                    // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                    // name change could result in expression changes on multiple objects:
                    if (Handler.Settings.AutoFixup) Handler.BeginUpdate("Set Property 'Name'"); // This batch will be ended in the corresponding OnPropertyChanged
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

                case Properties.DATACATEGORY:
                    return Handler.CompatibilityLevel >= 1455;
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

                if (oldValue == value || oldValue == null && string.IsNullOrEmpty(value)) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DETAILROWSEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

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
        internal static string GetNewName(Table table, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Measure";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (table.Model.AllMeasures.Any(m => m.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase))
                || table.Columns.Any(c => c.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }

        internal static string GetNewName(Model model, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Measure";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (model.AllMeasures.Any(m => m.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }

        internal override string GetNewName(string prefix = null)
        {
            // For measures, we must ensure that the new measure name is unique across all tables,
            // which is why we have to override the GetNewName method here. Also, we must make sure
            // that no columns on the same table, have the same name as the measure.
            return GetNewName(Table, prefix);
        }
    }
}

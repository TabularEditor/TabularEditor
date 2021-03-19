using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.Utils;
using System.Drawing.Design;

namespace TabularEditor.TOMWrapper
{
    public partial class Measure : IDaxObject, ITabularObjectContainer, IDaxDependantObject
    {
        private DependsOnList _dependsOn = null;

        /// <summary>
        /// Gets the visibility of the Measure. Takes into consideration that a measure is visible regardless of its parent table being visible.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => !IsHidden;

        /// <summary>
        /// Delete the measure from its current table and create a deep clone (including all translations, if any) in the destination table.
        /// </summary>
        /// <param name="destinationTable"></param>
        [IntelliSense("Delete the measure from its current table and create a deep clone (including all translations, if any) in the destination table.")]
        public void MoveTo(Table destinationTable)
        {
            Handler.BeginUpdate("Move measure");
            var name = Name;
            var newMeasure = Clone(null, true, destinationTable);
            Delete();
            newMeasure.Name = name;
            Handler.EndUpdate();
        }

        [IntelliSense("Delete the measure from its current table and create a deep clone (including all translations, if any) in the destination table.")]
        public void MoveTo(string destinationTable)
        {
            if (!Model.Tables.Contains(destinationTable)) throw new InvalidOperationException($"Model does not contain a table named '{destinationTable}'");
            var table = Model.Tables[destinationTable];
            MoveTo(destinationTable);
        }

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
                if (!string.IsNullOrEmpty(MetadataObject.ErrorMessage))
                    errorMessage += (Handler.CompatibilityLevel >= 1400 ? "Expression: " : "") + MetadataObject.ErrorMessage;
                if (Handler.CompatibilityLevel >= 1400 && !string.IsNullOrEmpty(MetadataObject.DetailRowsDefinition?.ErrorMessage))
                {
                    if (errorMessage != "") errorMessage += "\r\n";
                    errorMessage += "Detail rows expression: " + MetadataObject.DetailRowsDefinition.ErrorMessage;
                }

                /*if (Handler.CompatibilityLevel >= 1470 && !string.IsNullOrEmpty(MetadataObject.FormatStringDefinition?.ErrorMessage))
                {
                    if (errorMessage != "") errorMessage += "\r\n";
                    errorMessage += "Format string expression: " + MetadataObject.FormatStringDefinition.ErrorMessage;
                }*/

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
            Handler.BeginUpdate("Add KPI");
            if(KPI == null) KPI = KPI.CreateNew();
            Handler.EndUpdate();
            return KPI;
        }
        private bool CanAddKPI() => KPI == null;
        public void RemoveKPI()
        {
            Handler.BeginUpdate("Remove KPI");
            KPI = null;
            Handler.EndUpdate();
        }
        private bool CanRemoveKPI() => KPI != null;

        private KPI KPIBackup;
        private bool _needsValidation = false;

        internal override void RemoveReferences()
        {
            KPIBackup = KPI;
            base.RemoveReferences();
        }

        internal override void Reinit()
        {
            if (KPIBackup != null)
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
        [PropertyAction(nameof(AddKPI), nameof(RemoveKPI)), Editor(typeof(KpiEditor), typeof(UITypeEditor))]
        public KPI KPI
        {
            get
            {
                if (MetadataObject.KPI == null) return null;
                return Handler.WrapperLookup[MetadataObject.KPI] as KPI;
            }
            set
            {
                var oldValue = MetadataObject.KPI != null ? KPI : null;
                if (oldValue?.MetadataObject == value?.MetadataObject) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging("KPI", value, ref undoable, ref cancel);
                if (cancel) return;

                var newKPI = value?.MetadataObject;
                if (newKPI != null && newKPI.IsRemoved)
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

            if (!string.IsNullOrEmpty(ErrorMessage)) Table.AddError(this);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case Properties.DETAILROWSEXPRESSION:
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.EXPRESSION:
                    _needsValidation = true;
                    FormulaFixup.BuildDependencyTree(this);
                    break;

                case Properties.KPI:
                    Handler.Tree.OnStructureChanged(this);
                    break;

                case Properties.NAME:
                    if (Handler.Settings.AutoFixup)
                    {
                        // Fixup is not performed during an undo operation. We rely on the undo stack to fixup the expressions
                        // affected by the name change (the undo stack should contain the expression changes that were made
                        // when the name was initially changed).
                        if (!Handler.UndoManager.UndoInProgress) FormulaFixup.DoFixup(this, true);
                        FormulaFixup.BuildDependencyTree();
                        Handler.EndUpdate(); // This batch was started in OnPropertyChanging
                    }
                    break;
                case Properties.FORMATSTRING:
                    Handler.PowerBIGovernance.SuspendGovernance();
                    RemoveAnnotation("Format", true);
                    Handler.PowerBIGovernance.ResumeGovernance();
                    Handler.EndUpdate();
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
                case Properties.FORMATSTRING:
                    Handler.BeginUpdate("Set Property 'Format String'");
                    break;
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.DETAILROWSEXPRESSION: return Browsable(Properties.DETAILROWSDEFINITION);
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

                if (oldValue == value || (oldValue == null && value == string.Empty)) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DETAILROWSEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DetailRowsDefinition == null && !string.IsNullOrEmpty(value))
                    MetadataObject.DetailRowsDefinition = new TOM.DetailRowsDefinition();
                if (!string.IsNullOrEmpty(value))
                    MetadataObject.DetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value) && MetadataObject.DetailRowsDefinition != null)
                    MetadataObject.DetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.DETAILROWSEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.DETAILROWSEXPRESSION, oldValue, value);
            }
        }
        public bool ShouldSerializeDetailRowsExpression() { return false; }

        /*[DisplayName("Format String Expression")]
        [Category("Options"), IntelliSense("A DAX expression that returns a Format String for this measure.")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FormatStringExpression
        {
            get
            {
                return MetadataObject.FormatStringDefinition?.Expression;
            }
            set
            {
                var oldValue = FormatStringExpression;

                if (oldValue == value || oldValue == null && string.IsNullOrEmpty(value)) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.FORMATSTRINGEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.FormatStringDefinition == null) MetadataObject.FormatStringDefinition = new TOM.FormatStringDefinition();
                MetadataObject.FormatStringDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.FormatStringDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.FORMATSTRINGEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.FORMATSTRINGEXPRESSION, oldValue, value);
            }
        }
        public bool ShouldSerializeFormatStringExpression() { return false; }*/


        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("[{0}]", Name.Replace("]", "]]"));
            }
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX Identifier")]
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
        public const string FORMATSTRINGEXPRESSION = "FormatStringExpression";
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

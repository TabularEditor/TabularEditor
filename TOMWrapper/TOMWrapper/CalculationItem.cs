using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculationItem: IDaxDependantObject, ITabularTableObject
    {


        [Browsable(false)]
        public CalculationGroupTable CalculationGroup => Parent.Table as CalculationGroupTable;
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
                /*if (Handler.CompatibilityLevel >= 1470 && !string.IsNullOrEmpty(MetadataObject.FormatStringDefinition?.ErrorMessage))
                {
                    if (errorMessage != "") errorMessage += "\r\n";
                    errorMessage += "Format string expression: " + MetadataObject.FormatStringDefinition.ErrorMessage;
                }*/
                return errorMessage;
            }
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;
        /*
        [DisplayName("Format String Expression")]
        [Category("Options"), IntelliSense("A DAX expression that returns a Format String for this calculation item.")]
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
        public bool ShouldSerializeFormatStringExpression() { return false; }
        */

        [Browsable(false)]
        public CalculationGroupAttribute Field => CalculationGroup.Field;

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case Properties.DETAILROWSEXPRESSION:
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.EXPRESSION:
                    NeedsValidation = true;
                    FormulaFixup.BuildDependencyTree(this);
                    break;
            }
        }

        private DependsOnList _dependsOn;
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

        Table ITabularTableObject.Table => Parent.Table;
    }
}

using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using Microsoft.AnalysisServices.Tabular.Helper;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculationGroup
    {
        internal static CalculationGroup CreateFromMetadata(Table parent, TOM.CalculationGroup metadataObject)
        {
            var obj = new CalculationGroup(metadataObject);
            parent.MetadataObject.CalculationGroup = metadataObject;

            obj.Init();

            return obj;
        }

        /// <summary>
        /// The expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items can be applied.
        /// </summary>
        public string DefaultExpression
        {
            get
            {
                return MetadataObject.DefaultExpression?.Expression;
            }
            set
            {
                var oldValue = DefaultExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DEFAULTEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DefaultExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.DefaultExpression = new TOM.CalculationExpression();
                
                MetadataObject.DefaultExpression.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.DEFAULTEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.DEFAULTEXPRESSION, oldValue, value);
            }
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items can be applied.
        /// </summary>
        public string DefaultFormatStringExpression
        {
            get
            {
                return MetadataObject.DefaultExpression?.FormatStringDefinition?.Expression;
            }
            set
            {
                var oldValue = DefaultFormatStringExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(DefaultFormatStringExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (!string.IsNullOrEmpty(value))
                {
                    if (MetadataObject.DefaultExpression == null)
                        MetadataObject.DefaultExpression = new TOM.CalculationExpression();
                    if (MetadataObject.DefaultExpression.FormatStringDefinition == null)
                        MetadataObject.DefaultExpression.FormatStringDefinition = new TOM.FormatStringDefinition();
                }

                MetadataObject.DefaultExpression.FormatStringDefinition.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(DefaultFormatStringExpression), oldValue, value));
                OnPropertyChanged(nameof(DefaultFormatStringExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The description of the CalculationExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        public string DefaultExpressionDescription
        {
            get
            {
                return MetadataObject.DefaultExpression?.Description;
            }
            set
            {
                var oldValue = DefaultExpressionDescription;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(DefaultExpressionDescription), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DefaultExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.DefaultExpression = new TOM.CalculationExpression();

                MetadataObject.DefaultExpression.Description = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(DefaultExpressionDescription), oldValue, value));
                OnPropertyChanged(nameof(DefaultExpressionDescription), oldValue, value);
            }
        }

        private void ClearDefaultExpressionIfEmpty()
        {
            if (MetadataObject.DefaultExpression.IsNullOrEmpty())
                MetadataObject.DefaultExpression = null;
        }

        public override string ToString()
        {
            return "Calculation Group";
        }
    }
}

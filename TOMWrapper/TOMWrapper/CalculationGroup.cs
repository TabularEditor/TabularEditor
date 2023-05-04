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
        /// The expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are applied.
        /// </summary>
        public string MultiSelectionExpression
        {
            get
            {
                return MetadataObject.MultiSelectionExpression?.Expression;
            }
            set
            {
                var oldValue = MultiSelectionExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultiSelectionExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.MultiSelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.MultiSelectionExpression = new TOM.CalculationExpression();
                if (MetadataObject.MultiSelectionExpression != null)
                    MetadataObject.MultiSelectionExpression.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultiSelectionExpression), oldValue, value));
                OnPropertyChanged(nameof(MultiSelectionExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The description of the CalculationExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        public string MultiSelectionExpressionDescription
        {
            get
            {
                return MetadataObject.MultiSelectionExpression?.Description;
            }
            set
            {
                var oldValue = MultiSelectionExpressionDescription;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultiSelectionExpressionDescription), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.MultiSelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.MultiSelectionExpression = new TOM.CalculationExpression();
                if (MetadataObject.MultiSelectionExpression != null)
                    MetadataObject.MultiSelectionExpression.Description = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultiSelectionExpressionDescription), oldValue, value));
                OnPropertyChanged(nameof(MultiSelectionExpressionDescription), oldValue, value);
            }
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are applied.
        /// </summary>
        public string MultiSelectionFormatStringExpression
        {
            get
            {
                return MetadataObject.MultiSelectionExpression?.FormatStringDefinition?.Expression;
            }
            set
            {
                var oldValue = MultiSelectionFormatStringExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultiSelectionFormatStringExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (!string.IsNullOrEmpty(value))
                {
                    if (MetadataObject.MultiSelectionExpression == null)
                        MetadataObject.MultiSelectionExpression = new TOM.CalculationExpression();
                    if (MetadataObject.MultiSelectionExpression.FormatStringDefinition == null)
                        MetadataObject.MultiSelectionExpression.FormatStringDefinition = new TOM.FormatStringDefinition();
                }
                if (MetadataObject.MultiSelectionExpression != null && MetadataObject.MultiSelectionExpression.FormatStringDefinition != null)
                    MetadataObject.MultiSelectionExpression.FormatStringDefinition.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultiSelectionFormatStringExpression), oldValue, value));
                OnPropertyChanged(nameof(MultiSelectionFormatStringExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied.
        /// </summary>
        public string NoSelectionExpression
        {
            get
            {
                return MetadataObject.NoSelectionExpression?.Expression;
            }
            set
            {
                var oldValue = NoSelectionExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(NoSelectionExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.NoSelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.NoSelectionExpression = new TOM.CalculationExpression();
                if (MetadataObject.NoSelectionExpression != null)
                    MetadataObject.NoSelectionExpression.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(NoSelectionExpression), oldValue, value));
                OnPropertyChanged(nameof(NoSelectionExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The description of the CalculationExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        public string NoSelectionExpressionDescription
        {
            get
            {
                return MetadataObject.NoSelectionExpression?.Description;
            }
            set
            {
                var oldValue = NoSelectionExpressionDescription;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(NoSelectionExpressionDescription), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.NoSelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.NoSelectionExpression = new TOM.CalculationExpression();
                if (MetadataObject.NoSelectionExpression != null)
                    MetadataObject.NoSelectionExpression.Description = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(NoSelectionExpressionDescription), oldValue, value));
                OnPropertyChanged(nameof(NoSelectionExpressionDescription), oldValue, value);
            }
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied.
        /// </summary>
        public string NoSelectionFormatStringExpression
        {
            get
            {
                return MetadataObject.NoSelectionExpression?.FormatStringDefinition?.Expression;
            }
            set
            {
                var oldValue = NoSelectionFormatStringExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(NoSelectionFormatStringExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (!string.IsNullOrEmpty(value))
                {
                    if (MetadataObject.NoSelectionExpression == null)
                        MetadataObject.NoSelectionExpression = new TOM.CalculationExpression();
                    if (MetadataObject.NoSelectionExpression.FormatStringDefinition == null)
                        MetadataObject.NoSelectionExpression.FormatStringDefinition = new TOM.FormatStringDefinition();
                }
                if (MetadataObject.NoSelectionExpression != null && MetadataObject.NoSelectionExpression.FormatStringDefinition != null)
                    MetadataObject.NoSelectionExpression.FormatStringDefinition.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(NoSelectionFormatStringExpression), oldValue, value));
                OnPropertyChanged(nameof(NoSelectionFormatStringExpression), oldValue, value);
            }
        }

        private void ClearDefaultExpressionIfEmpty()
        {
            if (MetadataObject.MultiSelectionExpression.IsNullOrEmpty())
                MetadataObject.MultiSelectionExpression = null;
        }

        public override string ToString()
        {
            return "Calculation Group";
        }
    }
}

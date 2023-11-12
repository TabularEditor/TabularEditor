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
        public string MultipleOrEmptySelectionExpression
        {
            get
            {
                return MetadataObject.MultipleOrEmptySelectionExpression?.Expression;
            }
            set
            {
                var oldValue = MultipleOrEmptySelectionExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultipleOrEmptySelectionExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.MultipleOrEmptySelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.MultipleOrEmptySelectionExpression = new TOM.CalculationGroupExpression();
                if (MetadataObject.MultipleOrEmptySelectionExpression != null)
                    MetadataObject.MultipleOrEmptySelectionExpression.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultipleOrEmptySelectionExpression), oldValue, value));
                OnPropertyChanged(nameof(MultipleOrEmptySelectionExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        public string MultipleOrEmptySelectionDescription
        {
            get
            {
                return MetadataObject.MultipleOrEmptySelectionExpression?.Description;
            }
            set
            {
                var oldValue = MultipleOrEmptySelectionDescription;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultipleOrEmptySelectionDescription), value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.MultipleOrEmptySelectionExpression == null && !string.IsNullOrEmpty(value))
                    MetadataObject.MultipleOrEmptySelectionExpression = new TOM.CalculationGroupExpression();
                if (MetadataObject.MultipleOrEmptySelectionExpression != null)
                    MetadataObject.MultipleOrEmptySelectionExpression.Description = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultipleOrEmptySelectionDescription), oldValue, value));
                OnPropertyChanged(nameof(MultipleOrEmptySelectionDescription), oldValue, value);
            }
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are applied.
        /// </summary>
        public string MultipleOrEmptySelectionFormatStringExpression
        {
            get
            {
                return MetadataObject.MultipleOrEmptySelectionExpression?.FormatStringDefinition?.Expression;
            }
            set
            {
                var oldValue = MultipleOrEmptySelectionFormatStringExpression;
                if (oldValue == value || (oldValue == null && string.IsNullOrEmpty(value))) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(nameof(MultipleOrEmptySelectionFormatStringExpression), value, ref undoable, ref cancel);
                if (cancel) return;

                if (!string.IsNullOrEmpty(value))
                {
                    if (MetadataObject.MultipleOrEmptySelectionExpression == null)
                        MetadataObject.MultipleOrEmptySelectionExpression = new TOM.CalculationGroupExpression();
                    if (MetadataObject.MultipleOrEmptySelectionExpression.FormatStringDefinition == null)
                        MetadataObject.MultipleOrEmptySelectionExpression.FormatStringDefinition = new TOM.FormatStringDefinition();
                }
                if (MetadataObject.MultipleOrEmptySelectionExpression != null && MetadataObject.MultipleOrEmptySelectionExpression.FormatStringDefinition != null)
                    MetadataObject.MultipleOrEmptySelectionExpression.FormatStringDefinition.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(MultipleOrEmptySelectionFormatStringExpression), oldValue, value));
                OnPropertyChanged(nameof(MultipleOrEmptySelectionFormatStringExpression), oldValue, value);
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
                    MetadataObject.NoSelectionExpression = new TOM.CalculationGroupExpression();
                if (MetadataObject.NoSelectionExpression != null)
                    MetadataObject.NoSelectionExpression.Expression = value;
                ClearDefaultExpressionIfEmpty();

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, nameof(NoSelectionExpression), oldValue, value));
                OnPropertyChanged(nameof(NoSelectionExpression), oldValue, value);
            }
        }

        /// <summary>
        /// The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
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
                    MetadataObject.NoSelectionExpression = new TOM.CalculationGroupExpression();
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
                        MetadataObject.NoSelectionExpression = new TOM.CalculationGroupExpression();
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
            if (MetadataObject.MultipleOrEmptySelectionExpression.IsNullOrEmpty())
                MetadataObject.MultipleOrEmptySelectionExpression = null;
            if (MetadataObject.NoSelectionExpression.IsNullOrEmpty())
                MetadataObject.NoSelectionExpression = null;
        }

        public override string ToString()
        {
            return "Calculation Group";
        }
    }
}

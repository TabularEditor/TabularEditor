using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculationItem: IDaxDependantObject, ITabularTableObject
    {
        protected override void Init()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                CalculationGroupTable.AddError(this);
            base.Init();
        }

        [Browsable(false)]
        public CalculationGroupTable CalculationGroupTable => Parent.Table as CalculationGroupTable;
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
                if (Handler.CompatibilityLevel >= 1470 && !string.IsNullOrEmpty(MetadataObject.FormatStringDefinition?.ErrorMessage))
                {
                    if (errorMessage != "") errorMessage += "\r\n";
                    errorMessage += "Format string expression: " + MetadataObject.FormatStringDefinition.ErrorMessage;
                }
                return errorMessage;
            }
        }

        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;
        
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

                if (oldValue == value || (oldValue == null && value == string.Empty)) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.FORMATSTRINGEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.FormatStringDefinition == null && !string.IsNullOrEmpty(value))
                    MetadataObject.FormatStringDefinition = new TOM.FormatStringDefinition();
                if (!string.IsNullOrEmpty(value))
                    MetadataObject.FormatStringDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value) && MetadataObject.FormatStringDefinition != null)
                    MetadataObject.FormatStringDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.FORMATSTRINGEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.FORMATSTRINGEXPRESSION, oldValue, value);
            }
        }
        public bool ShouldSerializeFormatStringExpression() { return false; }
        
        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.ORDINAL)
            {
                // No automatic handling of Ordinal changes. We will handle it manually in the calculation group's FixItemOrder() method.
                cancel = true;
                this.MetadataObject.Ordinal = (int)newValue;
                CalculationGroupTable.FixItemOrder(this, (int)newValue);
                return;
            }

            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case Properties.DETAILROWSEXPRESSION:
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.EXPRESSION:
                    FormulaFixup.BuildDependencyTree(this);
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
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
    
    public partial class CalculationItemCollection : ITabularNamedObject, ITabularObjectContainer, ITabularTableObject, IDynamicPropertyObject
    {
        [ReadOnly(true)]
        string ITabularNamedObject.Name { get { return "Calculation Items"; } set { } }

        int ITabularNamedObject.MetadataIndex => -1;

        ObjectType ITabularObject.ObjectType => ObjectType.CalculationItemCollection;

        Model ITabularObject.Model => CalculationGroup.Model;

        bool ITabularObject.IsRemoved => false;

        Table ITabularTableObject.Table => CalculationGroup.Table;

        [Browsable(false)]
        public CalculationGroupTable CalculationGroupTable => CalculationGroup.Table as CalculationGroupTable;

        [ReadOnly(true), Category("Metadata"), DisplayName("Object Type")]
        public string ObjectTypeName => "Calculation Item Collection";

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This property points to the CalculationItemCollection itself. It is used only to display a clickable
        /// "Calculation Items" property in the Property Grid, which will open the CalculationItemCollectionEditor when
        /// clicked.
        /// </summary>
        [DisplayName("Calculation Items"), Description("The collection of Calculation Items on this Calculation Group.")]
        [Category("Basic"), IntelliSense("The collection of Calculation Items on this Calculation Group.")]
        [NoMultiselect(), Editor(typeof(CalculationItemCollectionEditor), typeof(UITypeEditor))]
        public CalculationItemCollection PropertyGridCalculationItems => this;

        bool ITabularNamedObject.CanDelete()
        {
            return false;
        }

        bool ITabularNamedObject.CanDelete(out string message)
        {
            message = Messages.CannotDeleteObject;
            return false;
        }

        bool ITabularNamedObject.CanEditName()
        {
            return false;
        }

        void ITabularNamedObject.Delete()
        {
            throw new NotSupportedException();
        }

        IEnumerable<ITabularNamedObject> ITabularObjectContainer.GetChildren()
        {
            return this;
        }

        public void SetAnnotation(int index, string value, bool undoable = false)
        {
            ((IInternalAnnotationObject)CalculationGroup).SetAnnotation(index, value, undoable);
        }

        public void SetAnnotation(string name, string value, bool undoable = false)
        {
            ((IInternalAnnotationObject)CalculationGroup).SetAnnotation(name, value, undoable);
        }

        public void RemoveAnnotation(string name, bool undoable = false)
        {
            ((IInternalAnnotationObject)CalculationGroup).RemoveAnnotation(name, undoable);
        }

        public bool HasAnnotation(string name)
        {
            return CalculationGroup.HasAnnotation(name);
        }

        public string GetAnnotation(int index)
        {
            return CalculationGroup.GetAnnotation(index);
        }

        public string GetAnnotation(string name)
        {
            return CalculationGroup.GetAnnotation(name);
        }

        public string GetNewAnnotationName()
        {
            return CalculationGroup.GetNewAnnotationName();
        }

        public void SetAnnotation(int index, string value)
        {
            ((IInternalAnnotationObject)CalculationGroup).SetAnnotation(index, value);
        }

        public void SetAnnotation(string name, string value)
        {
            ((IInternalAnnotationObject)CalculationGroup).SetAnnotation(name, value);
        }

        public void RemoveAnnotation(string name)
        {
            ((IInternalAnnotationObject)CalculationGroup).RemoveAnnotation(name);
        }

        public int GetAnnotationsCount()
        {
            return CalculationGroup.GetAnnotationsCount();
        }

        public IEnumerable<string> GetAnnotations()
        {
            return CalculationGroup.GetAnnotations();
        }

        public bool Browsable(string propertyName)
        {
            return Handler.PowerBIGovernance.VisibleProperty(ObjectType.CalculationItemCollection, propertyName);
        }

        public bool Editable(string propertyName)
        {
            return Handler.PowerBIGovernance.AllowEditProperty(ObjectType.CalculationItemCollection, propertyName);
        }
    }
}

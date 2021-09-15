using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper.Undo;
using System;
using TabularEditor.PropertyGridUI;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Linq;

namespace TabularEditor.TOMWrapper
{
    public class IntelliSenseAttribute: Attribute
    {
        public IntelliSenseAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }
    }

    /// <summary>
    /// TabularObjects that can contain other objects should use this interface, to allow easy enumerator of child objects.
    /// </summary>
    public interface ITabularObjectContainer: ITabularNamedObject
    {
        IEnumerable<ITabularNamedObject> GetChildren();
    }

    public enum ObjectType
    {
        // Special types needed by Tabular Editor (doesn't exist in the TOM):
        CalculationGroupTable = -7,
        CalculationItemCollection = -6,
        PartitionCollection = -4,
        KPIMeasure = -3,
        Group = -2,
        Folder = -1,

        // Default types:
        Null = 0,
        Model = 1,
        DataSource = 2,
        Table = 3,
        Column = 4,
        AttributeHierarchy = 5,
        Partition = 6,
        Relationship = 7,
        Measure = 8,
        Hierarchy = 9,
        Level = 10,
        Annotation = 11,
        KPI = 12,
        Culture = 13,
        ObjectTranslation = 14,
        LinguisticMetadata = 15,
        Perspective = 29,
        PerspectiveTable = 30,
        PerspectiveColumn = 31,
        PerspectiveHierarchy = 32,
        PerspectiveMeasure = 33,
        Role = 34,
        RoleMembership = 35,
        TablePermission = 36,
        Variation = 37,
        Expression = 41,
        ColumnPermission = 42,
        DetailRowsDefinition = 43,
        CalculationGroup = 46,
        CalculationItem = 47,
        AlternateOf = 48,
        Database = 1000
    }

    /// <summary>
    /// Base class for all TOM objects that are wrapped in the TOMWrapper. Supports INotifyPropertyChanged and INotifyPropertyChanging
    /// and undo/redo functionality via the TabularModelHandler. Every TabularObject holds a reference to the corresponding TOM MetadataObject.
    /// A TabularObject cannot exist without a corresponding TOM MetadataObject.
    /// 
    /// Protected constructor that takes a TOM MetadataObject as argument.
    /// </summary>
    public abstract class TabularObject: IInternalTabularObject, INotifyPropertyChanged, INotifyPropertyChanging, IDynamicPropertyObject
    {
        internal JObject SerializedFrom = null;
        internal ITabularObjectCollection Collection;

        void IInternalTabularObject.ReapplyReferences() => ReapplyReferences();

        protected void SetValue(object org, object value, Action<object> setter, [CallerMemberName] string propertyName = null)
        {
            var oldValue = org;
            if (oldValue == value) return;
            bool undoable = true;
            bool cancel = false;
            OnPropertyChanging(propertyName, value, ref undoable, ref cancel);
            if (cancel) return;
            setter(value);
            if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, propertyName, oldValue, value));
            OnPropertyChanged(propertyName, oldValue, value);
        }
        protected void SetValue<T>(T org, T value, Action<T> setter, [CallerMemberName] string propertyName = null)
        {
            var oldValue = org;
            if ((oldValue == null && value == null) || (oldValue != null && oldValue.Equals(value))) return;
            bool undoable = true;
            bool cancel = false;
            OnPropertyChanging(propertyName, value, ref undoable, ref cancel);
            if (cancel) return;
            setter(value);
            if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, propertyName, oldValue, value));
            OnPropertyChanged(propertyName, oldValue, value);
        }

        [Browsable(false)]
        public bool IsRemoved => _metadataObject.IsRemoved;
        private TOM.MetadataObject _metadataObject;
        protected internal TOM.MetadataObject MetadataObject { get { return _metadataObject; } protected set { _metadataObject = value; } }
        protected internal TabularModelHandler Handler;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        internal abstract void RenewMetadataObject();
        internal virtual void Undelete(ITabularObjectCollection collection, Type tomObjectType, string tomJson) { }
        internal virtual void ReapplyReferences() { }
        internal virtual void DeleteLinkedObjects(bool isChildOfDeleted) { }
        internal virtual void Reinit() { }

        internal static readonly TOM.SerializeOptions RenewMetadataOptions = new TOM.SerializeOptions
        {
            IgnoreInferredObjects = true,
            IgnoreInferredProperties = false,
            IgnoreTimestamps = true,
            IncludeRestrictedInformation = true,
            SplitMultilineStrings = false
        };

        internal virtual ITabularObjectCollection GetCollectionForChild(TabularObject child)
        {
            throw new NotSupportedException("This object does not have any child collections.");
        }


        protected virtual void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Handler.Tree.OnNodesInserted(this, e.NewItems.Cast<ITabularObject>());
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Handler.Tree.OnNodesRemoved(this, e.OldItems.Cast<ITabularObject>());
        }
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case Properties.EXPRESSION:
                case Properties.DETAILROWSEXPRESSION:
                case Properties.DEFAULTDETAILROWSEXPRESSION:
                case Properties.FILTEREXPRESSION:
                case Properties.STATUSEXPRESSION:
                case Properties.TARGETEXPRESSION:
                case Properties.TRENDEXPRESSION:
                case Properties.FORMATSTRINGEXPRESSION:
                    expressionChangeCounter += Handler.UndoManager.IsUndoing ? -1 : 1;
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Handler.DoObjectChanged(this, propertyName, oldValue, newValue);

            // Below seems to cause a lot of flickering, since every property change will fire it.
            // However, it should only be fired for property changes that require an UI update, and
            // this has already been taken care of by calling the method suitable places within the
            // individual property setters (such as the setter for Name, IsHidden, etc.):
            // Handler.UpdateObject(this);
        }

        /// <summary>
        /// Called before a property is changed on an object. Derived classes can control how the change is handled.
        /// Throw ArgumentException within this method, to display an error message in the UI.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        /// <param name="newValue">New value assigned to the property.</param>
        /// <param name="undoable">Return false if automatic undo of the property change is not needed.</param>
        /// <param name="cancel">Return true if the property change should not apply.</param>
        protected virtual void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if(!Editable(propertyName))
            {
                cancel = true;
                return;
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            if (cancel) return;
            Handler.DoObjectChanging(this, propertyName, newValue, ref cancel);
        }

        int expressionChangeCounter = 0;
        protected bool IsExpressionModified => expressionChangeCounter != 0;
        public void ResetModifiedState() { expressionChangeCounter = 0; }

        [Browsable(false),IntelliSense("The model this object belongs to.")]
        public Model Model {
            get {
                return MetadataObject.Model == null ? null : Handler.WrapperLookup[MetadataObject.Model] as Model;
            }
        }

        [Browsable(false),IntelliSense("The type of this object (Folder, Measure, Table, etc.).")]
        public virtual ObjectType ObjectType { get { return (ObjectType)MetadataObject.ObjectType; } }

        [Category("Metadata")]
        [DisplayName("Object Type"),IntelliSense("The type name of this object (\"Folder\", \"Measure\", \"Table\", etc.).")]
        public virtual string ObjectTypeName { get { return this.GetTypeName(); } }

        /// <summary>
        /// Creates a TabularObject representing the provided TOM MetadataObject.
        /// </summary>
        /// <param name="metadataObject"></param>
        protected TabularObject(TOM.MetadataObject metadataObject)
        {
            if (metadataObject == null) throw new ArgumentNullException("metadataObject");

            _metadataObject = metadataObject;
            Handler = TabularModelHandler.Singleton;
            Handler.WrapperLookup[metadataObject] = this;

            // Assign collection based on parent:
            if (metadataObject.Parent != null)
                Collection = (Handler.WrapperLookup[metadataObject.Parent] as TabularObject).GetCollectionForChild(this);
        }

        /// <summary>
        /// Derived members should override this method to instantiate child objects
        /// </summary>
        protected virtual void Init()
        {

        }
        bool IDynamicPropertyObject.Browsable(string propertyName) { return Browsable(propertyName); }
        internal virtual bool Browsable(string propertyName)
        {
            if (!Handler.PowerBIGovernance.VisibleProperty(ObjectType, propertyName)) return false;

            return IsBrowsable(propertyName);
        }
        bool IDynamicPropertyObject.Editable(string propertyName) { return Editable(propertyName); }
        internal bool Editable(string propertyName)
        {
            if (propertyName == Properties.NAME && (this is TabularNamedObject namedObj))
            {
                if (!Handler.PowerBIGovernance.AllowEditName(namedObj)) return false;
            }
            else 
                if (!Handler.PowerBIGovernance.AllowEditProperty(ObjectType, propertyName)) return false;

            return IsEditable(propertyName);
        }

        internal virtual bool IsBrowsable(string propertyName)
        {
            return true;
        }
        internal virtual bool IsEditable(string propertyName)
        {
            return true;
        }
    }
}

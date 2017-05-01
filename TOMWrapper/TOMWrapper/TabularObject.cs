using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.UndoFramework;
using System;

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
    public interface ITabularObjectContainer
    {
        IEnumerable<ITabularNamedObject> GetChildren();
    }

    public enum ObjectType
    {
        Group = -2,
        Folder = -1,

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
        Database = 1000
    }

    /// <summary>
    /// Base class for all TOM objects that are wrapped in the TOMWrapper. Supports INotifyPropertyChanged and INotifyPropertyChanging
    /// and undo/redo functionality via the TabularModelHandler. Every TabularObject holds a reference to the corresponding TOM MetadataObject.
    /// A TabularObject cannot exist without a corresponding TOM MetadataObject.
    /// 
    /// Protected constructor that takes a TOM MetadataObject as argument.
    /// </summary>
    public abstract class TabularObject: ITabularObject, INotifyPropertyChanged, INotifyPropertyChanging
    {
        protected internal ITabularObjectCollection Collection;

        private TOM.MetadataObject _metadataObject;
        protected internal TOM.MetadataObject MetadataObject { get { return _metadataObject; } protected set { _metadataObject = value; } }
        protected internal TabularModelHandler Handler;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            Handler.UndoManager.Add(new UndoPropertyChangedAction(this, propertyName, field, value));
            object oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
            return true;
        }

        internal abstract void RenewMetadataObject();

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [Browsable(false),IntelliSense("The model this object belongs to.")]
        public Model Model { get { return Handler.Model; } }

        [Browsable(false),IntelliSense("The type of this object (Folder, Measure, Table, etc.).")]
        public ObjectType ObjectType { get { return (ObjectType)MetadataObject.ObjectType; } }

        [Category("Basic")]
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

            //if(autoInit) Init();
        }

        /// <summary>
        /// Derived members should override this method to instantiate child objects
        /// </summary>
        protected virtual void Init()
        {

        }
    }
}

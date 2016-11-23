using Microsoft.AnalysisServices.Tabular;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AnalysisServices.Tabular.Helper;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// A TabularObject is a wrapper for the Microsoft.AnalysisServices.Tabular.NamedMetadataObject class.
    /// This wrapper is used for all objects that are to be viewable and editable in the Tabular Editor.
    /// The same base class is used for all kinds of objects in a Tabular Model. This base class provides
    /// method for editing the (localized) name and description.
    /// </summary>
    [DebuggerDisplay("{ObjectType} {Name}")]
    public abstract class TabularNamedObject: TabularObject, ITabularNamedObject, IComparable
    {
        /// <summary>
        /// Hacky workaround needed to undo a delete operations.
        /// Derived classes must take care to update any objects "owned" by the
        /// object in question. For example, a Measure must take care of updating
        /// the wrapper for its KPI (if any).
        /// </summary>
        /// <param name="collection"></param>
        internal virtual void Undelete(ITabularObjectCollection collection)
        {
            Collection = collection;
            Collection.Add(this);
            Handler.WrapperLookup.Add(MetadataObject, this);
        }

        public virtual void Delete()
        {
            TranslatedDescriptions.Clear();
            TranslatedDisplayFolders.Clear();
            TranslatedNames.Clear();

            if (Collection == null)
                throw new NotSupportedException("Object cannot be deleted since it does not belong to any collection.");
            Collection.Remove(this);
            Handler.WrapperLookup.Remove(MetadataObject);
        }

        public virtual TabularNamedObject Clone(string newName, bool includeTranslations)
        {
            throw new NotSupportedException("This object cannot be cloned.");
        }

        protected TabularNamedObject(TabularModelHandler handler, NamedMetadataObject metadataObject, bool autoInit = true) : base(handler, metadataObject, autoInit)
        {
            TranslatedNames = new TranslationIndexer(this, TranslatedProperty.Caption);
        }

        protected override void Init()
        {
            
        }

        protected internal new NamedMetadataObject MetadataObject { get { return base.MetadataObject as NamedMetadataObject; } protected set { base.MetadataObject = value; } }

        /// <summary>
        /// Collection of localized names for this object.
        /// </summary>
        [Browsable(true),DisplayName("Captions"),Category("Translations and Perspectives"),IntelliSense("A collection with all translated names of the object. Access individual items using the Culture name.\nExample: Measure.TranslatedNames[\"en-GB\"] = \"English name\".")]
        public TranslationIndexer TranslatedNames { get; private set; }

        private bool ShouldSerializeName() { return false; }

        public int CompareTo(object obj)
        {
            return Name.CompareTo((obj as TabularNamedObject).Name);
        }

        [Category("Basic"),NoMultiselect(),IntelliSense("The name of this object. Warning: Changing the name can break formula logic.")]
        public virtual string Name
        {
            get {
                return MetadataObject.Name;
            }
            set {
                var oldValue = Name;
                if (oldValue == value) return;
                if (string.IsNullOrEmpty(value?.Trim())) throw new ArgumentException("Name cannot be blank.");

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging("Name", value, ref undoable, ref cancel);
                if (cancel) return;

                // This will take care of throwing exception in case of duplicate names:
                MetadataObject.SetName(value, null);
                
                Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Name", oldValue, value));
                Handler.UpdateObject(this);
                OnPropertyChanged("Name", oldValue, value);

            }
        }
    }
}

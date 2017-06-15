using Microsoft.AnalysisServices.Tabular;
using System;
using System.Linq;
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
        /// Derived classes must take care to undelete any objects "owned" by the
        /// object in question. For example, a Measure must take care of calling
        /// Undelete on its KPI (if any), a Hierarchy must call Undelete on each
        /// of its levels, etc.
        /// </summary>
        /// <param name="collection"></param>
        internal virtual void Undelete(ITabularObjectCollection collection)
        {
            RenewMetadataObject();

            Collection = collection.GetCurrentCollection();
            Collection.Add(this);

            Init();
        }

        /// <summary>
        /// The Cleanup method is called when the object is deleted. Derived classes should 
        /// override this method (but remember to call base.Cleanup()) to provide their own
        /// housekeeping. For example, when a Level is deleted from a hierarchy, the ordinals
        /// for the remanining levels in the hierarchy should be compacted.
        /// 
        /// Before calling base.Cleanup(), the deleted object will still be available in the
        /// parent hierarchy. However, after the call to base.Cleanup(), this is no longer
        /// the case, so any reference to parent objects must be done before calling
        /// base.Cleanup() in derived classes.
        /// 
        /// The call to base.Cleanup() handles the following:
        ///  - Removing translations from the object (names, descriptions, display folders)
        ///  - Removing perspective memberships
        ///  - Clearing dependencies / dependants
        ///  - Removing the object from its parent object
        /// </summary>
        protected override void Cleanup()
        {
            // Remove translations for names, if this object supports translations:
            (this as ITranslatableObject)?.TranslatedNames?.Clear();

            // Remove translations for Display Folders, if this object has Display Folders:
            (this as IDetailObject)?.TranslatedDisplayFolders?.Clear();

            // Remove perspective membership if this object supports perspectives:
            (this as ITabularPerspectiveObject)?.InPerspective?.None();

            // Let dependencies know that this object is no longer a dependant (if applicable):
            var expObj = this as IExpressionObject;
            if (expObj != null)
            {
                expObj.Dependencies.Keys.ToList().ForEach(d => d.Dependants.Remove(expObj));
                expObj.Dependencies.Clear();
            }

            // Let dependants know that they can no longer depend on this object (if applicable):
            var daxObj = this as IDaxObject;
            if (daxObj != null)
            {
                daxObj.Dependants.ToList().ForEach(d => d.Dependencies.Remove(daxObj));
            }

            base.Cleanup();

            // NamedTabularObjects can belong to collections. Make sure the object is
            // removed from the collection it belongs to:
            if (Collection != null) Collection.Remove(this);
        }

        protected TabularNamedObject(NamedMetadataObject metadataObject) : base(metadataObject)
        {
        }

        /// <summary>
        /// Returns the index of this item in the parent metadata collection
        /// </summary>
        public int MetadataIndex
        {
            get
            {
                if (Collection != null)
                {
                    return Collection.IndexOf(this);
                }
                else return -1;
            }
        }

        protected override void Init()
        {
            
        }

        protected internal new NamedMetadataObject MetadataObject { get { return base.MetadataObject as NamedMetadataObject; } protected set { base.MetadataObject = value; } }

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

        protected virtual string GetNewName<T,P>(NamedMetadataObjectCollection<T, P> col, string prefix = null) where T: NamedMetadataObject where P: MetadataObject
        {
            return string.IsNullOrWhiteSpace(prefix) ? col.GetNewName() : col.GetNewName(prefix);
        }
    }
}

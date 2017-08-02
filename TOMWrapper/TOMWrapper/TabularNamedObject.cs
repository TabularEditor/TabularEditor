using Microsoft.AnalysisServices.Tabular;
using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AnalysisServices.Tabular.Helper;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using System.Collections.Specialized;

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
        /// Derived classes should override this method to prevent an object from being deleted.
        /// </summary>
        /// <param name="message">If an object CANNOT be deleted, this string should provide
        /// a reason why. If an object CAN be deleted, this string may optionally provide a
        /// suitable warning message that applies if the object is deleted immediately after
        /// the call to CanDelete.</param>
        /// <returns>True if an object can be deleted. False otherwise.</returns>
        public virtual bool CanDelete(out string message)
        {
            message = null;
            return true;
        }

        public bool CanDelete()
        {
            string dummy;
            return CanDelete(out dummy);
        }

        /// <summary>
        /// Deletes the object.
        /// </summary>
        public void Delete()
        {
            if (!CanDelete()) return;

            bool cancelDelete = false;
            Handler.DoObjectDeleting(this, ref cancelDelete);
            if (cancelDelete) return;

            Handler.UndoManager.BeginBatch(string.Format(Messages.OperationDelete, this.GetTypeName()));

            DeleteLinkedObjects(false);
            RemoveReferences();

            var _collection = Collection;

            // TabularObjects can belong to collections. Make sure the object is
            // removed from the collection it belongs to. This will add an undo
            // operation to the stack, meaning the parent collection will be
            // responsible for undeleting the object if the operation is undone:
            if (Collection != null) Collection.Remove(this);

            AfterRemoval(_collection);

            // Always remove the deleted object from the WrapperLookup:
            Handler.WrapperLookup.Remove(MetadataObject);

            Handler.UndoManager.EndBatch();
        }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            var container = this as ITabularObjectContainer;
            if (container != null) foreach (var child in container.GetChildren().OfType<TabularObject>()) child.DeleteLinkedObjects(true);
        }

        internal override void ReapplyReferences()
        {
            var container = this as ITabularObjectContainer;
            if (container != null) foreach (var child in container.GetChildren().OfType<TabularObject>()) child.ReapplyReferences();
        }

        /// <summary>
        /// Derived classes must take care to undelete any objects "owned" by the
        /// object in question. For example, a Measure must take care of calling
        /// Undelete on its KPI (if any), a Hierarchy must call Undelete on each
        /// of its levels, etc.
        /// </summary>
        /// <param name="collection"></param>
        internal override void Undelete(ITabularObjectCollection collection)
        {
            RenewMetadataObject();

            Collection = collection.GetCurrentCollection();
            Collection.Add(this);
        }

        /// <summary>
        /// The BeforeRemoval method is called before an object is deleted. Derived classes
        /// should override this to remove all references to this object, from other objects.
        /// When a parent object is deleted.
        /// 
        /// Remember to call base.BeforeRemoval(), as this will take care of calling the same
        /// method on any child objects, as well as removing the following references:
        ///  - Removing translations from the object (names, descriptions, display folders)
        ///  - Removing perspective memberships
        ///  - Clearing DAX dependencies / dependants
        /// </summary>
        internal virtual void RemoveReferences()
        {
            var container = this as ITabularObjectContainer;
            if (container != null) foreach (var child in container.GetChildren().OfType<TabularNamedObject>()) child.RemoveReferences();

            // Remove translations for names, if this object supports translations:
            (this as ITranslatableObject)?.TranslatedNames?.Clear();

            // Remove translations for descriptions, if this object supports translations:
            (this as ITranslatableObject)?.TranslatedDescriptions?.Clear();

            // Remove translations for Display Folders, if this object has Display Folders:
            (this as IDetailObject)?.TranslatedDisplayFolders?.Clear();

            // Remove perspective membership if this object supports perspectives:
            (this as ITabularPerspectiveObject)?.InPerspective?.None();

            // Let dependencies know that this object is no longer a dependant (if applicable):
            var expObj = this as IDAXExpressionObject;
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
        }

        /// <summary>
        /// This method is called after an object has been removed from a collection.
        /// Derived classes should override this to perform any cleanup necessary after
        /// removal. For example, when a level is removed from a hierarchy, the hierachy
        /// must compact the ordinal numbers of the remaining levels.
        /// 
        /// The base class will automatically call AfterRemoval on all child objects to
        /// the object that was removed.
        /// </summary>
        /// <param name="collection"></param>
        internal virtual void AfterRemoval(ITabularObjectCollection collection)
        {
            var container = this as ITabularObjectContainer;
            if (container != null) foreach (var child in container.GetChildren().OfType<TabularNamedObject>()) child.AfterRemoval(GetCollectionForChild(child));
        }

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

        protected TabularNamedObject(NamedMetadataObject metadataObject) : base(metadataObject)
        {
        }

        /// <summary>
        /// Returns the index of this item in the parent metadata collection
        /// </summary>
        [Browsable(false)]
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

        [Category("Basic"), NoMultiselect()]
        [Description("The name of this object. Warning: Changing the name can break formula logic, if Automatic Formula Fix-up is disabled.")]
        [IntelliSense("The name of this object. Warning: Changing the name can break formula logic, if Automatic Formula Fix-up is disabled.")]
        public virtual string Name
        {
            get {
                return MetadataObject.Name;
            }
            set {
                var oldValue = Name;
                if (oldValue == value) return;
                if (string.IsNullOrEmpty(value?.Trim()))
                    throw new ArgumentException(string.Format(Messages.ParameterBlankNotAllowed, Properties.NAME), Properties.NAME);

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.NAME, value, ref undoable, ref cancel);
                if (cancel) return;

                // This will take care of throwing exception in case of duplicate names:
                MetadataObject.SetName(value, null);
                
                Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.NAME, oldValue, value));
                Handler.UpdateObject(this);
                OnPropertyChanged(Properties.NAME, oldValue, value);

            }
        }
    }
}

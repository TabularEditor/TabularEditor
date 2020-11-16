using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    interface ITabularObjectCollection: INotifyCollectionChanged, IExpandableIndexer, IEnumerable
    {
        TabularModelHandler Handler { get; }
        void Add(TabularNamedObject obj);
        void Clear();
        void Remove(TabularNamedObject obj);
        bool Contains(TabularNamedObject obj);
        bool Contains(string key);
        string CollectionName { get; }
        int IndexOf(TabularNamedObject obj);
        TabularObject Parent { get; }
        void CreateChildrenFromMetadata();
        Type ItemType { get; }
    }

    /// <summary>
    /// Represents a collection of Tabular Object Model objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TabularObjectCollection<T> : ITabularObjectCollection, IReadOnlyList<T>
        where T: TabularNamedObject
    {
        // Functionality:
        #region CTOR
        internal TabularObjectCollection(string collectionName, TabularObject parent)
        {
            _parent = parent;
            _handler = TabularModelHandler.Singleton;
            CollectionName = collectionName;
        }
        #endregion
        #region Internal / private members
        private TabularObject _parent;
        private TabularModelHandler _handler;
        internal TabularObject Parent { get { return _parent; } }
        internal TabularModelHandler Handler { get { return _handler; } }

        internal void Add(T item)
        {
            InternalAdd(item);
        }

        protected virtual void InternalAdd(T item)
        {
            if (item.MetadataObject.Parent != null)
            {
                if (Handler.WrapperLookup[item.MetadataObject.Parent] == Parent) return;

                throw new ArgumentException("The item already belongs to a collection.");
            }

            //item.RenewMetadataObject();

            TOM_Add(item.MetadataObject);
            item.Collection = this;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Add));
            DoCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        protected virtual void DoCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        internal bool Remove(T item)
        {
            return InternalRemove(item);
        }

        protected virtual bool InternalRemove(T item)
        {
            if (!TOM_Contains(item.MetadataObject)) throw new InvalidOperationException();

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Remove));
            TOM_Remove(item.MetadataObject);
            item.Collection = null;

            DoCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return true;
        }

        internal void Clear()
        {
            Handler.UndoManager.Add(new UndoClearAction(this, this.ToArray()));
            TOM_Clear();
            DoCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        internal void CopyTo(T[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }
        #endregion
        #region Public members
        /// <summary>
        /// Gets the item with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual T this[string name]
        {
            get
            {
                return Handler.WrapperLookup[TOM_Get(name)] as T;
            }
        }

        public virtual T FindByName(string name)
        {
            var tom = TOM_Find(name);

            if (tom != null && Handler.WrapperLookup.TryGetValue(tom, out TabularObject value))
                return value as T;
            else
                return null;
        }

        bool IExpandableIndexer.EnableMultiLine => false;
        /// <summary>
        /// Gets the item on the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get
            {
                if (Handler.WrapperLookup.TryGetValue(TOM_Get(index), out TabularObject item))
                    return item as T;
                else
                    return null;
            }
        }
        /// <summary>
        /// Returns true if this collection contains the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [IntelliSense("Returns true if this collection contains the specified item.")]
        public bool Contains(T item)
        {
            return TOM_Contains(item.MetadataObject);
        }
        /// <summary>
        /// Returns true if this collection contains an item with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return TOM_ContainsName(name);
        }

        /// <summary>
        /// The name of this collection.
        /// </summary>
        [IntelliSense("The name of this collection."),Browsable(false)]
        public string CollectionName { get; private set; }
        /// <summary>
        /// Returns the index of the specified item.
        /// </summary>
        public int IndexOf(T item)
        {
            return IndexOf(item.MetadataObject);
        }

        #endregion

        // Abstract members:
        #region Public abstract members
        /// <summary>
        /// The number of items in this collection.
        /// </summary>
        [IntelliSense("The number of items in this collection."),Browsable(false)]
        public abstract int Count { get; }
        public abstract IEnumerator<T> GetEnumerator();
        internal abstract int IndexOf(TOM.MetadataObject value);
        #endregion
        #region Internal / protected abstract members
        internal abstract string GetNewName(string prefix = null);
        internal abstract void Reinit();
        internal abstract void ReapplyReferences();
        internal abstract void CreateChildrenFromMetadata();
        internal abstract Type GetItemType();
        internal abstract TOM.MetadataObject TOM_Get(string name);
        internal abstract TOM.MetadataObject TOM_Find(string name);
        internal abstract TOM.MetadataObject TOM_Get(int index);
        internal abstract void TOM_Add(TOM.MetadataObject obj);
        internal abstract void TOM_Remove(TOM.MetadataObject obj);
        internal abstract void TOM_Clear();
        internal abstract bool TOM_Contains(TOM.MetadataObject obj);
        internal abstract bool TOM_ContainsName(string name);
        #endregion

        // Interface implementations:
        #region ITabularObjectCollection members
        Type ITabularObjectCollection.ItemType { get { return GetItemType(); } }
        void ITabularObjectCollection.Clear()
        {
            Clear();
        }
        bool ITabularObjectCollection.Contains(string key)
        {
            return Contains(key);
        }
        bool ITabularObjectCollection.Contains(TabularNamedObject obj)
        {
            return Contains(obj as T);
        }

        void ITabularObjectCollection.CreateChildrenFromMetadata()
        {
            CreateChildrenFromMetadata();
        }
        TabularModelHandler ITabularObjectCollection.Handler { get { return _handler; } }
        TabularObject ITabularObjectCollection.Parent { get { return _parent; } }
        int ITabularObjectCollection.IndexOf(TabularNamedObject obj)
        {
            return IndexOf(obj as T);
        }
        void ITabularObjectCollection.Add(TabularNamedObject item)
        {
            Add(item as T);
        }

        void ITabularObjectCollection.Remove(TabularNamedObject item)
        {
            Remove(item as T);
        }

        #endregion
        #region IExpandableIndexer members

        object IExpandableIndexer.this[string name]
        {
            get
            {
                return this[name];
            }

            set
            {
                throw new NotSupportedException();
            }
        }
        string IExpandableIndexer.GetDisplayName(string key)
        {
            return key;
        }

        IEnumerable<string> IExpandableIndexer.Keys
        {
            get
            {
                return this.Select(obj => obj.Name);
            }
        }
        string IExpandableIndexer.Summary => ToString();

        public override string ToString()
        {
            return string.Format("{0} {1}", Count, typeof(T).GetTypeName(Count != 1)).ToLower();
        }

        #endregion
        #region INotifyCollectionChanged members
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion
        #region IEnumerable members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

}

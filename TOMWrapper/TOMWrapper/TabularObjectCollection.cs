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
        ITabularObjectCollection GetCurrentCollection();
        int IndexOf(TabularNamedObject obj);
        TabularNamedObject Parent { get; }
        void CreateChildrenFromMetadata();
        Type ItemType { get; }
    }

    public abstract class TabularObjectCollection<T> : ITabularObjectCollection, IReadOnlyList<T>
        where T: TabularNamedObject
    {
        // Functionality:
        #region CTOR
        protected TabularObjectCollection(string collectionName, TabularNamedObject parent)
        {
            _parent = parent;
            _handler = TabularModelHandler.Singleton;
            CollectionName = collectionName;
            Handler.WrapperCollections[CollectionName] = this;
        }
        #endregion
        #region Internal / private members
        private TabularNamedObject _parent;
        private TabularModelHandler _handler;
        internal protected TabularNamedObject Parent { get { return _parent; } }
        internal TabularModelHandler Handler { get { return _handler; } }

        internal virtual void Add(T item)
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
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        internal virtual bool Remove(T item)
        {
            if (!TOM_Contains(item.MetadataObject)) throw new InvalidOperationException();

            TOM_Remove(item.MetadataObject);
            item.Collection = null;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Remove));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        internal void Clear()
        {
            Handler.UndoManager.Add(new UndoClearAction(this, this.ToArray()));
            TOM_Clear();
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
        public virtual T this[string name]
        {
            get
            {
                return Handler.WrapperLookup[TOM_Get(name)] as T;
            }
        }

        bool IExpandableIndexer.EnableMultiLine => false;

        public virtual T this[int index]
        {
            get
            {
                return Handler.WrapperLookup[TOM_Get(index)] as T;
            }
        }

        [IntelliSense("Returns true if this collection contains the specified item.")]
        public bool Contains(T item)
        {
            return TOM_Contains(item.MetadataObject);
        }

        public bool Contains(string name)
        {
            return TOM_ContainsName(name);
        }

        [IntelliSense("The name of this collection."),Browsable(false)]
        public string CollectionName { get; private set; }

        public int IndexOf(T item)
        {
            return IndexOf(item.MetadataObject);
        }

        #endregion

        // Abstract members:
        #region Public abstract members
        [IntelliSense("The number of items in this collection."),Browsable(false)]
        public abstract int Count { get; }
        public abstract IEnumerator<T> GetEnumerator();
        public abstract int IndexOf(TOM.MetadataObject value);
        #endregion
        #region Internal / protected abstract members
        internal abstract string GetNewName(string prefix = null);
        internal abstract void Reinit();
        internal abstract void ReapplyReferences();
        internal abstract void CreateChildrenFromMetadata();
        internal abstract Type GetItemType();
        protected abstract TOM.MetadataObject TOM_Get(string name);
        protected abstract TOM.MetadataObject TOM_Get(int index);
        protected abstract void TOM_Add(TOM.MetadataObject obj);
        protected abstract void TOM_Remove(TOM.MetadataObject obj);
        protected abstract void TOM_Clear();
        protected abstract bool TOM_Contains(TOM.MetadataObject obj);
        protected abstract bool TOM_ContainsName(string name);
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
        TabularNamedObject ITabularObjectCollection.Parent { get { return _parent; } }
        ITabularObjectCollection ITabularObjectCollection.GetCurrentCollection()
        {
            return Handler.WrapperCollections[CollectionName];
        }
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
        string IExpandableIndexer.Summary
        {
            get
            {
                return string.Format("{0} {1}", Count, typeof(T).GetTypeName(Count != 1));
            }
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

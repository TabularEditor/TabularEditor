using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper
{
    internal interface ITabularObjectCollection: IEnumerable
    {
        TabularModelHandler Handler { get; }
        void Add(TabularNamedObject obj);
        void Remove(TabularNamedObject obj);
        void Clear();
        IEnumerable<string> Keys { get; }
        bool Contains(object value);
        bool Contains(string key);
        string CollectionName { get; }
        ITabularObjectCollection GetCurrentCollection();
        int IndexOf(TabularNamedObject obj);
        TabularNamedObject Parent { get; }
        void CreateChildrenFromMetadata();
    }

    public abstract class TabularObjectCollection<T> : INotifyCollectionChanged, ICollection<T>, IList<T>, IExpandableIndexer, ITabularObjectCollection
        where T: TabularNamedObject
    {
        //int IList.IndexOf(object value)
        //{
        //    return IndexOf(value as T);
        //}

        internal abstract void Reinit();
        internal abstract void ReapplyReferences();

        TabularNamedObject _parent;
        TabularNamedObject ITabularObjectCollection.Parent { get { return _parent; } }
        internal protected TabularNamedObject Parent { get { return _parent; } }

        int ITabularObjectCollection.IndexOf(TabularNamedObject obj)
        {
            return IndexOf(obj as T);
        }

        [IntelliSense("Provide a lambda statement that is executed once for each object in the collection.\nExample: .ForEach(obj => obj.Name += \" OLD\");")]
        public void ForEach(Action<T> action)
        {
            if(this is ColumnCollection)
            {
                // When iterating column collections, make sure to not include the row number:
                this.Where(obj => (obj as Column).Type != ColumnType.RowNumber).ToList().ForEach(action);
            }
            else
                this.ToList().ForEach(action);
        }

        public abstract void CreateChildrenFromMetadata();

        ITabularObjectCollection ITabularObjectCollection.GetCurrentCollection()
        {
            return Handler.WrapperCollections[CollectionName];
        }

        public TabularModelHandler Handler { get; private set; }
        public Model Model { get { return Handler.Model; } }
        [IntelliSense("The name of this collection.")]
        public string CollectionName { get; private set; }

        public void Refresh()
        {

        }

        [IntelliSense("The number of items in this collection.")]
        public abstract int Count { get; }


        [IntelliSense("Whether or not this collection is read-only.")]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [IntelliSense("A summary of this collection's content.")]
        public string Summary
        {
            get
            {
                return Count.ToString();
            }
        }

        public IEnumerable<string> Keys {
            get
            {
                return this.Select(obj => obj.Name);
            }
        }

        public bool IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //object IList.this[int index]
        //{
        //    get
        //    {
        //        return this[index];
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        object IExpandableIndexer.this[string index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        protected TabularObjectCollection(string collectionName, TabularNamedObject parent)
        {
            _parent = parent;
            Handler = TabularModelHandler.Singleton;
            CollectionName = collectionName;
            Handler.WrapperCollections[CollectionName] = this;
        }

        protected abstract TOM.MetadataObject TOM_Get(string name);
        protected abstract TOM.MetadataObject TOM_Get(int index);

        public T this[string name]
        {
            get
            {
                return Handler.WrapperLookup[TOM_Get(name)] as T;
            }
        }

        public virtual T this[int index]
        {
            get
            {
                return Handler.WrapperLookup[TOM_Get(index)] as T;
            }
            set
            {
                throw new InvalidOperationException("");
            }
        }

        public abstract string GetNewName(string prefix = null);

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Notifying child objects
        public virtual void Add(T item)
        {
            if(item.MetadataObject.Parent != null)
                throw new ArgumentException("The item already belongs to a collection.");

            //item.RenewMetadataObject();

            TOM_Add(item.MetadataObject);
            item.Collection = this;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Add));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected abstract void TOM_Add(TOM.MetadataObject obj);
        protected abstract void TOM_Remove(TOM.MetadataObject obj);
        protected abstract void TOM_Clear();
        protected abstract bool TOM_Contains(TOM.MetadataObject obj);

        void ITabularObjectCollection.Add(TabularNamedObject item)
        {
            Add(item as T);
        }

        void ITabularObjectCollection.Remove(TabularNamedObject item)
        {
            Remove(item as T);
        }

        public virtual bool Remove(T item)
        {
            if (!TOM_Contains(item.MetadataObject)) throw new InvalidOperationException();

            TOM_Remove(item.MetadataObject);
            item.Collection = null;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Remove));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        public void Clear()
        {
            Handler.UndoManager.Add(new UndoClearAction(this, this.ToArray()));
            TOM_Clear();
        }

        [IntelliSense("Returns true if this collection contains the specified item.")]
        public bool Contains(T item)
        {
            return TOM_Contains(item.MetadataObject);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            //if (array == null) return;

            for(var i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return IndexOf(item.MetadataObject);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /*public int Add(object value)
        {
            Add(value as T);
            return IndexOf(value as T);
        }*/

        public bool Contains(object value)
        {
            return Contains(value as T);
        }

        public bool Contains(string name)
        {
            return TOM_ContainsName(name);
        }

        protected abstract bool TOM_ContainsName(string name);

        public abstract int IndexOf(TOM.MetadataObject value);

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            Remove(value as T);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(array as T[], index);
        }

        public string GetDisplayName(string key)
        {
            return key;
        }
        #endregion
    }

}

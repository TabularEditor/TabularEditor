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
    public interface ITabularObjectCollection: IEnumerable
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
    }

    public abstract class TabularObjectCollection<T, TT, TP> : IList, INotifyCollectionChanged, ICollection<T>, IList<T>, ITabularObjectCollection, IExpandableIndexer
        where T: TabularNamedObject
        where TT: TOM.NamedMetadataObject
        where TP: TOM.MetadataObject
    {
        public int IndexOf(TabularNamedObject obj)
        {
            return MetadataObjectCollection?.IndexOf(obj.MetadataObject as TT) ?? -1;
        }

        [IntelliSense("Provide a lambda statement that is executed once for each object in the collection.\nExample: .ForEach(obj => obj.Name += \" OLD\");")]
        public void ForEach(Action<T> action)
        {
            if(this is ColumnCollection)
            {
                // When iterating column collections, make sure to not include the row number:
                this.Where(obj => (obj as Column).Type != TOM.ColumnType.RowNumber).ToList().ForEach(action);
            }
            else
                this.ToList().ForEach(action);
        }

        public ITabularObjectCollection GetCurrentCollection()
        {
            return Handler.WrapperCollections[CollectionName];
        }

        private bool init = false;
        public TabularModelHandler Handler { get; private set; }
        [IntelliSense("The name of this collection.")]
        public string CollectionName { get; private set; }

        protected internal TOM.NamedMetadataObjectCollection<TT, TP> MetadataObjectCollection { get; private set; }

        public virtual void Refresh()
        {

        }

        [IntelliSense("The number of items in this collection.")]
        public virtual int Count
        {
            get
            {
                return MetadataObjectCollection.Count;
            }
        }


        [IntelliSense("Whether or not this collection is read-only.")]
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [IntelliSense("A summary of this collection's content.")]
        public virtual string Summary
        {
            get
            {
                return Count.ToString();
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return MetadataObjectCollection.Select(i => i.Name);
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

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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

        protected TabularObjectCollection(string collectionName, TOM.NamedMetadataObjectCollection<TT, TP> metadataObjectCollection)
        {
            MetadataObjectCollection = metadataObjectCollection;
            Handler = TabularModelHandler.Singleton;
            CollectionName = collectionName;
            Handler.WrapperCollections[CollectionName] = this;
        }

        public T this[string name]
        {
            get
            {
                return Handler.WrapperLookup[MetadataObjectCollection[name]] as T;
            }
        }

        public T this[int index]
        {
            get
            {
                return Handler.WrapperLookup[MetadataObjectCollection[index]] as T;
            }
            set
            {
                throw new InvalidOperationException("");
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Notifying child objects
        public virtual void Add(T item)
        {
            if(item.MetadataObject.Parent != null)
                throw new ArgumentException("The item already belongs to a collection.");

            //item.RenewMetadataObject();

            MetadataObjectCollection.Add(item.MetadataObject as TT);
            item.Collection = this;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Add));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Add(TabularNamedObject item)
        {
            Add(item as T);
        }

        public void Remove(TabularNamedObject item)
        {
            Remove(item as T);
        }

        public virtual bool Remove(T item)
        {
            if (item.MetadataObject.Parent != MetadataObjectCollection.Parent)
                throw new InvalidOperationException();

            MetadataObjectCollection.Remove(item.MetadataObject as TT);
            item.Collection = null;

            Handler.UndoManager.Add(new UndoAddRemoveAction(this, item, UndoAddRemoveActionType.Remove));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        public virtual void Clear()
        {
            Handler.UndoManager.Add(new UndoClearAction(this, this.ToArray()));
            MetadataObjectCollection.Clear();
        }

        [IntelliSense("Returns true if this collection contains the specified item.")]
        public virtual bool Contains(T item)
        {
            return MetadataObjectCollection.Contains(item.MetadataObject);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            //if (array == null) return;

            for(var i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return MetadataObjectCollection.Select(obj => Handler.WrapperLookup[obj] as T).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return MetadataObjectCollection.IndexOf(item.MetadataObject as TT);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        public int Add(object value)
        {
            Add(value as T);
            return IndexOf(value as T);
        }

        public bool Contains(object value)
        {
            return Contains(value as T);
        }

        public bool Contains(string name)
        {
            return MetadataObjectCollection.ContainsName(name);
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

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

        public virtual string GetDisplayName(string key)
        {
            return key;
        }
        #endregion
    }

}

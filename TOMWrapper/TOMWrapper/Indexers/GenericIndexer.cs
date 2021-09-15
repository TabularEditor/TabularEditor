using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using System.Collections;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(IndexerConverter))]
    public abstract class GenericIndexer<T, T1> : IEnumerable<T1>, IExpandableIndexer, IGenericIndexer
        where T : TabularNamedObject
    {
        internal TabularObject ParentObject { get; private set; }
        TabularObject IGenericIndexer.ParentObject => ParentObject;
        protected TabularModelHandler Handler;
        protected Model Model { get { return Handler.Model; } }
        protected GenericIndexer(TabularObject parent)
        {
            this.ParentObject = parent;
            Handler = parent.Handler;
        }
        protected virtual bool EnableMultiLine => false;
        bool IExpandableIndexer.EnableMultiLine => EnableMultiLine;

        protected virtual T1 EmptyValue { get { return default(T1); } }
        protected abstract void SetValue(T key, T1 value);
        protected abstract T1 GetValue(T key);
        protected abstract TabularObjectCollection<T> GetCollection();
        protected virtual T GetObjectFromName(string name)
        {
            return Collection[name];
        }
        protected TabularObjectCollection<T> Collection { get { return GetCollection(); } }

        protected virtual bool IsEmptyValue(T1 value)
        {
            if (value == null && EmptyValue == null) return true;
            else if (value == null && EmptyValue != null) return false;
            else return value.Equals(EmptyValue);
        }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(
                Keys.Where(k => !IsEmptyValue(this[k]))
                    .ToDictionary(k => k, k => this[k])
            );
        }


        public Dictionary<string, T1> Copy()
        {
            return Keys.ToDictionary(k => k, k => this[k]);
        }

        public void CopyFrom(GenericIndexer<T, T1> source, Func<T1,T1> mutator = null)
        {
            foreach(var key in source.Keys)
            {
                var value = source[key];
                if (IsEmptyValue(value)) continue;
                if (mutator != null) value = mutator(value);
                this[key] = value;
            }
        }

        public void CopyFrom(IDictionary<string, T1> source)
        {
            foreach(var key in Keys)
            {
                T1 value;
                if (source.TryGetValue(key, out value))
                    this[key] = value;
                else
                    this[key] = EmptyValue;
            }
        }

        public bool IsEmpty
        {
            get { return Keys.All(k => IsEmptyValue(this[k])); }
        }

        public virtual void Clear()
        {
            Handler.BeginUpdate(DescribeAction("clear"));
            SetAll(EmptyValue);
            Handler.EndUpdate();
        }

        public void SetAll(T1 value)
        {
            Handler.BeginUpdate(DescribeAction("set all"));
            foreach (var key in Keys) this[key] = value;
            Handler.EndUpdate();
        }

        private string DescribeAction(string action)
        {
            return action + " " + typeof(T).GetTypeName(true);
        }

        object IExpandableIndexer.this[string index]
        {
            get { return this[index]; }
            set { this[index] = (T1)value; }
        }

        public T1 this[string index]
        {
            get
            {
                return this[GetObjectFromName(index)];
            }

            set
            {
                this[GetObjectFromName(index)] = value;
            }
        }

        public T1 this[T key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public virtual IEnumerable<string> Keys { get { return Collection.Select(obj => obj.Name); } }

        public virtual string Summary
        {
            get
            {
                return string.Format("Value assigned to {0} out of {1} {2}",
                    this.Count(v => !IsEmptyValue(v)),
                    Collection.Count,
                    typeof(T).GetTypeName(true));
            }
        }

        public virtual string GetDisplayName(string key)
        {
            return key;
        }

        public IEnumerator<T1> GetEnumerator()
        {
            return Collection.Select(k => GetValue(k)).GetEnumerator();
        }

        /*public virtual void Refresh()
        {

        }*/

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal interface IGenericIndexer
    {
        TabularObject ParentObject { get; }
    }
}

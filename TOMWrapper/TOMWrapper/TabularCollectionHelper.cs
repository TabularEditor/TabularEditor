using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    internal class TabularObjectCollectionWrapper : IList
    {
        public static TabularObjectCollectionWrapper Wrap(ITabularObjectCollection collection)
        {
            return new TabularObjectCollectionWrapper();
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
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

        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    public static class TabularCollectionHelper
    {

        [IntelliSense("Provide a lambda statement that is executed once for each object in the collection.\nExample: .ForEach(obj => obj.Name += \" OLD\");")]
        public static void ForEach<T>(this IEnumerable<ITabularNamedObject> collection, Action<T> action)
        {
            collection.ToList().ForEach(action);
        }

        public static void InPerspective(this IEnumerable<Table> tables, string perspective, bool value)
        {
            foreach (var m in tables) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Column> columns, string perspective, bool value)
        {
            foreach (var m in columns) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Hierarchy> hierarchies, string perspective, bool value)
        {
            foreach (var m in hierarchies) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Measure> measures, string perspective, bool value)
        {
            foreach(var m in measures) m.InPerspective[perspective] = value;
        }

        public static void InPerspective(this IEnumerable<Table> tables, Perspective perspective, bool value)
        {
            foreach (var m in tables) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Column> columns, Perspective perspective, bool value)
        {
            foreach (var m in columns) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Hierarchy> hierarchies, Perspective perspective, bool value)
        {
            foreach (var m in hierarchies) m.InPerspective[perspective] = value;
        }
        public static void InPerspective(this IEnumerable<Measure> measures, Perspective perspective, bool value)
        {
            foreach (var m in measures) m.InPerspective[perspective] = value;
        }

        public static void SetDisplayFolder(this IEnumerable<Measure> measures, string displayFolder)
        {
            foreach (var m in measures) m.DisplayFolder = displayFolder;
        }
    }
}

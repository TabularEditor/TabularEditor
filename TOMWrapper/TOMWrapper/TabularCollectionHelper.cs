using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
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

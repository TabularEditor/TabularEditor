using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Utils
{
    /// <summary>
    /// A DependsOnList holds a dictionary of all objects that a specific opject depends on. Each entry contains
    /// a list of ObjectReferences specifying the details of how the object is referenced.
    /// </summary>
    public class DependsOnList : IReadOnlyDictionary<IDaxObject, List<ObjectReference>>
    {
        /// <summary>
        /// Returns all objects used by the current object (directly or indirectly through other objects).
        /// </summary>
        public HashSet<IDaxObject> Deep()
        {
            var uniqueOnly = new HashSet<IDaxObject>();
            Deep_Internal(uniqueOnly);
            return uniqueOnly;
        }

        private void Deep_Internal(HashSet<IDaxObject> uniqueOnly)
        {
            foreach (var child in Keys)
            {
                if (uniqueOnly.Add(child))
                {
                    if ((child is IDaxDependantObject))
                    {
                        (child as IDaxDependantObject)?.DependsOn.Deep_Internal(uniqueOnly);
                    }
                }
            }
        }

        public static DependsOnList GetDependencies(IDaxDependantObject obj, string dax, DAXProperty prop)
        {
            return FormulaFixup.GetDependencies(obj, dax, prop);
        }

        public IDaxObject GetObjectAt(DAXProperty property, int charIndex)
        {
            foreach (var kvp in InternalDictionary)
            {
                if (kvp.Value.Any(r => r.property == property && charIndex >= r.from && charIndex <= r.to))
                    return kvp.Key;
            }
            return null;
        }

        internal readonly IDaxDependantObject Parent;
        internal DependsOnList(IDaxDependantObject parent)
        {
            Parent = parent;
        }

        private Dictionary<IDaxObject, List<ObjectReference>> InternalDictionary = new Dictionary<IDaxObject, List<ObjectReference>>();
        private List<IDaxObject> InternalList = new List<IDaxObject>();

        internal void Add(IDaxObject dependsOn, DAXProperty property, int fromChar, int toChar, bool fullyQualified)
        {
            var dep = new ObjectReference { property = property, from = fromChar, to = toChar, fullyQualified = fullyQualified };
            List<ObjectReference> depList;
            if (!InternalDictionary.TryGetValue(dependsOn, out depList))
            {
                depList = new List<ObjectReference>();
                InternalDictionary.Add(dependsOn, depList);
                InternalList.Add(dependsOn);
            }
            depList.Add(dep);
        }

        readonly int DaxPropertyCount = Enum.GetValues(typeof(DAXProperty)).Length;

        internal void UpdateRef(IDaxObject renamedObj)
        {
            // This method is called on a DependsOnList, which belongs to a IDaxDependantObject. The argument is the IDaxObject which was typically renamed.
            // For example: The DependsOnList could be attached to a Measure [MyMeasure] with an expression such as: "COUNTROWS('MyTable')". If the name of 
            // 'MyTable' was changed, this method would be called with 'MyTable' as the renamedObj and [MyMeasure] as Parent. It is then up to this method
            // to loop through all object references to 'MyTable' (see #1). As an IDaxDependantObject can have more than one DAX expression property, and an
            // object reference to 'MyTable' could exist in any of them, we must make sure to loop all the properties (#2)

            List<ObjectReference> depList;
            if (TryGetValue(renamedObj, out depList))   // #1 Get a list of object references for renamedObj
            {
                var orgDax = new string[DaxPropertyCount];
                var pos = new int[DaxPropertyCount];
                var sbs = new StringBuilder[DaxPropertyCount];

                foreach (var prop in Parent.GetDAXProperties())  // #2 Initialize a string buider and the original DAX for every DAX property on Parent
                {
                    var dax = Parent.GetDAX(prop);
                    if (dax != null)
                    {
                        sbs[(int)prop] = new StringBuilder();
                        orgDax[(int)prop] = dax.Replace("\r", ""); // Carriage Returns are excluded when the dependency tree is built
                    }
                }

                // Loop through all dependencies:
                foreach (var dep in depList)
                {
                    var propIx = (int)dep.property;

                    var sb = sbs[propIx];

                    sb.Append(orgDax[propIx].Substring(pos[propIx], dep.from - pos[propIx]));
                    sb.Append(dep.fullyQualified ? renamedObj.DaxObjectFullName : renamedObj.DaxObjectName);
                    pos[propIx] = dep.to + 1;
                }

                // Finalize:
                foreach (var prop in Parent.GetDAXProperties())
                {
                    var propIx = (int)prop;

                    if (pos[propIx] > 0)
                    {
                        sbs[propIx].Append(orgDax[propIx].Substring(pos[propIx]));
                        Parent.SetDAX(prop, sbs[propIx].ToString());
                    }
                }
            }
        }

        public IEnumerable<Measure> Measures { get { return InternalList.OfType<Measure>(); } }
        public IEnumerable<Column> Columns { get { return InternalList.OfType<Column>(); } }
        public IEnumerable<Table> Tables { get { return InternalList.OfType<Table>(); } }

        #region IDictionary members
        public void Clear()
        {
            InternalDictionary.Clear();
            InternalList.Clear();
        }
        public void Remove(IDaxObject key)
        {
            InternalDictionary.Remove(key);
            InternalList.Remove(key);
        }

        public List<ObjectReference> this[IDaxObject key]
        {
            get
            {
                return InternalDictionary[key];
            }
        }

        public int Count
        {
            get
            {
                return InternalDictionary.Count;
            }
        }

        public IEnumerable<IDaxObject> Keys
        {
            get
            {
                return InternalList;
            }
        }

        public IEnumerable<List<ObjectReference>> Values
        {
            get
            {
                return InternalDictionary.Values;
            }
        }

        public IDaxObject this[int index]
        {
            get
            {
                return InternalList[index];
            }
        }

        public bool ContainsKey(IDaxObject key)
        {
            return InternalDictionary.ContainsKey(key);
        }

        public bool TryGetValue(IDaxObject key, out List<ObjectReference> value)
        {
            return InternalDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<IDaxObject, List<ObjectReference>>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<IDaxObject, List<ObjectReference>>)InternalDictionary).GetEnumerator();
        }
        #endregion
    }

    public class ReferencedByList : HashSet<IDaxDependantObject>
    {
        /// <summary>
        /// Returns all objects that reference the current object (directly or indirectly through other objects).
        /// </summary>
        /// <returns></returns>
        public HashSet<IDaxDependantObject> Deep()
        {
            var uniqueOnly = new HashSet<IDaxDependantObject>();
            Deep_Internal(uniqueOnly);
            return uniqueOnly;
        }

        private void Deep_Internal(HashSet<IDaxDependantObject> uniqueOnly)
        {
            foreach (var child in this)
            {
                if (uniqueOnly.Add(child))
                {
                    if ((child is IDaxObject))
                    {
                        (child as IDaxObject)?.ReferencedBy.Deep_Internal(uniqueOnly);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates the entire dependency tree and determines if any objects referencing the current object
        /// (directly or indirectly through other objects) are visible. This may be used, for example, to
        /// create a Best Practice Rule that allows you to detect objects that can safely be removed since
        /// they do not have any references.
        /// </summary>
        public bool AnyVisible
        {
            get
            {
                foreach (var obj in Deep())
                {
                    if ((obj as IHideableObject)?.IsHidden == false && (obj as ITabularTableObject)?.Table?.IsHidden == false) return true;
                    if ((obj as KPI)?.Measure?.IsHidden == false && (obj as KPI)?.Measure?.Table?.IsHidden == false) return true;

                }
                return false;
            }
        }

        public IEnumerable<Measure> AllMeasures { get { return Deep().OfType<Measure>(); } }
        public IEnumerable<CalculatedColumn> AllColumns { get { return Deep().OfType<CalculatedColumn>(); } }
        public IEnumerable<CalculatedTable> AllTables { get { return Deep().OfType<CalculatedTable>(); } }

        public IEnumerable<Measure> Measures { get { return this.OfType<Measure>(); } }
        public IEnumerable<CalculatedColumn> Columns { get { return this.OfType<CalculatedColumn>(); } }
        public IEnumerable<CalculatedTable> Tables { get { return this.OfType<CalculatedTable>(); } }
        public IEnumerable<ModelRole> Roles { get { return this.OfType<TablePermission>().Select(r => r.Role); } }
    }

    public enum DAXProperty
    {
        Expression = 0,
        DetailRowsExpression = 1,
        TargetExpression = 2,
        StatusExpression = 3,
        TrendExpression = 4,
        DefaultDetailRowsExpression = 5,
        FormatStringExpression = 6
    }

    public static class DAXPropertyHelper
    {
        public static string GetDescription(this DAXProperty property)
        {
            switch (property)
            {
                case DAXProperty.DetailRowsExpression: return "Detail Rows Expression";
                case DAXProperty.DefaultDetailRowsExpression: return "Default Detail Rows Expression";
                case DAXProperty.TargetExpression: return "KPI Target Expression";
                case DAXProperty.StatusExpression: return "KPI Status Expression";
                case DAXProperty.TrendExpression: return "KPI Trend Expression";
                case DAXProperty.FormatStringExpression: return "Format String Expression";
                default: return "Expression";
            }
        }
    }

    public struct ObjectReference
    {
        public DAXProperty property;
        public int from;
        public int to;
        public bool fullyQualified;

    }

    internal static class DependencyHelper
    {
        static public void AddDep(this IDaxDependantObject target, IDaxObject dependsOn, DAXProperty property, int fromChar, int toChar, bool fullyQualified)
        {
            target.DependsOn.Add(dependsOn, property, fromChar, toChar, fullyQualified);
            if (!dependsOn.ReferencedBy.Contains(target)) dependsOn.ReferencedBy.Add(target);
        }
    }
}

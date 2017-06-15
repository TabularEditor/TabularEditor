using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(IndexerConverter))]
    public abstract class PerspectiveIndexer : IEnumerable<bool>, IExpandableIndexer
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(Keys.Where(k => this[k]).ToArray());
        }

        public Dictionary<string, bool> Copy()
        {
            return Keys.ToDictionary(k => k, k => this[k]);
        }

        public virtual void Refresh()
        {

        }

        [IntelliSense("Copies all perspectives from another perspective collection.")]
        public void CopyFrom(PerspectiveIndexer source)
        {
            foreach(var p in source.Keys)
            {
                var value = source[p];
                this[p] = value;
            }
        }

        public void CopyFrom(IEnumerable<string> source)
        {
            None();
            foreach(var persp in source)
            {
                this[persp] = true;
            }
        }

        public void CopyFrom(IDictionary<string, bool> source)
        {
            foreach(var kvp in source)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Includes the object in all perspectives.
        /// </summary>
        [IntelliSense("Includes the object in all perspectives.")]
        public void All()
        {
            TabularObject.Handler.BeginUpdate("all perspectives");
            foreach (var key in Keys.ToList()) this[key] = true;
            TabularObject.Handler.EndUpdate();
        }

        [IntelliSense("Removes the object from all perspectives.")]
        /// <summary>
        /// Removes the object from all perspectives.
        /// </summary>
        public void None()
        {
            TabularObject.Handler.BeginUpdate("no perspectives");
            foreach (var key in Keys.ToList()) this[key] = false;
            TabularObject.Handler.EndUpdate();
        }

        private Dictionary<Perspective, bool> perspectiveMap;
        protected TabularNamedObject TabularObject;

        public string Summary
        {
            get
            {
                return string.Format("Shown in {0} out of {1} perspectives", this.Count(p => p), TabularObject.Model.Perspectives.Count);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return PerspectiveMap.Keys.Select(k => k.Name);
            }
        }

        protected Dictionary<Perspective, bool> PerspectiveMap
        {
            get
            {
                if (perspectiveMap == null) Refresh();
                return perspectiveMap;
            }

            set
            {
                perspectiveMap = value;
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
                this[index] = (bool)value;
            }
        }

        protected PerspectiveIndexer(TabularNamedObject tabularObject)
        {
            TabularObject = tabularObject;
            if ((TabularObject as ITabularTableObject)?.Table != null) Refresh();
        }

        public virtual bool this[Perspective perspective]
        {
            get {
                if (PerspectiveMap == null) Refresh();
                if (!PerspectiveMap.ContainsKey(perspective)) Refresh();
                return PerspectiveMap[perspective];
            }
            set
            {
                var oldValue = this[perspective];
                if (value == oldValue) return;

                PerspectiveMap[perspective] = value;
                SetInPerspective(perspective, value);

                TabularObject.Handler.UndoManager.Add(
                    new UndoPropertyChangedAction(TabularObject, "InPerspective", oldValue, value, perspective.Name));

                // Only apply structure change if the perspective that was changed is the currently visible perspective:
                if (TabularObject.Handler.Tree.Perspective == perspective)
                    TabularObject.Handler.Tree.OnStructureChanged(TabularObject.Model);
            }
        }

        protected abstract void SetInPerspective(Perspective perspective, bool included);

        public bool this[string perspective]
        {
            get {
                return Perspectives.Contains(perspective) ? this[Perspectives[perspective]] : false;
            }
            set
            {
                if(Perspectives.Contains(perspective)) this[Perspectives[perspective]] = value;
            }
        }

        private PerspectiveCollection Perspectives
        {
            get
            {
                return TabularObject.Model.Perspectives;
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return PerspectiveMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string GetDisplayName(string key)
        {
            return key;
        }
    }

    public class PerspectiveTableIndexer : PerspectiveIndexer
    {
        protected Table Table { get { return TabularObject as Table; } }

        public PerspectiveTableIndexer(Table table) : base(table)
        {
        }

        public override void Refresh()
        {
            PerspectiveMap = Table.Model.Perspectives.ToDictionary(p => p, p => this[p]);
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            // Including/excluding a table from a perspective, is equivalent to including/excluding all child
            // objects. The PerspectiveTable will be created automatically if needed.
            Table.Measures.InPerspective(perspective, included);
            Table.Hierarchies.InPerspective(perspective, included);
            Table.Columns.InPerspective(perspective, included);

            var pts = perspective.MetadataObject.PerspectiveTables;
            if (!included && pts.Contains(Table.Name)) pts.Remove(Table.Name);
        }

        public override bool this[Perspective perspective]
        {
            get
            {
                return Table.GetChildren().OfType<ITabularPerspectiveObject>().Any(obj => obj.InPerspective[perspective]);
            }

            set
            {
                SetInPerspective(perspective, value);
            }
        }

        public TOM.PerspectiveTable EnsurePTExists(Perspective perspective)
        {
            var pts = perspective.MetadataObject.PerspectiveTables;
            var table = Table.MetadataObject;
            TOM.PerspectiveTable result;

            if (pts.Contains(table.Name))
                result = pts[table.Name];
            else
            {
                result = new TOM.PerspectiveTable { Table = table };
                pts.Add(result);
            }
            return result;
        }
    }
    public class PerspectiveColumnIndexer : PerspectiveIndexer
    {
        protected Column Column { get { return TabularObject as Column; } }

        public PerspectiveColumnIndexer(Column column) : base(column)
        {
        }

        public override void Refresh()
        {
            PerspectiveMap = Column.Model.Perspectives.ToDictionary(p => p,
                p => p.MetadataObject.PerspectiveTables.FirstOrDefault(pt => pt.Table == Column.Table.MetadataObject)
                        ?.PerspectiveColumns.Any(pc => pc.Column == Column.MetadataObject) ?? false);
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var c = Column.MetadataObject;
            var t = Column.Table;
            var pcs = (Column.Table.InPerspective as PerspectiveTableIndexer).EnsurePTExists(perspective).PerspectiveColumns;

            if (included)
            {
                if (!pcs.Contains(c.Name))
                {
                    pcs.Add(new TOM.PerspectiveColumn { Column = c });
                }
            } else
            {
                if (pcs != null)
                {
                    pcs.Remove(c.Name);
                }
            }
        }
    }

    public class PerspectiveMeasureIndexer : PerspectiveIndexer
    {
        protected Measure Measure { get { return TabularObject as Measure; } }

        public PerspectiveMeasureIndexer(Measure measure) : base(measure)
        {
        }

        public override void Refresh()
        {
            PerspectiveMap = Measure.Model.Perspectives.ToDictionary(p => p,
                p => p.MetadataObject.PerspectiveTables.FirstOrDefault(pt => pt.Table == Measure.Table.MetadataObject)
                        ?.PerspectiveMeasures.Any(pc => pc.Measure == Measure.MetadataObject) ?? false);
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var m = Measure.MetadataObject;
            var t = Measure.Table;
            var pms = (Measure.Table.InPerspective as PerspectiveTableIndexer).EnsurePTExists(perspective).PerspectiveMeasures;

            if (included)
            {
                if (!pms.Contains(m.Name))
                {
                    pms.Add(new TOM.PerspectiveMeasure { Measure = m });
                }
            }
            else
            {
                if (pms != null)
                {
                    pms.Remove(m.Name);
                }
            }
        }
    }

    public class PerspectiveHierarchyIndexer : PerspectiveIndexer
    {
        protected Hierarchy Hierarchy { get { return TabularObject as Hierarchy; } }

        public PerspectiveHierarchyIndexer(Hierarchy hierarchy) : base(hierarchy)
        {
        }

        public override void Refresh()
        {
            PerspectiveMap = Hierarchy.Model.Perspectives.ToDictionary(p => p,
                p => p.MetadataObject.PerspectiveTables.FirstOrDefault(pt => pt.Table == Hierarchy.Table.MetadataObject)
                        ?.PerspectiveHierarchies.Any(pc => pc.Hierarchy == Hierarchy.MetadataObject) ?? false);
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var h = Hierarchy.MetadataObject;
            var t = Hierarchy.Table;
            var phs = (Hierarchy.Table.InPerspective as PerspectiveTableIndexer).EnsurePTExists(perspective).PerspectiveHierarchies;

            if (included)
            {
                if (!phs.Contains(h.Name))
                {
                    phs.Add(new TOM.PerspectiveHierarchy { Hierarchy = h });
                }
            }
            else
            {
                if (phs != null)
                {
                    phs?.Remove(h.Name);
                }
            }
        }
    }

}

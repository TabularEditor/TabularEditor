using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(IndexerConverter))]
    public abstract class PerspectiveIndexer : GenericIndexer<Perspective, bool>
    {
        protected readonly ITabularPerspectiveObject PerspectiveObject;

        internal PerspectiveIndexer(ITabularPerspectiveObject perspectiveObject) : base(perspectiveObject as TabularObject)
        {
            PerspectiveObject = perspectiveObject;
        }

        protected override TabularObjectCollection<Perspective> GetCollection()
        {
            return Model.Perspectives;
        }

        protected abstract void SetInPerspective(Perspective perspective, bool include);

        protected override void SetValue(Perspective perspective, bool include)
        {
            var oldValue = this[perspective];

            if (!(this is PerspectiveTableIndexer) && oldValue != include)
                Handler.UndoManager.Add(
                    new UndoPropertyChangedAction(ParentObject, "InPerspective", oldValue, include, perspective.Name));

            // Call this method regardless of whether there was an actual change (that is, before we check
            // if the new value is equal to oldValue). This is because a call to InPerspective.None() happens
            // when an object is deleted, so we need to make sure this method is called, so that the actual
            // metadata perspective objects are deleted.
            SetInPerspective(perspective, include);

            // Only apply structure change if the perspective that was changed is the currently visible perspective:
            if (Handler.Tree.Perspective == perspective) Handler.Tree.OnStructureChanged(Model);
        }

        public void CopyFrom(string[] perspectives)
        {
            IsCopying = true;
            CopyFrom(perspectives.ToDictionary(s => s, s => true));
            IsCopying = false;
        }

        protected bool IsCopying { get; private set; } = false;

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(Keys.Where(k => this[k]).OrderBy(n => n).ToArray());
        }

        /// <summary>
        /// Removes the object from all perspectives.
        /// </summary>
        [IntelliSense("Removes the object from all perspectives.")]
        public void None()
        {
            Handler.BeginUpdate("no perspectives");
            SetAll(false);
            Handler.EndUpdate();
        }

        /// <summary>
        /// Includes the object in all perspectives.
        /// </summary>
        [IntelliSense("Includes the object in all perspectives.")]
        public void All()
        {
            Handler.BeginUpdate("all perspectives");
            SetAll(true);
            Handler.EndUpdate();
        }

        public override string Summary
        {
            get
            {
                return string.Format("Shown in {0} out of {1} perspectives", Keys.Count(k => this[k]), Model.Perspectives.Count);
            }
        }
    }

    public class PerspectiveTableIndexer : PerspectiveIndexer
    {
        protected Table Table { get { return PerspectiveObject as Table; } }

        internal PerspectiveTableIndexer(Table table) : base(table)
        {
        }

        protected override bool GetValue(Perspective perspective)
        {
            // Handle tables without children:
            if(!Table.GetChildren().Any())
            {
                return perspective.MetadataObject.PerspectiveTables.Contains(Table.Name);
            }

            // Tables with children:
            return Table.GetChildren().OfType<ITabularPerspectiveObject>().Any(obj => obj.InPerspective[perspective]);
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var pts = perspective.MetadataObject.PerspectiveTables;
            if (included)
            {
                if (!pts.Contains(Table.Name)) pts.Add(new Microsoft.AnalysisServices.Tabular.PerspectiveTable() { Table = Table.MetadataObject });
                else return; // If TOM already contains a PerspectiveTable then we don't need to do anything.
            }
            if (!included && !pts.Contains(Table.Name))
                return; // If no PerspectiveTable exists in TOM then we're done.

            // Including/excluding a table from a perspective, is equivalent to including/excluding all child
            // objects. The PerspectiveTable will be created automatically if needed.
            if (!IsCopying)
            {
                Table.Measures.InPerspective(perspective, included);
                Table.Hierarchies.InPerspective(perspective, included);
                Table.Columns.InPerspective(perspective, included);
            }

            if (!included && pts.Contains(Table.Name)) pts.Remove(Table.Name);
        }

        public TOM.PerspectiveTable EnsurePTExists(Perspective perspective)
        {
            var pts = perspective.MetadataObject.PerspectiveTables;
            var pt = pts.Find(Table.Name);

            if(pt == null) {
                pt = new TOM.PerspectiveTable { Table = Table.MetadataObject };
                pts.Add(pt);
            }
            return pt;
        }
    }
    public class PerspectiveColumnIndexer : PerspectiveIndexer
    {
        protected Column Column { get { return PerspectiveObject as Column; } }

        internal PerspectiveColumnIndexer(Column column) : base(column)
        {
        }

        private TOM.PerspectiveTable GetPerspectiveTable(Perspective perspective)
        {
            return perspective.MetadataObject.PerspectiveTables.Find(Column.MetadataObject.Table.Name);
        }

        protected override bool GetValue(Perspective key)
        {
            var pTable = GetPerspectiveTable(key);
            if (pTable == null) return false;

            var pColumns = pTable.PerspectiveColumns.Find(Column.Name);
            return pColumns != null;
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var column = Column.MetadataObject;
            var pColumns = GetPerspectiveTable(perspective)?.PerspectiveColumns;

            if (included)
            {
                pColumns = Column.Table.InPerspective.EnsurePTExists(perspective).PerspectiveColumns;

                if (!pColumns.Contains(column.Name))
                {
                    pColumns.Add(new TOM.PerspectiveColumn { Column = column });
                }
            }
            else
            {
                if (pColumns != null && pColumns.Contains(column.Name))
                {
                    pColumns.Remove(column.Name);
                }
            }
        }
    }

    public class PerspectiveMeasureIndexer : PerspectiveIndexer
    {
        protected Measure Measure { get { return PerspectiveObject as Measure; } }

        internal PerspectiveMeasureIndexer(Measure Measure) : base(Measure)
        {
        }

        private TOM.PerspectiveTable GetPerspectiveTable(Perspective perspective)
        {
            return perspective.MetadataObject.PerspectiveTables.Find(Measure.MetadataObject.Table.Name);
        }

        protected override bool GetValue(Perspective key)
        {
            var pTables = GetPerspectiveTable(key);
            if (pTables == null) return false;

            var pMeasures = pTables.PerspectiveMeasures.Find(Measure.Name);
            return pMeasures != null;
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var measure = Measure.MetadataObject;
            var pMeasures = GetPerspectiveTable(perspective)?.PerspectiveMeasures;

            if (included)
            {
                pMeasures = Measure.Table.InPerspective.EnsurePTExists(perspective).PerspectiveMeasures;

                if (!pMeasures.Contains(measure.Name))
                {
                    pMeasures.Add(new TOM.PerspectiveMeasure { Measure = measure });
                }
            }
            else
            {
                if (pMeasures != null && pMeasures.Contains(measure.Name))
                {
                    pMeasures.Remove(measure.Name);
                }
            }
        }
    }

    public class PerspectiveHierarchyIndexer : PerspectiveIndexer
    {
        protected Hierarchy Hierarchy { get { return PerspectiveObject as Hierarchy; } }

        public PerspectiveHierarchyIndexer(Hierarchy Hierarchy) : base(Hierarchy)
        {
        }

        private TOM.PerspectiveTable GetPerspectiveTable(Perspective perspective)
        {
            return perspective.MetadataObject.PerspectiveTables.Find(Hierarchy.MetadataObject.Table.Name);
        }

        protected override bool GetValue(Perspective key)
        {
            var pTables = GetPerspectiveTable(key);
            if (pTables == null) return false;

            var pHierarchys = pTables.PerspectiveHierarchies.Find(Hierarchy.Name);
            return pHierarchys != null;
        }

        protected override void SetInPerspective(Perspective perspective, bool included)
        {
            var hierarchy = Hierarchy.MetadataObject;
            var pHierarchys = GetPerspectiveTable(perspective)?.PerspectiveHierarchies;

            if (included)
            {
                pHierarchys = Hierarchy.Table.InPerspective.EnsurePTExists(perspective).PerspectiveHierarchies;

                if (!pHierarchys.Contains(hierarchy.Name))
                {
                    pHierarchys.Add(new TOM.PerspectiveHierarchy { Hierarchy = hierarchy });
                }
            }
            else
            {
                if (pHierarchys != null && pHierarchys.Contains(hierarchy.Name))
                {
                    pHierarchys.Remove(hierarchy.Name);
                }
            }
        }
    }
}

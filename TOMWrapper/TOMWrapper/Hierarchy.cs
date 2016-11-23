using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Hierarchy: ITabularObjectContainer, ITabularPerspectiveObject
    {
        public override void Delete()
        {
            InPerspective.None();

            base.Delete();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Hierarchy();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);

            Init();
        }

        #region Convenient methods
        public void AddLevels(IEnumerable<Column> columns, int ordinal = -1)
        {
            if (ordinal == -1) ordinal = Levels.Count;

            var numCols = columns.Count();
            if (numCols == 0) return;

            Handler.BeginUpdate("add levels");

            foreach (var c in columns.Reverse())
            {
                var level = AddLevel(c, ordinal: ordinal);
                level.TranslatedNames.CopyFrom(c.TranslatedNames);
            }

            Handler.EndUpdate();
        }


        public Level AddLevel(Column column, string levelName = null, int ordinal = -1)
        {
            if (column == null) throw new ArgumentNullException("column");
            if (column.Table != Table)
                throw new ArgumentException(string.Format("Column {0} cannot be added as a level in hierarchy {1} because it is in another table.", column.Name, Name));
            if (Levels.Any(l => l.Column == column))
                throw new ArgumentException(string.Format("Column {0} already exists as a level in hierarchy {1}.", column.Name, Name));

            Handler.BeginUpdate("add level");
            var level = new Level(column, this, levelName);
            if (levelName == null) level.TranslatedNames.CopyFrom(column.TranslatedNames);
            level.Ordinal = ordinal == -1 ? this.Levels.Count - 1 : ordinal;
            Handler.EndUpdate();

            return level;
        }

        public Level AddLevel(string columnName, string levelName = null, int ordinal = -1)
        {
            return AddLevel(Table.Columns[columnName], levelName, ordinal);
        }
        #endregion

        [Browsable(true), DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

        public IEnumerable<TabularNamedObject> GetChildren()
        {
            return Levels;
        }

        protected override void Init()
        {
            Levels = new LevelCollection(Handler, this.GetObjectPath() + ".Levels", MetadataObject.Levels);

            InPerspective = new PerspectiveHierarchyIndexer(this);
        }

        [Browsable(false)]
        public LevelCollection Levels { get; private set; }

        private bool _reordering = false;
        /// <summary>
        /// Set to true, when multiple levels are going to be re-ordered as one action.
        /// </summary>
        [Browsable(false)]
        public bool Reordering
        {
            get
            {
                return _reordering;
            }
            set
            {
                if(value)
                {
                    if (_reordering) throw new InvalidOperationException("Re-ordering is already in progress.");
                    originalOrder = Levels.OrderBy(l => l.Ordinal).ToList();
                } else
                {
                    if (!_reordering) throw new InvalidOperationException("No re-ordering is currently happening.");
                    CompactLevelOrdinals();
                    var newOrder = Levels.OrderBy(l => l.Ordinal).ToList();
                    Handler.UndoManager.Add(new UndoHierarchyLevelOrderAction(this, originalOrder, newOrder));
                    Handler.Tree.OnStructureChanged(this);
                }
                _reordering = value;
            }
        }

        private List<Level> originalOrder;

        public void CompactLevelOrdinals()
        {
            var ordinal = 0;
            foreach (var l in Levels.OrderBy(l => l.Ordinal)) {
                l.MetadataObject.Ordinal = ordinal;
                ordinal++;
            }
        }

        public void FixLevelOrder(Level level, int newOrdinal)
        {
            if (_reordering) return;

            var before = Levels.OrderBy(l => l.Ordinal).ToList();

            var ordinal = 0;
            foreach (var l in Levels.OrderBy(l => (l == level ? newOrdinal : l.Ordinal) * 2 - (l == level ? 1 : 0)))
            {
                l.MetadataObject.Ordinal = ordinal;
                ordinal++;
            }

            var after = Levels.OrderBy(l => l.Ordinal).ToList();

            Handler.UndoManager.Add(new UndoHierarchyLevelOrderAction(this, before, after));

            _reordering = false;
            Handler.Tree.OnStructureChanged(this);
        }

        public void SetLevelOrder(IList<Level> order)
        {
            if (order.Count != Levels.Count) throw new ArgumentException("Cannot order a hierarchy by a list that does not contain exactly the same levels as the hierarchy iteself.");

            for (var i = 0; i < Levels.Count; i++)
            {
                if (!Levels.Contains(order[i])) throw new ArgumentException("Cannot order a hierarchy by levels in another hierarchy.");
                order[i].MetadataObject.Ordinal = i;
            }

            Handler.Tree.OnStructureChanged(this);
        }
    }

    public class HierarchyColumnConverter: ColumnConverter
    {
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;

namespace TabularEditor.TOMWrapper
{
    partial class Hierarchy: ITabularObjectContainer, ITabularPerspectiveObject, IErrorMessageObject
    {
        /// <summary>
        /// Gets the visibility of the Hierarchy. Takes into consideration that a hierarchy is not visible if its parent table is hidden. 
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => !(IsHidden || Table.IsHidden);

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            // Make sure the hierarchy is no longer used in any Variations:
            if (Handler.CompatibilityLevel >= 1400)
            {
                UsedInVariations.ToList().ForEach(v => v.Delete());
            }

            base.DeleteLinkedObjects(isChildOfDeleted);
        }
        [Browsable(false)]
        public IEnumerable<Variation> UsedInVariations { get { return Model.AllColumns.SelectMany(c => c.Variations).Where(v => v.DefaultHierarchy == this); } }

        [Browsable(false)]
        public string DaxObjectFullName
        {
            get { return Table.DaxObjectFullName + "[" + Name + "]"; }
        }

        #region Convenient methods
        /// <summary>
        /// Adds a set of levels to the current hirearchy.
        /// </summary>
        [IntelliSense("Adds a set of levels to the current hierarchy")]
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

        /// <summary>
        /// Adds a level to the current hirearchy.
        /// </summary>
        [IntelliSense("Adds a level to the current hierarchy")]
        public Level AddLevel(Column column, string levelName = null, int ordinal = -1)
        {
            if (column == null) throw new ArgumentNullException("column");
            if (column.Table != Table)
                throw new ArgumentException(string.Format("Column {0} cannot be added as a level in hierarchy {1} because it is in another table.", column.Name, Name));
            if (Levels.Any(l => l.Column == column))
                throw new ArgumentException(string.Format("Column {0} already exists as a level in hierarchy {1}.", column.Name, Name));

            Handler.BeginUpdate("add level");
            var level = Level.CreateNew(this, levelName ?? column.Name);
            level.Column = column;
            if (levelName == null) level.TranslatedNames.CopyFrom(column.TranslatedNames);
            level.Ordinal = ordinal == -1 ? this.Levels.Count - 1 : ordinal;
            Handler.EndUpdate();

            return level;
        }

        /// <summary>
        /// Adds a level to the current hirearchy.
        /// </summary>
        [IntelliSense("Adds a level to the current hierarchy")]
        public Level AddLevel(string columnName, string levelName = null, int ordinal = -1)
        {
            return AddLevel(Table.Columns[columnName], levelName, ordinal);
        }
        #endregion

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Levels;
        }

        protected override void Init()
        {
            // Loop through all levels, to make sure that they point to the current columns (i.e. not "deleted" columns):
            foreach (var l in Levels) l.MetadataObject.Column = l.MetadataObject.Column == null ? null : Table.Columns[l.MetadataObject.Column.Name].MetadataObject;
        }

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
            internal set
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

        [Category("Metadata"),DisplayName("Error Message")]
        public string ErrorMessage
        {
            get
            {
                if (Levels.GroupBy(l => l.Column).Any(g => g.Count() > 1))
                {
                    return "A hierarchy cannot have multiple levels that use the same column.";
                }
                else return null;
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

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "HideMembers":
                    return Handler.CompatibilityLevel >= 1400;

                default: return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public abstract partial class Column: ITabularPerspectiveObject, IDaxObject
    {
        /// <summary>
        /// Collection of objects that depend on this column.
        /// </summary>
        [Browsable(false)]
        public HashSet<IDAXExpressionObject> Dependants { get; } = new HashSet<IDAXExpressionObject>();

        #region Convenient collections
        /// <summary>
        /// Enumerates all hierarchies in which this column is used as a level.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Hierarchy> UsedInHierarchies { get { return Table.Hierarchies.Where(h => h.Levels.Any(l => l.Column == this)); } }
        /// <summary>
        /// Enumerates all hierarchy levels that are based on this column.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Level> UsedInLevels { get { return Table.Hierarchies.SelectMany(h => h.Levels.Where(l => l.Column == this)); } }
        /// <summary>
        /// Enumerates all columns where this column is used as the SortBy column.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Column> UsedInSortBy { get { return Table.Columns.Where(c => c.SortByColumn == this); } }
        /// <summary>
        /// Enumerates all relationships in which this column participates (either as <see cref="SingleColumnRelationship.FromColumn">FromColumn</see> or <see cref="SingleColumnRelationship.ToColumn">ToColumn</see>).
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Relationship> UsedInRelationships { get { return Model.Relationships.Where(r => r.FromColumn == this || r.ToColumn == this); } }
        #endregion

        internal override void RemoveReferences()
        {
            // Remove IsKey column property if set:
            if (IsKey) IsKey = false;

            base.RemoveReferences();
        }

        public override bool CanDelete(out string message)
        {
            if(this is CalculatedTableColumn)
            {
                message = Messages.CannotDeleteCalculatedTableColumn;
                return false;
            }

            message = string.Empty;
            if (UsedInHierarchies.Any()) message += Messages.ColumnUsedInHierarchy + " ";
            if (UsedInRelationships.Any()) message += Messages.ColumnUsedInRelationship + " ";
            if (Dependants.Count > 0) message += Messages.ReferencedByDAX + " ";

            if (message == string.Empty) message = null;
            return true;
        }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            // Remove any relationships this column participates in:
            UsedInRelationships.ToList().ForEach(r => r.Delete());

            if(!isChildOfDeleted)
            {
                // Remove any hierarchy levels this column is used in:
                UsedInLevels.ToList().ForEach(l => l.Delete());

                // Make sure the column is no longer used as a Sort By column:
                UsedInSortBy.ToList().ForEach(c => c.SortByColumn = null);
            }

            base.DeleteLinkedObjects(isChildOfDeleted);
        }

#if CL1400
        [Browsable(true), DisplayName("Object Level Security"), Category("Security")]
        public ColumnOLSIndexer ObjectLevelSecurity { get; private set; }

        public void InitOLSIndexer()
        {
            ObjectLevelSecurity = new ColumnOLSIndexer(this);
        }
#endif

        protected override void Init()
        {
#if CL1400
            Variations = new VariationCollection("Variations", MetadataObject.Variations, this);
#endif
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.NAME)
            {
                Handler.BuildDependencyTree();

                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.AutoFixup) Handler.UndoManager.BeginBatch("Set Property 'Name'");
            }
            if(propertyName == Properties.ISKEY && (bool)newValue == true)
            {
                // When the IsKey column is set to "true", all other columns must have their IsKey set to false.
                // This has to happen within one undo-batch, so the change can be perfectly restored.
                Handler.UndoManager.BeginBatch("key column");
                foreach(var col in Table.Columns.Where(c => c.IsKey))
                {
                    col.IsKey = false;
                }
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == Properties.ISKEY && IsKey == true)
            {
                Handler.UndoManager.EndBatch();
            }
            if (propertyName == Properties.NAME && Handler.AutoFixup)
            {
                Handler.DoFixup(this);
                Handler.UndoManager.EndBatch();
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
#if CL1400
                case Properties.VARIATIONS:
                    return Model.Database.CompatibilityLevel >= 1400;
#endif
                default: return true;
            }
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return string.Format("{0}{1}", DaxTableName, DaxObjectName);
            }
        }

        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("[{0}]", Name.Replace("]", "]]"));
            }
        }

        [Browsable(false)]
        public string DaxTableName
        {
            get { return Table.DaxTableName; }
        }

    }

    public partial class ColumnCollection
    {
        public override IEnumerator<Column> GetEnumerator()
        {
            return TOM_Collection.Where(c => c.Type != TOM.ColumnType.RowNumber).Select(c => Handler.WrapperLookup[c] as Column).GetEnumerator();
        }

        public override int IndexOf(TOM.MetadataObject value)
        {
            var ix = TOM_Collection.IndexOf(value as TOM.Column);
            var rnIx = GetRnColIndex();
            if (ix == rnIx) throw new KeyNotFoundException();
            if (ix > rnIx) ix--;
            return ix;
        }

        private int GetRnColIndex()
        {
            var rnCol = TOM_Collection.FirstOrDefault(c => c.Type == TOM.ColumnType.RowNumber);
            if (rnCol != null) return TOM_Collection.IndexOf(rnCol);
            return -1;
        }

        private bool HasRowNumColumn
        {
            get
            {
                return TOM_Collection.Any(c => c.Type == TOM.ColumnType.RowNumber);
            }
        }

        public override int Count
        {
            get
            {
                return HasRowNumColumn ? (TOM_Collection.Count - 1) : TOM_Collection.Count;
            }
        }

        public override Column this[int index]
        {
            get
            {
                if(index >= GetRnColIndex()) index++;
                return base[index];
            }
        }

        private bool HasRowNumColumn
        {
            get
            {
                return (MetadataObjectCollection as TOM.ColumnCollection)?.Any(c => c.Type == TOM.ColumnType.RowNumber) ?? false;
            }
        }

        public override int Count
        {
            get
            {
                return HasRowNumColumn ? (base.Count - 1) : base.Count;
            }
        }

        public override Column this[int index]
        {
            get
            {
                var rnCol = MetadataObjectCollection.FirstOrDefault(c => c.Type == TOM.ColumnType.RowNumber);
                if (rnCol != null)
                {
                    var rnColIndex = MetadataObjectCollection.IndexOf(rnCol);
                    if (index >= rnColIndex) index++;
                }
                return base[index];
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public abstract partial class Column: ITabularPerspectiveObject, IDaxObject
    {

        #region Convenient Methods
        [IntelliSense("Creates a relationship from the current column to another column.")]
        public SingleColumnRelationship RelateTo(string tableName, string columnName) {
            if (!Model.Tables.Contains(tableName)) throw new ArgumentException(string.Format("Model does not contain a table named '{0}'.", tableName), "tableName");
            if (!Model.Tables[tableName].Columns.Contains(columnName)) throw new ArgumentException(string.Format("Table '{0}' does not contain a column named '{1}'.", tableName, columnName), "columnName");

            return RelateTo(Model.Tables[tableName].Columns[columnName]);
        }

        [IntelliSense("Creates a relationship from the current column to another column.")]
        public SingleColumnRelationship RelateTo(Column column)
        {
            if (column.DataType != DataType) throw new InvalidOperationException("Cannot create a relationship between columns of different data types.");
            if (column.Table == Table) throw new InvalidOperationException("Cannot create a relationship between columns in the same table.");

            Handler.BeginUpdate("Relate Column");
            var sr = Model.AddRelationship();
            sr.FromColumn = this;
            sr.ToColumn = column;
            Handler.EndUpdate();

            return sr;
        }
        #endregion

        /// <summary>
        /// Collection of objects that depend on this column.
        /// </summary>
        [Browsable(false)]
        public ReferencedByList ReferencedBy { get; } = new ReferencedByList();

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

        [Browsable(false)]
        public IEnumerable<Variation> UsedInVariations { get { return Model.AllColumns.SelectMany(c => c.Variations).Where(v => v.DefaultColumn == this); } }

        /// <summary>
        /// Enumerates all relationships in which this column participates (either as <see cref="SingleColumnRelationship.FromColumn">FromColumn</see> or <see cref="SingleColumnRelationship.ToColumn">ToColumn</see>).
        /// </summary>
        [Browsable(false)]
        public IEnumerable<SingleColumnRelationship> UsedInRelationships { get { return Model.Relationships.Where(r => r.FromColumn == this || r.ToColumn == this); } }
        #endregion

        internal override void RemoveReferences()
        {
            // Remove IsKey column property if set:
            if (IsKey) IsKey = false;

            base.RemoveReferences();
        }

        protected override bool AllowDelete(out string message)
        {
            if(this is CalculatedTableColumn)
            {
                message = Messages.CannotDeleteCalculatedTableColumn;
                return false;
            }

            message = string.Empty;
            if (UsedInHierarchies.Any()) message += Messages.ColumnUsedInHierarchy + " ";
            if (UsedInRelationships.Any()) message += Messages.ColumnUsedInRelationship + " ";
            if (ReferencedBy.Count > 0) message += Messages.ReferencedByDAX + " ";

            if (message == string.Empty) message = null;
            return true;
        }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            // Remove any relationships this column participates in:
            UsedInRelationships.ToList().ForEach(r => r.Delete());

            if(!isChildOfDeleted)
            {
                if(Handler.CompatibilityLevel >= 1400) ObjectLevelSecurity.Clear();

                // Remove any hierarchy levels this column is used in:
                UsedInLevels.ToList().ForEach(l => l.Delete());

                // Make sure the column is no longer used as a Sort By column:
                UsedInSortBy.ToList().ForEach(c => c.SortByColumn = null);

                // Make sure the column is no longer used in any Calculated Tables:
                foreach (var ctc in OriginForCalculatedTableColumns.ToList())
                {
                    ctc.InternalDelete();
                }
            }

            // Make sure the column is no longer used in any Variations:
            if (Handler.CompatibilityLevel >= 1400)
                UsedInVariations.ToList().ForEach(v => v.Delete());

            base.DeleteLinkedObjects(isChildOfDeleted);
        }

        [DisplayName("Object Level Security"), Category("Security")]
        public ColumnOLSIndexer ObjectLevelSecurity { get; private set; }
        

        protected override void Init()
        {
            if(Handler.CompatibilityLevel >= 1400)
            {
                Variations = new VariationCollection("Variations", MetadataObject.Variations, this);
                ObjectLevelSecurity = new ColumnOLSIndexer(this);
            }

            base.Init();
        }

        [Browsable(false)]
        internal IEnumerable<CalculatedTableColumn> OriginForCalculatedTableColumns
        {
            get
            {
                return Model.Tables.OfType<CalculatedTable>()
                    .SelectMany(t => t.Columns.OfType<CalculatedTableColumn>().Where(c => c.ColumnOrigin == this));
            }
        }

        private List<CalculatedTableColumn> _originForCalculatedTableColumnsCache;

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.NAME)
            {
                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects. We also need to
                // start a new batch, in case this column is used as an origin for a calculated table column,
                // as SourceColumn properties on CalculatedTableColumns could change.
                _originForCalculatedTableColumnsCache = OriginForCalculatedTableColumns.ToList();
                if (Handler.Settings.AutoFixup || _originForCalculatedTableColumnsCache.Count > 0) Handler.BeginUpdate("Set Property 'Name'");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            // Make sure only one column has the IsKey property set:
            if(propertyName == Properties.ISKEY && IsKey == true)
            {
                foreach (var c in Table.MetadataObject.Columns.Where(c => c.Type != TOM.ColumnType.RowNumber && c != this.MetadataObject)) c.IsKey = false;
            }

            // Make sure that Calculated Table Columns that originate from this column are updated to reflect name changes:
            if (propertyName == Properties.NAME)
            {
                // Fixup is not performed during an undo operation. We rely on the undo stack to fixup the expressions
                // affected by the name change (the undo stack should contain the expression changes that were made
                // when the name was initially changed).
                if (Handler.Settings.AutoFixup && !Handler.UndoManager.UndoInProgress) FormulaFixup.DoFixup(this, true);
                FormulaFixup.BuildDependencyTree();
                foreach (var ctc in _originForCalculatedTableColumnsCache)
                {
                    if (ctc.IsNameInferred)
                    {
                        ctc.Name = Name;
                        ctc.IsNameInferred = true;
                    }
                    // This will inform the TOM what the origin of the new column is:
                    ctc.SourceColumn = ctc.SourceColumn.Replace($"[{oldValue}]", $"[{newValue}]");
                }

                // End the batch that was started in OnPropertyChanging:
                if (Handler.Settings.AutoFixup || _originForCalculatedTableColumnsCache.Count > 0) Handler.EndUpdate();

                // Update relationship "names" if this column participates in any relationships:
                var rels = UsedInRelationships.ToList();
                if (rels.Count > 1) Handler.Tree.BeginUpdate();
                rels.ForEach(r => r.UpdateName());
                if (rels.Count > 1) Handler.Tree.EndUpdate();
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case "ObjectLevelSecurity":
                    return Handler.CompatibilityLevel >= 1400 && Model.Roles.Any();
                case Properties.VARIATIONS:
                case Properties.ENCODINGHINT:
                    return Handler.CompatibilityLevel >= 1400;                    
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
            // Make sure we never enumerate the RowNumber column:
            foreach (var c in TOM_Collection) {
                if(c.Type != TOM.ColumnType.RowNumber)
                yield return Handler.WrapperLookup[c] as Column;
            }
        }

        internal override string GetNewName(string prefix = null)
        {
            // For columns, we must ensure that the new column name is unique in the current table,
            // and that no measure on the same table has the same name.

            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Column";

            string testName = prefix;
            int suffix = 0;

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (Table.Columns.Any(c => c.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase))
                || Table.Measures.Any(c => c.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase)))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }

        public override int IndexOf(TOM.MetadataObject value)
        {
            var ix = TOM_Collection.IndexOf(value as TOM.Column);
            var rnIx = GetRnColIndex();
            if (ix == rnIx) throw new KeyNotFoundException();
            if (ix > rnIx && rnIx > -1) ix--;
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
                var rnColIndex = GetRnColIndex();
                if(rnColIndex > -1 && index >= rnColIndex) index++;
                return base[index];
            }
        }
    }
}

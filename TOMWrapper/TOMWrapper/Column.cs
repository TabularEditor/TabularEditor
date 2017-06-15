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
        [Browsable(false)]
        public HashSet<IExpressionObject> Dependants { get; private set; } = new HashSet<IExpressionObject>();

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

        protected override void Cleanup()
        {
            // Remove IsKey column property if set:
            if (IsKey) IsKey = false;

            // Remove any relationships this column participates in:
            UsedInRelationships.ToList().ForEach(r => r.Delete());

            // Remove any hierarchy levels that use this column
            UsedInLevels.ToList().ForEach(l => l.Delete());

            // Remove the column from other columns SortByColumn property:
            UsedInSortBy.ToList().ForEach(c => c.SortByColumn = null);
            if (SortByColumn != null) SortByColumn = null;

            base.Cleanup();
        }

#if CL1400
        [Category("Options")]
        public VariationCollection Variations { get; private set; }

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
            if (propertyName == "Name")
            {
                Handler.BuildDependencyTree();

                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.AutoFixup) Handler.UndoManager.BeginBatch("Name change");
            }
            if(propertyName == "IsKey" && (bool)newValue == true)
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
            if(propertyName == "IsKey" && IsKey == true)
            {
                Handler.UndoManager.EndBatch();
            }
            if (propertyName == "Name" && Handler.AutoFixup)
            {
                Handler.DoFixup(this, (string)newValue);
                Handler.UndoManager.EndBatch();
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case "Variations":
                    return Model.Database.CompatibilityLevel >= 1400;
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
            return MetadataObjectCollection
                .Where(c => c.Type != TOM.ColumnType.RowNumber).Select(c => Handler.WrapperLookup[c] as Column).GetEnumerator();
        }
    }
}

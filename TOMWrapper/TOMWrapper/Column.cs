using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public abstract partial class Column: ITabularPerspectiveObject, IDaxObject, IDynamicPropertyObject
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
        /// Enumerates all relationships in which this column participates (either as <see cref="SingleColumnRelationship.FromColumn">FromColumn</see> or <see cref="SingleColumnRelationship.ToColumn">ToColumn</see>).
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Relationship> UsedInRelationships { get { return Model.Relationships.Where(r => r.FromColumn == this || r.ToColumn == this); } }
        #endregion

        [Browsable(true),DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

        [IntelliSense("Deletes the Column from the table.")]
        public override void Delete()
        {
            Handler.UndoManager.BeginBatch("Delete column");

            var t = Table;

            // Remove IsKey column property if set:
            if (IsKey) IsKey = false;

            // Remove any relationships this column participates in:
            foreach (var r in Model.Relationships.OfType<SingleColumnRelationship>().Where(r => r.FromColumn == this || r.ToColumn == this)) r.Delete();

            // Remove any hierarchy levels that use this column
            foreach (var h in UsedInHierarchies) h.Levels.First(l => l.Column == this).Delete();

            // Remove the column from other columns SortByColumn property:
            if (SortByColumn != null) SortByColumn = null;
            foreach (var c in Table.Columns.Where(c => c.SortByColumn == this))
            {
                c.SortByColumn = null;
            }

            InPerspective.None();
            base.Delete();

            t.MetadataObject.Columns.Remove(MetadataObject);

            Handler.UndoManager.EndBatch();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = this is DataColumn ? (TOM.Column)new TOM.DataColumn() :
                    this is CalculatedColumn ? (TOM.Column)new TOM.CalculatedColumn() :
                    this is CalculatedTableColumn ? (TOM.Column)new TOM.CalculatedTableColumn() :
                    null;

            MetadataObject.IsKey = false;
            MetadataObject.CopyTo(tom);
            ////tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        [Category("Options")]
        public VariationCollection Variations { get; private set; }

        protected override void Init()
        {
            InPerspective = new PerspectiveColumnIndexer(this);
            Variations = new VariationCollection(Handler, "Variations", MetadataObject.Variations, this);
        }

        [Browsable(true), DisplayName("Object Level Security"), Category("Security")]
        public ColumnOLSIndexer ObjectLevelSecurity { get; private set; }

        public void InitOLSIndexer()
        {
            ObjectLevelSecurity = new ColumnOLSIndexer(this);
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

        public virtual bool Browsable(string propertyName)
        {
            switch (propertyName)
            {
                case "Variations":
                    return Model.Database.CompatibilityLevel >= 1400;
                default: return true;
            }
        }

        public bool Editable(string propertyName)
        {
            return true;
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

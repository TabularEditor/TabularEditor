using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public class GroupingColumnCollection : TabularObjectCollection<Column>
    {
        private readonly TOM.Column valueColumn;
        private TOM.GroupByColumnCollection tomCollection => valueColumn.RelatedColumnDetails?.GroupByColumns;

        internal GroupingColumnCollection(Column valueColumn): base(valueColumn.GetObjectPath() + ".RelatedColumnDetails", valueColumn)
        {
            this.valueColumn = valueColumn.MetadataObject;
        }

        /// <summary>
        /// Adds the specified column as a related column
        /// </summary>
        /// <param name="item"></param>
        [IntelliSense("Adds the specified column as a related column")]
        public new void Add(Column item)
        {
            base.Add(item);
        }

        protected override void InternalAdd(Column item)
        {
            TOM_Add(item.MetadataObject);
            Handler.UndoManager.Add(new UndoAddRemoveReferenceAction(this, item, UndoAddRemoveActionType.Add));
            DoCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        /// <summary>
        /// Removes the specified column from the collection of related columns
        /// </summary>
        /// <param name="item"></param>
        [IntelliSense("Removes the specified column from the collection of related columns")]
        public new bool Remove(Column item)
        {
            return base.Remove(item);
        }

        protected override bool InternalRemove(Column item)
        {
            if (!TOM_Contains(item.MetadataObject)) return false;

            TOM_Remove(item.MetadataObject);
            Handler.UndoManager.Add(new UndoAddRemoveReferenceAction(this, item, UndoAddRemoveActionType.Remove));
            DoCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            if (Count == 0)
                valueColumn.RelatedColumnDetails = null;
            return true;
        }

        /// <summary>
        /// Removes all columns from the collection of related columns
        /// </summary>
        [IntelliSense("Removes all columns from the collection of related columns")]
        public new void Clear()
        {
            base.Clear();
            valueColumn.RelatedColumnDetails = null;
        }

        public override int Count => tomCollection?.Count ?? 0;

        public override IEnumerator<Column> GetEnumerator()
        {
            if (tomCollection == null)
                return Enumerable.Empty<Column>().GetEnumerator();

            return
                tomCollection.Select(gbc => Handler.WrapperLookup[gbc.GroupingColumn]).OfType<Column>().GetEnumerator();
        }

        internal override void CreateChildrenFromMetadata()
        {
        }

        internal override Type GetItemType()
        {
            return typeof(Column);
        }

        internal override string GetNewName(string prefix = null)
        {
            throw new NotSupportedException();
        }

        internal override int IndexOf(TOM.MetadataObject value)
        {
            return tomCollection == null ? -1 : tomCollection.Select(tc => tc.GroupingColumn).ToList().IndexOf(value as TOM.Column);
        }

        internal override void ReapplyReferences()
        {            
        }

        internal override void Reinit()
        {
        }

        internal override void TOM_Add(TOM.MetadataObject obj)
        {
            if (!(obj is TOM.Column column)) throw new ArgumentException("Object not of type Column", "obj");

            if (tomCollection == null)
                valueColumn.RelatedColumnDetails = new TOM.RelatedColumnDetails();

            tomCollection.Add(new TOM.GroupByColumn { GroupingColumn = column });
        }

        internal override void TOM_Clear()
        {
            tomCollection?.Clear();
        }

        internal override bool TOM_Contains(TOM.MetadataObject obj)
        {
            return tomCollection?.Any(gbc => gbc.GroupingColumn == obj) ?? false;
        }

        internal override bool TOM_ContainsName(string name)
        {
            return TOM_Find(name) != null;
        }

        internal override TOM.MetadataObject TOM_Find(string name)
        {
            return tomCollection?.FirstOrDefault(gbc => gbc.GroupingColumn.Name.EqualsI(name));
        }

        internal override TOM.MetadataObject TOM_Get(string name)
        {
            return tomCollection?.First(gbc => gbc.GroupingColumn.Name.EqualsI(name));
        }

        internal override TOM.MetadataObject TOM_Get(int index)
        {
            return tomCollection[index];
        }

        internal override void TOM_Remove(TOM.MetadataObject obj)
        {
            var groupByColumn = tomCollection?.FirstOrDefault(gbc => gbc.GroupingColumn == obj);
            if (groupByColumn != null) tomCollection.Remove(groupByColumn);
        }
    }
}

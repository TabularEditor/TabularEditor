using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TabularEditor.TOMWrapper;

public class AssociatedColumnCollection: TabularNamedObjectCollection<Column>
{
    private readonly CalendarColumnGroup _columnGroup;

    internal AssociatedColumnCollection(CalendarColumnGroup columnGroup, string collectionName) : base(columnGroup.GetObjectPath() + "." + collectionName, columnGroup)
    {
        _columnGroup = columnGroup;
    }

    private ICollection<TOM.Column> TomCollection =>
        _columnGroup.MetadataObject is TOM.TimeUnitColumnAssociation timeAssoc ? timeAssoc.AssociatedColumns
        : _columnGroup.MetadataObject is TOM.TimeRelatedColumnGroup timeRelated ? timeRelated.Columns
        : throw new NotSupportedException();

    public override int Count => TomCollection.Count;

    /// <summary>
    /// Adds the specified column as a related column
    /// </summary>
    /// <param name="item"></param>
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
    public new bool Remove(Column item) => base.Remove(item);

    protected override bool InternalRemove(Column item)
    {
        if (!TOM_Contains(item.MetadataObject)) return false;

        TOM_Remove(item.MetadataObject);
        Handler.UndoManager.Add(new UndoAddRemoveReferenceAction(this, item, UndoAddRemoveActionType.Remove));
        DoCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        return true;
    }

    /// <summary>
    /// Removes all columns from the collection of related columns
    /// </summary>
    public new void Clear()
    {
        base.Clear();
    }

    public override IEnumerator<Column> GetEnumerator()
    {
        return
            TomCollection.Select(c => Handler.WrapperLookup[c]).OfType<Column>().GetEnumerator();
    }

    internal override void CreateChildrenFromMetadata()
    {
    }

    internal override Type GetItemType() => typeof(Column);

    internal override string GetNewName(string prefix = null) => throw new NotSupportedException();

    internal override int IndexOf(TOM.MetadataObject value)
    {
        return TomCollection.IndexOf(c => c == value);
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

        TomCollection.Add(column);
    }

    internal override void TOM_Clear()
    {
        TomCollection?.Clear();
    }

    internal override bool TOM_Contains(TOM.MetadataObject obj)
    {
        return TomCollection?.Any(c => c == obj) ?? false;
    }

    internal override bool TOM_ContainsName(string name) => TOM_Find(name) != null;

    internal override TOM.MetadataObject TOM_Find(string name)
    {
        return TomCollection?.FirstOrDefault(c => c.Name.EqualsI(name));
    }

    internal override TOM.MetadataObject TOM_Get(string name)
    {
        return TomCollection?.First(c => c.Name.EqualsI(name));
    }

    internal override TOM.MetadataObject TOM_Get(int index) => TomCollection.Skip(index).First();

    internal override void TOM_Remove(TOM.MetadataObject obj)
    {
        TomCollection.Remove(obj as TOM.Column);
    }
}

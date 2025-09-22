using System;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI;

// We need this explicit collection editor, because the type of items stored in the collection (BindingInfo) is a superclass of the
// actual type of items that we want the collection editor to create (DataBindingHint).
internal class BindingInfoCollectionEditor: ClonableObjectCollectionEditor<BindingInfo>
{
    public BindingInfoCollectionEditor(Type type) : base(type)
    {

    }

    BindingInfoCollection Collection => Context.Instance as BindingInfoCollection ?? (Context.Instance as Model).BindingInfoCollection;

    protected override Type[] CreateNewItemTypes()
    {
        return new[] { typeof(DataBindingHint) };
    }

    protected override object CreateInstance(Type itemType)
    {
        if (itemType == typeof(DataBindingHint)) return DataBindingHint.CreateNew(Collection.Model);
        return base.CreateInstance(itemType);
    }
}

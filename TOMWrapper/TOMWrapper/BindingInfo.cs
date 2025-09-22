#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace TabularEditor.TOMWrapper;

public partial class BindingInfo: IClonableObject
{
    public TabularNamedObject Clone(string newName = null, bool includeTranslations = false,
        TabularNamedObject newParent = null) =>
        ((IClonableObject)this).Clone(newName, includeTranslations, newParent);

    internal override bool Editable(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Type): return false;
        }

        return base.Editable(propertyName);
    }
}

using System.ComponentModel;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TabularEditor.TOMWrapper;

public partial class QueryGroup
{
    internal static int RequiredCompatibilityLevel => 1480;

    [ReadOnly(true)]
    public override string ObjectTypeName => "Query Group";

    public override string Name
    {
        set => Folder = value;
    }

    protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
    {
        if (propertyName == nameof(Folder)) OnPropertyChanged(nameof(Name), oldValue, newValue);
        base.OnPropertyChanged(propertyName, oldValue, newValue);
    }
}

public partial class QueryGroupCollection
{
    protected override bool InternalRemove(QueryGroup item)
    {
        Handler.BeginUpdate("Remove Query Group");

        foreach (var namedExpr in Model.Expressions)
            if (namedExpr.QueryGroup == item) namedExpr.QueryGroup = null;
        foreach (var partition in Model.AllPartitions)
            if (partition.QueryGroup == item) partition.QueryGroup = null;

        var result = base.InternalRemove(item);

        Handler.EndUpdate();
        return result;
    }
}

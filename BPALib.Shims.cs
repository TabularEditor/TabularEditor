using System.Collections.Generic;
using System.Linq;
using System.Globalization;

#if NETSTANDARD || NETFRAMEWORK

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }
}

#endif

namespace TabularEditor
{
    internal static class Program
    {
        internal static TestRun testRun;
    }
    
    internal class ColumnSelectConverter { }
    internal class NoEditor { }

    interface IDropDownProperties
    {
        string[] GetDropDownItems(string propertyName);
    }
}

namespace TabularEditor.PropertyGridUI
{
    namespace CollectionEditors { }

    internal class AnnotationCollectionEditor { }
    internal class CalculationItemCollectionEditor { }
    internal class CultureCollectionEditor { }
    internal class DataCoverageDefinitionEditor { }
    internal class AlternateOfEditor { }
    internal class VariationCollectionEditor { }
    internal class ExtendedPropertyCollectionEditor { }
    internal class CustomDialogEditor { }
    internal class ColumnSetCollectionEditor { }
    internal class ConnectionAddressPropertyCollectionEditor { }
    internal class CredentialPropertyCollectionEditor { }
    internal class DataSourceOptionsPropertyCollectionEditor { }
    internal class KpiEditor { }
    internal class PartitionCollectionEditor { }
    internal class RoleMemberCollectionEditor { }
    internal class SetCollectionEditor { }
    internal class ClonableObjectCollectionEditor<T> { }

    internal class AllColumnConverter { }
    internal class AllOtherTablesColumnConverter { }
    internal class AllHierarchyConverter { }
    internal class AllRelationshipConverter { }
    internal class ColumnConverter { }
    internal class ColumnDataCategoryConverter { }
    internal class ConnectionStringConverter { }

    internal class CultureConverter
    {
        public static Dictionary<string, CultureInfo> Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToDictionary(c => c.Name, c => c);

    }
    internal class DataTypeEnumConverter { }
    internal class DataSourceConverter { }
    internal class FormatStringConverter { }
    internal class IndexerConverter { }
    internal class HierarchyColumnConverter { }
    internal class KPIStatusGraphicConverter { }
    internal class KPITrendGraphicConverter { }
    internal class NamedExpressionConverter { }
    internal class OtherTablesConverter { }
    internal class TableColumnConverter { }
    internal class TableDataCategoryConverter { }

    /// <summary>
    /// This interface must be implemented by dictionary-type properties on a class, such as
    /// annotations, translations, etc.
    /// </summary>
    internal interface IExpandableIndexer
    {
        string Summary { get; }
        IEnumerable<string> Keys { get; }
        object this[string index] { get; set; }
        string GetDisplayName(string key);
        bool EnableMultiLine { get; }
    }
}

namespace TabularEditor.PropertyGridUI.Converters
{
}

namespace TabularEditor.TOMWrapper
{
    public class TabularCommonActions
    {
        internal TabularCommonActions(object _)
        {
            
        }
    }

    public partial class TabularModelHandler
    {

        internal void DoObjectDeleting(ITabularObject obj, ref bool cancel)
        {
        }

        internal void DoObjectDeleted(ITabularObject obj, ITabularNamedObject parentBeforeDeletion)
        {
        }

        internal void DoObjectChanging(ITabularObject obj, string propertyName, object newValue, ref bool cancel)
        {
        }

        internal void DoObjectChanged(ITabularObject obj, string propertyName, object oldValue, object newValue)
        {
        }

    }

    /*
    internal static class DatabaseHelper
    {
        public const string TabularEditorTag = "__TEdtr";
        internal static void AddTabularEditorTag(this TOM.Database database) {}
        internal static void RemoveTabularEditorTag(this TOM.Database database) { }
    }
    */
}

namespace TabularEditor.TOMWrapper.Serialization
{
}

namespace TabularEditor.TOMWrapper.Undo
{
}

namespace TabularEditor.TOMWrapper.Utils
{
    public class TabularDeployer
    {
        public static DeploymentResult GetLastDeploymentResults(object database)
        {
            return new DeploymentResult();
        }
    }

    public class DeploymentResult { }

    internal class CTCBackup
    {
        internal static CTCBackup BackupColumn(object _) => default;
    }
}

namespace TabularEditor.UIServices
{
    internal static class UpdateService
    {
        public const string VERSION_MANIFEST_URL = "https://raw.githubusercontent.com/TabularEditor/TabularEditor/master/TabularEditor/version.txt";
    }

    internal class Preferences
    {
        public string ProxyAddress = string.Empty;
        public string ProxyUser = string.Empty;
        public bool ProxyUseSystem = true;
        public string ProxyPasswordEncrypted = string.Empty;

        internal static Preferences Current { get; }
    }

}

namespace TabularEditor.Utils
{
}

namespace System.Drawing.Design
{
}

namespace System.Drawing.Design
{
#if !NETFRAMEWORK
    internal class UITypeEditor { }
#endif
}

namespace System.ComponentModel.Design
{
    internal class MultilineStringEditor { }
}
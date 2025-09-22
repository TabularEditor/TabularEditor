using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;

namespace TabularEditor.UI
{
    public static class SelectionHelper
    {
        public static string Summary(this IEnumerable<ITabularNamedObject> items, bool onlyTypeNames = false)
        {
            var count = items.Count();
            switch (count)
            {
                case 0: return onlyTypeNames ? "Nothing" : "(Nothing Selected)";
                case 1: return onlyTypeNames ? "1 " + items.First().GetTypeName(false).ToLower() : "\"" + items.First().Name + "\"";
                default:
                    var types = items.Select(obj => obj.ObjectType).Distinct().ToList();
                    return string.Format("{0} {1}", count, types.Count > 1 ? "objects" : types[0].GetTypeName(true).ToLower());
            }
        }

        /// <summary>
        /// Convenient extension method for converting an IEnumerable<T> to a UISelection<T>, the latter containing
        /// a wider range of methods for easily manipulating multiple objects at once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static UISelectionList<T> Edit<T>(this IEnumerable<T> items) where T: ITabularNamedObject
        {
            return new UISelectionList<T>(items);
        }

        public static T FindByDAX<T>(this IEnumerable<T> items, string name) where T : ITabularNamedObject
        {
            // TODO: Handle escape characters / use DAX Lexer
            var result = items.Where(i =>

                // Search everything by name:
                i.Name == name ||

                // Search by object name enclosed by square brackets:
                (((i is Measure) || (i is Column)) && string.Format("[{0}]", i.Name) == name) ||

                // Search by table name in single quotes:
                ((i is Table) && string.Format("'{0}'", i.Name) == name) ||

                // Search by table name and object name in square brackets:
                ((i is ITabularTableObject) && string.Format("{0}[{1}]", (i as ITabularTableObject).Table.Name, i.Name) == name) ||

                // Search by table name in single quotes and object name in square brackets:
                ((i is ITabularTableObject) && string.Format("'{0}'[{1}]", (i as ITabularTableObject).Table.Name, i.Name) == name)

            ).ToList();

            if (result.Count > 1) throw new ArgumentException(string.Format("There are multiple objects in the collection with the name \"{0}\". If accessing columns or measures, try to use the fully qualified DAX name to limit the search.", name));
            if (result.Count == 0) throw new ArgumentException(string.Format("There is no object in the collection with the name \"{0}\".", name));
            return result[0];
        }
    }

    public static class TypesHelper
    {
        /// <summary>
        /// Returns true if context contains one or more of the specified flags.
        /// </summary>
        public static bool HasX(this Context context, Context flags)
        {
            return (context & flags) != 0;
        }
        /// <summary>
        /// Returns true if context contains exactly one of the specified flags.
        /// </summary>
        public static bool Has1(this Context context, Context flags)
        {
            var x = (context & flags);
            return x != 0 && (x & (x - 1)) == 0;
        }
        public static Context Combine(this IEnumerable<Context> contexts)
        {
            if (!contexts.Any()) return (Context)0;
            return contexts.Aggregate((r1, r2) => r1 | r2);
        }
        /// <summary>
        /// Checks if one and only one bit is set in the specified context enum flags.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Just1(this Context context)
        {
            var x = context;
            return x != 0 && (x & (x - 1)) == 0;
        }
    }

    /// <summary>
    /// Determines the context of the current selection in the Explorer Tree. Only one of the contexts
    /// can be selected at a time. For example, it is not possible to simultaneously select a table and
    /// a data source. Objects inside tables (columns, measures, hierarchies, folders) may be simultaneously
    /// selected, and therefore the context when one or more of these objects are selected is simply
    /// "TableObject".
    /// 
    /// While a given selection in the Explorer Tree can only have one context, it is possible for context
    /// menu items to be visible under several contexts, which is why the enum is treated as Flags.
    /// </summary>
    [Flags]
    public enum Context: long
    {
        /// <summary>
        /// Nothing selected in the tree
        /// </summary>
        None = 0,

        /// <summary>
        /// Context menu opened on Model node
        /// </summary>
        Model = 1 << 0,

        /// <summary>
        /// Context menu opened on the "Tables" group node
        /// </summary>
        Tables = 1 << 1,

        /// <summary>
        /// Context menu opened on the "Data Sources" group node
        /// </summary>
        DataSources = 1 << 2,

        /// <summary>
        /// Context menu opened on the "Perspectives" group node
        /// </summary>
        Perspectives = 1 << 3,

        /// <summary>
        /// Context menu opened on the "Translations" group node
        /// </summary>
        Translations = 1 << 4,

        /// <summary>
        /// Context menu opened on the "Roles" group node
        /// </summary>
        Roles = 1 << 5,

        /// <summary>
        /// Context menu opened on the "Relationships" group node
        /// </summary>
        Relationships = 1 << 6,

        /// <summary>
        /// Context menu opened on the "Partitions" node
        /// </summary>
        PartitionCollection = 1 << 7,

        /// <summary>
        /// Context menu opened on the "Shared Expressions" group node
        /// </summary>
        Expressions = 1 << 8,

        /// <summary>
        /// Context menu opened on a Table Permission
        /// </summary>
        TablePermission = 1 << 9,

        /// <summary>
        /// Context menu opened on one or more tables
        /// </summary>
        Table = 1 << 11,
        
        /// <summary>
        /// Context menu opened on one or more measures (or on a display folder containing measures)
        /// </summary>
        Measure = 1 << 12,

        /// <summary>
        /// Context menu opened on one or more columns (or on a display folder containing columns)
        /// </summary>
        Column = 1 << 13,

        /// <summary>
        /// Context menu opened on one or more hierarchies (or on a display folder containing hierarchies)
        /// </summary>
        Hierarchy = 1 << 14,

        /// <summary>
        /// Context menu opened on one or more hierarchy levels
        /// </summary>
        Level = 1 << 15,

        /// <summary>
        /// Context menu opened on one or more table partitions
        /// </summary>
        Partition = 1 << 16,

        /// <summary>
        /// Context menu opened on one or more relationships
        /// </summary>
        Relationship = 1 << 17,

        /// <summary>
        /// Context menu opened on one or more data sources
        /// </summary>
        DataSource = 1 << 18,

        /// <summary>
        /// Context menu opened on one or more roles
        /// </summary>
        Role = 1 << 19,

        /// <summary>
        /// Context menu opened on one or more perspectives
        /// </summary>
        Perspective = 1 << 20,

        /// <summary>
        /// Context menu opened on one or more cultures
        /// </summary>
        Translation = 1 << 21,

        /// <summary>
        /// Context menu opened on a KPI object
        /// </summary>
        KPI = 1 << 22,

        Expression = 1 << 23,

        CalculationGroupTable = 1 << 24,
        CalculationItem = 1 << 25,
        CalculationItemCollection = 1 << 26,

        /// <summary>
        /// Context menu opened on a "Function" object
        /// </summary>
        Function = 1 << 27,

        /// <summary>
        /// Special context for actions that can be executed regardless of the current selection,
        /// but where the action should show up in the "Tools" menu only.
        /// </summary>
        Tool = 1 << 28,

        Calendar = 1 << 29,
        CalendarCollection = 1 << 30,

        Functions = 1L << 31,

        Everywhere = 0x7FFFFFFF,
        TableObject = Measure | Column | Hierarchy,
        SingularObjects = Model | Table | TableObject | Level | Partition | Relationship | DataSource | Role | TablePermission | Perspective | Translation | KPI | Expression | CalculationGroupTable | CalculationItem | Function | Calendar,
        Groups = Tables | Relationships | DataSources | Roles | Perspectives | Translations | Expressions | Functions,
        DataObjects = CalculationGroupTable | Table | TableObject,
        Scriptable = CalculationGroupTable | Table | Partition | DataSource | Role
    }

    /// <summary>
    /// Provides a range of collections containing the TOM objects that are currently selected
    /// in the TreeView. Each collection is statically typed, making it very easy to work with
    /// the objects in the collection. A handful of convenient methods for adding, deleting,
    /// duplicating and changing multiple object's properties at once are also provided.
    /// 
    /// If one or more Display Folders are selected, their child objects are also considered
    /// selected.
    /// 
    /// Additionally, a range of properties have been provided, to give some overall information
    /// about the selection.
    /// </summary>
    public class UITreeSelection: UISelectionList<ITabularNamedObject>
    {
        public static UITreeSelection Empty { get; } = new UITreeSelection(Enumerable.Empty<ITabularNamedObject>());

        IReadOnlyCollection<TreeNodeAdv> _selectedNodes;

        [IntelliSense("Indicates where in the Explorer Tree the current selection has been made.")]
        public Context Context { get; private set; } = Context.None;

        /// <summary>
        /// Iterates through the passed list of nodes, to return the combined context of the items represented by the nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static public Context GetNodeContexts(IList<TreeNodeAdv> nodes)
        {
            var result = Context.None;
            foreach (var node in nodes) result |= GetNodeContext(node);
            return result;
        }

        /// <summary>
        /// Returns the "context" commonly associated with a given Explorer Tree node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static public Context GetNodeContext(TreeNodeAdv node)
        {
            var result = Context.None;
            var obj = node.Tag as ITabularNamedObject;
            switch (obj.ObjectType)
            {
                case ObjectType.CalendarCollection: return Context.CalendarCollection;
                case ObjectType.Calendar: return Context.Calendar;
                case ObjectType.PartitionCollection: return Context.PartitionCollection;
                case ObjectType.Expression: return Context.Expression;
                case ObjectType.Model: return Context.Model;
                case ObjectType.Culture: return Context.Translation;
                case ObjectType.DataSource: return Context.DataSource;
                case ObjectType.Perspective: return Context.Perspective;
                case ObjectType.Function: return Context.Function;
                case ObjectType.Partition: return Context.Partition;
                case ObjectType.Role: return Context.Role;
                case ObjectType.Relationship: return Context.Relationship;
                case ObjectType.Table: return Context.Table;
                case ObjectType.CalculationGroupTable: return Context.CalculationGroupTable;
                case ObjectType.CalculationItem: return Context.CalculationItem;
                case ObjectType.CalculationItemCollection: return Context.CalculationItemCollection;
                case ObjectType.TablePermission: return Context.TablePermission;
                case ObjectType.Level: return Context.Level;
                case ObjectType.KPI: return Context.KPI;
                case ObjectType.Column: return Context.Column;
                case ObjectType.Measure: return Context.Measure;
                case ObjectType.Hierarchy: return Context.Hierarchy;
                case ObjectType.Group:
                    switch ((node.Tag as LogicalGroup).Name)
                    {
                        case LogicalGroups.TABLES: return Context.Tables;
                        case LogicalGroups.DATASOURCES: return Context.DataSources;
                        case LogicalGroups.PERSPECTIVES: return Context.Perspectives;
                        case LogicalGroups.FUNCTIONS: return UI.Context.Functions;
                        case LogicalGroups.ROLES: return Context.Roles;
                        case LogicalGroups.TRANSLATIONS: return Context.Translations;
                        case LogicalGroups.RELATIONSHIPS: return Context.Relationships;
                        case LogicalGroups.EXPRESSIONS: return Context.Expressions;
                    }
                    break;                   
            }
            return result;
        }

        public UITreeSelection(IEnumerable<ITabularNamedObject> selection) : base(selection)
        {
            Folders = this.OfType<Folder>();
            Groups = this.OfType<LogicalGroup>();
            Direct = new UISelectionList<ITabularNamedObject>(this.OfType<ITabularNamedObject>());
            AssignCollections();
        }

        public UITreeSelection(IReadOnlyCollection<TreeNodeAdv> selectedNodes)
        {
            var allNodes = GetDeep(selectedNodes).ToList();
            SetItems(allNodes.Select(n => n.Tag).OfType<ITabularNamedObject>()
                .Where(n => !(n is Folder)));

            _selectedNodes = selectedNodes;

            if (allNodes.Count == 0) Context = Context.None;
            else if (allNodes.Count == 1) Context = GetNodeContext(allNodes[0]);
            else Context = GetNodeContexts(allNodes);

            Folders = selectedNodes.Select(n => n.Tag).OfType<Folder>();
            Groups = selectedNodes.Select(n => n.Tag).OfType<LogicalGroup>();
            Direct = new UISelectionList<ITabularNamedObject>(selectedNodes.Select(n => n.Tag).OfType<ITabularNamedObject>());

            AssignCollections();
        }

        private void AssignCollections()
        {
            Measures = new UISelectionList<Measure>(this.OfType<Measure>());
            Hierarchies = new UISelectionList<Hierarchy>(this.OfType<Hierarchy>());
            Levels = new UISelectionList<Level>(this.OfType<Level>());
            Columns = new UISelectionList<Column>(this.OfType<Column>());
            Cultures = new UISelectionList<Culture>(this.OfType<Culture>());
            Roles = new UISelectionList<ModelRole>(this.OfType<ModelRole>());
            DataSources = new UISelectionList<DataSource>(this.OfType<DataSource>());
            Perspectives = new UISelectionList<Perspective>(this.OfType<Perspective>());
            Functions = new UISelectionList<Function>(this.OfType<Function>());
            CalculatedColumns = new UISelectionList<CalculatedColumn>(this.OfType<CalculatedColumn>());
            CalculatedTableColumns = new UISelectionList<CalculatedTableColumn>(this.OfType<CalculatedTableColumn>());
            CalculationGroups = new UISelectionList<CalculationGroupTable>(this.OfType<CalculationGroupTable>());
            CalculationItems = new UISelectionList<CalculationItem>(this.OfType<CalculationItem>());
            TablePermissions = new UISelectionList<TablePermission>(this.OfType<TablePermission>());
            SingleColumnRelationships = new UISelectionList<SingleColumnRelationship>(this.OfType<SingleColumnRelationship>());
            DataColumns = new UISelectionList<DataColumn>(this.OfType<DataColumn>());
            Tables = new UISelectionList<Table>(this.OfType<Table>());
            Partitions = new UISelectionList<Partition>(this.OfType<Partition>());
            Calendars = new UISelectionList<Calendar>(this.OfType<Calendar>());
        }

        private T One<T>() where T: TabularObject
        {
            var obj = this.FirstOrDefault() as T;
            if (obj == null) throw new Exception("The selection does not contain any objects of type " + typeof(T).Name);
            else if (this.Skip(1).Any()) throw new Exception("The selection contains more than one object of type " + typeof(T).Name);
            else return obj;
        }

        /// <summary>
        /// The number of objects directly selected in the Explorer Tree (not counting any child objects).
        /// </summary>
        [IntelliSense("The number of objects directly selected in the Explorer Tree (not counting any child objects).")]
        public int DirectCount { get { return _selectedNodes.Count; } }

        #region Sub collections
        [IntelliSense("The currently selected measure (if exactly one measure is selected in the explorer tree).")]
        public Measure Measure { get { return One<Measure>(); } }

        [IntelliSense("Gets the current folder of the selection. If multiple folders are selected, the parent folder is returned.")]
        public string CurrentFolder
        {
            get
            {
                if (Direct.FirstOrDefault() is Table) return "";
                if (DirectCount == 0) return "";
                if (DirectCount == 1)
                {
                    var obj = Direct.First();
                    if (obj is Folder) return (obj as Folder).Path;
                    if (obj is IFolderObject) return (obj as IFolderObject).DisplayFolder;
                    if (obj is Level) return (obj as Level).Hierarchy.DisplayFolder;
                    return "";
                } else
                {
                    var obj = Direct.First();
                    if (obj is IFolderObject) return (obj as IFolderObject).DisplayFolder;
                    if (obj is Level) return (obj as Level).Hierarchy.DisplayFolder;
                    return "";
                }
            }
        }

        [IntelliSense("All currently selected measures (including measures within selected Display Folders).")]
        public UISelectionList<Measure> Measures { get; private set; }

        [IntelliSense("The currently selected hierarchy (if exactly one hierarchy is selected in the explorer tree).")]
        public Hierarchy Hierarchy { get { return One<Hierarchy>(); } }

        [IntelliSense("All currently selected hierarchies (including hierarchies within selected Display Folders).")]
        public UISelectionList<Hierarchy> Hierarchies { get; private set; }

        [IntelliSense("The currently selected level (if exactly one level is selected in the explorer tree).")]
        public Level Level { get { return One<Level>(); } }

        [IntelliSense("The currently selected KPI.")]
        public KPI KPI { get { return One<KPI>(); } }

        [IntelliSense("The currently selected levels.")]
        public UISelectionList<Level> Levels { get; private set; }

        [IntelliSense("The currently selected column (if exactly one column is selected in the explorer tree).")]
        public Column Column { get { return One<Column>(); } }

        [IntelliSense("All currently selected columns (including columns within selected Display Folders).")]
        public UISelectionList<Column> Columns { get; private set; }

        [IntelliSense("All currently selected cultures.")]
        public UISelectionList<Culture> Cultures { get; private set; }

        [IntelliSense("All currently selected data sources.")]
        public UISelectionList<DataSource> DataSources { get; private set; }

        [IntelliSense("The currently selected data source.")]
        public DataSource DataSource { get { return One<DataSource>(); } }

        [IntelliSense("All currently selected roles.")]
        public UISelectionList<ModelRole> Roles { get; private set; }

        [IntelliSense("All currently selected relationships.")]
        public UISelectionList<SingleColumnRelationship> SingleColumnRelationships { get; private set; }

        [IntelliSense("All currently selected perspectives.")]
        public UISelectionList<Perspective> Perspectives { get; private set; }

        [IntelliSense("All currently selected functions.")]
        public UISelectionList<Function> Functions { get; private set; }

        [IntelliSense("The currently selected function.")]
        public Function Function { get { return One<Function>(); } }

        [IntelliSense("The currently selected calculation item (if exactly one calculation item is selected in the explorer tree.)")]
        public CalculationItem CalculationItem { get { return One<CalculationItem>(); } }

        [IntelliSense("All currently selected calculation items.")]
        public UISelectionList<CalculationItem> CalculationItems { get; private set; }

        [IntelliSense("All currently selected table permissions.")]
        public UISelectionList<TablePermission> TablePermissions { get; private set; }
        [IntelliSense("The currently selected calculation group (if exactly one calculation group is selected in the explorer tree.)")]
        public CalculationGroupTable CalculationGroup
        {
            get
            {
                if (this.FirstOrDefault() is ITabularTableObject t && t.Table is CalculationGroupTable cgt) return cgt;
                return One<CalculationGroupTable>();
            }
        }

        [IntelliSense("All currently selected calculation groups.")]
        public UISelectionList<CalculationGroupTable> CalculationGroups { get; private set; }

        [IntelliSense("The currently selected calculated column (if exactly one calculated column is selected in the explorer tree).")]
        public CalculatedColumn CalculatedColumn { get { return One<CalculatedColumn>(); } }

        [IntelliSense("All currently selected calculated columns (including calculated columns within selected Display Folders).")]
        public UISelectionList<CalculatedColumn> CalculatedColumns { get; private set; }

        [IntelliSense("All currently selected calculated table columns (including calculated table columns within selected Display Folders).")]
        public UISelectionList<CalculatedTableColumn> CalculatedTableColumns { get; private set; }

        [IntelliSense("The currently selected data column (if exactly one data column is selected in the explorer tree).")]
        public DataColumn DataColumn { get { return One<DataColumn>(); } }

        [IntelliSense("All currently selected data columns (including data columns within selected Display Folders).")]
        public UISelectionList<DataColumn> DataColumns { get; private set; }

        [IntelliSense("All currently selected partitions.")]
        public UISelectionList<Partition> Partitions { get; private set; }

        [IntelliSense("The currently selected partition.")]
        public Partition Partition { get { return One<Partition>(); } }

        [IntelliSense("All currently selected calendars.")]
        public UISelectionList<Calendar> Calendars { get; private set; }

        [IntelliSense("The currently selected calendar.")]
        public Calendar Calendar { get { return One<Calendar>(); } }


        [IntelliSense("The currently selected perspective in Tabular Editor.")]
        public Perspective Perspective
        {
            get
            {
                if (UIController.Current.TreeModel.Perspective == null) throw new Exception("No perspective is currently selected in Tabular Editor.");
                return UIController.Current.TreeModel.Perspective;
            }
        }
        [IntelliSense("The currently selected culture in Tabular Editor.")]
        public Culture Culture
        {
            get
            {
                if (UIController.Current.TreeModel.Culture == null) throw new Exception("No culture is currently selected in Tabular Editor.");
                return UIController.Current.TreeModel.Culture;
            }
        }

        [IntelliSense("The currently selected table (if exactly one table is selected in the explorer tree) or the table of selected child objects.")]
        public Table Table
        {
            get
            {
                if (this.FirstOrDefault() is ITabularTableObject) return (this.FirstOrDefault() as ITabularTableObject).Table;
                return One<Table>();
            }
        }

        [IntelliSense("The currently selected tables (if one or more tables have been directly selected in the explorer tree).")]
        public UISelectionList<Table> Tables { get; private set; }

        [IntelliSense("A collection of objects (including folders) that are directly selected in the explorer tree.")]
        public UISelectionList<ITabularNamedObject> Direct { get; private set; }

        internal IEnumerable<Folder> Folders { get; private set; }
        internal IEnumerable<LogicalGroup> Groups { get; private set; }
        #endregion

        private static IEnumerable<TreeNodeAdv> GetDeep(IEnumerable<TreeNodeAdv> nodes)
        {
            foreach (var n in nodes)
            {
                yield return n;
                if (n.Tag is Folder) foreach (var c in GetDeep(n.Children)) yield return c;
            }
        }
    }
}

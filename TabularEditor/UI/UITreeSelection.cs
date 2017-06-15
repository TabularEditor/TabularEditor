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

    /// <summary>
    /// A bit mask of Table Object types that can be simultaneously selected in the Explorer Tree.
    /// Only relevant when Context = TableObject.
    /// </summary>
    [Flags]
    public enum Types
    {
        None = 0,
        Folder =                1 << 0,
        Hierarchy =             1 << 1,
        Measure =               1 << 2,
        Column =                1 << 3,
        CalculatedColumn =      1 << 4,
        DataColumn =            1 << 5,
        CalculatedTableColumn = 1 << 6
    }

    public static class TypesHelper
    {
        /// <summary>
        /// Returns true if types contains one or more of the specified flags.
        /// </summary>
        public static bool HasX(this Types types, Types flags)
        {
            return (types & flags) != 0;
        }
        /// <summary>
        /// Returns true if types contains exactly one of the specified flags.
        /// </summary>
        public static bool Has1(this Types types, Types flags)
        {
            var x = (types & flags);
            return x != 0 && (x & (x - 1)) == 0;
        }

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

        /// <summary>
        /// Returns the number of flags set.
        /// </summary>
        public static int Count(this Types types)
        {
            int count = 0;
            while (types != 0)
            {
                count++;
                types &= (types - 1);
            }
            return count;
        }
        /// <summary>
        /// Returns true if types contains none of the specified flags.
        /// </summary>
        public static bool Has0(this Types types, Types flags)
        {
            return (types & flags) == 0;
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
    public enum Context
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
        /// Context menu opened on one or more tables
        /// </summary>
        Table = 1 << 7,

        /// <summary>
        /// Context menu opened on one or more table objects (measures, columns, hierarchies or folders - use Selected.Types to determine type of objects)
        /// </summary>
        TableObject = 1 << 8,

        /// <summary>
        /// Context menu opened on one or more hierarchy levels
        /// </summary>
        Level = 1 << 9,

        /// <summary>
        /// Context menu opened on one or more table partitions
        /// </summary>
        Partition = 1 << 10,

        /// <summary>
        /// Context menu opened on one or more relationships
        /// </summary>
        Relationship = 1 << 11,

        /// <summary>
        /// Context menu opened on one or more data sources
        /// </summary>
        DataSource = 1 << 12,

        /// <summary>
        /// Context menu opened on one or more roles
        /// </summary>
        Role = 1 << 13,

        /// <summary>
        /// Context menu opened on one or more perspectives
        /// </summary>
        Perspective = 1 << 14,

        /// <summary>
        /// Context menu opened on one or more cultures
        /// </summary>
        Translation = 1 << 15,

        Everywhere = 0xFFFFFF,
        SingularObjects = Table | TableObject | Level | Partition | Relationship | DataSource | Role | Perspective | Translation,
        Groups = Model | Tables | Relationships | DataSources | Roles | Perspectives | Translations,
        DataObjects = Table | TableObject
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
    public class UITreeSelection: UISelectionList<TabularNamedObject>
    {
        IReadOnlyCollection<TreeNodeAdv> _selectedNodes;

        [IntelliSense("Indicates where in the Explorer Tree the current selection has been made.")]
        public Context Context { get; private set; } = Context.None;

        [IntelliSense("A bit mask specifiying which TableObjects are currently selected.\nExample: if(Selected.Types.HasFlag(Types.Measure)) { ... }")]
        public Types Types { get; private set; } = Types.None;

        /// <summary>
        /// Returns a bitmask of Table Object types present in the collection of Explorer Tree nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        static public Types GetNodeTypes(IReadOnlyCollection<TreeNodeAdv> nodes)
        {
            var result = Types.None;
            foreach(var item in GetDeep(nodes).Select(n => n.Tag).OfType<TabularNamedObject>())
            {
                switch(item.ObjectType)
                {
                    case ObjectType.Measure: result |= Types.Measure; break;
                    case ObjectType.Hierarchy: result |= Types.Hierarchy; break;
                    case ObjectType.Folder: result |= Types.Folder; break;
                    case ObjectType.Column:
                        result |= Types.Column;
                        if (item is CalculatedColumn) result |= Types.CalculatedColumn;
                        else if (item is DataColumn) result |= Types.DataColumn;
                        else if (item is CalculatedTableColumn) result |= Types.CalculatedTableColumn;
                        break;
                }
            }
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
            switch ((node.Tag as ITabularNamedObject).ObjectType)
            {
                case ObjectType.Model: result = Context.Model; break;
                case ObjectType.Culture: result = Context.Translation; break;
                case ObjectType.DataSource: result = Context.DataSource; break;
                case ObjectType.Perspective: result = Context.Perspective; break;
                case ObjectType.Partition: result = Context.Partition; break;
                case ObjectType.Role: result = Context.Role; break;
                case ObjectType.Relationship: result = Context.Relationship; break;
                case ObjectType.Table: result = Context.Table; break;
                case ObjectType.Level: result = Context.Level; break;
                case ObjectType.Column:
                case ObjectType.Measure:
                case ObjectType.Hierarchy:
                case ObjectType.Folder:
                    result = Context.TableObject; break;
                case ObjectType.Group:
                    switch ((node.Tag as LogicalGroup).Name)
                    {
                        case "Tables": result = Context.Tables; break;
                        case "Data Sources": result = Context.DataSources; break;
                        case "Perspectives": result = Context.Perspectives; break;
                        case "Roles": result = Context.Roles; break;
                        case "Translations": result = Context.Translations; break;
                        case "Relationships": result = Context.Relationships; break;
                    }
                    break;
            }
            return result;
        }

        public UITreeSelection(IReadOnlyCollection<TreeNodeAdv> selectedNodes) : 
            base(GetDeep(selectedNodes).Select(n => n.Tag).OfType<TabularNamedObject>())
        {
            _selectedNodes = selectedNodes;

            if (selectedNodes.Count > 0)
            {
                Context = GetNodeContext(selectedNodes.First());
                if (Context == Context.TableObject) Types = GetNodeTypes(selectedNodes);
            }

            Folders = selectedNodes.Select(n => n.Tag).OfType<Folder>();
            Groups = selectedNodes.Select(n => n.Tag).OfType<LogicalGroup>();
            Measures = new UISelectionList<Measure>(this.OfType<Measure>());
            Hierarchies = new UISelectionList<Hierarchy>(this.OfType<Hierarchy>());
            Levels = new UISelectionList<Level>(this.OfType<Level>());
            Columns = new UISelectionList<Column>(this.OfType<Column>());
            Cultures = new UISelectionList<Culture>(this.OfType<Culture>());
            Roles = new UISelectionList<ModelRole>(this.OfType<ModelRole>());
            DataSources = new UISelectionList<DataSource>(this.OfType<DataSource>());
            Perspectives = new UISelectionList<Perspective>(this.OfType<Perspective>());
            CalculatedColumns = new UISelectionList<CalculatedColumn>(this.OfType<CalculatedColumn>());
            CalculatedTableColumns = new UISelectionList<CalculatedTableColumn>(this.OfType<CalculatedTableColumn>());
            DataColumns = new UISelectionList<DataColumn>(this.OfType<DataColumn>());
            Tables = new UI.UISelectionList<Table>(this.OfType<Table>());
            // TODO: Make this work when selecting a table in the "Table Partitions" group

            Direct = new UISelectionList<ITabularNamedObject>(selectedNodes.Select(n => n.Tag).OfType<ITabularNamedObject>());
        }

        private T One<T>() where T: TabularObject
        {
            var obj = this.FirstOrDefault() as T;
            if (obj == null) throw new Exception("The collection does not contain any objects of type " + typeof(T).Name);
            else if (this.Skip(1).Any()) throw new Exception("The collection contains more than one object of type " + typeof(T).Name);
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
                    if (obj is IDetailObject) return (obj as IDetailObject).DisplayFolder;
                    if (obj is Level) return (obj as Level).Hierarchy.DisplayFolder;
                    return "";
                } else
                {
                    var obj = Direct.First();
                    if (obj is IDetailObject) return (obj as IDetailObject).DisplayFolder;
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

        [IntelliSense("All currently selected roles.")]
        public UISelectionList<ModelRole> Roles { get; private set; }

        [IntelliSense("All currently selected perspectives.")]
        public UISelectionList<Perspective> Perspectives { get; private set; }

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

        [IntelliSense("The currently selected perspective in Tabular Editor.")]
        public Perspective Perspective
        {
            get
            {
                if (UIController.Current.Tree.Perspective == null) throw new Exception("No perspective is currently selected in Tabular Editor.");
                return UIController.Current.Tree.Perspective;
            }
        }
        [IntelliSense("The currently selected culture in Tabular Editor.")]
        public Culture Culture
        {
            get
            {
                if (UIController.Current.Tree.Culture == null) throw new Exception("No culture is currently selected in Tabular Editor.");
                return UIController.Current.Tree.Culture;
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
                if(n.Tag is Folder) foreach (var c in GetDeep(n.Children)) yield return c;
            }
        }
    }
}

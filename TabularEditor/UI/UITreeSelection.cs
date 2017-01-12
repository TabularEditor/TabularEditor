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

    [Flags]
    public enum Types
    {
        None = 0x00,
        Model = 0x01,
        Table = 0x02, 
        Hierarchy = 0x04,
        Level = 0x08,
        Measure = 0x10,
        Column = 0x100,
        CalculatedColumn = 0x200,
        DataColumn = 0x400,
        CalculatedTableColumn = 0x800,
        TableObject = Types.Hierarchy | Types.Measure | Types.Column | Types.Folder,
        Folder = 0x1000
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

        Types _Types = Types.None;
        [IntelliSense("A bit mask specifiying which types are currently selected.\nExample: if(Selected.Types.HasFlag(Types.Measure)) { ... }")]
        public Types Types {
            get
            {
                if(_Types == Types.None)
                {
                    var x = 0;
                    foreach(var item in this)
                    {
                        switch(item.ObjectType)
                        {
                            case ObjectType.Model: _Types |= Types.Model; break;
                            case ObjectType.Table: _Types |= Types.Table; break;
                            case ObjectType.Measure: _Types |= Types.Measure; break;
                            case ObjectType.Column:
                                _Types |= Types.Column;
                                if (item is CalculatedColumn) _Types |= Types.CalculatedColumn;
                                else if (item is DataColumn) _Types |= Types.DataColumn;
                                break;
                            case ObjectType.Hierarchy: _Types |= Types.Hierarchy; break;
                            case ObjectType.Level: _Types |= Types.Level; break;
                        }
                        x++;
                    }
                    if (Folders.Any()) _Types |= Types.Folder;
                }
                return _Types;
            }
        }

        public UITreeSelection(IReadOnlyCollection<TreeNodeAdv> selectedNodes) : 
            base(GetDeep(selectedNodes).Select(n => n.Tag).OfType<TabularNamedObject>())
        {
            _selectedNodes = selectedNodes;

            Folders = selectedNodes.Select(n => n.Tag).OfType<Folder>();
            Measures = new UISelectionList<Measure>(this.OfType<Measure>());
            Hierarchies = new UISelectionList<Hierarchy>(this.OfType<Hierarchy>());
            Levels = new UISelectionList<Level>(this.OfType<Level>());
            Columns = new UISelectionList<Column>(this.OfType<Column>());
            CalculatedColumns = new UISelectionList<CalculatedColumn>(this.OfType<CalculatedColumn>());
            DataColumns = new UISelectionList<DataColumn>(this.OfType<DataColumn>());
            Tables = new UI.UISelectionList<Table>(this.OfType<Table>());
            Direct = new UISelectionList<ITabularNamedObject>(selectedNodes.Select(n => n.Tag).OfType<ITabularNamedObject>());
        }

        private T One<T>() where T: TabularObject
        {
            var obj = this.FirstOrDefault() as T;
            if (obj == null) throw new Exception("The collection does not contain any objects of type " + typeof(T).Name);
            else if (this.Skip(1).Any()) throw new Exception("The collection contains more than one object of type " + typeof(T).Name);
            else return obj;
        }

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

        [IntelliSense("The currently selected calculated column (if exactly one calculated column is selected in the explorer tree).")]
        public CalculatedColumn CalculatedColumn { get { return One<CalculatedColumn>(); } }

        [IntelliSense("All currently selected calculated columns (including calculated columns within selected Display Folders).")]
        public UISelectionList<CalculatedColumn> CalculatedColumns { get; private set; }

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

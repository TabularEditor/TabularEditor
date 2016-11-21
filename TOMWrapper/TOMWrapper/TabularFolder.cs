using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Used as an interface for objects that can hold other TabularTableObjects.
    /// For now, this only covers TabularTables and TabularFolders.
    /// </summary>
    public interface ITabularTableObjectCollection
    {
        IEnumerable<TabularTableObject> Children { get; }
        Table Table { get; }
        string LocalDisplayFolder { get; set; }
        string GetDisplayFolder(Culture culture);
        string FullPath { get; }
        string Path { get; }

        TabularMeasure AddMeasure(string name);
        TabularCalculatedColumn AddCalculatedColumn(string name);
    }

    public class TabularFolderCollection : KeyedCollection<string, TabularFolder>
    {
        protected override string GetKeyForItem(TabularFolder item)
        {
            return item.Path;
        }

        public TabularFolderCollection(Table table, TabularObjectCache cache, Culture culture, TabularLogicalTree treeModel)
        {
            // Get a collection of all display folder paths used in the table:
            List<string> paths;

            paths = table.Measures.Select(m => m.GetDisplayFolder(culture).TrimFolder()).Union(
                table.Columns.Select(c => c.GetDisplayFolder(culture).TrimFolder()).Union(
                    table.Hierarchies.Select(h => h.GetDisplayFolder(culture).TrimFolder())))
                    .Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n).ToList();

            foreach (var path in paths)
            {
                TabularFolder folder = null;
                var p = string.Empty;
                foreach (var pathBit in path.Split('\\'))
                {
                    p += string.IsNullOrEmpty(p) ? pathBit : ("\\" + pathBit);
                    if (Contains(p)) folder = this[p];
                    else
                    {
                        var newFolder = new TabularFolder(table, culture, p, cache, treeModel);
                        this.Add(newFolder);
                        if(folder != null) folder.ChildFolders.Add(newFolder);
                        folder = newFolder;
                    }
                }
            }
        }

        public IEnumerable<TabularFolder> RootFolders
        {
            get
            {
                if (Dictionary == null) return Enumerable.Empty<TabularFolder>();
                return Dictionary.Values.Where(f => f.IsRoot);
            }
        }
    }

    /// <summary>
    /// The TabularFolder is a special kind of TabularObject that does not exist in the TOM.
    /// It is only used for properly displaying Display Folders in a TreeModel.
    /// </summary>
    public class TabularFolder : TabularTableObject, ITabularTableObjectCollection
    {
        public override void Clone(string newName, bool includeTranslations)
        {
            // Display Folders are never cloned
            throw new NotImplementedException();
        }
        public override void Delete()
        {
            // Deleting a display folder should equal setting it's name to blank
            throw new NotImplementedException();
        }

        [Browsable(false)]
        public override string Description { get; set; }
        [Browsable(false)]
        public TabularLogicalTree TreeModel { get; private set; }

        public TabularMeasure AddMeasure(string name)
        {
            return (_cache[Table] as TabularTable).AddMeasure(name, Path);
        }
        public TabularCalculatedColumn AddCalculatedColumn(string name)
        {
            return (_cache[Table] as TabularTable).AddCalculatedColumn(name, Path);
        }

        /// <summary>
        /// Gets the display folder path prefixed by the table name. Uses the culture of the
        /// current LogicalTree.
        /// </summary>
        [Browsable(false)]
        public string FullPath { get { return Table.Name + "\\" + Path; } }
        public override string GetDisplayFolder(Culture culture)
        {
            return ParentPath;
        }
        public override void SetDisplayFolder(string folder, Culture culture)
        {
            SetDisplayFolder(folder, culture, true);
        }
        public void SetDisplayFolder(string folder, Culture culture, bool applyToChildObjects)
        {
            var newPath = folder + (string.IsNullOrEmpty(folder) ? "" : "\\") + Name;

            if (applyToChildObjects) TreeModel.ModifyDisplayFolder(Table, Path, newPath, culture);

            foreach (var f in ChildFolders) f.SetDisplayFolder(newPath, Culture, false);

            Path = newPath;
        }

        public override string DisplayFolder { get { return base.DisplayFolder; } set { base.DisplayFolder = value; } }
        [Browsable(false)]
        public bool IsRoot { get { return !Path.Contains("\\"); } }

        [Browsable(false)]
        public string Path { get; private set; }
        [Browsable(false)]
        public string ParentPath
        {
            get
            {
                var pathBits = Path.Split('\\');
                if (pathBits.Length == 1) return string.Empty;
                string[] parentPathBits = new string[pathBits.Length - 1];
                Array.Copy(pathBits, parentPathBits, pathBits.Length - 1);
                return string.Join("\\", parentPathBits);
            }
        }
        TabularObjectCache _cache;
        private Table _table;
        public override Table Table { get { return _table; } }
        [Browsable(false)]
        public Culture Culture;
        public override string Name
        {
            get { return Path.Split('\\').Last(); }
            set { SetFolderName(value); }
        }

        

        public void SetFolderName(string name)
        {
            var newPath = ParentPath.ConcatPath(name);
            
            // Update the non-folder objects within this folder:
            TreeModel.ModifyDisplayFolder(Table, Path, newPath, Culture);

            // Update the folder objects within this folder (so the tree does not need to be rebuilt):
            foreach (var f in ChildFolders) f.SetDisplayFolder(newPath, Culture, false);

            Path = newPath;
            
        }

        public override int Icon { get { return TabularIcons.ICON_FOLDER; } }

        internal List<TabularFolder> ChildFolders;
        /// <summary>
        /// Gets the immediate children of this display folder including child folders
        /// </summary>
        [Browsable(false)]
        public IEnumerable<TabularTableObject> Children
        {
            get
            {
                foreach (var m in Table.Measures.Where(m => m.GetDisplayFolder(Culture) == Path)) yield return _cache[m] as TabularMeasure;
                foreach (var c in Table.Columns.Where(c => c.GetDisplayFolder(Culture) == Path)) yield return _cache[c] as TabularColumn;
                foreach (var h in Table.Hierarchies.Where(h => h.GetDisplayFolder(Culture) == Path)) yield return _cache[h] as TabularHierarchy;
                foreach (var cf in ChildFolders) yield return cf;
            }
        }
        public override TabularObjectType Type { get { return TabularObjectType.DisplayFolder; } }

        public TabularFolder(Table table, Culture culture, string path, TabularObjectCache cache, TabularLogicalTree treeModel)
        {
            ChildFolders = new List<TabularFolder>();

            TreeModel = treeModel;
            _table = table;
            Culture = culture;
            Path = path;
            _cache = cache;
        }
        [Browsable(false)]
        public override bool Visible
        {
            get {
                return Children.Any(c => c.Visible);
            }
            set
            {
                foreach (var c in Children) c.Visible = value;
            }
        }
        public override bool InPerspective(Perspective perspective)
        {
            return Children.Any(c => c.InPerspective(perspective));
        }
        public override void SetPerspective(string perspectiveName, bool include)
        {
            Children.ToList().ForEach(c => c.SetPerspective(perspectiveName, include));
        }

        [Browsable(false)]
        public override IDictionary<string, string> DescriptionTranslations { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, string> NameTranslations { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, string> DisplayFolderTranslations { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, bool> PerspectiveMembership { get { return null; } }

        [Browsable(false)]
        public override string LocalDescription { get; set; }

        // Tabular Folders only exist in the context of the current Culture, so LocalName is always equal to Name.
        [Browsable(false)]
        public override string LocalName { get { return Name; } set { Name = value; } }
        [Browsable(false)]
        public override string LocalDisplayFolder { get { return DisplayFolder; } set { DisplayFolder = value; } }

    }

}

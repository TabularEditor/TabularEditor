using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Represents a Folder in the TreeView. Does not correspond to any object in the TOM.
    /// Implements IDisplayFolderObject since a Folder can itself be located within another
    /// display folder.
    /// Implements IParentObject since a Folder can contain child objects.
    /// </summary>
    [DebuggerDisplay("{ObjectType} {Path}")]
    public class Folder : IDetailObject, ITabularObjectContainer, IDetailObjectContainer, ITabularNamedObject, ITabularTableObject,
        IErrorMessageObject
    {
        [Browsable(false)]
        public IDetailObjectContainer Container
        {
            get
            {
                return Tree.FolderTree[Table.Name.ConcatPath(DisplayFolder)];
            }
        }

        [Browsable(false)]
        public Table ParentTable { get { return Table; } }

        public string ErrorMessage { get; private set; }

        static public Folder CreateFolder(Table table, string path = "", bool useFixedCulture = false, Culture fixedCulture = null)
        {
            // Always attempt to re-use folders:
            var fullPath = table.Name.ConcatPath(path);
            Folder result;
            if(!table.Handler.Tree.FolderTree.TryGetValue(fullPath, out result))
            {
                result = new Folder(table, path);
            }
            if (useFixedCulture)
            {
                result.useFixedCulture = useFixedCulture;
                result.fixedCulture = fixedCulture;
            }
            result.CheckChildrenErrors();

            return result;
        }

        public void CheckChildrenErrors()
        {
            var errObj = GetChildren().OfType<IErrorMessageObject>().FirstOrDefault(c => !string.IsNullOrEmpty(c.ErrorMessage));
            if (errObj != null && (!(errObj as IExpressionObject)?.NeedsValidation ?? true))
            {
                ErrorMessage = "Error on " + (errObj as TabularNamedObject).Name + ": " + errObj.ErrorMessage;
            }
            else
            {
                ErrorMessage = null;
            }
        }

        /// <summary>
        /// Deleting a folder does not delete child objects - it just removes the folder.
        /// Any child folders are retained (but will be moved up the display folder hierarchy).
        /// </summary>
        public void Delete()
        {
            SetFolderName("");
        }

        private Folder(Table table, string path = "")
        {
            Table = table;
            _path = path;

            Tree.UpdateFolder(this);
        }

        [Browsable(false)]
        public TabularModelHandler Handler { get { return Table.Handler; } }
        [Browsable(false)]
        private TabularTree Tree { get { return Handler.Tree; } }
        [Browsable(false)]
        public Culture Culture { get { return useFixedCulture ? fixedCulture : Tree.Culture; } }

        private bool useFixedCulture = false;
        private Culture fixedCulture;
        
            [Browsable(false)]
        public Model Model { get { return Table.Model; } }

        public ObjectType ObjectType
        {
            get { return ObjectType.Folder; }
        }

        public string Name
        {
            get {
                return Path.Split('\\').Last();
            }
            set {
                SetFolderName(value);
            }
        }

        [Browsable(false)]
        public Table Table { get; private set; }

        public void SetFolderName(string newName)
        {
            var pathBits = Path.Split('\\');
            pathBits[pathBits.Length - 1] = newName;
            Path = string.Join("\\", pathBits);
        }

        [Browsable(false)]
        public string FullPath
        {
            get
            {
                return Table.Name.ConcatPath(Path);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string DisplayFolder
        {
            get
            {
                var pathBits = Path.Split('\\');
                return string.Join("\\", pathBits.Take(pathBits.Length - 1));
            }

            set
            {
                Path = value.ConcatPath(Name);
                Handler.UpdateFolders(Table);
            }
        }

        private string _path;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void UndoSetPath(string value)
        {
            var oldFullPath = FullPath;
            _path = value;
            Tree.UpdateFolder(this, oldFullPath);
        }

        [ReadOnly(true)]
        public string Path {
            get {
                return _path;
            }
            private set
            {
                if (value == _path) return;

                Handler.BeginUpdate("folder rename");

                // Handle child objects (only those currently visible in the tree):
                foreach(var c in GetChildrenByFolders(true).Where(c => Tree.VisibleInTree(c)))
                {
                    c.SetDisplayFolder(value.ConcatPath(c.GetDisplayFolder(Culture).Substring(_path.Length)), Culture);
                }

                var oldFullPath = FullPath;
                _path = value.TrimFolder();

                Tree.UpdateFolder(this, oldFullPath);

                Handler.EndUpdate();
            }
        }

        [Browsable(false)]
        public TranslationIndexer TranslatedDisplayFolders
        {
            get
            {
                throw new InvalidOperationException();
            }
        }
        [Browsable(false)]
        public TranslationIndexer TranslatedNames
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public IEnumerable<IDetailObject> GetChildrenByFolders(bool recursive = false)
        {
            var allChildren = Table.GetChildren().OfType<IDetailObject>();
            var folders = Enumerable.Empty<Folder>();
            IEnumerable<IDetailObject> children;

            if (recursive)
            {
                children = allChildren.Where(c => c.HasAncestor(this, Culture));
            }
            else
            {
                children = allChildren.Where(c => c.HasParent(this, Culture));
                folders = GetChildPaths().Select(f => CreateFolder(Table, f));
            }

            return folders.Concat(children);
        }

        private IEnumerable<string> GetChildPaths()
        {
            var level = Path.Level();

            var result = Table.GetChildren().OfType<IDetailObject>().Where(c => c.HasAncestor(this, Culture))
                .Select(c => c.GetDisplayFolder(Culture))
                .Where(f => f.Level() > level)
                .Select(f => f.Split('\\').Take(level + 1).ConcatPath()).Distinct();

            return result;
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return GetChildrenByFolders(true).OfType<ITabularNamedObject>();
        }
    }
}

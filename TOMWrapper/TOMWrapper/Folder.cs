using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TabularEditor.TOMWrapper.Undo;

namespace TabularEditor.TOMWrapper
{

    /// <summary>
    /// Represents a Folder in the TreeView. Does not correspond to any object in the TOM.
    /// Implements IDisplayFolderObject since a Folder can itself be located within another
    /// display folder.
    /// Implements IParentObject since a Folder can contain child objects.
    /// </summary>
    [DebuggerDisplay("{ObjectType} {Path}")]
    public class Folder : IFolderObject, IFolder, ITabularTableObject, IErrorMessageObject, IHideableObject
    {
        internal List<IFolderObject> Children { get; private set; } = new List<IFolderObject>();
        [Browsable(false)]
        public IFolder Container
        {
            get
            {
                return Table.FolderCache[DisplayFolder];
            }
        }
        public bool IsRemoved => false;

        bool ITabularNamedObject.CanEditName() { return Handler.PowerBIGovernance.AllowEditProperty(ObjectType.Measure, Properties.DISPLAYFOLDER); }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Table.GetHashCode();
                hash = hash * 23 + _path.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return GetType() == obj.GetType() && Equals((Folder)obj);
        }

        public bool Equals(Folder other)
        {
            return Table == other.Table && Path == other.Path;
        }

        public static bool operator== (Folder obj1, Folder obj2)
        {
            if (ReferenceEquals(obj1, obj2)) return true;
            if (ReferenceEquals(obj1, null)) return false;
            if (ReferenceEquals(obj2, null)) return false;

            return obj1.Equals(obj2);
        }
        public static bool operator!= (Folder obj1, Folder obj2)
        {
            return !(obj1 == obj2);
        }

        [Browsable(false)]
        public int MetadataIndex
        {
            get
            {
                return -1;
            }
        }

        [Browsable(false)]
        public Table ParentTable { get { return Table; } }

        [Browsable(false)]
        public string ErrorMessage { get; private set; }

        static public Folder CreateFolder(Table table, string path = "")
        {
            // Always attempt to re-use folders:
            Folder result;
            if(!table.FolderCache.TryGetValue(path, out result))
            {
                result = new Folder(table, path);

                // Add to parent folder:
                if (!string.IsNullOrEmpty(path))
                {
                    var parent = CreateFolder(table, result.DisplayFolder);
                    parent.Children.Add(result);
                }
            }
            return result;
        }

        internal void CheckChildrenErrors()
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

        internal void ClearError()
        {
            ErrorMessage = null;
        }
        internal void AddError(IFolderObject folderObject)
        {
            if(ErrorMessage == null)
            {
                ErrorMessage = "Child objects with errors:";
                var parentFolder = this.GetFolder(Tree.Culture);
                if (parentFolder != null && parentFolder.Name != "") parentFolder.AddError(this);
                else Table.AddError(this);
            }
            if (folderObject is Folder f) ErrorMessage += "\r\nObjects inside the '" + f.Name + "' folder.";
            else ErrorMessage += "\r\n" + folderObject.GetTypeName() + " " + folderObject.GetName();
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
        public TabularModelHandler Handler => Table.Handler;
        [Browsable(false)]
        private TabularTree Tree => Table.Handler.Tree;
        [Browsable(false)]
        public Culture Culture => Table.Handler.Tree.Culture;
        
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

        [ReadOnly(true)]
        public string Path {
            get {
                return _path;
            }
            private set
            {
                if (value == _path) return;

                Handler.BeginUpdate("folder rename");

                // Rename child objects:
                foreach(var c in GetChildrenByFolders())
                {
                    var folderStrings = c.GetDisplayFolder(Culture).Split(';');
                    if(folderStrings.Length == 1)
                        c.SetDisplayFolder(value.ConcatPath(folderStrings[0].Substring(_path.Length)), Culture);
                    else
                    {
                        var ix = Array.FindIndex(folderStrings, s => s.EqualsI(Path));
                        if(ix >= 0)
                        {
                            folderStrings[ix] = value.ConcatPath(folderStrings[ix].Substring(_path.Length));
                            c.SetDisplayFolder(string.Join(";", folderStrings), Culture);
                        }
                    }
                }

                var oldPath = _path;
                _path = value.TrimFolder();

                Tree.UpdateFolder(this, oldPath);

                Handler.EndUpdate();

            }
        }

        [Browsable(false)]
        public TranslationIndexer TranslatedDisplayFolders
        {
            get
            {
                return null;
            }
        }
        [Browsable(false)]
        public TranslationIndexer TranslatedNames
        {
            get
            {
                return null;
            }
        }

        [ReadOnly(true),Browsable(false)]
        public bool IsHidden
        {
            get
            {
                return GetChildrenByFolders().OfType<IHideableObject>().All(item => item.IsHidden);
            }

            set { }
        }

        /// <summary>
        /// Gets the visibility of the Folder. Shorthand for !<see cref="IsHidden"/>.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => !IsHidden;


        public IEnumerable<IFolderObject> GetChildrenByFolders()
        {
            var items = Children.OrderBy(i => i.GetDisplayOrder());
            if (Tree.Options.HasFlag(LogicalTreeOptions.OrderByName)) items = items.ThenBy(i => i.Name);
            return items;
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            foreach(var child in Children)
            {
                yield return child;
                var childAsFolder = child as Folder;
                if (childAsFolder != null) foreach (var cc in childAsFolder.GetChildren()) yield return cc;
            }
        }

        public bool CanDelete()
        {
            return true;
        }

        public bool CanDelete(out string message)
        {
            message = null;
            return true;
        }
    }
}

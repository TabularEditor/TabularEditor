using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    [Flags]
    public enum LogicalTreeOptions
    {
        DisplayFolders = 0x01,
        Columns = 0x02,
        Measures = 0x04,
        KPIs = 0x08,
        Hierarchies = 0x10,
        Levels = 0x20,
        ShowHidden = 0x40,
        AllObjectTypes = 0x80,
        ShowRoot = 0x100,
        Default = DisplayFolders | Columns | Measures | KPIs | Hierarchies | Levels | ShowRoot | AllObjectTypes
    }

    /// <summary>
    /// The TabularLogicalModel controls the relation between TabularObjects for display in the TreeViewAdv
    /// control. Each individual TabularObject does not know or care about its logical relation to other
    /// objects (for example, through DisplayFolders in a specific culture). TabularObjects only care
    /// about their physical relations which are inherited from the Tabular Object Model directly (i.e.,
    /// a measure belongs to a table, etc.).
    /// </summary>
    public abstract class TabularTree: INotifyPropertyChanged
    {
        #region Properties
        public LogicalTreeOptions Options
        {
            get
            {
                return _options;
            }

            set
            {
                if (_options == value) return;
                _options = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Options"));
                OnStructureChanged();
            }
        }
        private LogicalTreeOptions _options;

        public string Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (_filter == value) return;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filter"));
                _filter = value;
                OnStructureChanged();
            }
        }
        private string _filter;

        public Culture Culture
        {
            get
            {
                return _culture;
            }

            set
            {
                if (_culture == value) return;
                _culture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Culture"));
                FolderTree.Clear();
                OnStructureChanged();
            }
        }
        private Culture _culture;
        public void SetCulture(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
                Culture = null;
            else
                Culture = Model.Cultures[cultureName];
        }

        public Perspective Perspective
        {
            get
            {
                return _perspective;
            }

            set
            {
                if (_perspective == value) return;
                _perspective = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Perspective"));
                OnStructureChanged();
            }
        }
        private Perspective _perspective;
        public void SetPerspective(string perspectiveName)
        {
            if (string.IsNullOrEmpty(perspectiveName))
                Perspective = null;
            else
                Perspective = Model.Perspectives[perspectiveName];
        }

        #endregion        


        protected Model Model { get; private set; }
        protected TabularModelHandler Handler { get { return Model.Handler; } }

        public TabularTree(Model model)
        {
            Model = model;
            model.Handler.Tree = this;
        }

        protected int UpdateLocks { get; private set; }

        public virtual void BeginUpdate()
        {
            UpdateLocks++;
        }

        public virtual void EndUpdate()
        {
            if (UpdateLocks == 0) throw new InvalidOperationException("EndUpdate() called before BeginUpdate()");
            UpdateLocks--;
        }        

        #region Handling Display Folders
            
        /// <summary>
        /// Updates the DisplayFolder property of all tabular objects within one table. Objects residing
        /// in subfolders to the updated path, will also be updated.
        /// </summary>
        /// <param name="table">The table in which to perform the update</param>
        /// <param name="oldPath">The current DisplayFolder (parent) path of the objecs</param>
        /// <param name="newPath">The new DisplayFolder path, replacing oldPath</param>
        /// <param name="culture">The culture to which the change should apply</param>
        public void ModifyDisplayFolder(Table table, string oldPath, string newPath, Culture culture)
        {
            var tab = table;
            foreach(var c in tab.GetChildren().OfType<IDetailObject>())
            {
                var currentPath = c.GetDisplayFolder(culture) + "\\";
                if (currentPath.StartsWith(oldPath + "\\", StringComparison.InvariantCultureIgnoreCase))
                {
                    c.SetDisplayFolder(newPath.ConcatPath(currentPath.Substring(oldPath.Length)), culture);
                }
            }
        }

        public Func<string, string> GetFolderMutation(object source, object destination)
        {
            string srcPath = FolderHelper.GetFullPath(source as ITabularNamedObject);
            string dstPath = FolderHelper.GetFullPath(destination as ITabularNamedObject);

            return GetFolderMutation(srcPath, dstPath);
        }

        public Func<string, string> GetFolderMutation(string oldPath, string newPath)
        {
            return new Func<string, string>((s) => {
                return newPath.ConcatPath(s.Substring(oldPath.Length));
            });
        }

        
        #endregion

        #region ITreeModel implementation
        

        /// <summary>
        /// This method encapsulates the logic of how the tree representation of the tabular model should be structured
        /// </summary>
        protected IEnumerable GetChildren(ITabularObjectContainer tabularObject)
        {
            bool useDF = Options.HasFlag(LogicalTreeOptions.DisplayFolders);

            if(tabularObject is LogicalGroup)
            {
                switch ((tabularObject as LogicalGroup).Name)
                {
                    case "Tables": return Model.Tables.Where(t => VisibleInTree(t));
                    case "Roles": return Model.Roles;
                    case "Perspectives": return Model.Perspectives;
                    case "Translations": return Model.Cultures;
                    case "Relationships": return Model.Relationships;
                    case "Data Sources": return Model.DataSources;
                }
                return Enumerable.Empty<TabularNamedObject>();
            }
            if(tabularObject is Model && !Options.HasFlag(LogicalTreeOptions.AllObjectTypes))
            {
                return Model.Tables.Where(t => VisibleInTree(t));
            }
            if(tabularObject is Table)
            {
                var table = tabularObject as Table;

                if (useDF)
                {
                    var rootFolder = Folder.CreateFolder(table, "");
                    return rootFolder.GetChildrenByFolders().Where(c => VisibleInTree(c));
                } else
                {
                    return table.GetChildren().Where(c => VisibleInTree(c));
                }
            }
            if (tabularObject is Folder)
            {
                var folder = tabularObject as Folder;
                return folder.GetChildrenByFolders().Where(c => VisibleInTree(c));
            }
            if (tabularObject is Hierarchy)
            {
                var hier = tabularObject as Hierarchy;
                return hier.Levels;
            }
            if(tabularObject is ITabularObjectContainer)
            {
                return (tabularObject as ITabularObjectContainer).GetChildren();
            }
            return Enumerable.Empty<TabularNamedObject>();
        }

        public bool VisibleInTree(ITabularNamedObject tabularObject)
        {
            // Never show the RowNumber column:
            if ((tabularObject as Column)?.Type == TOM.ColumnType.RowNumber) return false;

            // Don't show invisible objects:
            if ((tabularObject is IHideableObject) && (tabularObject as IHideableObject).IsHidden && !Options.HasFlag(LogicalTreeOptions.ShowHidden)) return false;

            // Empty folders are never shown:
            if (tabularObject is Folder) {
                if ((tabularObject as Folder).GetChildren().All(c => !VisibleInTree(c))) return false;
            }

            // Don't show objects not in the current perspective:
            if (Perspective != null && tabularObject is ITabularPerspectiveObject && !(tabularObject as ITabularPerspectiveObject).InPerspective[Perspective]) return false;

            // Hide items not matching the filter text:
            if (!string.IsNullOrEmpty(Filter))
            {
                switch (tabularObject.ObjectType)
                {
                    case ObjectType.Column:
                    case ObjectType.Hierarchy:
                    case ObjectType.Measure:
                        if (tabularObject.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) == -1) return false;
                        break;
                }
            }

            // Type dependent display:
            switch(tabularObject.ObjectType)
            {
                case ObjectType.Column:
                    return (Options.HasFlag(LogicalTreeOptions.Columns));
                case ObjectType.Measure:
                    return (Options.HasFlag(LogicalTreeOptions.Measures));
                case ObjectType.Hierarchy:
                    return (Options.HasFlag(LogicalTreeOptions.Hierarchies));
            }

            // Always hide empty tabels
            /*if (tabularObject is Table)
            {
                var table = tabularObject as Table;
                return table.GetChildren().Any(o => VisibleInTree(o));
            }*/

            // All other objects should be visible by default:
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal readonly Dictionary<string, Folder> FolderTree = new Dictionary<string, Folder>();
        internal void UpdateFolder(Folder folder, string oldFullPath = null)
        {
            if (!string.IsNullOrEmpty(oldFullPath)) FolderTree.Remove(oldFullPath);

            if(FolderTree.ContainsKey(folder.FullPath))
            {
                FolderTree[folder.FullPath] = folder;
            }
            else
            {
                FolderTree.Add(folder.FullPath, folder);
            }
        }

        public abstract void OnStructureChanged(TabularNamedObject obj = null);

        public abstract void OnNodesRemoved(ITabularObject parent, params ITabularObject[] children);

        public void OnNodesRemoved(ITabularObject parent, IEnumerable<ITabularObject> children)
        {
            OnNodesRemoved(parent, children.ToArray());
        }

        public abstract void OnNodesInserted(ITabularObject parent, params ITabularObject[] children);

        public void OnNodesInserted(ITabularObject parent, IEnumerable<ITabularObject> children)
        {
            OnNodesInserted(parent, children.ToArray());
        }

        public abstract void OnNodesChanged(ITabularObject nodeItem);

        #endregion

    }


}

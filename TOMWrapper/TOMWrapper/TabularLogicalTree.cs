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
        OrderByName = 0x200,
        Default = DisplayFolders | Columns | Measures | KPIs | Hierarchies | Levels | ShowRoot | AllObjectTypes
    }

    internal static class ObjectOrderHelper
    {
        public static int GetDisplayOrder(this ITabularNamedObject item)
        {
            switch(item.ObjectType)
            {
                case ObjectType.PartitionCollection: return 0;
                case ObjectType.Folder: return 1;
                case ObjectType.Measure: return 2;
                case ObjectType.Column: return 3;
                case ObjectType.Hierarchy: return 4;
                default: return 5;
            }
        }
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
                var oldOptions = _options;
                if (_options == value) return;
                _options = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Options"));
                if(oldOptions.HasFlag(LogicalTreeOptions.DisplayFolders) ^ _options.HasFlag(LogicalTreeOptions.DisplayFolders)) RebuildFolderCache();
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
                RebuildFolderCache();
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

        public void RebuildFolderCache()
        {
            foreach (var table in Model.Tables)
            {
                RebuildFolderCacheForTable(table);
            }
        }

        private HashSet<Table> folderCachesToBeRebuilt = new HashSet<Table>();

        public void RebuildFolderCacheForTable(Table table)
        {
            if (!Options.HasFlag(LogicalTreeOptions.DisplayFolders)) return;

            if (UpdateLocks > 0)
            {
                folderCachesToBeRebuilt.Add(table);
                return;
            }
            
            table.FolderCache.Clear();
            foreach (var m in table.Measures) BuildFolderForObject(m);
            foreach (var c in table.Columns) BuildFolderForObject(c);
            foreach (var h in table.Hierarchies) BuildFolderForObject(h);
        }

        private void BuildFolderForObject(IFolderObject obj)
        {
            var folder = Folder.CreateFolder(obj.Table, obj.GetDisplayFolder(Culture));
            folder.Children.Add(obj);
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
                RebuildFolderCache();
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

        public event EventHandler UpdateComplete;

        protected Model Model { get; private set; }
        protected TabularModelHandler Handler { get { return Model.Handler; } }

        public TabularTree(Model model)
        {
            if (model == null) return;
            Model = model;
            model.Handler.Tree = this;
        }

        internal protected int UpdateLocks { get; private set; }

        public virtual void BeginUpdate()
        {
            UpdateLocks++;
        }

        public virtual void EndUpdate()
        {
            if (UpdateLocks == 0) throw new InvalidOperationException("EndUpdate() called before BeginUpdate()");
            UpdateLocks--;

            if (UpdateLocks == 0)
            {
                // Rebuilt folder caches if necessary:
                if(folderCachesToBeRebuilt.Count > 0 && Options.HasFlag(LogicalTreeOptions.DisplayFolders))
                {
                    foreach (var table in folderCachesToBeRebuilt) RebuildFolderCacheForTable(table);
                    folderCachesToBeRebuilt.Clear();
                }

                if(Handler.UndoManager.BatchSize > 0) UpdateComplete?.Invoke(this, new EventArgs());
            }
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
            foreach(var c in tab.GetChildren().OfType<IFolderObject>())
            {
                var currentPath = c.GetDisplayFolder(culture) + "\\";
                if (currentPath.StartsWith(oldPath + "\\", StringComparison.InvariantCultureIgnoreCase))
                {
                    c.SetDisplayFolder(newPath.ConcatPath(currentPath.Substring(oldPath.Length)), culture);
                }
            }
        }

        /*public Func<string, string> GetFolderMutation(object source, object destination)
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
        }*/

        
        #endregion

        #region ITreeModel implementation
        
        private IEnumerable<ITabularNamedObject> GetChildrenForTable(Table table)
        {
            if (table.SourceType != TOM.PartitionSourceType.Calculated)
            {
                // Don't show the "Partitions" node below tables, when none of the partitions match the filter
                if (!string.IsNullOrEmpty(Filter))
                {
                    // Match on table name:
                    if (table.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0) yield return table.Partitions;
                    else if (table.Partitions.Any(p => p.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0)) yield return table.Partitions;
                }
                else
                    yield return table.Partitions;
            }

            IEnumerable<ITabularNamedObject> items;

            if (Options.HasFlag(LogicalTreeOptions.DisplayFolders))
            {
                var rootFolder = Folder.CreateFolder(table, "");
                items = rootFolder.GetChildrenByFolders();
            }
            else
            {
                items = table.GetChildren();
                if (Options.HasFlag(LogicalTreeOptions.OrderByName))
                    items = items.OrderBy(i => i.GetDisplayOrder()).ThenBy(i => i.Name);
            }
            items = items.Where(i => VisibleInTree(i));

            foreach (var item in items)
                yield return item;
            yield break;
        }

        /// <summary>
        /// This method encapsulates the logic of how the tree representation of the tabular model should be structured
        /// </summary>
        protected IEnumerable GetChildren(ITabularObjectContainer tabularObject)
        {
            IEnumerable<ITabularNamedObject> result = Enumerable.Empty<ITabularNamedObject>();

            if(tabularObject is LogicalGroup)
            {
                result = (tabularObject as LogicalGroup).GetChildren().Where(o => VisibleInTree(o));
            }
            else if(tabularObject is Model)
            {
                // If all object types should be shown, simply let the Model.GetChildren() method
                // return the objects needed:
                if (Options.HasFlag(LogicalTreeOptions.AllObjectTypes))
                {
                    result = Model.GetChildren().Where(o => VisibleInTree(o));
                }
                // Otherwise, only show tables:
                else result = Model.Tables.Where(t => VisibleInTree(t));
            }
            else if(tabularObject is Table)
            {
                // Handles sorting internally:
                return GetChildrenForTable(tabularObject as Table);
            }
            else if (tabularObject is Folder)
            {
                // Handles sorting internally:
                return (tabularObject as Folder).GetChildrenByFolders().Where(c => VisibleInTree(c));
            }
            else if (tabularObject is Hierarchy)
            {
                // Never sort:
                return (tabularObject as Hierarchy).Levels.OrderBy(l => l.Ordinal);
            }
            else if(tabularObject is ITabularObjectContainer)
            {
                // All other types:
                result = (tabularObject as ITabularObjectContainer).GetChildren();
            }

            return Options.HasFlag(LogicalTreeOptions.OrderByName) ? result.OrderBy(o => o.Name) : result;

        }

        public bool VisibleInTree(ITabularNamedObject tabularObject)
        {
            // Never show the RowNumber column:
            if ((tabularObject as Column)?.Type == ColumnType.RowNumber) return false;

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
                    case ObjectType.Relationship:
                    case ObjectType.Partition:
                    case ObjectType.DataSource:
                    case ObjectType.Perspective:
                    case ObjectType.Role:
                    case ObjectType.Culture:
                    case ObjectType.Column:
                    case ObjectType.Hierarchy:
                    case ObjectType.Measure:
                        if(tabularObject is ITabularTableObject)
                        {
                            // If the parent table's name matches the filter criteria, show all objects inside the table, even
                            // though they don't match the filter:
                            if ((tabularObject as ITabularTableObject).Table.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0) break;
                        }
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

            // Always hide empty tables when filtering, unless the table name matches the filter criteria
            if (tabularObject is Table)
            {
                var table = tabularObject as Table;
                return string.IsNullOrEmpty(Filter) || (table.GetChildren().Any(o => VisibleInTree(o)) || table.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            // Same goes for PartitionViewTables:
            if (tabularObject is PartitionViewTable)
            {
                var table = tabularObject as PartitionViewTable;
                return string.IsNullOrEmpty(Filter) || (table.GetChildren().Any(o => VisibleInTree(o)) || table.Name.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            // If a filter is in place, Logical Groups are only shown when they contain objects:
            if (tabularObject is LogicalGroup)
            {
                return string.IsNullOrEmpty(Filter) || (tabularObject as LogicalGroup).GetChildren().Any(o => VisibleInTree(o));
            }

            // All other objects should be visible by default:
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void ClearFolderErrors()
        {
            if (Model == null) return;
            foreach (var t in Model.Tables) {
                foreach (var f in t.FolderCache.Values) f.ClearError();
            }
        }
        internal void UpdateFolder(Folder folder, string oldPath = null)
        {
            var cache = folder.Table.FolderCache;
            if (!string.IsNullOrEmpty(oldPath)) cache.Remove(oldPath);

            if(cache.ContainsKey(folder.Path))
            {
                cache[folder.Path] = folder;
            }
            else
            {
                cache.Add(folder.Path, folder);
            }
        }

        public abstract void OnStructureChanged(ITabularNamedObject obj = null);

        public abstract void OnNodesRemoved(ITabularObject parent, params ITabularObject[] children);

        public void OnNodesRemoved(ITabularObject parent, IEnumerable<ITabularObject> children)
        {
            OnNodesRemoved(parent, children.ToArray());
        }

        public abstract void OnNodesInserted(ITabularObject parent, params ITabularObject[] children);

        public void OnNodesInserted(ITabularObject parent, IEnumerable<ITabularObject> children)
        {
            var table = parent as Table;
            if(table != null && Options.HasFlag(LogicalTreeOptions.DisplayFolders))
            {
                foreach (var child in children.OfType<IFolderObject>())
                    Folder.CreateFolder(table, child.DisplayFolder).Children.Add(child);
            }
            OnNodesInserted(parent, children.ToArray());
        }

        public virtual void OnNodesChanged() { }

        /// <summary>
        /// Call this method to signal to the TreeView that a node needs repaint, typically
        /// because a property was changed that affects how the node should be rendered (but not
        /// WHERE the node should be rendered).
        /// </summary>
        /// <param name="nodeItem"></param>
        public abstract void OnNodesChanged(ITabularObject nodeItem);

        /// <summary>
        /// Call this method to signal to the TreeView that a nodes name was changed. When
        /// using the OrderByName option, this typically requires a call to OnStructureChanged
        /// on the parent node.
        /// </summary>
        /// <param name="nodeItem"></param>
        public void OnNodeNameChanged(ITabularNamedObject nodeItem)
        {
            if (Options.HasFlag(LogicalTreeOptions.OrderByName))
                OnStructureChanged(nodeItem.GetContainer());
            else
                OnNodesChanged(nodeItem);
        }

        #endregion

    }


}

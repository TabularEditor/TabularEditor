extern alias json;

using Aga.Controls.Tree;
using json.Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Actions;
using TabularEditor.UI.Tree;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        TabularNodeTextBox TreeView_NameCol;

        public void Goto(ITabularNamedObject obj)
        {
            var node = UI.TreeView.FindNodeByTag(obj);
            if(node == null)
            {
                TreeModel.BeginUpdate();
                var options = TreeModel.Options;

                // Adjust the LogicalTreeOptions based on the object itself. For example, if the object is hidden,
                // make sure the tree displays hidden objects, etc.:
                if ((obj is IHideableObject) && (obj as IHideableObject).IsHidden) options = options | LogicalTreeOptions.ShowHidden;
                if ((obj is Column)) options = options | LogicalTreeOptions.Columns;
                else if ((obj is Measure)) options = options | LogicalTreeOptions.Measures;
                else if ((obj is Hierarchy)) options = options | LogicalTreeOptions.Hierarchies;
                else if ((obj is Table) || (obj is Model)) { /* Do nothing */ }
                else options = options | LogicalTreeOptions.AllObjectTypes;

                TreeModel.Options = options;

                //InternalApplyFilter("");
                UI.FormMain.UpdateTreeUIButtons();
                TreeModel.EndUpdate();
                node = UI.TreeView.FindNodeByTag(obj);
            }

            if (node != null)
            {
                UI.TreeView.EnsureVisible(node);
                UI.TreeView.SelectedNode = node;
                UI.FormMain.Activate();
                UI.TreeView.Focus();
            }
        }

        private Stack<IExpressionObject> Back;
        private Stack<IExpressionObject> Forward;

        public bool CanNavigateForward => Forward != null && Forward.Count > 0;
        public bool CanNavigateBack => Back != null && Back.Count > 0;
        private bool IsNavigatingBack = false;

        public void Tree_NavigateForward()
        {
            if (!CanNavigateForward) return;

            while (Forward.Count > 0)
            {
                var nav = Forward.Pop();
                if (nav == null || nav.IsRemoved) continue;

                UI.FormMain.Activate();
                var node = UI.TreeView.FindNodeByTag(nav);
                if (node != null)
                {
                    UI.TreeView.EnsureVisible(node);
                    UI.TreeView.SelectedNode = node;
                    UI.TreeView.Focus();
                }
                else
                {
                    UI.TreeView.SelectedNode = null;
                    ExpressionEditor_Preview(nav);
                    PropertyGrid_UpdateFromObject(nav);
                    UI.ExpressionEditor.SelectAll();
                    UI.ExpressionEditor.Focus();
                }
                break;
            }
        }

        public void Tree_NavigateBack()
        {
            if (!CanNavigateBack) return;

            IsNavigatingBack = true;
            while (Back.Count > 0)
            {
                var nav = Back.Pop();
                if (nav == null || nav.IsRemoved) continue;

                UI.FormMain.Activate();
                var node = UI.TreeView.FindNodeByTag(nav);
                if (node != null)
                {
                    UI.TreeView.EnsureVisible(node);
                    UI.TreeView.SelectedNode = node;
                    UI.TreeView.Focus();
                }
                else
                {
                    UI.TreeView.SelectedNode = null;
                    ExpressionEditor_Preview(nav);
                    PropertyGrid_UpdateFromObject(nav);
                    UI.ExpressionEditor.SelectAll();
                    UI.ExpressionEditor.Focus();
                }
                break;
            }
            IsNavigatingBack = false;
        }

        private void Tree_Init()
        {
            FilterMode = (FilterMode)Preferences.Current.View_SearchResults;

            // Set up custom node controls:
            UI.TreeView.NodeControls.Insert(0, new TreeViewAdvExtension.NodeArrow { ParentColumn = UI.TreeView.Columns[0] });
            UI.TreeView.NodeControls.Insert(1, new TabularIcon { Images = FormMain.Singleton.tabularTreeImages.Images, ParentColumn = UI.TreeView.Columns[0] });
            TreeView_NameCol = new TabularNodeTextBox(this) { DataPropertyName = "LocalName", IncrementalSearchEnabled = true, ParentColumn = UI.TreeView.Columns[0], Trimming = StringTrimming.EllipsisCharacter, EditEnabled = true, UseCompatibleTextRendering = true };
            UI.TreeView.NodeControls.Insert(2, TreeView_NameCol);

            TreeView_NameCol.ChangesApplied += TvName_ChangesApplied;

            // Hook up events:
            UI.TreeView.SelectionChanged += TreeView_SelectionChanged;
            UI.TreeView.DragDrop += TreeView_DragDrop;
            UI.TreeView.ItemDrag += TreeView_ItemDrag;
            UI.TreeView.DragOver += TreeView_DragOver;
            UI.TreeView.DragEnter += TreeView_DragEnter;
            UI.TreeView.Expanded += TreeView_Expanded;
            UI.TreeView.Leave += TreeView_Leave;
            UI.TreeView.NodeMouseDoubleClick += TreeView_NodeMouseDoubleClick;
            UI.TreeView.KeyDown += TreeView_KeyDown;
            UI.TreeView.BeforeMultiSelect += TreeView_BeforeMultiSelect;
            TreeView_NameCol.EditorShowing += TvName_EditorShowing;

            UI.TreeView.DragLeave += TreeView_DragLeave;

            // Set up context menu:
            var menu = new ContextMenuStrip();
            menu.Opening += ContextMenu_Opening;
            UI.TreeView.ContextMenuStrip = menu;
            UI.ToolsMenu.DropDown.Opening += ToolsMenu_Opening;
            UI.ModelMenu.DropDown.Opening += ModelMenu_Opening;
            UI.DynamicMenu.DropDown.Opening += DynamicMenu_Opening;
        }

        private void TreeView_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            if(e.Node.Tag is CalculationGroupTable)
            {
                e.Node.ExpandAll();
            }
        }

        private void TvName_EditorShowing(object sender, CancelEventArgs e)
        {
            if(UI.TreeView.SelectedNode != null && UI.TreeView.SelectedNode.Tag is ITabularNamedObject tno)
            {
                e.Cancel = !tno.CanEditName();
            }
        }

        private void TreeView_BeforeMultiSelect(object sender, TreeViewAdvCancelEventArgs e)
        {
            if (e.Node.Tag is LogicalGroup) e.Cancel = true;
        }

        TreeNodeAdv[] draggedNodes = null;

        private void TreeView_DragLeave(object sender, EventArgs e)
        {
            if (Handler == null) return;

            if (draggedNodes != null && Tree_CurrentDragObject != null && Tree_CurrentDragObject.GetDataPresent(typeof(TreeNodeAdv[])))
            {
                draggedNodes = Tree_CurrentDragObject.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
                Tree_CurrentDragObject.SetData(typeof(TreeNodeAdv[]), null);
            }
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (Handler == null) return;

            if (draggedNodes != null)
            {
                e.Data.SetData("Aga.Controls.Tree.TreeNodeAdv[]", draggedNodes);
            } else if (e.Data.GetDataPresent("Aga.Controls.Tree.TreeNodeAdv[]"))
            {
                // Disabled below with build v. 2.6 as drag/drop between instances is buggy. Use CTRL+C / CTRL+V instead.

                // If the draggedNodes array is null, but the data object is supposed to contain tree nodes,
                // it means that nodes are being dragged from another instance of Tabular Editor. In this case,
                // we cannot deserialize the nodes on the data object, but will try to reconstruct them from
                // the serialized string value:
                //if (e.Data.GetDataPresent("Text"))
                //{
                //    var objects = Serializer.DeserializeObjects(e.Data.GetData("Text") as string);
                //    draggedNodes = objects.Select(obj => new TreeNodeAdv(obj)).ToArray();
                //}
            }
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Handler == null) return;

            if (e.KeyCode == Keys.Enter)
            {
                var node = UI.TreeView.SelectedNode;
                if (node == null || UI.TreeView.CurrentEditor != null) return;

                if (node.Tag is IExpressionObject) ExpressionEditor_Edit(node.Tag as IExpressionObject);
                else if (!node.IsLeaf) if (node.IsExpanded) node.Collapse(); else node.Expand();
                e.Handled = true;
            }
            if(e.KeyCode == Keys.F && e.Modifiers.HasFlag(Keys.Control))
            {
                UI.FormMain.FocusFilter();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Delete && Actions.Delete.Enabled(null))
            {
                Actions.Delete.Execute(null);
                e.Handled = true;
            }
            if (e.Modifiers.HasFlag(Keys.Alt) && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
            {
                e.Handled = true; return;
            }
        }

        /// <summary>
        /// This method toggles Explorer Tree options in a way suitable to show the given item.
        /// For example, of a filter is currently specified, but the given item does not fulfill
        /// the filter criteria, Filtering will be toggled off in the UI.
        /// </summary>
        /// <param name="item"></param>
        public TreeNodeAdv EnsureNodeVisible(ITabularNamedObject item)
        {
            var node = UI.TreeView.FindNode(TreeModel.GetPath(item));
            if (node != null) return node;

            LogicalTreeOptions optionsToApply = 0;

            if((item as IHideableObject)?.IsHidden ?? false && !TreeModel.Options.HasFlag(LogicalTreeOptions.ShowHidden))
            {
                // If the object is hidden, make sure the tree is set up to show hidden objects:
                optionsToApply |= LogicalTreeOptions.ShowHidden;
                UI.FormMain.actToggleHidden.Checked = true;
            }

            switch(item.ObjectType)
            {
                case ObjectType.Table:
                    // Tables will always be visible
                case ObjectType.Column:
                case ObjectType.Measure:
                case ObjectType.Hierarchy:
                case ObjectType.Level:
                case ObjectType.KPI:
                default:
                    break;
            }

            return null;
        }

        public void EditName(ITabularNamedObject item)
        {
            var node = EnsureNodeVisible(item);

            if (node != null)
            {
                UI.TreeView.SelectedNode = node;
                TreeView_NameCol.BeginEdit();
            }
        }

        public void ExpandItem(ITabularNamedObject item)
        {
            var treePath = TreeModel.GetPath(item);
            var node = UI.TreeView.FindNode(treePath);
            //var node = UI.TreeView.FindNodeByTag(item);
            if (node != null) node.Expand();
        }

        public void SetInfoColumns(bool showInfoColumns)
        {
            if (UI.TreeView.UseColumns == showInfoColumns) return;

            for (int i = 1; i < UI.TreeView.Columns.Count; i++)
            {
                UI.TreeView.Columns[i].IsVisible = showInfoColumns && 
                    (UI.TreeView.Columns[i] != UI.FormMain._colTable || FilterMode == FilterMode.Flat);
            }

            UI.TreeView.UseColumns = showInfoColumns;
            UI.TreeView.FullRowSelect = showInfoColumns;
        }

        #region TreeView events
        private void TreeView_SelectionChanged(object sender, EventArgs e)
        {
            SelectionInvalid = true;
            PropertyGrid_UpdateFromSelection();

            var obj = UI.TreeView.SelectedNode?.Tag as IExpressionObject;

            // Tables have "Default Detail Rows Expressions", but only for CompatibilityLevel 1400 or newer.
            if (obj is Table && !(obj is CalculatedTable) && Handler.CompatibilityLevel < 1400) obj = null;

            // CalculationGroupTables derive from Tables, but should never enable the expression editor:
            if (obj is CalculationGroupTable) obj = null; 

            ExpressionEditor_Preview(obj);
            ScriptEditor_HideErrors();

            if (UI.TreeView.SelectedNode != null || ShowSelectionStatus)
            {
                UI.StatusLabel.Text = Selection.Summary(true) + " selected.";
                ShowSelectionStatus = true;
            }

            DynamicMenu_Update();
        }

        bool ShowSelectionStatus = false;

        #endregion
        
        private void TreeView_Leave(object sender, EventArgs e)
        {
            if (UI.TreeView.CurrentEditor != null)
            {
                UI.TreeView.HideEditor(false);
                UI.TreeView.Refresh();
            }
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            // Tables shouldn't be editable in the Expression Editor in CL 1200 or less (as they don't have Detail Rows Expressions).
            if (e.Node.Tag is Table && Handler.CompatibilityLevel < 1400) return;

            if (e.Node.Tag is IExpressionObject)
                ExpressionEditor_Edit(e.Node.Tag as IExpressionObject);
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            Tree_CurrentDragObject = new DataObject();

            draggedNodes = UI.TreeView.SelectedNodes.Contains(UI.TreeView.CurrentNode) ? UI.TreeView.SelectedNodes.ToArray() : new[] { UI.TreeView.CurrentNode };

            var scriptableObjects = new HashSet<ObjectType>() { ObjectType.Table, ObjectType.Role, ObjectType.DataSource, ObjectType.Partition };

            // Only generate TMSL script when dragging a single object:
            if (draggedNodes.Length == 1 && UI.TreeView.CurrentNode.Tag is Model) Tree_CurrentDragObject.SetData(Scripter.ScriptCreateOrReplace());
            else if (draggedNodes.Length == 1 && UI.TreeView.CurrentNode.Tag is LogicalGroup)
            {
                Tree_CurrentDragObject.SetData(Serializer.SerializeObjects(UI.TreeView.CurrentNode.Children.Select(n => n.Tag).OfType<TabularObject>()));
            }
            else if (draggedNodes.Length == 1 && UI.TreeView.CurrentNode.Tag is TabularNamedObject && scriptableObjects.Contains((UI.TreeView.CurrentNode.Tag as TabularNamedObject).ObjectType))
                Tree_CurrentDragObject.SetData(Scripter.ScriptCreateOrReplace(UI.TreeView.CurrentNode.Tag as TabularNamedObject));
            else Tree_CurrentDragObject.SetData(Serializer.SerializeObjects(Selection.OfType<TabularObject>()));

            Tree_CurrentDragObject.SetData(draggedNodes);
            UI.TreeView.DoDragDrop(Tree_CurrentDragObject, DragDropEffects.Move | DragDropEffects.Copy);
            draggedNodes = null;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (Handler == null) return;

            e.Effect = DragDropEffects.None;

            if (draggedNodes != null && UI.TreeView.DropPosition.Node != null)
            {
                TreeNodeAdv dropTarget = UI.TreeView.DropPosition.Node;

                if (TreeModel.CanDrop(draggedNodes, dropTarget, UI.TreeView.DropPosition.Position)) e.Effect = DragDropEffects.Move;
            }
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (Handler == null) return;

            if (draggedNodes != null && UI.TreeView.DropPosition.Node != null)
            {
                // TODO: Handle this through actions
                TreeNodeAdv dropTarget = UI.TreeView.DropPosition.Node;
                if (!dropTarget.IsLeaf) dropTarget.Expand();
                var draggedObjects = draggedNodes.Select(n => n.Tag).OfType<ITabularObject>().ToList();
                TreeModel.DoDrop(draggedNodes, dropTarget, UI.TreeView.DropPosition.Position);

                /*if (newNodes.Length > 0)
                {
                    UI.TreeView.SuspendSelectionEvent = true;
                    UI.TreeView.ClearSelectionInternal();
                    UI.TreeView.Selection.AddRange(newNodes);
                    UI.TreeView.SuspendSelectionEvent = false;
                }*/
            }

            Tree_CurrentDragObject = null;
            Tree_DragBackup = null;
            draggedNodes = null;
        }

        private void TvName_ChangesApplied(object sender, EventArgs e)
        {
            UI.PropertyGrid.Refresh();
        }

        private LogicalTreeOptions CurrentOptions = LogicalTreeOptions.Default;

        /*public void ApplyFilter(string filter)
        {
            UI.FormMain.txtFilter.Text = filter;
            UI.FormMain.actToggleFilter.Checked = !string.IsNullOrEmpty(filter);
            InternalApplyFilter(filter);
        }


        private string CurrentFilter;*/

        public void SetFilterMode(FilterMode mode)
        {
            if (TreeModel == null) return;
            FilterMode = mode;
            if (FilterEnabled)
            {
                TreeModel.FilterMode = mode;
                TreeModel.OnStructureChanged();

                UI.FormMain._colTable.IsVisible = TreeModel.FilterMode == FilterMode.Flat && UI.TreeView.UseColumns;
            } else
            {
                UI.FormMain._colTable.IsVisible = false;
            }
        }

        public FilterMode FilterMode = FilterMode.Parent;
        public bool FilterEnabled;

        private void InternalApplyFilter(string filter)
        {
            FilterEnabled = !string.IsNullOrEmpty(filter);
            if (UI.FormMain._colTable.IsVisible) UI.FormMain._colTable.IsVisible = false;
            TreeModel.Filter = filter;
            TreeModel.FilterMode = FilterMode;
            TreeModel.OnStructureChanged();
            
            /*
            // Regular name filtering:
            if (string.IsNullOrEmpty(filter) || !filter.StartsWith(":"))
            {
                TreeModel.Filter = filter;
                TreeModel.FilterMode = FilterMode.Parent;
            }

            // LINQ filter (filter string starts with ":"):
            if (!string.IsNullOrEmpty(filter) && filter.StartsWith(":"))
            {
                EnableLinqMode(filter.Substring(1));
            }
            // LINQ filter removed (filter string empty or no longer starts with ":"):
            else if ((string.IsNullOrEmpty(filter) || !filter.StartsWith(":")) && LinqMode)
            {
                DisableLinqMode();
            }

            CurrentFilter = filter;*/
        }

        public void SetDisplayOptions(bool showHidden, bool showDisplayFolders, bool showColumns, 
            bool showMeasures, bool showHierarchies, bool showPartitions, bool showAllObjectTypes, bool orderByName, string filter = null)
        {
            CurrentOptions = 
                (showHidden ? LogicalTreeOptions.ShowHidden : 0) |
                (showDisplayFolders ? LogicalTreeOptions.DisplayFolders : 0) |
                (showColumns ? LogicalTreeOptions.Columns : 0) |
                (showMeasures ? LogicalTreeOptions.Measures : 0) |
                (showHierarchies ? LogicalTreeOptions.Hierarchies : 0) |
                (showPartitions ? LogicalTreeOptions.Partitions : 0) |
                (showAllObjectTypes ? LogicalTreeOptions.AllObjectTypes : 0) |
                (orderByName ? LogicalTreeOptions.OrderByName : 0) |
                LogicalTreeOptions.ShowRoot;

            if (TreeModel == null) return;

            /*var cmp = (UI.TreeView.Model as SortedTreeModel)?.Comparer as TabularObjectComparer;
            if(cmp != null)
            {
                cmp.Order = alphabeticalSort ? ObjectOrder.Alphabetical : ObjectOrder.Metadata;
            }*/

            TreeModel.Options = CurrentOptions;

            InternalApplyFilter(filter);
        }

        /*private void EnableLinqMode(string filter)
        {
            if (!LinqMode)
            {
                LinqMode = true;
                var useInfoColumns = UI.TreeView.UseColumns;
                UI.FormMain._colTable.IsVisible = true;
                SetInfoColumns(true);
            }
            TreeModel.LinqFilter = filter;
            UI.StatusLabel.Text = string.Format("Found {0} object{1} in {2} ms", 
                TreeModel.FilterResultCount,
                TreeModel.FilterResultCount == 1 ? "" : "s", 
                TreeModel.FilterExecutionTime);
        }

        private void DisableLinqMode()
        {
            if (!LinqMode) return;

            LinqMode = false;

            UI.FormMain._colTable.IsVisible = false;
            SetInfoColumns(Preferences.Current.View_MetadataInformation);

            TreeModel.LinqFilter = null;
            if(UI.TreeView.Root.Children.Count > 0)
                UI.TreeView.Root.Children[0].Expand(); // TODO: Expand same as before
        }

        private bool LinqMode = false;*/
    }
}

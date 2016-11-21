using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AnalysisServices.Tabular.Helper;
using TabularEditor.TOMWrapper;
using Aga.Controls.Tree;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace TabularEditor.UI
{
    /// <summary>
    /// Used to communicate UI state and actions between the UI and the underlying classes.
    /// Contains properties that can be bound to UI elements to automatically apply state
    /// changes.
    /// </summary>
    public class UI_Deprecated
    {
        #region Old code (DIE!!!)

        internal TabularTree LogicalTree;
        public static TabularModelHandler Handler;
        internal TreeViewAdv TreeView;
        public static UITreeSelection Selected = new UITreeSelection(Enumerable.Empty<TreeNodeAdv>());



        public UI_Deprecated(TreeViewAdv treeView, TabularModelHandler handler, TabularTree logicalTree)
        {
            LogicalTree = logicalTree;
            Handler = handler;
            TreeView = treeView;
        }

        public void UIRemoveFolders()
        {
            Handler.BeginUpdate("remove folders");
            foreach (var f in Selected.FolderObjects) f.SetFolderName("");
            Handler.EndUpdate();
        }

        /// <summary>
        /// Rebuilds the tree with a range of settings
        /// </summary>
        public void UIRebuildTree(bool showHidden, bool showDisplayFolders, bool showColumns, bool showMeasures, bool showHierarchies)
        {
            LogicalTree.Options =
                      (showHidden ? LogicalTreeOptions.ShowHidden : 0) |
                      (showDisplayFolders ? LogicalTreeOptions.DisplayFolders : 0) |
                      (showColumns ? LogicalTreeOptions.Columns : 0) |
                      (showMeasures ? LogicalTreeOptions.Measures : 0) |
                      (showHierarchies ? LogicalTreeOptions.Hierarchies : 0) |
                      LogicalTreeOptions.ShowRoot;
        }

        public void UIRebuildTree()
        {
            LogicalTree.OnStructureChanged();
        }

        /// <summary>
        /// Rebuilds the tree using the specified filter.
        /// </summary>
        /// <param name="filter"></param>
        public void UIRebuildTree(string filter)
        {
            LogicalTree.Filter = filter;
        }

        public void UIDuplicateObjects(Func<string, string> renameFunc, bool includeTranslations)
        {
            Handler.BeginUpdate("duplicate");
            foreach (var c in Selected) c.Clone(renameFunc(c.Name), includeTranslations);
            Handler.EndUpdate();
        }
        #endregion
    }
}

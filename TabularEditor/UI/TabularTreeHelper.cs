using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Tree
{
    public static class TabularTreeHelper
    {


        public static HashSet<ITabularObjectContainer> GetExpandedNodes(this TreeViewAdv treeView)
        {
            return new HashSet<ITabularObjectContainer>(treeView.AllNodes.Where(n => n.IsExpanded)
                .Select(n => n.Tag as ITabularObjectContainer));
        }

        public static void ExpandNodes(this TreeViewAdv treeView, HashSet<ITabularObjectContainer> nodeObjects)
        {
            treeView.BeginUpdate();
            treeView.AllNodes
                .Where(n => nodeObjects.Contains(n.Tag))
                .ToList().ForEach(n => n.Expand());

            // Expand folders by paths (necessary when the folder structure has changed):
            treeView.AllNodes.Where(n => n.Tag is Folder &&
                nodeObjects.Any(o => o is Folder
                && (o as Folder).FullPath == (n.Tag as Folder).FullPath)).ToList().ForEach(n => n.Expand());

            treeView.EndUpdate();
        }
    }
}

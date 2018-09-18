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
            foreach(var node in treeView.AllNodes)
            {
                var folder = node.Tag as Folder;
                if(folder != null)
                {
                    if (nodeObjects.OfType<Folder>().Any(f => f.Table == folder.Table && f.Path == folder.Path)) node.Expand();
                }
            }
        }
    }
}

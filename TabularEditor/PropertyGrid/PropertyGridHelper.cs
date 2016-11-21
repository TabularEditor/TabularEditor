using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor
{
    public static class PropertyGridHelper
    {
        /// <summary>
        /// Returns a list of items that are currently expanded in the grid.
        /// Call the "ExpandItemsByLabel" with the same list as an argument,
        /// to attempt to expand the same items, after the grid has been
        /// loaded with a new object.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetExpandedItemLabels(this System.Windows.Forms.PropertyGrid grid)
        {
            var result = new List<string>();
            GridItem root = grid.SelectedGridItem;
            
            //Get the parent
            while (root?.Parent != null)
                root = root.Parent;

            if (root != null)
            {
                GetExpandedLabelsRecursive(root, result);
            }

            return result;
        }

        private static void GetExpandedLabelsRecursive(GridItem item, IList<string> expandedLabels)
        {
            if(item.Expanded)
            {
                expandedLabels.Add(item.Label);
                foreach (GridItem g in item.GridItems)
                {
                    GetExpandedLabelsRecursive(g, expandedLabels);
                }
            }
        }

        /// <summary>
        /// Call this method to expand all items in the grid, whose label appears
        /// in <paramref name="expandedItems"/>.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="expandedItems"></param>
        public static void ExpandItemsByLabel(this System.Windows.Forms.PropertyGrid grid, IEnumerable<string> expandedItems)
        {
            var itemList = new HashSet<string>(expandedItems);
            GridItem root = grid.SelectedGridItem;
            //Get the parent
            while (root?.Parent != null)
                root = root.Parent;

            if (root != null)
            {
                ExpandLabelsRecursive(root, itemList);
            }
        }

        private static void ExpandLabelsRecursive(GridItem item, HashSet<string> expandLabels)
        {
            if(expandLabels.Contains(item.Label) || item.Parent == null)
            {
                item.Expanded = true;
                foreach(GridItem g in item.GridItems)
                {
                    ExpandLabelsRecursive(g, expandLabels);
                }
            }
        }

        public static IEnumerable<GridItem> EnumerateAllItems(this PropertyGrid grid)
        {
            if (grid == null) throw new ArgumentException();

            // get to root item
            GridItem start = grid.SelectedGridItem;
            while (start.Parent != null)
            {
                start = start.Parent;
            }

            return start.EnumerateAllItems();
        }

        public static IEnumerable<GridItem> EnumerateAllItems(this GridItem item)
        {
            if (item == null) throw new ArgumentException();

            yield return item;
            if (item.Expanded)
            {
                foreach (GridItem child in item.GridItems)
                {
                    foreach (var gc in child.EnumerateAllItems()) yield return gc;
                }
            }
        }


    }
}

using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TreeViewAdvExtension
{
    public static class TreeNodeAdvHelper
    {
        public static bool HasAncestor(this TreeNodeAdv node, TreeNodeAdv ancestor)
        {
            if (node.Parent == null) return false;
            if (node.Parent == ancestor) return true;
            return node.Parent.HasAncestor(ancestor);
        }
    }
}

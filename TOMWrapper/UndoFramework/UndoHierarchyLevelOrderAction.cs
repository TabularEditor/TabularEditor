using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoHierarchyLevelOrderAction : IUndoAction
    {
        IList<Level> before;
        IList<Level> after;
        Hierarchy hierarchy;

        public string ActionName { get { return "level reorder"; } }

        public UndoHierarchyLevelOrderAction(Hierarchy hierarchy, IList<Level> before, IList<Level> after)
        {
            this.hierarchy = hierarchy;
            this.before = before;
            this.after = after;
        }

        public void Undo()
        {
            hierarchy.SetLevelOrder(before);
        }

        public void Redo()
        {
            hierarchy.SetLevelOrder(after);
        }

        public string GetSummary()
        {
            return string.Format("Changed {{{0}}} level order: {1} -> {2}", hierarchy.GetObjectPath(), 
                string.Join(",", before.Select(l => "[" + l.Name + "]").ToArray()),
                string.Join(",", after.Select(l => "[" + l.Name + "]").ToArray()));
        }

        public string GetCode()
        {
            return "// " + GetSummary();
        }
    }
}

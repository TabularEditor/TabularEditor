using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoCalculationItemsOrderAction : IUndoAction
    {
        IList<CalculationItem> before;
        IList<CalculationItem> after;
        CalculationGroupTable calculationGroup;

        public string ActionName { get { return "level reorder"; } }

        public UndoCalculationItemsOrderAction(CalculationGroupTable calculationGroup, IList<CalculationItem> before, IList<CalculationItem> after)
        {
            this.calculationGroup = calculationGroup;
            this.before = before;
            this.after = after;
        }

        public void Undo()
        {
            calculationGroup.SetLevelOrder(before);
        }

        public void Redo()
        {
            calculationGroup.SetLevelOrder(after);
        }

        public string GetSummary()
        {
            return string.Format("Changed {{{0}}} level order: {1} -> {2}", calculationGroup.GetObjectPath(), 
                string.Join(",", before.Select(l => "[" + l.Name + "]").ToArray()),
                string.Join(",", after.Select(l => "[" + l.Name + "]").ToArray()));
        }

        public string GetCode()
        {
            return "// " + GetSummary();
        }
    }
}

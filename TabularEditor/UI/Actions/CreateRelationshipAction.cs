using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Actions;

namespace TabularEditor.UI.Actions
{
    public enum CreateRelationshipDirection
    {
        To,
        From
    }

    public class CreateRelationshipAction : IModelMultiAction
    {
        UIController UI;
        CreateRelationshipDirection Direction;
        public CreateRelationshipAction(CreateRelationshipDirection direction)
        {
            Direction = direction;
            UI = UIController.Current;
        }

        private Column SelectedColumn { get { return UI.Selection.Columns.FirstOrDefault(); } }

        public IDictionary ArgNames
        {
            get
            {
                var col = SelectedColumn;
                if (col == null) return new Dictionary<string, object>();

                var candidateCols = UI.Handler.Model.Tables
                    .Where(t => t != col.Table) // All tables except parent table of current column
                    .Where(t => !col.Table.RelatedTables.Contains(t)) // Tables that are not already related
                    .SelectMany(t => t.Columns)     // All columns of those tables
                    .Where(c => c.DataType == col.DataType); // Matching data types

                var result = new OrderedDictionary();

                var matchingNames = candidateCols.Where(c =>
                    c.Name.Replace(" ", "").EndsWith(col.Name.Replace(" ", "")) ||
                    col.Name.Replace(" ", "").EndsWith(c.Name.Replace(" ", ""))).OrderBy(c => c.Table.Name);

                foreach (var m in matchingNames) result.Add(m.Table.Name.ConcatPath(m.Name), m);
                foreach (var m in matchingNames.Select(m => m.Table).Distinct()) result.Add(m.Name.ConcatPath("---"), null);
                foreach (var m in candidateCols.Except(matchingNames).OrderBy(c => c.Table.Name + c.Name)) result.Add(m.Table.Name.ConcatPath(m.Name), m);

                return result;
            }
        }

        public bool HideWhenDisabled
        {
            get
            {
                return true;
            }
        }

        public string Path
        {
            get
            {
                return @"Create New\Relationship " + (Direction == CreateRelationshipDirection.To ? "To" : "From");
            }
        }

        public string ToolTip
        {
            get
            {
                if(Direction == CreateRelationshipDirection.To)
                    return "Create a new relationship from the currently selected column to the chosen column";
                else
                    return "Create a new relationship to the currently selected column from the chosen column";
            }
        }

        public Context ValidContexts
        {
            get
            {
                return Context.TableObject;
            }
        }

        public bool Enabled(object arg)
        {
            return UI.Selection.Types.HasFlag(Types.Column) && UI.Selection.Count == 1;
        }

        public void Execute(object arg)
        {
            if (!(arg is Column)) return;
            var rel = UI.Handler.Model.AddRelationship();

            if (Direction == CreateRelationshipDirection.To)
            {
                rel.FromColumn = SelectedColumn;
                rel.ToColumn = arg as Column;
            }
            else
            {
                rel.ToColumn = SelectedColumn;
                rel.FromColumn = arg as Column;
            }
            rel.Edit();
        }
    }
}

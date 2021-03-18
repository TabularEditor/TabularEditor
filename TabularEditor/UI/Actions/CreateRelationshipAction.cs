using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.PowerBI;
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
                var compactName = col.Name.Replace(" ", "");
                if (col == null) return new Dictionary<string, object>();
                var relatedTables = new HashSet<Table>(col.Table.RelatedTables);

                var candidateCols = col.Model.Tables
                    .Where(t => t != col.Table) // All tables except parent table of current column
                    .Where(t => !relatedTables.Contains(t)) // Tables that are not already related
                    .SelectMany(t => t.Columns)     // All columns of those tables
                    .Where(c => c.DataType == col.DataType) // Matching data types
                    .Select(c =>
                        {
                            var n = c.Name.Replace(" ", "");
                            return new
                            {
                                column = c,
                                nameMatch = n.EndsWith(compactName) || compactName.EndsWith(n)
                            };
                        }
                    )
                    .ToList();      // Materialise

                var result = new OrderedDictionary();

                Table lastTable = null;
                bool lastMatch = false;
                // Populate the dictionary ordered by table names, columns with partial name match, column name:
                foreach (var c in candidateCols.OrderBy(c => c.column.Table.Name).ThenBy(c => !c.nameMatch).ThenBy(c => c.column.Name))
                {
                    // This adds a seperator between the columns that have a partial name match:
                    if (lastTable == c.column.Table && lastMatch && !c.nameMatch)
                        result.Add(c.column.Table.Name.ConcatPath("---"), null);

                    result.Add(c.column.Table.Name.ConcatPath(c.column.Name), c.column);

                    lastTable = c.column.Table;
                    lastMatch = c.nameMatch;
                }

                // Add tables that already have a relationship:
                if (relatedTables.Count > 0)
                {
                    candidateCols = col.Model.Tables
                       .Where(t => t != col.Table) // All tables except parent table of current column
                       .Where(t => relatedTables.Contains(t)) // Tables that are already related
                       .SelectMany(t => t.Columns)     // All columns of those tables
                       .Where(c => c.DataType == col.DataType && !col.Model.Relationships.Any(r => (r.FromColumn == col && r.ToColumn == c) || (r.FromColumn == c && r.ToColumn == col))) // Matching data types
                       .Select(c =>
                       {
                           var n = c.Name.Replace(" ", "");
                           return new
                           {
                               column = c,
                               nameMatch = n.EndsWith(compactName) || compactName.EndsWith(n)
                           };
                       }
                       )
                       .ToList();      // Materialise

                    if (candidateCols.Count > 0)
                    {
                        result.Add("---", null);

                        lastTable = null;
                        lastMatch = false;
                        // Populate the dictionary ordered by table names, columns with partial name match, column name:
                        foreach (var c in candidateCols.OrderBy(c => c.column.Table.Name).ThenBy(c => !c.nameMatch).ThenBy(c => c.column.Name))
                        {
                            // This adds a seperator between the columns that have a partial name match:
                            if (lastTable == c.column.Table && lastMatch && !c.nameMatch)
                                result.Add(c.column.Table.Name.ConcatPath("---"), null);

                            result.Add(c.column.Table.Name.ConcatPath(c.column.Name), c.column);

                            lastTable = c.column.Table;
                            lastMatch = c.nameMatch;
                        }
                    }
                }

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
            return UIController.Current.Handler.PowerBIGovernance.AllowCreate(typeof(SingleColumnRelationship)) && UI.Selection.Context == Context.Column && UI.Selection.Count == 1;
        }

        public void Execute(object arg)
        {
            if (!(arg is Column)) return;
            UI.Handler.BeginUpdate("Create relationship");
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
            UI.Handler.EndUpdate();
            UI.EditName(rel);
        }
    }
}

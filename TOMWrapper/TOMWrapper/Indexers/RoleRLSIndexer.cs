using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// The RoleRLSIndexer is used to browse all filters across all tables in the model, for
    /// one specific role. This is in contrast to the TableRLSIndexer, which browses the
    /// filters across all roles in the model, for one specific table.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public sealed class RoleRLSIndexer : GenericIndexer<Table, string>
    {
        public readonly ModelRole Role;

        protected override bool IsEmptyValue(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }


        internal Dictionary<Table, RLSFilterExpression> _filterExpressions = new Dictionary<Table, RLSFilterExpression>();
        public IReadOnlyDictionary<Table, RLSFilterExpression> FilterExpressions => _filterExpressions;

        internal RoleRLSIndexer(ModelRole role) : base(role)
        {
            Role = role;
        }

        protected override bool EnableMultiLine => true;

        protected override TabularObjectCollection<Table> GetCollection()
        {
            return Model.Tables;
        }

        protected override string GetValue(Table table)
        {
            return Role.MetadataObject.TablePermissions.Find(table.Name)?.FilterExpression;
        }

        protected override void SetValue(Table table, string filterExpression)
        {
            var tps = Role.MetadataObject.TablePermissions;
            var tp = tps.Find(table.Name);

            // Filter expression removed:
            if (string.IsNullOrWhiteSpace(filterExpression)) {

                // Don't do anything if there is no TablePermission for this table anyway:
                if (tp == null) return;

                // Otherwise, remove the TablePermission:
                tps.Remove(tp);
                Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "RowLevelSecurity", tp.FilterExpression, null, table.Name));

                RLSFilterExpression rls;
                if (FilterExpressions.TryGetValue(table, out rls))
                {
                    FormulaFixup.ClearDependsOn(rls);
                    _filterExpressions.Remove(table);
                }
            }
            else // Filter expression assigned:
            {
                // Create a new TablePermission if we don't already have one for this table:
                if (tp == null)
                {
                    tp = new TOM.TablePermission() { Table = table.MetadataObject };
                    tps.Add(tp);
                }

                // Assign the new expression to the TablePermission:
                var oldValue = tp.FilterExpression;
                tp.FilterExpression = filterExpression;

                Role.Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "RowLevelSecurity", oldValue, filterExpression, table.Name));

                var rls = RLSFilterExpression.Get(Role, table);
                FormulaFixup.BuildDependencyTree(rls);
            }
        }
    }
}

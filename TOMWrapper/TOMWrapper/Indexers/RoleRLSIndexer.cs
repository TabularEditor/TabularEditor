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

        private IEnumerable<Table> NonCalculationGroupTables => Model.Tables.Where(t => !(t is CalculationGroupTable));

        public override IEnumerable<string> Keys { get { return NonCalculationGroupTables.Select(t => t.Name); } }

        public override string Summary
        {
            get
            {
                int tableCount = NonCalculationGroupTables.Count();
                return string.Format("RLS enabled on {0} out of {1} table{2}",
                    this.Count(v => !string.IsNullOrWhiteSpace(v)),
                    NonCalculationGroupTables.Count(),
                    tableCount == 1 ? "" : "s");
            }
        }
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
            var tp = Role.TablePermissions.FindByName(table.Name);

            // Filter expression removed:
            if (string.IsNullOrWhiteSpace(filterExpression)) {

                // Don't do anything if there is no TablePermission for this table anyway:
                if (tp == null) return;

                // Otherwise, remove the filter expression:
                if (Handler.CompatibilityLevel >= 1400 && (tp.MetadataObject.MetadataPermission != TOM.MetadataPermission.Default ||
                    tp.MetadataObject.ColumnPermissions.Any(cp => cp.MetadataPermission != TOM.MetadataPermission.Default)))
                    tp.FilterExpression = string.Empty;
                else
                    tp.Delete();
            }
            else // Filter expression assigned:
            {
                // Create a new TablePermission if we don't already have one for this table:
                if (tp == null) tp = TablePermission.CreateFromMetadata(Role, new TOM.TablePermission { Table = table.MetadataObject });

                tp.FilterExpression = filterExpression;
            }
        }
    }
}

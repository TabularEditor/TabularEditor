using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{

    /// <summary>
    /// The RolePermissionIndexer is used to browse all metadata permissions across all
    /// tables in the model, for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class RoleOLSIndexer: GenericIndexer<Table, MetadataPermission>
    {
        public readonly ModelRole Role;

        internal RoleOLSIndexer(ModelRole role) : base(role)
        {
            Role = role;
        }

        private IEnumerable<Table> NonCalculatedGroupTables => Model.Tables.Where(t => !(t is CalculationGroupTable));

        public override IEnumerable<string> Keys { get { return NonCalculatedGroupTables.Select(t => t.Name); } }

        public override string Summary
        {
            get
            {
                int tableCount = NonCalculatedGroupTables.Count();
                return string.Format("OLS enabled on {0} out of {1} table{2}",
                    this.Count(v => v != MetadataPermission.Default),
                    NonCalculatedGroupTables.Count(),
                    tableCount == 1 ? "" : "s");

            }
        }

    protected override TabularObjectCollection<Table> GetCollection()
        {
            return Model.Tables;
        }

        protected override MetadataPermission GetValue(Table table)
        {
            return (MetadataPermission) ( Role.MetadataObject.TablePermissions.Find(table.Name)?.MetadataPermission ?? TOM.MetadataPermission.Default);
        }

        protected override void SetValue(Table table, MetadataPermission permission)
        {
            var tp = Role.TablePermissions.FindByName(table.Name);
            if (tp == null && permission == MetadataPermission.Default) return;

            if (tp == null) tp = TablePermission.CreateFromMetadata(Role, new TOM.TablePermission { Table = table.MetadataObject });

            tp.MetadataPermission = permission;
        }
    }
}

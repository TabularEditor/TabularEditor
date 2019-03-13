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
    public class RoleColumnOLSIndexer: GenericIndexer<Column, MetadataPermission>
    {
        public readonly TablePermission TablePermission;

        internal RoleColumnOLSIndexer(TablePermission tps) : base(tps)
        {
            TablePermission = tps;
        }

        private IEnumerable<Column> Columns => TablePermission.Table.Columns;

        public override IEnumerable<string> Keys { get { return Columns.Select(t => t.Name); } }

        public override string Summary
        {
            get
            {
                int columnCount = TablePermission.Table.Columns.Count;
                return string.Format("OLS enabled on {0} out of {1} column{2}",
                    this.Count(v => v != MetadataPermission.Default),
                    columnCount,
                    columnCount == 1 ? "" : "s");

            }
        }

        protected override TabularObjectCollection<Column> GetCollection()
        {
            return TablePermission.Table.Columns;
        }

        protected override MetadataPermission GetValue(Column column)
        {
            return column.ObjectLevelSecurity[TablePermission.Role];
        }

        protected override void SetValue(Column column, MetadataPermission permission)
        {
            column.ObjectLevelSecurity[TablePermission.Role] = permission;
        }
    }
}

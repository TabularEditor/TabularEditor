using System;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// The TableOLSIndexer is used to browse all filters defined on one specific table, across
    /// all roles in the model. This is in contrast to the RoleOLSIndexer, which browses the
    /// filters across all tables for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public sealed class TableOLSIndexer : GenericIndexer<ModelRole, MetadataPermission>
    {
        public readonly Table Table;

        internal TableOLSIndexer(Table table) : base(table)
        {
            if (Handler.CompatibilityLevel < 1400) throw new InvalidOperationException(Messages.CompatibilityError_ObjectLevelSecurity);
            Table = table;
        }

        protected override TabularObjectCollection<ModelRole> GetCollection()
        {
            return Model.Roles;
        }

        protected override MetadataPermission GetValue(ModelRole role)
        {
            // Get the acutal value from the RoleOLSIndexer:
            return role.MetadataPermission[Table];
        }

        protected override void SetValue(ModelRole role, MetadataPermission permission)
        {
            // Let the RoleOLSIndexer handle the actual value assignment:
            role.MetadataPermission[Table] = permission;
        }
    }
}

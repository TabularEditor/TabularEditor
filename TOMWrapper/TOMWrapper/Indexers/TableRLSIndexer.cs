using System.ComponentModel;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// The TableRLSIndexer is used to browse all filters defined on one specific table, across
    /// all roles in the model. This is in contrast to the RoleRLSIndexer, which browses the
    /// filters across all tables for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]  // TODO: Options for adding property grid actions (such as "Clear all", etc., that appear below the grid)
    public sealed class TableRLSIndexer : GenericIndexer<ModelRole, string>
    {
        public readonly Table Table;
        internal TableRLSIndexer(Table table) : base(table)
        {
            Table = table;
        }
        protected override bool EnableMultiLine => true;

        protected override TabularObjectCollection<ModelRole> GetCollection()
        {
            return Model.Roles;
        }

        protected override string GetValue(ModelRole role)
        {
            // Get the actual value from the RoleRLSIndexer:
            return role.RowLevelSecurity[Table];
        }

        protected override void SetValue(ModelRole role, string filterExpression)
        {
            // Let the RoleRLSIndexer handle the actual value assignment:
            role.RowLevelSecurity[Table] = filterExpression;
        }

        protected override bool IsEmptyValue(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}

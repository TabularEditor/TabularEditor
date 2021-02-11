using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Dialogs;

namespace TabularEditor.UIServices
{
    public enum MetadataChangeType
    {
        SourceColumnAdded = 1,
        SourceColumnNotFound = 2,
        DataTypeChange = 3,
        SourceQueryError = 4,
        PartitionInconsistency = 5
    }

    public class MetadataChange
    {
        public Partition Partition;
        public Table ModelTable;
        public string SourceColumn;
        public DataType SourceType;
        public string SourceProviderType;
        public TOMWrapper.DataColumn ModelColumn;
        public MetadataChangeType ChangeType;
        public string SourceQuery;

        public override string ToString()
        {
            switch(ChangeType)
            {
                case MetadataChangeType.DataTypeChange:
                    return $"Column {ModelColumn.DaxObjectFullName} is imported as {ModelColumn.DataType} but the source type should normally map to {SourceType}.";
                case MetadataChangeType.SourceColumnAdded:
                    return $"Column named '{SourceColumn}' exists in the source query for table '{ModelTable.Name}', but is not mapped to any Tabular Model column.";
                case MetadataChangeType.SourceColumnNotFound:
                    return $"Column {ModelColumn.DaxObjectFullName} refers to source column {ModelColumn.SourceColumn} which does not seem to exist in the source query.";
                case MetadataChangeType.SourceQueryError:
                    return $"Unable to retrieve column metadata for table '{ModelTable.Name}'. Check partition query.";
                case MetadataChangeType.PartitionInconsistency:
                    return $"Source query on Partition \"{Partition.Name}\" returns metadata which differs from other partitions on table '{ModelTable.Name}'. This may cause errors during processing.";
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public static class TableMetadata
    {
        class DataTypeMapping
        {
            public readonly DataType MappedType;
            public readonly string ProviderType;

            public DataTypeMapping(string providerType, DataType mappedType)
            {
                this.MappedType = mappedType;
                this.ProviderType = providerType;
            }
        }

        public static List<MetadataChange> CheckPartitionsIdentical(Table table)
        {
            var primary = GetSourceSchema(table.Partitions[0]);
            var result = new List<MetadataChange>();
            for(int i = 1; i < table.Partitions.Count; i++)
            {
                var p = table.Partitions[i];
                var other = GetSourceSchema(p);

                if(other.Count != primary.Count)
                {
                    result.Add(new MetadataChange { ChangeType = MetadataChangeType.PartitionInconsistency, Partition = p, ModelTable = table, SourceQuery = p.Query });
                }
                else
                {
                    foreach(var c in other)
                    {
                        if(primary.TryGetValue(c.Key, out DataTypeMapping mapping))
                        {
                            if(mapping.MappedType != c.Value.MappedType)
                            {
                                result.Add(new MetadataChange { ChangeType = MetadataChangeType.PartitionInconsistency, Partition = p, ModelTable = table, SourceQuery = p.Query });
                                break;
                            }
                        }
                        else
                        {
                            result.Add(new MetadataChange { ChangeType = MetadataChangeType.PartitionInconsistency, Partition = p, ModelTable = table, SourceQuery = p.Query });
                            break;
                        }

                    }
                }
            }
            return result;
        }

        private static Dictionary<string, DataTypeMapping> GetSourceSchema(Partition partition)
        {
            var result = new List<MetadataChange>();

            if (!(partition.DataSource is ProviderDataSource))
            {
                return null;
            }

            var tds = TypedDataSource.GetFromTabularDs(partition.DataSource as ProviderDataSource);
            var query = partition.Query;
            if (partition.CheckFlag(TE_ADDFALSECRITERIA) || partition.Table.CheckFlag(TE_ADDFALSECRITERIA) || 
                (partition.DataSource != null && partition.DataSource.CheckFlag(TE_ADDFALSECRITERIA)))
            {
                query = query + " WHERE 1 = 0";
            }
            var schemaTable = tds.GetSchemaTable(query);
            if (schemaTable == null)
            {
                return null;
            }
            var ignoredColumns = new HashSet<string>();
            var sourceSchema = new Dictionary<string, DataTypeMapping>(StringComparer.InvariantCultureIgnoreCase);
            foreach (DataRow row in schemaTable.Rows)
            {
                var colName = row["ColumnName"].ToString();
                var dataType =
                    schemaTable.Columns.Contains("DataTypeName") ?
                        row["DataTypeName"].ToString() :
                        (row["DataType"] as Type).Name;
                var mappedType = DataTypeMap(dataType);
                if (sourceSchema.ContainsKey(colName))
                {
                    ignoredColumns.Add(colName);
                }
                else
                    sourceSchema.Add(colName, new DataTypeMapping(dataType, mappedType));
            }

            if(ignoredColumns.Count > 0)
            {
                var plural = ignoredColumns.Count > 1 ? "s" : "";
                var columns = string.Join("", ignoredColumns.Select(c => "\r\n\t" + c).ToArray());
                var partitionInfo = partition.Table.Partitions.Count > 1 ? $", partition \"{partition.Name}\"" : "";
                MessageBox.Show($"The query on table '{partition.Table.Name}'{partitionInfo} specified the following column{plural} more than once:\r\n{columns}", "Repeated columns", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return sourceSchema;
        }

        public const string TE_NOWARN = "TabularEditor_SchemaCheckNoWarnings";
        public const string TE_SKIPCHECK_TYPO = "TabularEditor_SkipSkemaCheck"; // For backwards compatibility because of a typo...
        public const string TE_SKIPCHECK = "TabularEditor_SkipSchemaCheck";
        public const string TE_IGNOREADDED = "TabularEditor_IgnoreSourceColumnAdded";
        public const string TE_IGNORETYPES = "TabularEditor_IgnoreDataTypeChange";
        public const string TE_IGNOREREMOVED = "TabularEditor_IgnoreMissingSourceColumn";
        public const string TE_ADDFALSECRITERIA = "TabularEditor_AddFalseWhereClause";

        public static List<MetadataChange> GetChanges(Partition partition)
        {

            var result = new List<MetadataChange>();
            var table = partition.Table;

            if (table.CheckFlag(TE_SKIPCHECK) || table.CheckFlag(TE_SKIPCHECK_TYPO)) return result;

            var sourceSchema = GetSourceSchema(partition);
            if(sourceSchema == null)
            {
                result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceQueryError, SourceQuery = partition.Query });
                return result;
            }
            HashSet<Column> matchedColumns = new HashSet<Column>();
            foreach(var kvp in sourceSchema)
            {
                var colName = kvp.Key;
                var typeMapping = kvp.Value;

                var tCols = table.DataColumns.Where(col => col.SourceColumn.EqualsI(colName) || col.SourceColumn.EqualsI("[" + colName + "]"));
                if (tCols.Count() == 0)
                {
                    if (!table.CheckFlag(TE_IGNOREADDED))
                        result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceColumnAdded, SourceColumn = colName, SourceType = typeMapping.MappedType, SourceProviderType = typeMapping.ProviderType });
                }

                if (!table.CheckFlag(TE_IGNORETYPES))
                {
                    foreach (var tCol in tCols)
                    {
                        matchedColumns.Add(tCol);
                        if (tCol.DataType != typeMapping.MappedType && !tCol.CheckFlag(TE_IGNORETYPES))
                        {
                            result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.DataTypeChange, ModelColumn = tCol, SourceColumn = colName, SourceType = typeMapping.MappedType, SourceProviderType = typeMapping.ProviderType });
                        }
                    }
                }
            }

            if (!table.CheckFlag(TE_IGNOREREMOVED))
            {
                foreach (var col in table.DataColumns.Where(c => !matchedColumns.Contains(c)))
                {
                    if (!col.CheckFlag(TE_IGNOREREMOVED))
                        result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceColumnNotFound, ModelColumn = col });
                }
            }

            return result;
        }

        public static List<MetadataChange> GetChanges(Table table)
        {
            var changes = GetChanges(table.Partitions[0]);

            if (changes.Count == 0 && table.Partitions.Count > 1)
                changes.AddRange(CheckPartitionsIdentical(table));

            return changes;
        }

        public static List<MetadataChange> GetChanges(ProviderDataSource dataSource)
        {
            var result = new List<MetadataChange>();
            var tds = TypedDataSource.GetFromTabularDs(dataSource);
            foreach (var table in dataSource.Model.Tables.Where(t => t.Partitions[0].DataSource == dataSource))
            {
                result.AddRange(GetChanges(table));
            }

            return result;
        }

        public static DataType DataTypeMap(string dataTypeName)
        {
            DataType result;
            var srcType = dataTypeName.ToLower();
            switch (srcType)
            {
                case "binary.type":
                case "binary":
                case "varbinary":
                case "variant":
                case "sqlvariant":
                    result = DataType.Binary; break;

                case "any.type":
                case "text.type":
                case "text":
                case "string":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    result = DataType.String; break;

                case "number.type":
                case "double.type":
                case "single.type":
                case "percentage.type":
                case "duration.type":
                case "number":
                case "real":
                case "float":
                case "single":
                case "double":
                    result = DataType.Double; break;

                case "currency.type":
                case "decimal.type":
                case "currency":
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    result = DataType.Decimal; break;

                case "int64.type":
                case "int32.type":
                case "int16.type":
                case "byte.type":
                case "int":
                case "integer":
                case "whole":
                case "byte":
                case "bigint":
                case "smallint":
                case "tinyint":
                case "int64":
                case "int32":
                case "int16":
                case "long":
                    result = DataType.Int64; break;

                case "datetimezone.type":
                case "datetime.type":
                case "date.type":
                case "time.type":
                case "datetime":
                case "date":
                case "time":
                    result = DataType.DateTime; break;

                case "logical.type":
                case "boolean":
                case "bool":
                case "bit":
                    result = DataType.Boolean; break;

                default:
                    if (srcType.Contains("int"))
                        result = DataType.Int64;
                    else if (srcType.Contains("date"))
                        result = DataType.DateTime;
                    else
                        result = DataType.String;
                    break;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Dialogs;

namespace TabularEditor.UIServices
{
    public enum MetadataChangeType
    {
        SourceColumnAdded = 1,
        SourceColumnNotFound = 2,
        DataTypeChange = 3,
        SourceQueryError = 4
    }

    public class MetadataChange
    {
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
                    return $"Column {ModelColumn.DaxObjectFullName} referes to source column {ModelColumn.SourceColumn} which does not seem to exist in the source query.";
                case MetadataChangeType.SourceQueryError:
                    return $"Unable to retrieve column metadata for table '{ModelTable.Name}'. Check partition query.";
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public static class TableMetadata
    {
        public static List<MetadataChange> GetChanges(ProviderDataSource dataSource)
        {
            var result = new List<MetadataChange>();
            var tds = TypedDataSource.GetFromTabularDs(dataSource);
            foreach (var table in dataSource.Model.Tables.Where(t => t.Partitions[0].DataSource == dataSource))
            {
                var schemaTable = tds.GetSchemaTable(table.Partitions[0].Query);
                if(schemaTable == null)
                {
                    result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceQueryError, SourceQuery = table.Partitions[0].Query });
                    continue;
                }
                HashSet<Column> matchedColumns = new HashSet<Column>();
                foreach (DataRow row in schemaTable.Rows)
                {
                    var colName = row["ColumnName"].ToString();
                    var dataType = row["DataTypeName"].ToString();
                    var mappedType = DataTypeMap(dataType);
                    var tCols = table.DataColumns.Where(col => col.SourceColumn.EqualsI(colName) || col.SourceColumn.EqualsI("[" + colName + "]"));
                    if (tCols.Count() == 0)
                    {
                        result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceColumnAdded, SourceColumn = colName, SourceType = mappedType, SourceProviderType = dataType });
                        continue;
                    }
                    foreach (var tCol in tCols)
                    {
                        matchedColumns.Add(tCol);
                        if (tCol.DataType != mappedType)
                        {
                            result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.DataTypeChange, ModelColumn = tCol, SourceColumn = colName, SourceType = mappedType, SourceProviderType = dataType });
                        }
                    }
                }
                foreach (var col in table.DataColumns.Where(c => !matchedColumns.Contains(c)))
                {
                    result.Add(new MetadataChange { ModelTable = table, ChangeType = MetadataChangeType.SourceColumnNotFound, ModelColumn = col });
                }
            }

            return result;
        }

        public static DataType DataTypeMap(string dataTypeName)
        {
            DataType result;
            switch (dataTypeName.ToLower())
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
                    result = DataType.String; break;
            }
            return result;
        }
    }
}

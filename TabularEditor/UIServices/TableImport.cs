extern alias json;

using json::Newtonsoft.Json;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
        public enum ProviderType
        {
            Sql,
            OleDb,
            ODBC,
            Oracle,
            Unknown
        }

    public enum SchemaNodeType
    {
        Root,
        Database,
        Table,
        View
    }

    public class SchemaNode
    {
        public string Name;
        public string Schema;
        public string Database;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SchemaNode FromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonConvert.DeserializeObject<SchemaNode>(json);
            }
            catch
            {
                return null;
            }
        }

        [JsonIgnore]
        public SchemaNodeType Type;
        [JsonIgnore]
        public bool Selected;
        [JsonIgnore]
        public string DisplayName => Type == SchemaNodeType.Database || Type == SchemaNodeType.Root ? Name : TwoPartName;
        [JsonIgnore]
        public string ThreePartName => $"[{Database}].[{Schema}].[{Name}]";
        [JsonIgnore]
        public string TwoPartName => $"[{Schema}].[{Name}]";
        public List<string> IncludedColumns { get; } = new List<string>();
        public void LoadColumnsFromSample(DataTable sampleData)
        {
            if(SelectAll == true)
            {
                foreach (DataColumn col in sampleData.Columns) IncludedColumns.Add(col.ColumnName);
            }
        }
        public bool SelectAll { get; set; } = true;

        public string GetSql(bool indented = true, bool useThreePartName = false)
        {
            if (SelectAll)
            {
                return (indented ? "SELECT\n\t*\nFROM\n\t" : "SELECT * FROM ") + (useThreePartName ? ThreePartName : TwoPartName);
            }
            else
            {
                var sqlText = "SELECT";
                var first = true;
                foreach (var col in IncludedColumns)
                {
                    sqlText += (indented ? "\n\t" : "") + (first ? " " : ",");
                    sqlText += col;
                    first = false;

                }
                sqlText += indented ? "\n" : " ";
                sqlText += "FROM";
                sqlText += indented ? "\n\t" : " ";
                sqlText += useThreePartName ? ThreePartName : TwoPartName;
                return sqlText;
            }
        }
    }

    public class OtherDataSource: TypedDataSource
    {
        public override ProviderType ProviderType => ProviderType.Unknown;
        public override DataSource DataSource => null;
        public override DataProvider DataProvider => null;
        public override DbProviderFactory DbFactory
        {
            get
            {
                try
                {
                    return DbProviderFactories.GetFactory(ProviderName);
                }
                catch
                {
                    return null;
                }
            }
        }
        public override IEnumerable<SchemaNode> GetDatabases()
        {
            return new List<SchemaNode>() { new SchemaNode { Name = "Data Source", Type = SchemaNodeType.Database } };
        }

        public override IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            return Enumerable.Empty<SchemaNode>();
        }
        public override string QuoteColumn(string unQuotedColumnName)
        {
            return $"[{unQuotedColumnName}]";
        }
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError)
        {
            throw new NotSupportedException();
        }
        public override DataTable GetSchemaTable(SchemaNode tableOrView)
        {
            throw new NotImplementedException();
        }
        public override DataTable GetSchemaTable(string sql)
        {
            throw new NotImplementedException();
        }
        public override string SuggestSourceName()
        {
            throw new NotImplementedException();
        }
    }

    public class SqlDataSource: TypedDataSource
    {
        public override ProviderType ProviderType => ProviderType.Sql;
        public override DataSource DataSource => DataSource.SqlDataSource;
        public override DataProvider DataProvider => DataProvider.SqlDataProvider;
        public override DbProviderFactory DbFactory => DbProviderFactories.GetFactory("System.Data.SqlClient");

        public override IEnumerable<SchemaNode> GetDatabases()
        {
            return GetSchema("Databases").AsEnumerable().Select(r => r.Field<string>("database_name"))
                        .Where(n => n != "master" && n != "msdb" && n != "model" && n != "tempdb")
                        .Where(n => UseThreePartName || n.EqualsI(DatabaseName))
                        .Select(n => new SchemaNode { Name = n, Type = SchemaNodeType.Database });
        }

        public override IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString) { InitialCatalog = databaseName };
            return GetSchema("Tables", csb.ConnectionString).AsEnumerable().Select(r =>
                new SchemaNode
                {
                    Database = databaseName,
                    Name = r.Field<string>("TABLE_NAME"),
                    Schema = r.Field<string>("TABLE_SCHEMA"),
                    Type = r.Field<string>("TABLE_TYPE") == "VIEW" ? SchemaNodeType.View : SchemaNodeType.Table
                });
        }
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError)
        {
            try
            {
                var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString);
                var adapter = new System.Data.SqlClient.SqlDataAdapter($"SELECT TOP 200 * FROM {(UseThreePartName ? tableOrView.ThreePartName : tableOrView.DisplayName)} WITH (NOLOCK)", csb.ConnectionString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(result);
                isError = false;
                return result;
            }
            catch (Exception ex)
            {
                ErrorTable.Rows[0][0] = ex.Message;
                isError = true;
                return ErrorTable;
            }

        }

        public override DataTable GetSchemaTable(SchemaNode tableOrView)
        {
            return GetSchemaTable(tableOrView.GetSql(false, UseThreePartName));
        }

        public override bool UseThreePartName => string.IsNullOrWhiteSpace(DatabaseName);
        public string DatabaseName
        {
            get
            {
                var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString);
                return csb.InitialCatalog;
            }
        }
        public string ServerName
        {
            get
            {
                var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString);
                return csb.DataSource;
            }
        }

        public override DataTable GetSchemaTable(string sql)
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(ProviderString))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                    var rdr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    return rdr.GetSchemaTable();
                }
            }
            catch
            {
                return null;
            }
        }

        public override string QuoteColumn(string unQuotedColumnName)
        {
            return $"[{unQuotedColumnName}]";
        }
        public override string SuggestSourceName()
        {
            return string.IsNullOrEmpty(DatabaseName) ? ServerName : (ServerName + " " + DatabaseName);
        }

    }

    public class OleDbDataSource : TypedDataSource
    {
        public override ProviderType ProviderType => ProviderType.OleDb;
        public override DataSource DataSource => DataSource.SqlDataSource;
        public override DataProvider DataProvider => DataProvider.OleDBDataProvider;
        public override DbProviderFactory DbFactory => DbProviderFactories.GetFactory("System.Data.OleDb");

        public override IEnumerable<SchemaNode> GetDatabases()
        {
            return GetSchema("Catalogs").AsEnumerable().Select(r => r.Field<string>("CATALOG_NAME"))
            .Where(n => n != "master" && n != "msdb" && n != "model" && n != "tempdb")
            .Select(n => new SchemaNode { Name = n, Type = SchemaNodeType.Database });

        }

        public override IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            var csb = new System.Data.OleDb.OleDbConnectionStringBuilder(ProviderString);
            if (csb.ContainsKey("Initial Catalog")) csb.Remove("Initial Catalog");
            var restrictionList = new string[4];
            restrictionList[0] = databaseName;
            return GetSchema("Tables", csb.ConnectionString, restrictionList).AsEnumerable().Select(r =>
                new SchemaNode
                {
                    Database = databaseName,
                    Name = r.Field<string>("TABLE_NAME"),
                    Schema = r.Field<string>("TABLE_SCHEMA"),
                    Type = r.Field<string>("TABLE_TYPE") == "VIEW" ? SchemaNodeType.View : SchemaNodeType.Table
                }).Where(n => !n.Schema.EqualsI("sys") && !n.Schema.EqualsI("INFORMATION_SCHEMA")); ;
        }

        public override string QuoteColumn(string unQuotedColumnName)
        {
            return $"[{unQuotedColumnName}]";
        }

        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError)
        {
            try
            {
                var adapter = new System.Data.OleDb.OleDbDataAdapter($"SELECT TOP 200 * FROM {(UseThreePartName ? tableOrView.ThreePartName : tableOrView.DisplayName)} WITH (NOLOCK)", ProviderString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(result);
                isError = false;
                return result;
            }
            catch (Exception ex)
            {
                ErrorTable.Rows[0][0] = ex.Message;
                isError = true;
                return ErrorTable;
            }

        }

        public override bool UseThreePartName => string.IsNullOrWhiteSpace(DatabaseName);
        public string DatabaseName
        {
            get
            {
                var csb = new DbConnectionStringBuilder() { ConnectionString = ProviderString };
                return csb.ContainsKey("Database") ? csb["Database"].ToString() :
                       csb.ContainsKey("Initial Catalog") ? csb["Initial Catalog"].ToString() :
                       csb.ContainsKey("InitialCatalog") ? csb["InitialCatalog"].ToString() : null;
            }
        }
        public string ServerName
        {
            get
            {
                var csb = new DbConnectionStringBuilder() { ConnectionString = ProviderString };
                return csb.ContainsKey("DataSource") ? csb["DataSource"].ToString() :
                    csb.ContainsKey("Data Source") ? csb["Data Source"].ToString() :
                    csb.ContainsKey("Server") ? csb["Server"].ToString() : null;
            }
        }


        public override DataTable GetSchemaTable(SchemaNode tableOrView)
        {
            return GetSchemaTable(tableOrView.GetSql(false, UseThreePartName));
        }
        public override DataTable GetSchemaTable(string sql)
        {
            try
            {
                using (var conn = new System.Data.OleDb.OleDbConnection(ProviderString))
                {
                    conn.Open();
                    var cmd = new System.Data.OleDb.OleDbCommand(sql, conn);
                    var rdr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    return rdr.GetSchemaTable();
                }
            }
            catch
            {
                return null;
            }
        }
        public override string SuggestSourceName()
        {
            return string.IsNullOrEmpty(DatabaseName) ? ServerName : (ServerName + " " + DatabaseName);
        }
    }

    public class OracleDataSource : TypedDataSource
    {
        public override ProviderType ProviderType => ProviderType.Oracle;
        public override DataSource DataSource => DataSource.OracleDataSource;
        public override DataProvider DataProvider => DataProvider.OracleDataProvider;
        public override DbProviderFactory DbFactory => DbProviderFactories.GetFactory("System.Data.OracleClient");

        public override IEnumerable<SchemaNode> GetDatabases()
        {
            var csb = DbFactory.CreateConnectionStringBuilder();
            csb.ConnectionString = ProviderString;
            
            return new List<SchemaNode>() { new SchemaNode { Name = csb.ContainsKey("Data Source") ? csb["Data Source"].ToString() : "Data Source", Type = SchemaNodeType.Database } };
        }

        public override IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override string QuoteColumn(string unQuotedColumnName)
        {
            return $"{unQuotedColumnName}";
        }
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetSchemaTable(SchemaNode tableOrView)
        {
            throw new NotImplementedException();
        }
        public override DataTable GetSchemaTable(string sql)
        {
            throw new NotImplementedException();
        }
        public override string SuggestSourceName()
        {
            throw new NotImplementedException();
        }
    }

    public class OdbcDataSource : TypedDataSource
    {
        public override string SuggestSourceName()
        {
            throw new NotImplementedException();
        }
        public override ProviderType ProviderType => ProviderType.ODBC;
        public override DataSource DataSource => DataSource.OdbcDataSource;
        public override DataProvider DataProvider => DataProvider.OdbcDataProvider;
        public override DbProviderFactory DbFactory => DbProviderFactories.GetFactory("System.Data.Odbc");

        public override IEnumerable<SchemaNode> GetDatabases()
        {
            var csb = DbFactory.CreateConnectionStringBuilder();
            csb.ConnectionString = ProviderString;
            return new List<SchemaNode>() { new SchemaNode { Name = csb.ContainsKey("Database") ? csb["Database"].ToString() : "Data Source", Type = SchemaNodeType.Database } };
        }

        public override IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            return GetSchema("Tables").AsEnumerable().Concat(GetSchema("Views").AsEnumerable()).Select(r => new SchemaNode
            {
                Database = databaseName,
                Name = r.Field<string>("TABLE_NAME"),
                Schema = r.Field<string>("TABLE_SCHEM"),
                Type = r.Field<string>("TABLE_TYPE") == "VIEW" ? SchemaNodeType.View : SchemaNodeType.Table
            }).Where(n => !n.Schema.EqualsI("sys") && !n.Schema.EqualsI("INFORMATION_SCHEMA"));
        }

        public override DataTable GetSchemaTable(SchemaNode tableOrView)
        {
            throw new NotImplementedException();
        }
        public override DataTable GetSchemaTable(string sql)
        {
            throw new NotImplementedException();
        }

        public override string QuoteColumn(string unQuotedColumnName)
        {
            return $"[{unQuotedColumnName}]";
        }
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError)
        {
            try
            {
                var adapter = new System.Data.Odbc.OdbcDataAdapter($"SELECT TOP 200 * FROM {tableOrView.DisplayName} WITH (NOLOCK)", ProviderString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(result);
                isError = false;
                return result;
            }
            catch (Exception ex)
            {
                ErrorTable.Rows[0][0] = ex.Message;
                isError = true;
                return ErrorTable;
            }
        }
    }

    public abstract class TypedDataSource
    {
        public abstract ProviderType ProviderType { get; }
        public abstract DataSource DataSource { get; }
        public abstract DataProvider DataProvider { get; }
        public abstract DbProviderFactory DbFactory { get; }
        public abstract string QuoteColumn(string unQuotedColumnName);

        public virtual bool UseThreePartName => false;

        private string _providerString;
        public string ProviderString
        {
            get
            {
                return _providerString;
            }
            protected set
            {
                var factory = DbFactory;
                if (factory is null) { _providerString = value; return; }

                // SSAS adds "Provider" to the connection string, which is not valid, so let's remove it if it is present:
                var csb = new DbConnectionStringBuilder() { ConnectionString = value };
                var validKeys = factory.CreateConnectionStringBuilder().Keys.OfType<string>();
                if (csb.ContainsKey("Provider") && !validKeys.Contains("Provider", StringComparer.InvariantCultureIgnoreCase))
                {
                    csb.Remove("Provider");
                    _providerString = csb.ConnectionString;
                }
                else
                {
                    _providerString = value;
                }

            }
        }
        public abstract string SuggestSourceName();

        public string ProviderName { get; private set; }
        public string TabularDsName { get; internal set; }

        static public TypedDataSource GetFromTabularDs(TOMWrapper.ProviderDataSource tabularDataSource)
        {
            TypedDataSource ds;
            var csb = new DbConnectionStringBuilder() { ConnectionString = tabularDataSource.ConnectionString };
            var providerName = !string.IsNullOrWhiteSpace(tabularDataSource.Provider) ? tabularDataSource.Provider : csb.ContainsKey("Provider") ? csb["Provider"].ToString() : "";
            var pName = providerName.ToUpper();

            if (pName.Contains("OLEDB")) ds = new OleDbDataSource();
            else if (pName.Contains("SQLNCLI") || pName.Contains("SQLCLIENT")) ds = new SqlDataSource();
            else if (pName.Contains("ODBC")) ds = new OdbcDataSource();
            else if (pName.Contains("ORACLE")) ds = new OracleDataSource();
            else ds = new OtherDataSource();

            ds.TabularDsName = tabularDataSource.Name;
            ds.ProviderString = tabularDataSource.ConnectionString;

            return ds;
        }

        static public TypedDataSource GetFromConnectionUi(DataConnectionDialog connectionDialog)
        {
            TypedDataSource ds;

            if (connectionDialog.SelectedDataProvider == DataProvider.SqlDataProvider) ds = new SqlDataSource();
            else if (connectionDialog.SelectedDataProvider == DataProvider.OleDBDataProvider) ds = new OleDbDataSource();
            else if (connectionDialog.SelectedDataProvider == DataProvider.OdbcDataProvider) ds = new OdbcDataSource();
            else if (connectionDialog.SelectedDataProvider == DataProvider.OracleDataProvider) ds = new OracleDataSource();
            else ds = new OtherDataSource();
            ds.ProviderString = connectionDialog.ConnectionString;

            return ds;
        }

        public DataTable GetSchema(string schemaName)
        {
            using (var conn = DbFactory.CreateConnection())
            {
                conn.ConnectionString = ProviderString;
                conn.Open();
                var cmd = conn.CreateCommand();
                return conn.GetSchema(schemaName);
            }
        }

        public DataTable GetSchema(string schemaName, string providerString)
        {
            using (var conn = DbFactory.CreateConnection())
            {
                conn.ConnectionString = providerString;
                conn.Open();
                var cmd = conn.CreateCommand();
                return conn.GetSchema(schemaName);
            }
        }

        public DataTable GetSchema(string schemaName, string providerString, string[] restrictionList)
        {
            using (var conn = DbFactory.CreateConnection())
            {
                conn.ConnectionString = providerString;
                conn.Open();
                var cmd = conn.CreateCommand();
                return conn.GetSchema(schemaName, restrictionList);
            }
        }

        public abstract IEnumerable<SchemaNode> GetDatabases();

        public abstract IEnumerable<SchemaNode> GetTablesAndViews(string databaseName);

        public DataTable GetSampleData(SchemaNode tableOrView, out bool isError)
        {
            var result = InternalGetSampleData(tableOrView, out isError);
            tableOrView.LoadColumnsFromSample(result);
            return result;
        }

        protected abstract DataTable InternalGetSampleData(SchemaNode tableOrView, out bool isError);

        public abstract DataTable GetSchemaTable(SchemaNode tableOrView);
        public abstract DataTable GetSchemaTable(string sql);

        private DataTable _errorTable;
        protected DataTable ErrorTable
        {
            get
            {
                if (_errorTable == null)
                    _errorTable = InitErrorTable();
                return _errorTable;
            }
        }

        private DataTable InitErrorTable()
        {
            var result = new DataTable();
            result.Columns.Add("Error message", typeof(string));
            result.Rows.Add("");
            return result;
        }
    }

    public static class ConnectionUIHelper
    {

        public static string ApplyToTabularDs(this DataConnectionDialog dcd, TOMWrapper.ProviderDataSource tabularDataSource)
        {
            tabularDataSource.Provider = dcd.SelectedDataProvider.Name;
            tabularDataSource.ConnectionString = dcd.ConnectionString;
            return dcd.ConnectionString;
        }
    }
}

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
        Database,
        Table,
        View
    }

    public class SchemaNode
    {
        public string Name;
        public string Schema;
        public string Database;
        public SchemaNodeType Type;
        public bool Selected;
        public string DisplayName => Type == SchemaNodeType.Database ? Name : TwoPartName;
        public string ThreePartName => $"[{Database}].[{Schema}].[{Name}]";
        public string TwoPartName => $"[{Schema}].[{Name}]";

    }

    public class TypedDataSource
    {
        public ProviderType ProviderType { get; private set; }
        public DataSource DataSource
        {
            get
            {
                switch (ProviderType)
                {
                    case ProviderType.Sql: return DataSource.SqlDataSource;
                    case ProviderType.OleDb: return DataSource.SqlDataSource;
                    case ProviderType.ODBC: return DataSource.OdbcDataSource;
                    case ProviderType.Oracle: return DataSource.OracleDataSource;
                    default: return null;
                }
            }
        }
        public DataProvider DataProvider
        {
            get
            {
                switch (ProviderType)
                {
                    case ProviderType.Sql: return DataProvider.SqlDataProvider;
                    case ProviderType.OleDb: return DataProvider.OleDBDataProvider;
                    case ProviderType.ODBC: return DataProvider.OdbcDataProvider;
                    case ProviderType.Oracle: return DataProvider.OracleDataProvider;
                    default: return null;
                }
            }
        }
        public DbProviderFactory DbFactory
        {
            get
            {
                switch (ProviderType)
                {
                    case ProviderType.Sql: return System.Data.SqlClient.SqlClientFactory.Instance;
                    case ProviderType.OleDb: return System.Data.OleDb.OleDbFactory.Instance;
                    case ProviderType.ODBC: return System.Data.Odbc.OdbcFactory.Instance;
                    default:
                        try
                        {
                            return System.Data.Common.DbProviderFactories.GetFactory(ProviderName);
                        }
                        catch
                        {
                            return null;
                        }
                }
            }
        }
        public string ProviderString { get; private set; }
        public string ProviderName { get; private set; }

        static public TypedDataSource GetFromTabularDs(TOMWrapper.ProviderDataSource tabularDataSource)
        {
            var csb = new DbConnectionStringBuilder() { ConnectionString = tabularDataSource.ConnectionString };

            var ds = new TypedDataSource();
            ds.ProviderName = !string.IsNullOrWhiteSpace(tabularDataSource.Provider) ? tabularDataSource.Provider : csb.ContainsKey("Provider") ? csb["Provider"].ToString() : "";
            var pName = ds.ProviderName.ToUpper();

            if (pName.Contains("OLEDB")) ds.ProviderType = ProviderType.OleDb;
            else if (pName.Contains("SQLNCLI") || pName.Contains("SQLCLIENT")) ds.ProviderType = ProviderType.Sql;
            else if (pName.Contains("ODBC")) ds.ProviderType = ProviderType.ODBC;
            else if (pName.Contains("ORACLE")) ds.ProviderType = ProviderType.Oracle;
            else ds.ProviderType = ProviderType.Unknown;

            if(ds.DbFactory != null)
            {
                var validKeys = ds.DbFactory.CreateConnectionStringBuilder().Keys.OfType<string>();
                if (csb.ContainsKey("Provider") && !validKeys.Contains("Provider", StringComparer.InvariantCultureIgnoreCase)) csb.Remove("Provider");
            }
            ds.ProviderString = csb.ConnectionString;

            return ds;
        }

        static public TypedDataSource GetFromConnectionUi(DataConnectionDialog connectionDialog)
        {
            var ds = new TypedDataSource();

            if (connectionDialog.SelectedDataProvider == DataProvider.SqlDataProvider) ds.ProviderType = ProviderType.Sql;
            else if (connectionDialog.SelectedDataProvider == DataProvider.OleDBDataProvider) ds.ProviderType = ProviderType.OleDb;
            else if (connectionDialog.SelectedDataProvider == DataProvider.OdbcDataProvider) ds.ProviderType = ProviderType.ODBC;
            else if (connectionDialog.SelectedDataProvider == DataProvider.OracleDataProvider) ds.ProviderType = ProviderType.Oracle;
            else ds.ProviderType = ProviderType.Unknown;
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

        public IEnumerable<SchemaNode> GetDatabases()
        {
            switch(ProviderType)
            {
                case ProviderType.Sql:
                    return GetSchema("Databases").AsEnumerable().Select(r => r.Field<string>("database_name"))
                        .Where(n => n != "master" && n != "msdb" && n != "model" && n != "tempdb")
                        .Select(n => new SchemaNode { Name = n, Type = SchemaNodeType.Database });
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<SchemaNode> GetTablesAndViews(string databaseName)
        {
            switch (ProviderType)
            {
                case ProviderType.Sql:
                    var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString) { InitialCatalog = databaseName };
                    return GetSchema("Tables", csb.ConnectionString).AsEnumerable().Select(r =>
                        new SchemaNode {
                            Database = databaseName,
                            Name = r.Field<string>("TABLE_NAME"),
                            Schema = r.Field<string>("TABLE_SCHEMA"),
                            Type = r.Field<string>("TABLE_TYPE") == "VIEW" ? SchemaNodeType.View : SchemaNodeType.Table });
                default:
                    throw new NotImplementedException();
            }
        }
        public DataTable GetSampleData(SchemaNode tableOrView)
        {

            switch (ProviderType)
            {
                case ProviderType.Sql:
                    return GetSampleDataSql(tableOrView);
                default:
                    throw new NotImplementedException();
            }
        }

        private DataTable GetSampleDataSql(SchemaNode tableOrView)
        {
            try
            {
                var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(ProviderString);
                var useThreePartName = csb.InitialCatalog != tableOrView.Database;
                var adapter = new System.Data.SqlClient.SqlDataAdapter($"SELECT TOP 200 * FROM {(useThreePartName ? tableOrView.ThreePartName : tableOrView.DisplayName)} WITH (NOLOCK)", csb.ConnectionString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(result);
                return result;
            }
            catch (Exception ex)
            {
                ErrorTable.Rows[0][0] = ex.Message;
                return ErrorTable;
            }
        }

        private DataTable _errorTable;
        private DataTable ErrorTable
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

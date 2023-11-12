using Newtonsoft.Json;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using TabularEditor.UI.Dialogs;
using SC = System.Data.SqlClient;
using TOM = TabularEditor.TOMWrapper;

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
        public string DisplayName => Type == SchemaNodeType.Database || Type == SchemaNodeType.Root ? Name : $"[{Schema}].[{Name}]";        
        public List<string> IncludedColumns { get; } = new List<string>();
        public void LoadColumnsFromSample(DataTable sampleData)
        {
            if(SelectAll == true)
            {
                foreach (DataColumn col in sampleData.Columns) IncludedColumns.Add(col.ColumnName);
            }
        }
        public bool SelectAll { get; set; } = true;

        public string GetRef(IdentifierQuoting identifierQuoting, bool useThreePartName)
        {
            switch(identifierQuoting)
            {
                case IdentifierQuoting.None:
                    return useThreePartName ? $"{Database}.{Schema}.{Name}" : $"{Schema}.{Name}";
                case IdentifierQuoting.DoubleQuote:
                    return useThreePartName ? $"\"{Database}\".\"{Schema}\".\"{Name}\"" : $"\"{Schema}\".\"{Name}\"";
                case IdentifierQuoting.SingleQuote:
                    return useThreePartName ? $"'{Database}'.'{Schema}'.'{Name}'" : $"'{Schema}'.'{Name}'";
                case IdentifierQuoting.Backtick:
                    return useThreePartName ? $"`{Database}`.`{Schema}`.`{Name}`" : $"`{Schema}`.`{Name}`";
                default:
                    return useThreePartName ? $"[{Database}].[{Schema}].[{Name}]" : $"[{Schema}].[{Name}]";
            }
        }

        public string GetSql(IdentifierQuoting identifierQuoting, bool indented = true, bool useThreePartName = false)
        {
            if (SelectAll)
            {
                return (indented ? "SELECT\n\t*\nFROM\n\t" : "SELECT * FROM ") + GetRef(identifierQuoting, useThreePartName);
            }
            else
            {
                var sqlText = "SELECT";
                var first = true;
                foreach (var col in IncludedColumns)
                {
                    sqlText += (indented ? "\n\t" : "") + (first ? " " : ",");
                    switch(identifierQuoting)
                    {
                        case IdentifierQuoting.None:        sqlText += $"{col}"; break;
                        case IdentifierQuoting.DoubleQuote: sqlText += $"\"{col}\""; break;
                        case IdentifierQuoting.SingleQuote: sqlText += $"'{col}'"; break;
                        case IdentifierQuoting.Backtick:    sqlText += $"`{col}`"; break;
                        default:                            sqlText += $"[{col}]"; break;
                    }
                    
                    first = false;

                }
                sqlText += indented ? "\n" : " ";
                sqlText += "FROM";
                sqlText += indented ? "\n\t" : " ";
                sqlText += GetRef(identifierQuoting, useThreePartName);
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
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            throw new NotSupportedException();
        }
        public override DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting)
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
            var csb = new SC.SqlConnectionStringBuilder(ProviderString) { InitialCatalog = databaseName };
            return GetSchema("Tables", csb.ConnectionString).AsEnumerable().Select(r =>
                new SchemaNode
                {
                    Database = databaseName,
                    Name = r.Field<string>("TABLE_NAME"),
                    Schema = r.Field<string>("TABLE_SCHEMA"),
                    Type = r.Field<string>("TABLE_TYPE") == "VIEW" ? SchemaNodeType.View : SchemaNodeType.Table
                });
        }
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            try
            {
                var csb = new SC.SqlConnectionStringBuilder(ProviderString);
                var adapter = new SC.SqlDataAdapter($"SELECT TOP 200 * FROM {tableOrView.GetRef(identifierQuoting, UseThreePartName)}" + (rowLimitClause == RowLimitClause.Top ? " WITH (NOLOCK)" : ""), csb.ConnectionString);
                adapter.SelectCommand.CommandTimeout = 30;
                
                var result = new DataTable();
                adapter.Fill(0, 200, result);
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

        public override DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting)
        {
            return GetSchemaTable(tableOrView.GetSql(identifierQuoting, false, UseThreePartName));
        }

        public override bool UseThreePartName => string.IsNullOrWhiteSpace(DatabaseName);
        public string DatabaseName
        {
            get
            {
                var csb = new SC.SqlConnectionStringBuilder(ProviderString);
                return csb.InitialCatalog;
            }
        }
        public string ServerName
        {
            get
            {
                var csb = new SC.SqlConnectionStringBuilder(ProviderString);
                return csb.DataSource;
            }
        }

        public override DataTable GetSchemaTable(string sql)
        {
            try
            {
                using (var conn = new SC.SqlConnection(ProviderString))
                {
                    conn.Open();
                    var cmd = new SC.SqlCommand(sql, conn);
                    if(ModelDataSource != null && ModelDataSource.Timeout != 0)
                    {
                        cmd.CommandTimeout = ModelDataSource.Timeout;
                    }
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

        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            try
            {
                var tableRef = tableOrView.GetRef(identifierQuoting, UseThreePartName);
                var sql = GetSampleSql(rowLimitClause, tableRef, 200);
                var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, ProviderString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(0, 200, result);
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


        public override DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting)
        {
            return GetSchemaTable(tableOrView.GetSql(identifierQuoting, false, UseThreePartName));
        }
        public override DataTable GetSchemaTable(string sql)
        {
            try
            {
                using (var conn = new System.Data.OleDb.OleDbConnection(ProviderString))
                {
                    conn.Open();
                    var cmd = new System.Data.OleDb.OleDbCommand(sql, conn);
                    if(ModelDataSource != null && ModelDataSource.Timeout != 0)
                    {
                        cmd.CommandTimeout = ModelDataSource.Timeout;
                    }
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
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting)
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
            return "New ODBC Data Source"; // TODO: Maybe we should use the DSN name instead?
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

        public override DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting)
        {
            return GetSchemaTable(tableOrView.GetSql(identifierQuoting));
        }
        public override DataTable GetSchemaTable(string sql)
        {
            try
            {
                using (var conn = new System.Data.Odbc.OdbcConnection(ProviderString))
                {
                    conn.Open();
                    var cmd = new System.Data.Odbc.OdbcCommand(sql, conn);
                    if (ModelDataSource != null && ModelDataSource.Timeout != 0)
                    {
                        cmd.CommandTimeout = ModelDataSource.Timeout;
                    }
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
        protected override DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            try
            {
                var sql = GetSampleSql(rowLimitClause, tableOrView.GetRef(identifierQuoting, false), 200);
                var adapter = new System.Data.Odbc.OdbcDataAdapter(sql, ProviderString);
                adapter.SelectCommand.CommandTimeout = 30;
                var result = new DataTable();
                adapter.Fill(0, 200, result);
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

    public enum IdentifierQuoting
    {
        None = 0,
        SquareBracket = 1,
        DoubleQuote = 2,
        SingleQuote = 3,
        Backtick = 4
    }

    public enum RowLimitClause
    {
        None = 0,
        Top = 1,
        First = 2,
        LimitOffset = 3,
        Limit = 4,
        Sample = 5,
        ANSI = 6,
        TopWithoutNolock = 7
    }

    public abstract class TypedDataSource
    {
        public TOM.ProviderDataSource ModelDataSource { get; init; }

        protected string GetSampleSql(RowLimitClause limitClause, string tableRef, int maxRecords)
        {
            switch(limitClause)
            {
                case RowLimitClause.Top: return $"SELECT TOP {maxRecords} * FROM {tableRef} WITH (NOLOCK)";
                case RowLimitClause.TopWithoutNolock: return $"SELECT TOP {maxRecords} * FROM {tableRef}";
                case RowLimitClause.First: return $"SELECT FIRST {maxRecords} * FROM {tableRef}";
                case RowLimitClause.Limit: return $"SELECT * FROM {tableRef} LIMIT {maxRecords}";
                case RowLimitClause.LimitOffset: return $"SELECT * FROM {tableRef} LIMIT {maxRecords} OFFSET 0";
                case RowLimitClause.Sample: return $"SELECT * FROM {tableRef} SAMPLE {maxRecords}";
                case RowLimitClause.ANSI: return $"SELECT * FROM {tableRef} FETCH FIRST {maxRecords} ROWS ONLY";
                default:
                    return $"SELECT * FROM {tableRef}";
            }
        }

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

        private static Dictionary<string, string> GlobalPasswords = new Dictionary<string, string>();
        private string MissingPwdConnectionString;
        public void SetPassword(string password)
        {
            if (GlobalPasswords.ContainsKey(MissingPwdConnectionString))
                GlobalPasswords.Remove(MissingPwdConnectionString);
            GlobalPasswords.Add(MissingPwdConnectionString, password);

            var csb = new DbConnectionStringBuilder() { ConnectionString = MissingPwdConnectionString };
            if (csb.ContainsKey("password")) csb["password"] = password;
            else if (csb.ContainsKey("pwd")) csb["pwd"] = password;

            ProviderString = csb.ConnectionString;

            NeedsPassword = false;
        }

        public bool NeedsPassword { get; private set; } = false;
        public string Username { get; private set; }

        static public TypedDataSource GetFromTabularDs(TOMWrapper.ProviderDataSource tabularDataSource)
        {
            TypedDataSource ds;
            bool needsPassword = false;
            string cs = tabularDataSource.ConnectionString;
            if(!string.IsNullOrWhiteSpace(tabularDataSource.GetPreviewConnectionString()))
            {
                cs = tabularDataSource.GetPreviewConnectionString();
            }
            var csb = new DbConnectionStringBuilder() { ConnectionString = cs };

            if(!string.IsNullOrEmpty(tabularDataSource.Password) && tabularDataSource.Password != "********")
            {
                csb.Add("password", tabularDataSource.Password);
                csb.Add("user id", tabularDataSource.Account);
            }

            if (csb.ContainsKey("password") && (string)csb["password"] == "********")
            {
                if (GlobalPasswords.TryGetValue(tabularDataSource.ConnectionString, out string password))
                    csb["password"] = password;
                else
                {
                    needsPassword = true;
                }
            }
            if (csb.ContainsKey("pwd") && (string)csb["pwd"] == "********")
            {
                if (GlobalPasswords.TryGetValue(tabularDataSource.ConnectionString, out string password))
                    csb["pwd"] = password;
                else
                    needsPassword = true;
            }

            var providerName = !string.IsNullOrWhiteSpace(tabularDataSource.Provider) ? tabularDataSource.Provider : csb.ContainsKey("Provider") ? csb["Provider"].ToString() : "";
            var pName = providerName.ToUpper();

            if (pName.Contains("MSADSQL") || pName.Contains("OLEDB")) ds = new OleDbDataSource() { ModelDataSource = tabularDataSource };
            else if (pName.Contains("SQLNCLI") || pName.Contains("SQLCLIENT")) ds = new SqlDataSource() { ModelDataSource = tabularDataSource };
            else if (pName.Contains("ODBC")) ds = new OdbcDataSource() { ModelDataSource = tabularDataSource };
            else if (pName.Contains("ORACLE")) ds = new OracleDataSource() { ModelDataSource = tabularDataSource };
            else ds = new OtherDataSource() { ModelDataSource = tabularDataSource };

            ds.NeedsPassword = needsPassword;
            if (needsPassword) ds.MissingPwdConnectionString = tabularDataSource.ConnectionString;
            ds.TabularDsName = tabularDataSource.Name;

            ds.ProviderString = string.Join(";", csb.Keys.OfType<string>().Select(k => k + "=" + csb[k].ToString()).ToArray());

            if (!string.IsNullOrEmpty(tabularDataSource.Account))
            {
                ds.Username = tabularDataSource.Account;
            } else
            {
                if (csb.ContainsKey("user id")) ds.Username = csb["user id"].ToString();
                else if (csb.ContainsKey("uid")) ds.Username = csb["uid"].ToString();
                else if (csb.ContainsKey("username")) ds.Username = csb["username"].ToString();
                else if (csb.ContainsKey("account")) ds.Username = csb["account"].ToString();
            }



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

        public DataTable GetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError)
        {
            var result = InternalGetSampleData(tableOrView, rowLimitClause, identifierQuoting, out isError);
            tableOrView.LoadColumnsFromSample(result);
            return result;
        }

        protected abstract DataTable InternalGetSampleData(SchemaNode tableOrView, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting, out bool isError);

        public abstract DataTable GetSchemaTable(SchemaNode tableOrView, IdentifierQuoting identifierQuoting);
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

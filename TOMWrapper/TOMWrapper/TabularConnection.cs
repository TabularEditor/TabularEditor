using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public static class TabularConnection
    {
        private const string ApplicationNameKey = "Application Name";
        private const string DataSourceKey = "Data Source";
        private const string ProviderKey = "Provider";
        private const string UsernameKey = "User ID";
        private const string PasswordKey = "Password";

        private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString)
        {
            // Special handling of a connection string on the form: "powerbi://api.powerbi.com/v1.0/myorg/workspace;initial catalog=dbname"
            if (serverNameOrConnectionString.StartsWith("powerbi://", StringComparison.OrdinalIgnoreCase) && serverNameOrConnectionString.Contains(";"))
            {
                serverNameOrConnectionString = "Provider=MSOLAP;Data Source=" + serverNameOrConnectionString;
            }
            DbConnectionStringBuilder csb = new DbConnectionStringBuilder();
            if (serverNameOrConnectionString.Contains("="))
            {
                try
                {
                    csb.ConnectionString = serverNameOrConnectionString;
                }
                catch (ArgumentException)
                {
                }
            }

            if (!csb.ContainsKey(ProviderKey)) csb.Add(ProviderKey, "MSOLAP");
            if (!csb.ContainsAny(DataSourceKey, "DataSource")) csb.Add(DataSourceKey, serverNameOrConnectionString);

            return csb;
        }
        private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString, string applicationName)
        {
            var csb = GetBuilder(serverNameOrConnectionString);
            if(!csb.ContainsKey(ApplicationNameKey))
                csb[ApplicationNameKey] = applicationName;
            return csb;
        }

        public static string GetConnectionString(string serverNameOrConnectionString, string applicationName)
        {
            return GetBuilder(serverNameOrConnectionString, applicationName).ToString();
        }

        public static string StripApplicationName(string connectionString)
        {
            var csb = GetBuilder(connectionString);
            if (csb.ContainsKey(ApplicationNameKey)) csb.Remove(ApplicationNameKey);
            return csb.ToString();
        }

        private static bool ContainsAny(this DbConnectionStringBuilder csb, params string[] keys)
        {
            foreach (var key in keys)
                if (csb.ContainsKey(key)) return true;
            return false;
        }

        public static string GetConnectionString(string serverNameOrConnectionString, string userName, string password, string applicationName)
        {
            var csb = GetBuilder(serverNameOrConnectionString, applicationName);

            if (!csb.ContainsAny("User ID", "UID", "UserName"))
                csb.Add(UsernameKey, userName);
            if (!csb.ContainsAny("Password", "PWD"))
                csb.Add(PasswordKey, password);

            return csb.ToString();
        }

        public static bool IsSensitive(string connectionString)
        {
            var csb = new DbConnectionStringBuilder();
            csb.ConnectionString = connectionString;
            return csb.ContainsAny("Password", "PWD");
        }

        public static string StripSensitive(string connectionString)
        {
            var csb = GetBuilder(connectionString);
            if (csb.ContainsKey("Password")) csb.Remove("Password");
            if (csb.ContainsKey("PWD")) csb.Remove("PWD");
            return csb.ToString();
        }
    }
}

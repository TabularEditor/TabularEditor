using System;
using System.ComponentModel;
using System.Data.Common;
using Microsoft.AnalysisServices;

namespace TabularEditor.TOMWrapper;

internal static class TabularConnection
{
    private const string ApplicationNameKey = "Application Name";
    private const string DataSourceKey = "Data Source";
    private const string ProviderKey = "Provider";
    private const string UsernameKey = "User ID";
    private const string PasswordKey = "Password";
    private const string InitialCatalogKey = "Initial Catalog";
    private const string ProtocolFormatKey = "Protocol Format";
    private const string InteractiveLoginKey = "Interactive Login";
    private const string IdentityModeKey = "Identity Mode";

    private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString, ProtocolFormat protocolFormat = ProtocolFormat.Default, InteractiveLogin interactiveLogin = InteractiveLogin.Default, IdentityMode identityMode = IdentityMode.Default)
    {
        // Special handling of a connection string on the form: "powerbi://api.powerbi.com/v1.0/myorg/workspace;initial catalog=dbname"
        if (serverNameOrConnectionString.StartsWith("powerbi://", StringComparison.OrdinalIgnoreCase) && serverNameOrConnectionString.Contains(";")) serverNameOrConnectionString = "Provider=MSOLAP;Data Source=" + serverNameOrConnectionString;
        var csb = new DbConnectionStringBuilder();
        if (serverNameOrConnectionString.Contains("="))
            try
            {
                csb.ConnectionString = serverNameOrConnectionString;
            }
            catch (ArgumentException)
            {
            }

        if (!csb.ContainsKey(ProviderKey)) csb.Add(ProviderKey, "MSOLAP");
        if (!csb.ContainsAny(DataSourceKey, "DataSource")) csb.Add(DataSourceKey, serverNameOrConnectionString);
        if (protocolFormat != ProtocolFormat.Default) csb.Add(ProtocolFormatKey, protocolFormat == ProtocolFormat.Xml ? "XML" : "Binary");
        if (interactiveLogin != InteractiveLogin.Default) csb.Add(InteractiveLoginKey, interactiveLogin.ToString());
        if (identityMode != IdentityMode.Default) csb.Add(IdentityModeKey, identityMode.ToString());

        return csb;
    }

    private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString, string applicationName, ProtocolFormat protocolFormat, InteractiveLogin interactiveLogin, IdentityMode identityMode)
    {
        var csb = GetBuilder(serverNameOrConnectionString, protocolFormat, interactiveLogin, identityMode);
        csb[ApplicationNameKey] = applicationName;
        return csb;
    }

    private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString, string applicationName, string databaseName, ProtocolFormat protocolFormat, InteractiveLogin interactiveLogin, IdentityMode identityMode)
    {
        var csb = GetBuilder(serverNameOrConnectionString, applicationName, protocolFormat, interactiveLogin, identityMode);
        csb[InitialCatalogKey] = databaseName;
        return csb;
    }

    public static string GetConnectionString(string serverNameOrConnectionString, string applicationName, string databaseName, ProtocolFormat protocolFormat = ProtocolFormat.Default, InteractiveLogin interactiveLogin = InteractiveLogin.Default, IdentityMode identityMode = IdentityMode.Default)
        => GetBuilder(serverNameOrConnectionString, applicationName, databaseName, protocolFormat, interactiveLogin, identityMode).ToString();

    public static string GetConnectionString(string serverNameOrConnectionString, string applicationName, ProtocolFormat protocolFormat, InteractiveLogin interactiveLogin, IdentityMode identityMode, out string databaseName)
    {
        var csb = GetBuilder(serverNameOrConnectionString, applicationName, protocolFormat, interactiveLogin, identityMode);
        databaseName = ExtractDatabaseName(csb);
        return csb.ToString();
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

    public static string GetConnectionString(string serverNameOrConnectionString, string userName, string password, string applicationName, ProtocolFormat protocolFormat, InteractiveLogin interactiveLogin, IdentityMode identityMode) => GetConnectionString(serverNameOrConnectionString, userName, password, applicationName, protocolFormat, interactiveLogin, identityMode, out _);

    private static string ExtractDatabaseName(DbConnectionStringBuilder csb) =>
        csb.ContainsKey("Initial Catalog") ? csb["Initial Catalog"].ToString() :
        csb.ContainsKey("InitialCatalog") ? csb["InitialCatalog"].ToString() :
        csb.ContainsKey("Database") ? csb["Database"].ToString() : null;

    [Localizable(false)]
    public static string GetConnectionString(string serverNameOrConnectionString, string userName, string password, string applicationName, ProtocolFormat protocolFormat, InteractiveLogin interactiveLogin, IdentityMode identityMode, out string databaseName)
    {
        var csb = GetBuilder(serverNameOrConnectionString, applicationName, protocolFormat, interactiveLogin, identityMode);

        if (!csb.ContainsAny("User ID", "UID", "UserName") && userName != null)
            csb.Add(UsernameKey, userName);
        if (!csb.ContainsAny("Password", "PWD") && password != null)
            csb.Add(PasswordKey, password);

        databaseName = ExtractDatabaseName(csb);

        return csb.ToString();
    }
    public static string SetConnectionStringUserId(string serverNameOrConnectionString, string userId)
    {
        var csb = GetBuilder(serverNameOrConnectionString);
        csb.Add(UsernameKey, userId);
        return csb.ToString();
    }
}

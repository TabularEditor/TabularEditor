using Microsoft.AnalysisServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public class TabularDeployer
    {
        public static string GetTMSL(TOM.Database db, TOM.Server server, string targetDatabaseID, DeploymentOptions options)
        {
            if (db == null) throw new ArgumentNullException("db");
            if (string.IsNullOrWhiteSpace(targetDatabaseID)) throw new ArgumentNullException("targetDatabaseID");
            if (options.DeployRoleMembers && !options.DeployRoles) throw new ArgumentException("Cannot deploy Role Members when Role deployment is disabled.");

            if (server.Databases.Contains(targetDatabaseID) && options.DeployMode == DeploymentMode.CreateDatabase) throw new ArgumentException("The specified database already exists.");
            if (!server.Databases.Contains(targetDatabaseID) && options.DeployMode == DeploymentMode.CreateOrAlter) throw new ArgumentException("The specified database does not exist.");

            string result;

            if (!server.Databases.Contains(targetDatabaseID)) result = DeployNew(db, server, targetDatabaseID, options);
            else result = DeployExisting(db, server, targetDatabaseID, options);

            return result;
        }

        public static void Deploy(TOM.Database db, string targetConnectionString, string targetDatabaseName)
        {
            Deploy(db, targetConnectionString, targetDatabaseName, DeploymentOptions.Default);
        }

        public static void Deploy(TOM.Database db, string targetConnectionString, string targetDatabaseID, DeploymentOptions options)
        {
            if (string.IsNullOrWhiteSpace(targetConnectionString)) throw new ArgumentNullException("targetConnectionString");
            var s = new TOM.Server();
            s.Connect(targetConnectionString);

            var tmsl = GetTMSL(db, s, targetDatabaseID, options);
            var result = s.Execute(tmsl);
            if(result.ContainsErrors)
            {
                throw new Exception(string.Join("\n", result.Cast<XmlaResult>().SelectMany(r => r.Messages.Cast<XmlaMessage>().Select(m => m.Description)).ToArray()));
            }
        }

        private static string DeployNew(TOM.Database db, TOM.Server server, string newDbID, DeploymentOptions options)
        {
            var rawScript = TOM.JsonScripter.ScriptCreate(db);
            var jObj = JObject.Parse(rawScript);

            jObj["create"]["database"]["id"] = newDbID;
            jObj["create"]["database"]["name"] = newDbID;

            if (!options.DeployRoles)
            {
                // Remove roles if present
                var roles = jObj.SelectToken("create.database.model.roles") as JArray;
                roles.Clear();
            }

            return jObj.ToString();
        }

        private static string DeployExisting(TOM.Database db, TOM.Server server, string dbId, DeploymentOptions options)
        {
            var orgDb = server.Databases[dbId];

            var rawScript = TOM.JsonScripter.ScriptCreateOrReplace(db);
            var jObj = JObject.Parse(rawScript);

            jObj["createOrReplace"]["object"]["database"] = dbId;
            jObj["createOrReplace"]["database"]["id"] = dbId;
            jObj["createOrReplace"]["database"]["name"] = orgDb.Name;

            var model = jObj.SelectToken("createOrReplace.database.model");

            var roles = model["roles"] as JArray;
            if (!options.DeployRoles)
            {
                // Remove roles if present and add original:
                roles = new JArray();
                model["roles"] = roles;
                foreach (var role in orgDb.Model.Roles) roles.Add(JObject.Parse(TOM.JsonSerializer.SerializeObject(role)));
            }
            else if (!options.DeployRoleMembers)
            {
                foreach(var role in roles)
                {
                    var members = new JArray();
                    role["members"] = members;

                    // Remove members if present and add original:
                    var roleName = role["name"].Value<string>();
                    if (orgDb.Model.Roles.Contains(roleName))
                    {
                        foreach (var member in orgDb.Model.Roles[roleName].Members)
                            members.Add(JObject.Parse(TOM.JsonSerializer.SerializeObject(member)));
                    }
                }
            }

            if (!options.DeployConnections)
            {
                // Remove dataSources if present
                var dataSources = new JArray();
                model["dataSources"] = dataSources;
                foreach (var ds in orgDb.Model.DataSources) dataSources.Add(JObject.Parse(TOM.JsonSerializer.SerializeObject(ds)));
            }

            if (!options.DeployPartitions)
            {
                var tables = jObj.SelectToken("createOrReplace.database.model.tables") as JArray;
                foreach (var table in tables)
                {
                    var tableName = table["name"].Value<string>();
                    if (orgDb.Model.Tables.Contains(tableName)) {
                        var t = orgDb.Model.Tables[tableName];

                        var partitions = new JArray();
                        table["partitions"] = partitions;
                        foreach (var pt in t.Partitions) partitions.Add(JObject.Parse(TOM.JsonSerializer.SerializeObject(pt)));
                    }
                }
            }

            return jObj.ToString();
        }
    }

    public class DeploymentOptions
    {
        public DeploymentMode DeployMode = DeploymentMode.CreateOrAlter;
        public bool DeployConnections = false;
        public bool DeployPartitions = false;
        public bool DeployRoles = true;
        public bool DeployRoleMembers = false;

        public static DeploymentOptions Default = new DeploymentOptions();
        public static DeploymentOptions StructureOnly = new DeploymentOptions() { DeployRoles = false };
    }

    public enum DeploymentMode
    {
        CreateDatabase,
        CreateOrAlter
    }
}

using Microsoft.AnalysisServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            string result;

            if (!server.Databases.Contains(targetDatabaseID)) result = DeployNewTMSL(db, targetDatabaseID, options);
            else result = DeployExistingTMSL(db, server, targetDatabaseID, options);

            // TODO: Check if invalid CalculatedTableColumn perspectives/translations can give us any issues here
            // Should likely be handled similar to what we do in TabularModelHandler.SaveDB()

            return result;
        }

        public static void Deploy(TOM.Database db, string targetConnectionString, string targetDatabaseName)
        {
            Deploy(db, targetConnectionString, targetDatabaseName, DeploymentOptions.Default);
        }

        public static void SaveModelMetadataBackup(string connectionString, string targetDatabaseID, string backupFilePath)
        {
            using (var s = new TOM.Server())
            {
                s.Connect(connectionString);
                if (s.Databases.Contains(targetDatabaseID))
                {
                    var db = s.Databases[targetDatabaseID];

                    var dbcontent = TOM.JsonSerializer.SerializeDatabase(db);
                    WriteZip(backupFilePath, dbcontent);
                }
                s.Disconnect();
            }
        }

        public static void WriteZip(string fileName, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (var fileStream = new FileStream(fileName, FileMode.CreateNew))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, false))
                {
                    var entry = archive.CreateEntry("Model.bim");
                    using (var zipStream = entry.Open())
                    {
                        var buffer = Encoding.UTF8.GetBytes(content);
                        zipStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Deploys the specified database to the specified target server and database ID, using the specified options.
        /// Returns a list of DAX errors (if any) on objects inside the database, in case the deployment was succesful.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="targetConnectionString"></param>
        /// <param name="targetDatabaseID"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static DeploymentResult Deploy(TOM.Database db, string targetConnectionString, string targetDatabaseID, DeploymentOptions options)
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

            s.Refresh();
            var deployedDB = s.Databases[targetDatabaseID];
            return
                new DeploymentResult(
                    TabularModelHandler.CheckErrors(deployedDB).Select(t => string.Format("Error on {0}: {1}", GetName(t.Item1), t.Item2)),
                    TabularModelHandler.GetObjectsNotReady(deployedDB).Where(t => t.Item2 == TOM.ObjectState.DependencyError || t.Item2 == TOM.ObjectState.EvaluationError || t.Item2 == TOM.ObjectState.SemanticError)
                        .Select(t => string.Format("Warning! Object not in \"Ready\"-state: {0} ({1})", GetName(t.Item1), t.Item2.ToString())),
                    TabularModelHandler.GetObjectsNotReady(deployedDB).Where(t => t.Item2 == TOM.ObjectState.CalculationNeeded || t.Item2 == TOM.ObjectState.NoData)
                        .Select(t => string.Format("Information: Unprocessed object: {0} ({1})", GetName(t.Item1), t.Item2.ToString()))
                );
        }

        private static string GetName(TOM.NamedMetadataObject obj)
        {
            if (obj is TOM.Hierarchy) return string.Format("hierarchy {0}[{1}]", GetName((obj as TOM.Hierarchy).Table), obj.Name);
            if (obj is TOM.Measure) return string.Format("measure {0}[{1}]", GetName((obj as TOM.Measure).Table), obj.Name);
            if (obj is TOM.Column) return string.Format("column {0}[{1}]", GetName((obj as TOM.Column).Table), obj.Name);
            if (obj is TOM.Partition) return string.Format("partition '{0}' on table {1}", obj.Name, GetName((obj as TOM.Partition).Table));
            if (obj is TOM.Table) return string.Format("'{0}'", obj.Name);
            else return string.Format("{0} '{1}'", ((ObjectType)obj.ObjectType).GetTypeName(), obj.Name);
        }

        private static string DeployNewTMSL(TOM.Database db, string newDbID, DeploymentOptions options)
        {
            var rawScript = TOM.JsonScripter.ScriptCreate(db);
            var jObj = JObject.Parse(rawScript);

            jObj["create"]["database"]["id"] = newDbID;
            jObj["create"]["database"]["name"] = newDbID;

            if (!options.DeployRoles)
            {
                // Remove roles if present
                var roles = jObj.SelectToken("create.database.model.roles") as JArray;
                if(roles != null) roles.Clear();
            }

            return jObj.ToString();
        }

        private static string DeployExistingTMSL(TOM.Database db, TOM.Server server, string dbId, DeploymentOptions options)
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

    public class DeploymentResult
    {
        public readonly IReadOnlyList<string> Issues;
        public readonly IReadOnlyList<string> Warnings;
        public readonly IReadOnlyList<string> Unprocessed;
        public DeploymentResult(IEnumerable<string> issues, IEnumerable<string> warnings, IEnumerable<string> unprocessed)
        {
            Issues = issues.ToList();
            Warnings = warnings.ToList();
            Unprocessed = unprocessed.ToList();
        }
    }
    public class DeploymentOptions
    {
        public DeploymentMode DeployMode = DeploymentMode.CreateOrAlter;
        public bool DeployConnections = false;
        public bool DeployPartitions = false;
        public bool DeployRoles = true;
        public bool DeployRoleMembers = false;

        /// <summary>
        /// Default deployment. Does not overwrite connections, partitions or role members.
        /// </summary>
        public static DeploymentOptions Default = new DeploymentOptions();

        /// <summary>
        /// Full deployment.
        /// </summary>
        public static DeploymentOptions Full = new DeploymentOptions() { DeployConnections = true, DeployPartitions = true, DeployRoles = true, DeployRoleMembers = true };

        /// <summary>
        /// StructureOnly deployment. Does not overwrite roles or role members.
        /// </summary>
        public static DeploymentOptions StructureOnly = new DeploymentOptions() { DeployRoles = false };
    }

    public enum DeploymentMode
    {
        CreateDatabase,
        CreateOrAlter
    }
}

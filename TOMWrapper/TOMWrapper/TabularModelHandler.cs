using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TextServices;
using Antlr4.Runtime;
using System.Diagnostics;
using System.Text;

namespace TabularEditor.TOMWrapper
{
    public enum DeploymentStatus
    {
        ChangesSaved,
        DeployComplete,
        DeployCancelled
    }

    public enum AddObjectType
    {
        Measure = 1,
        CalculatedColumn = 2,
        Hierarchy = 3
    }

    public struct Dependency
    {
        public int from;
        public int to;
        public bool fullyQualified;

    }

    public static class DependencyHelper
    {
        static public void AddDep(this IExpressionObject target, IDaxObject dependsOn, int fromChar, int toChar, bool fullyQualified)
        {
            var dep = new Dependency { from = fromChar, to = toChar, fullyQualified = fullyQualified };
            List<Dependency> depList;
            if(!target.Dependencies.TryGetValue(dependsOn, out depList))
            {
                depList = new List<Dependency>();
                target.Dependencies.Add(dependsOn, depList);
            }
            depList.Add(dep);
            if(!dependsOn.Dependants.Contains(target)) dependsOn.Dependants.Add(target);
        }

        /// <summary>
        /// Removes qualifiers such as ' ' and [ ] around a name.
        /// </summary>
        static public string NoQ(this string objectName, bool table = false)
        {
            if(table)
            {
                return objectName.StartsWith("'") ? objectName.Substring(1, objectName.Length - 2) : objectName;
            }
            else
            {
                return objectName.StartsWith("[") ? objectName.Substring(1, objectName.Length - 2) : objectName;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TabularModelHandler: IDisposable
    {
        public const string PROP_HASUNSAVEDCHANGES = "HasUnsavedChanges";
        public const string PROP_ISCONNECTED = "IsConnected";
        public const string PROP_STATUS = "Status";
        public const string PROP_ERRORS = "Errors";

        /// <summary>
        /// Specifies whether object name changes (tables, column, measures) should result in 
        /// automatic DAX expression updates to reflect the changed names. When set to true,
        /// all expressions in the model are parsed, to build a dependency tree.
        /// </summary>
        public bool AutoFixup { get; set; }

        /// <summary>
        /// Changes all references to object "obj", to reflect "newName"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newName"></param>
        public void DoFixup(IDaxObject obj, string newName)
        {
            //foreach (var d in obj.Model.Tables.OfType<IExpressionObject>().Concat(obj.Model.Tables.SelectMany(t => t.GetChildren().OfType<IExpressionObject>())))
            foreach (var d in obj.Dependants.ToList())
            {
                List<Dependency> depList;
                if(d.Dependencies.TryGetValue(obj, out depList))
                {
                    var pos = 0;
                    var sb = new StringBuilder();
                    foreach(var dep in depList)
                    {
                        sb.Append(d.Expression.Substring(pos, dep.from - pos));
                        sb.Append(dep.fullyQualified ? obj.DaxObjectFullName : obj.DaxObjectName);
                        pos = dep.to + 1;
                    }
                    sb.Append(d.Expression.Substring(pos));
                    d.Expression = sb.ToString();
                }
            }
        }

        public void BuildDependencyTree(IExpressionObject expressionObj)
        {
            foreach (var d in expressionObj.Dependencies.Keys) d.Dependants.Remove(expressionObj);
            expressionObj.Dependencies.Clear();

            var tokens = new DAXLexer(new AntlrInputStream(expressionObj.Expression ?? "")).GetAllTokens();

            IToken lastTableRef = null;

            for (var i = 0; i < tokens.Count; i++)
            {
                // TODO: This parsing could be used to check for invalid object references, for example to use in syntax highlighting or validation of expressions

                var tok = tokens[i];
                switch (tok.Type)
                {
                    case DAXLexer.TABLE:
                    case DAXLexer.TABLE_OR_VARIABLE:
                        if (lastTableRef != null)
                        {
                            if (Model.Tables.Contains(lastTableRef.Text.NoQ(true))) expressionObj.AddDep(Model.Tables[lastTableRef.Text.NoQ(true)], lastTableRef.StartIndex, lastTableRef.StopIndex, true);
                        }
                        lastTableRef = tok;
                        break;
                    case DAXLexer.COLUMN_OR_MEASURE:
                        var tableName = lastTableRef?.Text.NoQ(true);

                        // Referencing a table just before the object reference
                        if (tableName != null && Model.Tables.Contains(tableName))
                        {
                            var table = Model.Tables[tableName];
                            // Referencing a column on a specific table
                            if (table.Columns.Contains(tok.Text.NoQ()))
                                expressionObj.AddDep(table.Columns[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                            // Referencing a measure on a specific table
                            else if (table.Measures.Contains(tok.Text.NoQ()))
                                expressionObj.AddDep(table.Measures[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                        }
                        // No table reference before the object reference
                        else
                        {
                            var table = (expressionObj as ITabularTableObject)?.Table;
                            // Referencing a column without specifying a table (assume column in same table):
                            if (table != null && table.Columns.Contains(tok.Text.NoQ()))
                            {
                                expressionObj.AddDep(table.Columns[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                            }
                            // Referencing a measure without specifying a table
                            else
                            {
                                Measure m = null;
                                if (table != null && table.Measures.Contains(tok.Text.NoQ())) m = table.Measures[tok.Text.NoQ()];
                                else
                                    m = Model.Tables.FirstOrDefault(t => t.Measures.Contains(tok.Text.NoQ()))?.Measures[tok.Text.NoQ()];

                                if (m != null)
                                    expressionObj.AddDep(m, tok.StartIndex, tok.StopIndex, false);
                            }
                        }
                        break;
                    case DAXLexer.WHITESPACES:
                        break;
                    default:
                        if (lastTableRef != null)
                        {
                            if (Model.Tables.Contains(lastTableRef.Text.NoQ(true))) expressionObj.AddDep(Model.Tables[lastTableRef.Text.NoQ(true)], lastTableRef.StartIndex, lastTableRef.StopIndex, true);
                            lastTableRef = null;
                        }
                        break;
                }

            }
        }

        public void BuildDependencyTree()
        {
            if (UndoManager.UndoInProgress)
            {
                DelayBuildDependencyTree = true;
                UndoManager.RebuildDependencyTree = true;
            }
            if (DelayBuildDependencyTree) return;
            var sw = new Stopwatch();
            sw.Start();

            foreach(var eo in Model.Tables.SelectMany(t => t.GetChildren()).Concat(Model.Tables).OfType<IExpressionObject>())
            {
                BuildDependencyTree(eo);
            }

            sw.Stop();

            Console.WriteLine("Dependency tree built in {0} ms", sw.ElapsedMilliseconds);
        }

        public bool DelayBuildDependencyTree { get; set; } = false;

        public UndoManager UndoManager { get; private set; }
        public TabularCommonActions Actions { get; private set; }

        private TOM.Server server;
        private TOM.Database database;

        public Model Model { get; private set; }
        public TOM.Database Database { get { return database; } }

        /// <summary>
        /// Scripts the entire database
        /// </summary>
        /// <returns></returns>
        public string ScriptCreateOrReplace()
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(Database);
        }

        /// <summary>
        /// Scripts the object
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public string ScriptCreateOrReplace(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(obj.MetadataObject);
        }

        public string SerializeObjects(IEnumerable<TabularNamedObject> objects)
        {
            var json = "[" + string.Join(",", objects.Select(obj => TOM.JsonSerializer.SerializeObject(obj.MetadataObject))) + "]";
            return json;
        }

        public string ScriptTranslations(IEnumerable<Culture> translations)
        {
            return "{ cultures: " + SerializeObjects(translations) + "}";
        }

        /// <summary>
        /// Applys translation from a JSON string.
        /// </summary>
        /// <param name="culturesJson"></param>
        /// <param name="overwriteExisting"></param>
        /// <param name="ignoreInvalid"></param>
        /// <returns>False if ignoreInvalid is set to false and an invalid object is encountered</returns>
        public bool ImportTranslations(string culturesJson, bool overwriteExisting, bool ignoreInvalid)
        {
            BeginUpdate("Import translations");
            var result = TabularCultureHelper.ImportTranslations(culturesJson, Model, overwriteExisting, !ignoreInvalid);

            // Rolls back translation changes if an error were encountered
            EndUpdate(true, !result);

            return result;
        }

        public IList<TabularNamedObject> DeserializeObjects(string json)
        {
            var result = new List<TabularNamedObject>();

            JArray jArr;
            try
            {
                jArr = JArray.Parse(json);
            }
            catch
            {
                return result;
            }

            foreach (var jObj in jArr)
            {
                TabularNamedObject obj = null;

                if (jObj["type"]?.Value<string>() == "calculated")
                {
                    // Calculated column
                    var tom = TOM.JsonSerializer.DeserializeObject<TOM.CalculatedColumn>(jObj.ToString());
                    obj = new CalculatedColumn(this, tom);
                }
                else if (jObj["expression"] != null)
                {
                    // Measure
                    var tom = TOM.JsonSerializer.DeserializeObject<TOM.Measure>(jObj.ToString());
                    obj = new Measure(this, tom);
                }

                if(obj != null) result.Add(obj);
            }

            return result;
        }

        private void Init()
        {
            if (database.CompatibilityLevel > 1200) throw new InvalidOperationException("This version of Tabular Editor only supports Tabular databases of Compatibility Level 1200.\n\nTo edit databases of newer compatibility levels, please download Tabular Editor for SQL Server 2017.");
            if (database.CompatibilityLevel < 1200) throw new InvalidOperationException("Tabular Databases of compatibility level 1100 or 1103 are not supported in Tabular Editor.");
            UndoManager = new UndoFramework.UndoManager(this);
            Actions = new TabularCommonActions(this);
            Model = new Model(this, database.Model);
            Model.Database = new Database(database);
            Model.LoadChildObjects();
            CheckErrors();

            BuildDependencyTree();
        }

        internal readonly Dictionary<string, ITabularObjectCollection> WrapperCollections = new Dictionary<string, ITabularObjectCollection>();
        internal readonly Dictionary<TOM.MetadataObject, TabularObject> WrapperLookup = new Dictionary<TOM.MetadataObject, TabularObject>();

        /// <summary>
        /// Loads an Analysis Services tabular database (Compatibility Level 1200 or newer) from a file
        /// or folder.
        /// </summary>
        /// <param name="path"></param>
        public TabularModelHandler(string path)
        {
            Singleton = this;
            server = null;

            var fi = new FileInfo(path);
            string data;
            if (!fi.Exists || fi.Name == "database.json")
            {
                if (fi.Name == "database.json") data = CombineFolderJson(fi.DirectoryName);
                else if (Directory.Exists(path)) data = CombineFolderJson(path);
                else throw new FileNotFoundException();
            } else
            {
                data = File.ReadAllText(path);
            }
            database = TOM.JsonSerializer.DeserializeDatabase(data);

            Status = "File loaded succesfully.";
            Init();
        }

        /// <summary>
        /// Connects to a SQL Server 2016 Analysis Services instance and loads a tabular model
        /// from one of the deployed databases on the instance.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        public TabularModelHandler(string serverName, string databaseName)
        {
            Singleton = this;
            server = new TOM.Server();
            server.Connect(serverName);
            
            if(databaseName == null)
            {
                if (server.Databases.Count >= 1) database = server.Databases[0];
                else throw new InvalidOperationException("This instance does not contain any databases, or the user does not have access.");
            }
            else
            {
                database = server.Databases[databaseName];
            }

            if (database.CompatibilityLevel < 1200) throw new InvalidOperationException("Only databases with Compatibility Level 1200 or higher can be loaded in Tabular Editor.");

            Status = "Connected succesfully.";
            Version = database.Version;
            Init();
        }

        internal static TabularModelHandler Singleton { get; private set; }

        public bool IsConnected { get { return Version != -1; } }
        public long Version { get; private set; } = -1;

        public bool HasUnsavedChanges
        {
            get
            {
                return !UndoManager.AtCheckpoint;
            }
        }

        private string _status;
        public string Status { get { return _status; } set { _status = value; } }

        public Model GetModel()
        {
            return this.Model;
        }

        public void SaveFile(string fileName)
        {
            var dbcontent = TOM.JsonSerializer.SerializeDatabase(database);
            File.WriteAllText(fileName, dbcontent);

            Status = "File saved.";
            if (!IsConnected) UndoManager.SetCheckpoint();
        }

        public IList<Tuple<TOM.NamedMetadataObject, string>> Errors { get; private set; }

        public struct ConflictInfo {
            public long DatabaseVersion;
            public long LoadedVersion;
            public DateTime DatabaseLastUpdate;
            public bool Conflict;
        }

        public ConflictInfo CheckConflicts()
        {
            if (database == null || database?.Server == null) return new ConflictInfo { DatabaseVersion = -1 };
            var s = new TOM.Server();
            s.Connect(database?.Server.ConnectionString);
            var db = s.Databases[database?.Name];
            return new ConflictInfo {
                DatabaseVersion = db.Version,
                LoadedVersion = Version,
                DatabaseLastUpdate = db.LastUpdate,
                Conflict = db.Version != Version
            };
        }
        
        /// <summary>
        /// Saves the changes to the database. It is the users responsibility to check if changes were made
        /// to the database since it was loaded to the TOMWrapper. You can use Handler.CheckConflicts() for
        /// this purpose.
        /// </summary>
        public void SaveDB()
        {
            if (database?.Server == null || Version == -1)
            {
                throw new InvalidOperationException("The model is currently not connected to any server. Please use Deploy() instead of SaveDB().");
            }

            UndoManager.Enabled = false;
            DetachCalculatedTableMetadata();
            try
            {
                // TODO: Deleting a column with IsKey = true, then undoing, then saving causes an error... wWTF?!?
                database.Model.SaveChanges();
            }
            catch(Exception ex)
            {
                Status = "Save to DB error!";
                throw ex;
            }
            finally
            {
                AttachCalculatedTableMetadata();
                UndoManager.Enabled = true;
            }

            Version = CheckConflicts().DatabaseVersion;

            if (IsConnected) UndoManager.SetCheckpoint();

            Status = "Changes saved.";
            CheckErrors();
        }

        /// <summary>
        /// Temporarily removes all perspective and translation information from the CalculatedTableColumns of a
        /// CalculatedTable that has had its expression changed (NeedsValidation = true). Otherwise, we may get
        /// errors when deploying the model, if the CalculatedTable expression have changed such that one or more
        /// of these columns are removed.
        /// </summary>
        private void DetachCalculatedTableMetadata()
        {
            CTCMetadataBackup = new List<CTCMetadata>();

            foreach(var ctc in Model.Tables.OfType<CalculatedTable>().Where(ctc => ctc.NeedsValidation).SelectMany(t => t.Columns.OfType<CalculatedTableColumn>()))
            {
                CTCMetadataBackup.Add(new CTCMetadata(ctc));
                ctc.InPerspective.None();
                ctc.TranslatedNames.Clear();
                ctc.TranslatedDisplayFolders.Clear();
                ctc.TranslatedDescriptions.Clear();
            }
        }

        private List<CTCMetadata> CTCMetadataBackup;

        private class CTCMetadata {
            public CTCMetadata(CalculatedTableColumn ctc)
            {
                TableName = ctc.Table.Name;
                ColumnName = ctc.Name;

                InPerspective = ctc.InPerspective.Copy();
                TranslatedNames = ctc.TranslatedNames.Copy();
                TranslatedDisplayFolders = ctc.TranslatedDisplayFolders.Copy();
                TranslatedDescriptions = ctc.TranslatedDescriptions.Copy();
            }

            public string TableName;
            public string ColumnName;
            public Dictionary<string, bool> InPerspective;
            public Dictionary<string, string> TranslatedNames;
            public Dictionary<string, string> TranslatedDisplayFolders;
            public Dictionary<string, string> TranslatedDescriptions;
        }

        /// <summary>
        /// Reattaches any metadata removed from CalculatedTableColumns that are still present (by name) after
        /// succesful deployment.
        /// </summary>
        private void AttachCalculatedTableMetadata()
        {
            foreach (var ct in Model.Tables.OfType<CalculatedTable>()) ct.ReinitColumns();

            foreach(var ctcbackup in CTCMetadataBackup)
            {
                if(Model.Tables.Contains(ctcbackup.TableName))
                {
                    var t = Model.Tables[ctcbackup.TableName];
                    if (t.Columns.Contains(ctcbackup.ColumnName))
                    {
                        var ctc = t.Columns[ctcbackup.ColumnName];
                        ctc.InPerspective.CopyFrom(ctcbackup.InPerspective);
                        ctc.TranslatedNames.CopyFrom(ctcbackup.TranslatedNames);
                        ctc.TranslatedDisplayFolders.CopyFrom(ctcbackup.TranslatedDisplayFolders);
                        ctc.TranslatedDescriptions.CopyFrom(ctcbackup.TranslatedDescriptions);
                    }
                }
            }
        }

        private string CombineFolderJson(string path)
        {
            if (!File.Exists(path + "\\database.json")) throw new FileNotFoundException("This folder does not contain a database.json file");

            var jobj = JObject.Parse(File.ReadAllText(path + "\\database.json"));
            var model = jobj["model"] as JObject;

            InArray(path, "dataSources", model);
            var tables = new JArray();
            foreach (var tablePath in Directory.GetDirectories(path + "\\tables"))
            {
                var tableName = new DirectoryInfo(tablePath).Name;
                var table = JObject.Parse(File.ReadAllText(string.Format("{0}\\{1}.json", tablePath, tableName)));
                InArray(tablePath, "columns", table);
                InArray(tablePath, "partitions", table);
                InArray(tablePath, "measures", table);
                InArray(tablePath, "hierarchies", table);
                InArray(tablePath, "annotations", table);
                tables.Add(table);
            }
            model.Add("tables", tables);
            InArray(path, "relationships", model);
            InArray(path, "cultures", model);
            InArray(path, "perspectives", model);
            InArray(path, "roles", model);

            return jobj.ToString();
        }

        private void InArray(string path, string arrayName, JObject baseObject)
        {
            var array = new JArray();
            if (Directory.Exists(path + "\\" + arrayName))
            {
                foreach (var file in Directory.GetFiles(path + "\\" + arrayName, "*.json"))
                {
                    array.Add(JObject.Parse(File.ReadAllText(file)));
                }
            }
            baseObject.Add(arrayName, array);
        }

        private HashSet<string> CurrentFiles;

        public void SaveToFolder(string path)
        {
            var json = TOM.JsonSerializer.SerializeDatabase(database, new TOM.SerializeOptions() { IgnoreInferredObjects = true, IgnoreTimestamps = true, IgnoreInferredProperties = true });
            var jobj = JObject.Parse(json);

            var model = jobj["model"] as JObject;
            var dataSources = PopArray(model, "dataSources");
            var tables = PopArray(model, "tables");
            var relationships = PopArray(model, "relationships");
            var cultures = PopArray(model, "cultures");
            var perspectives = PopArray(model, "perspectives");
            var roles = PopArray(model, "roles");

            CurrentFiles = new HashSet<string>();
            WriteIfChanged(path + "\\database.json", jobj.ToString(Newtonsoft.Json.Formatting.Indented));

            if (relationships != null) OutArray(path, "relationships", relationships);
            if (perspectives != null) OutArray(path, "perspectives", perspectives);
            if (cultures != null) OutArray(path, "cultures", cultures);
            if (dataSources != null) OutArray(path, "dataSources", dataSources);
            if (roles != null) OutArray(path, "roles", roles);

            if (tables != null)
            {
                foreach (JObject t in tables)
                {
                    var columns = PopArray(t, "columns");
                    var partitions = PopArray(t, "partitions");
                    var measures = PopArray(t, "measures");
                    var hierarchies = PopArray(t, "hierarchies");
                    var annotations = PopArray(t, "annotations");

                    var tableName = t["name"].ToString().Replace("\\", "_").Replace("/", "_");
                    var p = path + "\\tables\\" + tableName + "\\" + tableName + ".json";
                    var fi = new FileInfo(p);
                    if (!fi.Directory.Exists) fi.Directory.Create();
                    WriteIfChanged(p, t.ToString(Newtonsoft.Json.Formatting.Indented));

                    var table = Model.Tables[t["name"].ToString()].MetadataObject;

                    if (measures != null) OutArray(path + "\\tables\\" + tableName, "measures", measures);
                    if (columns != null) OutArray(path + "\\tables\\" + tableName, "columns", columns);
                    if (hierarchies != null) OutArray(path + "\\tables\\" + tableName, "hierarchies", hierarchies);
                    if (partitions != null) OutArray(path + "\\tables\\" + tableName, "partitions", partitions);
                    if (annotations != null) OutArray(path + "\\tables\\" + tableName, "annotations", annotations);
                }
            }

            RemoveUnusedFiles(path);
            Status = "Model saved.";
            if (!IsConnected) UndoManager.SetCheckpoint();

        }

        private void RemoveUnusedFiles(string path)
        {
            foreach (var f in Directory.GetFiles(path, "*.json"))
            {
                if (!CurrentFiles.Contains(f, StringComparer.InvariantCultureIgnoreCase))
                    File.Delete(f);
            }

            foreach (var d in Directory.GetDirectories(path))
            {
                RemoveUnusedFiles(d);
                if (!Directory.EnumerateFileSystemEntries(d).Any()) Directory.Delete(d);
            }
        }


        /// <summary>
        /// Writes textual data to a file, but only if the file does not already contain the exact same text.
        /// Automatically creates a directory for the file, if it doesn't already exist.
        /// </summary>
        private void WriteIfChanged(string path, string content)
        {
            CurrentFiles.Add(path);
            var fi = new FileInfo(path);
            if (!fi.Directory.Exists) fi.Directory.Create();
            else if(fi.Exists)
            {
                var s = File.ReadAllText(path);
                if (content.Equals(s, StringComparison.InvariantCulture)) return;
            }
            File.WriteAllText(path, content);
        }

        private void OutArray(string path, string arrayName, JArray array)
        {
            foreach (var t in array)
            {
                var p = path + "\\" + arrayName + "\\" + t["name"].ToString().Replace("\\","_").Replace("/","_") + ".json";
                var fi = new FileInfo(p);
                if (!fi.Directory.Exists) fi.Directory.Create();
                WriteIfChanged(p, t.ToString(Newtonsoft.Json.Formatting.Indented));
            }
        }

        private JArray PopArray(JObject obj, string arrayName)
        {
            var result = obj[arrayName] as JArray;
            obj.Remove(arrayName);
            return result;
        }

        public IDetailObject Add(AddObjectType objectType, IDetailObjectContainer container)
        {
            var table = container as Table ?? (container as IDetailObject).Table;
            var df = (container as Folder)?.Path;
            IDetailObject result;

            UndoManager.BeginBatch("add " + objectType.ToString().SplitCamelCase().ToLower());
            switch(objectType)
            {
                case AddObjectType.Measure:
                    result = new Measure(table);
                    break;

                case AddObjectType.CalculatedColumn:
                    result = new CalculatedColumn(table);
                    break;

                case AddObjectType.Hierarchy:
                    result = new Hierarchy(table);
                    break;

                default:
                    throw new NotImplementedException();
            }

            result.DisplayFolder = df;
            UndoManager.EndBatch();

            return result;
        }

        internal static List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>> CheckProcessingState(TOM.Database database)
        {
            var result = new List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>>();

            // Find partitions that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Partitions).Where(p => p.State != TOM.ObjectState.Ready && p.State != TOM.ObjectState.NoData)
                    .Select(p => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(p, p.State))
                    );

            // Find calculated columns that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Columns.OfType<TOM.CalculatedColumn>()).Where(c => c.State != TOM.ObjectState.Ready && c.State != TOM.ObjectState.NoData)
                    .Select(c => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(c, c.State))
                );

            return result;
        }

        internal static List<Tuple<TOM.NamedMetadataObject, string>> CheckErrors(TOM.Database database)
        {
            var result = new List<Tuple<TOM.NamedMetadataObject, string>>();
            foreach (var t in database.Model.Tables)
            {
                result.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(m => new Tuple<TOM.NamedMetadataObject, string>(m, m.ErrorMessage)));
                result.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(c => new Tuple<TOM.NamedMetadataObject, string>(c, c.ErrorMessage)));
                result.AddRange(t.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)).Select(p => new Tuple<TOM.NamedMetadataObject, string>(p, p.ErrorMessage)));
            }
            return result;
        }

        private void CheckErrors()
        {
            var errorList = new List<Tuple<TOM.NamedMetadataObject, string>>();

            foreach (var t in database.Model.Tables)
            {
                errorList.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(m => new Tuple<TOM.NamedMetadataObject, string>(m, m.ErrorMessage)));
                errorList.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(c => new Tuple<TOM.NamedMetadataObject, string>(c, c.ErrorMessage)));
                errorList.AddRange(t.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)).Select(p => new Tuple<TOM.NamedMetadataObject, string>(p, p.ErrorMessage)));

                var table = (WrapperLookup[t] as Table);
                
                table?.CheckChildrenErrors();
                WrapperLookup.Values.OfType<IExpressionObject>().ToList().ForEach(i => i.NeedsValidation = false);
            }
            if (errorList.Count > 0 || Errors?.Count > 0)
            {
                Errors = errorList;
            }
        }

        public void BeginUpdate(string undoName)
        {
            Tree.BeginUpdate();
            if(!string.IsNullOrEmpty(undoName)) UndoManager.BeginBatch(undoName);
        }

        public int EndUpdate(bool undoable = true, bool rollback = false)
        {
            var actionCount = 0;
            if(undoable || rollback) actionCount = UndoManager.EndBatch(rollback);
            Tree.EndUpdate();

            return actionCount;
        }
        public int EndUpdateAll(bool rollback = false)
        {
            var actionCount = 0;
            while (UndoManager.BatchDepth > 0)
            {
                actionCount = UndoManager.EndBatch(rollback);
            }
            Tree.EndUpdate();
            return actionCount;
        }


        internal void UpdateObject(ITabularObject obj)
        {

            Tree.OnNodesChanged(obj);
        }

        internal void UpdateTables()
        {
            if (Tree.Options.HasFlag(LogicalTreeOptions.AllObjectTypes))
                Tree.OnStructureChanged(Model.GroupTables);
            else
                Tree.OnStructureChanged(Model);
        }

        internal void UpdateFolders(Table table)
        {
            Tree.OnStructureChanged(table);
        }
        internal void UpdateLevels(Hierarchy hierarchy)
        {
            Tree.OnStructureChanged(hierarchy);
        }

        public void Dispose()
        {
            if(server != null)
            {
                server.Dispose();
            }
        }

        public TabularTree Tree { get; set; }
    }
}
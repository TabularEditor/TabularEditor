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

        public void DoFixup(IDaxObject obj, string newName)
        {
            foreach (var d in obj.Model.Tables.OfType<IExpressionObject>().Concat(obj.Model.Tables.SelectMany(t => t.GetChildren().OfType<IExpressionObject>())))
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
            expressionObj.Dependencies.Clear();

            var tokens = new DAXLexer(new AntlrInputStream(expressionObj.Expression)).GetAllTokens();

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
            UndoManager = new UndoFramework.UndoManager(this);
            Actions = new TabularCommonActions(this);
            Model = new Model(this, database.Model);
            Model.LoadChildObjects();
            CheckErrors();

            BuildDependencyTree();
        }

        internal readonly Dictionary<string, ITabularObjectCollection> WrapperCollections = new Dictionary<string, ITabularObjectCollection>();
        internal readonly Dictionary<TOM.MetadataObject, TabularObject> WrapperLookup = new Dictionary<TOM.MetadataObject, TabularObject>();

        /// <summary>
        /// Loads an Analysis Services Tabular Model (Compatibility Level 1200) from a file.
        /// </summary>
        /// <param name="fileName"></param>
        public TabularModelHandler(string fileName)
        {
            Singleton = this;
            server = null;
            try
            {
                database = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(fileName));
            }
            catch
            {
                throw new InvalidOperationException("This does not appear to be a valid Compatibility Level 1200 (or newer) Model.bim file.");
            }
            Status = "File loaded succesfully.";

            Init();
        }

        public TabularModelHandler(string path, bool fromFolder)
        {
            Singleton = this;
            server = null;
            try
            {
                var s = CombineFolderJson(path);
                database = TOM.JsonSerializer.DeserializeDatabase(s);
            }
            catch
            {

            }
            Status = "File loaded succsefully.";
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

            database.Model.SaveChanges();

            Version = CheckConflicts().DatabaseVersion;

            if (IsConnected) UndoManager.SetCheckpoint();

            Status = "Changes saved.";
            CheckErrors();
        }

        private string CombineFolderJson(string path)
        {
            // TODO: Combine JSON from files, then load entire database
            //var database = JObject.Parse()
            return "";
        }

        public void SaveToFolder(string path)
        {
            var json = TOM.JsonSerializer.SerializeDatabase(database, new TOM.SerializeOptions() { IgnoreInferredObjects = true, IgnoreTimestamps = true, IgnoreInferredProperties = true });
            var jobj = JObject.Parse(json);

            var model = jobj["model"] as JObject;
            var tables = PopArray(model, "tables");
            var relationships = PopArray(model, "relationships");
            var perspectives = PopArray(model, "perspectives");
            var cultures = PopArray(model, "cultures");
            var dataSources = PopArray(model, "dataSources");
            var roles = PopArray(model, "roles");

            WriteIfChanged(path + "\\database.json", jobj.ToString(Newtonsoft.Json.Formatting.Indented));

            OutArray(path, "relationships", relationships);
            OutArray(path, "perspectives", perspectives);
            OutArray(path, "cultures", cultures);
            OutArray(path, "dataSources", dataSources);
            OutArray(path, "roles", roles);

            foreach(JObject t in tables)
            {
                var measures = PopArray(t, "measures");
                var columns = PopArray(t, "columns");
                var hierarchies = PopArray(t, "hierarchies");
                var partitions = PopArray(t, "partitions");

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
            }
        }

        /// <summary>
        /// Writes textual data to a file, but only if the file does not already contain the exact same text.
        /// Automatically creates a directory for the file, if it doesn't already exist.
        /// </summary>
        private void WriteIfChanged(string path, string content)
        {
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

        private void CheckErrors()
        {
            var errorList = new List<Tuple<TOM.NamedMetadataObject, string>>();

            foreach (var t in database.Model.Tables)
            {
                errorList.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(m => new Tuple<TOM.NamedMetadataObject, string>(m, m.ErrorMessage)));
                errorList.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(c => new Tuple<TOM.NamedMetadataObject, string>(c, c.ErrorMessage)));

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
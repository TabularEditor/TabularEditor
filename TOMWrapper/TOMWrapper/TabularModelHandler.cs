using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.IO.Compression;
using Newtonsoft.Json.Linq;

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

    /// <summary>
    /// 
    /// </summary>
    public sealed class TabularModelHandler: IDisposable
    {
        public const string PROP_HASUNSAVEDCHANGES = "HasUnsavedChanges";
        public const string PROP_ISCONNECTED = "IsConnected";
        public const string PROP_STATUS = "Status";
        public const string PROP_ERRORS = "Errors";

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
        /// Scripts the table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string ScriptCreateOrReplace(Table table)
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(table.MetadataObject);
        }

        public string SerializeObjects(IEnumerable<TabularNamedObject> objects)
        {
            var json = "[" + string.Join(",", objects.Select(obj => TOM.JsonSerializer.SerializeObject(obj.MetadataObject))) + "]";
            return json;
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
        }

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
                throw new InvalidOperationException("This does not appear to be a valid Compatibility Level 1200 Model.bim file.");
            }
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
            

            database = server.Databases[databaseName];
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

        public void WriteZip(string fileName)
        {
            var content = TOM.JsonSerializer.SerializeDatabase(database);

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

        public void SaveDB()
        {
            if (database?.Server == null || Version == -1)
            {
                throw new InvalidOperationException("The model is currently not connected to any server. Please use Deploy() instead of SaveDB().");
            }

            // TODO: Consider using Database.Update( ... ) instead of SaveChanges(), to handle conflicting situations. For example,
            // if a measure was already added to the server, to avoid getting an exception, as SaveChanges will try to script "Create"
            // the measure that was added in Tabular Editor.
            database.Model.SaveChanges();

            Version = CheckConflicts().DatabaseVersion;

            if (IsConnected) UndoManager.SetCheckpoint();

            Status = "Changes saved.";
            CheckErrors();
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
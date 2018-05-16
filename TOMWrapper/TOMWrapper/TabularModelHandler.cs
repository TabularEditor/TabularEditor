extern alias json;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using json.Newtonsoft.Json.Linq;
using TabularEditor.TextServices;
using Antlr4.Runtime;
using System.Diagnostics;
using System.Text;
using json.Newtonsoft.Json;
using System.ComponentModel;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.Utils;

namespace TabularEditor.TOMWrapper
{

    public class ObjectChangedEventArgs
    {
        public TabularObject TabularObject { get; private set; }
        public string PropertyName { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public ObjectChangedEventArgs(TabularObject tabularObject, string propertyName, object oldValue, object newValue)
        {
            TabularObject = tabularObject;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ObjectChangingEventArgs
    {
        public bool Cancel { get; set; } = false;
        public TabularObject TabularObject { get; private set; }
        public string PropertyName { get; private set; }
        public object NewValue { get; private set; }

        public ObjectChangingEventArgs(TabularObject tabularObject, string propertyName, object newValue)
        {
            TabularObject = tabularObject;
            PropertyName = propertyName;
            NewValue = newValue;
            Cancel = false;
        }
    }

    public class ObjectDeletingEventArgs
    {
        public bool Cancel { get; set; } = false;
        public TabularObject TabularObject { get; private set; }
        public ObjectDeletingEventArgs(TabularObject tabularObject)
        {
            TabularObject = tabularObject;
        }
    }

    public class ObjectDeletedEventArgs
    {
        public TabularObject TabularObject { get; private set; }
        public ObjectDeletedEventArgs(TabularObject tabularObject)
        {
            TabularObject = tabularObject;
        }
    }

    public delegate void ObjectChangingEventHandler(object sender, ObjectChangingEventArgs e);
    public delegate void ObjectChangedEventHandler(object sender, ObjectChangedEventArgs e);
    public delegate void ObjectDeletingEventHandler(object sender, ObjectDeletingEventArgs e);
    public delegate void ObjectDeletedEventHandler(object sender, ObjectDeletedEventArgs e);

    public enum SaveFormat
    {
        /// <summary>
        /// Saves only the Model Schema as a Model.bim file
        /// </summary>
        ModelSchemaOnly,

        /// <summary>
        /// Saves the Model Schema to an existing .pbit (Power BI Template) file
        /// </summary>
        PowerBiTemplate,

        /// <summary>
        /// Saves the Model Schema together with a Visual Studio Tabular Project file and user settings file
        /// </summary>
        VisualStudioProject,

        /// <summary>
        /// Saves the Model Schema as a Tabular Editor folder structure
        /// </summary>
        TabularEditorFolder
    }

    public enum ModelSourceType
    {
        /// <summary>
        /// SSAS Tabular database Compatibility Level 1200 or 1400
        /// </summary>
        Database,

        /// <summary>
        /// Model.bim Compatibility Level 1200 or 1400 JSON file
        /// </summary>
        File,

        /// <summary>
        /// Model.bim exploded into a folder structure by Tabular Editor
        /// </summary>
        Folder,

        /// <summary>
        /// Power BI Template file (.pbit)
        /// </summary>
        Pbit
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TabularModelHandler: IDisposable
    {
        internal Guid InstanceID = Guid.NewGuid();
        public const string PROP_HASUNSAVEDCHANGES = "HasUnsavedChanges";
        public const string PROP_ISCONNECTED = "IsConnected";
        public const string PROP_STATUS = "Status";
        public const string PROP_ERRORS = "Errors";

        private TabularModelHandlerSettings _settings = TabularModelHandlerSettings.Default;
        public TabularModelHandlerSettings Settings
        {
            get
            { return _settings; }
            set
            {
                _settings = value;
                _tree?.OnStructureChanged();
            }
        }

        public UndoManager UndoManager { get; private set; }
        public TabularCommonActions Actions { get; private set; }

        private TOM.Server server;
        private TOM.Database database;

        public Model Model { get; private set; }
        public TOM.Database Database { get { return database; } }

        public int CompatibilityLevel { get; private set; }

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

        private void Init()
        {
            UndoManager = new UndoManager(this);
            Actions = new TabularCommonActions(this);
            Model = Model.CreateFromMetadata(database.Model);
            Model.Database = new Database(database);
            CheckErrors();

            FormulaFixup.BuildDependencyTree();
        }

        internal readonly Dictionary<string, ITabularObjectCollection> WrapperCollections = new Dictionary<string, ITabularObjectCollection>();
        public TabularObject GetWrapperObject(TOM.MetadataObject obj) { return WrapperLookup[obj]; }
        internal readonly Dictionary<TOM.MetadataObject, TabularObject> WrapperLookup = new Dictionary<TOM.MetadataObject, TabularObject>();

        public ModelSourceType SourceType { get; private set; }
        public string Source { get; private set; }

        /// <summary>
        /// Creates a new blank Tabular Model
        /// </summary>
        public TabularModelHandler(int compatibilityLevel = 1200, TabularModelHandlerSettings settings = null)
        {
            Settings = settings ?? TabularModelHandlerSettings.Default;

            Singleton = this;
            server = null;

            database = new TOM.Database("New Tabular Database") { CompatibilityLevel = compatibilityLevel };
            CompatibilityLevel = compatibilityLevel;
            database.Model = new TOM.Model();

            SourceType = ModelSourceType.File;
            Source = "Model.bim";

            Status = "Succesfully created new model.";
            Init();
        }

        // TODO: Make public setter - remove options for setting this elsewhere
        public SerializeOptions SerializeOptions { get; private set; } = SerializeOptions.Default;

        /// <summary>
        /// Gets a value that indicates whether Power BI feature restriction is enforced for the
        /// currently loaded model. When this is TRUE, some properties are read-only and certain
        /// object types cannot be created/deleted.
        /// </summary>
        public bool UsePowerBIGovernance
        {
            get
            {
                return (SourceType == ModelSourceType.Pbit || Database?.Server?.ServerMode == Microsoft.AnalysisServices.ServerMode.SharePoint) && Settings.PBIFeaturesOnly;
            }
        }

        private PowerBiTemplate pbit;

        /// <summary>
        /// Loads an Analysis Services tabular database (Compatibility Level 1200 or newer) from a file
        /// or folder.
        /// </summary>
        /// <param name="path"></param>
        public TabularModelHandler(string path, TabularModelHandlerSettings settings = null)
        {
            Settings = settings ?? TabularModelHandlerSettings.Default;

            Singleton = this;
            server = null;

            var fi = new FileInfo(path);
            string data;

            if (fi.Exists && fi.Extension == ".pbit")
            {
                pbit = new PowerBiTemplate(path);
                data = pbit.ModelJson;
                SourceType = ModelSourceType.Pbit;
                Source = path;
            }
            else
                        
            if (!fi.Exists || fi.Name == "database.json")
            {
                if (fi.Name == "database.json") path = fi.DirectoryName;

                if (Directory.Exists(path)) data = CombineFolderJson(path);
                else throw new FileNotFoundException();

                SourceType = ModelSourceType.Folder;
                Source = path;
            } else
            {
                data = File.ReadAllText(path);
                SourceType = ModelSourceType.File;
                Source = path;
            }
            database = TOM.JsonSerializer.DeserializeDatabase(data);
            CompatibilityLevel = database.CompatibilityLevel;

            Status = "File loaded succesfully.";
            Init();

            var serializeOptionsAnnotation = Model.GetAnnotation("TabularEditor_SerializeOptions");
            if (serializeOptionsAnnotation != null) SerializeOptions = JsonConvert.DeserializeObject<SerializeOptions>(serializeOptionsAnnotation);

            // Check if translations / perspectives are stored locally in the model:
            if (SourceType == ModelSourceType.Folder && (SerializeOptions.LocalTranslations || SerializeOptions.LocalPerspectives))
            {
                UndoManager.Enabled = false;
                BeginUpdate("Apply translations and perspectives from annotations");

                var translationsJson = Model.GetAnnotation("TabularEditor_Cultures");
                if (SerializeOptions.LocalTranslations && translationsJson != null)
                {
                    Model.Cultures.FromJson(translationsJson);

                    foreach (var item in AllTranslatableObjects) item.LoadTranslations();
                }

                var perspectivesJson = Model.GetAnnotation("TabularEditor_Perspectives");
                if (SerializeOptions.LocalPerspectives && perspectivesJson != null)
                {
                    Model.Perspectives.FromJson(perspectivesJson);

                    foreach (var item in AllPerspectiveObjects) item.LoadPerspectives();
                }

                EndUpdate();
                UndoManager.Enabled = true;
            }
        }

        /// <summary>
        /// Connects to a SQL Server 2016 Analysis Services instance and loads a tabular model
        /// from one of the deployed databases on the instance.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseId"></param>
        public TabularModelHandler(string serverName, string databaseId, TabularModelHandlerSettings settings = null)
        {
            Settings = settings ?? TabularModelHandlerSettings.Default;

            Singleton = this;
            server = new TOM.Server();
            server.Connect(serverName);
            
            if(databaseId == null)
            {
                if (server.Databases.Count >= 1) database = server.Databases[0];
                else throw new InvalidOperationException("This instance does not contain any databases, or the user does not have access.");
            }
            else
            {
                database = server.Databases[databaseId];
            }
            CompatibilityLevel = database.CompatibilityLevel;

            if (CompatibilityLevel < 1200) throw new InvalidOperationException("Only databases with Compatibility Level 1200 or higher can be loaded in Tabular Editor.");

            SourceType = ModelSourceType.Database;
            Source = database.Server.Name + "." + database.Name;

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

        public void Save(string path, SaveFormat format, SerializeOptions options, bool useAnnotatedSerializeOptions = false)
        {
            if (useAnnotatedSerializeOptions)
            {
                var annotatedSerializeOptions = Model.GetAnnotation("TabularEditor_SerializeOptions");
                if (annotatedSerializeOptions != null) options = JsonConvert.DeserializeObject<SerializeOptions>(annotatedSerializeOptions);
            }
            if (options == null) throw new ArgumentNullException("options");
            if(format != SaveFormat.PowerBiTemplate) Model.SetAnnotation("TabularEditor_SerializeOptions", JsonConvert.SerializeObject(options), false);

            switch (format)
            {
                case SaveFormat.ModelSchemaOnly:
                    SaveFile(path, options);
                    break;
                case SaveFormat.PowerBiTemplate:
                    SavePbit(path);
                    break;
                case SaveFormat.TabularEditorFolder:
                    SaveFolder(path, options);
                    break;

                case SaveFormat.VisualStudioProject:
                    // TODO
                    throw new NotImplementedException();
                    // break;
            }
        }

        private void SavePbit(string fileName)
        {
            if (SourceType != ModelSourceType.Pbit || pbit == null)
            {
                Status = "Save failed!";
                throw new InvalidOperationException("Tabular Editor cannot currently convert an Analysis Services Tabular model to a Power BI Template. Please choose a different save format.");
            }            

            var dbcontent = Serializer.SerializeDB(SerializeOptions.PowerBi);

            // Save to .pbit file:
            pbit.SetModelJson(dbcontent);
            pbit.SaveAs(fileName);

            Status = "File saved.";
            if (!IsConnected) UndoManager.SetCheckpoint();
        }

        private void SaveFile(string fileName, SerializeOptions options)
        {
            var dbcontent = Serializer.SerializeDB(options);
            (new FileInfo(fileName)).Directory.Create();

            // Save to Model.bim:
            File.WriteAllText(fileName, dbcontent);

            Status = "File saved.";
            if (!IsConnected) UndoManager.SetCheckpoint();
        }

        public IList<IErrorMessageObject> Errors { get; private set; }

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
            var db = s.Databases[database?.ID];
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

            _disableUpdates = true;
            UndoManager.Enabled = false;

            var sw = Stopwatch.StartNew();

            // TODO: The current detach/attach functionality for calculated tables is not good enough. A better approach is this:
            // Upon saving the model (this method), for each calculated table that needs validation (= has its expression changed),
            // a new calculated table is created, using the new DAX expression, where as the original DAX expression is kept on the
            // original table. After saving, the new table will hold all columns produced by the DAX, which we can then use to check
            // if columns on the original table are still valid. If the old table holds columns that do not appear in the new table,
            // we can show a warning to the user - for example in cases where those columns have translations, perspectives or are
            // used in hierarchies, as SortByColumn properties, etc...

            //DetachCalculatedTableMetadata();
            try
            {
                // TODO: Deleting a column with IsKey = true, then undoing, then saving causes an error... Check if this is still the case.
                database.Model.SaveChanges();

                AttachCalculatedTableMetadata();

                // If reattaching Calculated Table metadata caused local changes, let's save the model again:
                //if (database.Model.HasLocalChanges) database.Model.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = "Save to DB error!";
                throw ex;
            }
            finally
            {
                UndoManager.Enabled = true;
                _disableUpdates = false;
            }

            sw.Stop();
            Console.WriteLine("Save took: {0} ms", sw.ElapsedMilliseconds);

            Version = CheckConflicts().DatabaseVersion;

            if (IsConnected) UndoManager.SetCheckpoint();

            Status = "Changes saved.";
            CheckErrors();
        }

        /// <summary>
        /// Temporarily removes all perspective and translation information from the CalculatedTableColumns of
        /// all CalculatedTables in the model. Otherwise, we may get errors when deploying the model, if the
        /// CalculatedTable expression or the source objects have changed such that one or more columns on the
        /// CalculatedTable is removed.
        /// </summary>
        private void DetachCalculatedTableMetadata()
        {
            CTCMetadataBackup = new List<CTCBackup>();

            foreach (var ct in Model.Tables.OfType<CalculatedTable>())
            {
                foreach (var ctc in ct.Columns.OfType<CalculatedTableColumn>())
                {
                    var columnBackup = CTCBackup.BackupColumn(ctc);

                    CTCMetadataBackup.Add(columnBackup);
                }
            }

            // TODO: See issue https://github.com/otykier/TabularEditor/issues/110
            // To handle all possible use cases, we should also clear all SortByColumn properties, Variations
            // Levels in hierarchies, Relationships, and any other properties that reference columns on the
            // calculated table, since we cannot know if these columns will still exist after the model is
            // saved.
        }

        private List<CTCBackup> CTCMetadataBackup;

        /// <summary>
        /// Reattaches any metadata removed from CalculatedTableColumns that are still present (by name) after
        /// succesful deployment.
        /// </summary>
        private void AttachCalculatedTableMetadata()
        {
            foreach (var ct in Model.Tables.OfType<CalculatedTable>())
            {
                ct.Columns.CreateChildrenFromMetadata();
                Tree.OnStructureChanged(ct);
            }
            /*if (CTCMetadataBackup == null) return;
            foreach (var ctcbackup in CTCMetadataBackup)
            {
                ctcbackup.Restore(Model);
            }*/
        }

        private string CombineFolderJson(string path)
        {
            if (!File.Exists(path + "\\database.json")) throw new FileNotFoundException("This folder does not contain a database.json file");

            var jobj = JObject.Parse(File.ReadAllText(path + "\\database.json"));
            var model = jobj["model"] as JObject;

            InArray(path, "dataSources", model);
            if (Directory.Exists(path + "\\tables"))
            {
                var tables = new JArray();
                foreach (var tablePath in Directory.GetDirectories(path + "\\tables"))
                {
                    var filesInTableFolder = Directory.GetFiles(tablePath, "*.json");
                    if (filesInTableFolder.Length != 1) throw new FileNotFoundException(string.Format("Folder '{0}' is expected to contain exactly one .json file.", tablePath));
                    var tableFile = filesInTableFolder[0];

                    var table = JObject.Parse(File.ReadAllText(tableFile));
                    InArray(tablePath, "columns", table);
                    InArray(tablePath, "partitions", table);
                    InArray(tablePath, "measures", table);
                    InArray(tablePath, "hierarchies", table);
                    InArray(tablePath, "annotations", table);
                    tables.Add(table);
                }
                model.Add("tables", tables);
            }
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
                foreach (var file in Directory.GetFiles(path + "\\" + arrayName, "*.json").OrderBy(n => n))
                {
                    array.Add(JObject.Parse(File.ReadAllText(file)));
                }
                baseObject.Add(arrayName, array);
            }
        }

        private HashSet<string> CurrentFiles;
        private HashSet<char> InvalidFileChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        private string Sanitize(string fileName)
        {
            var sb = new StringBuilder();
            foreach(var c in fileName)
            {
                if (InvalidFileChars.Contains(c))
                {
                    sb.Append("%");
                    sb.Append(((byte)c).ToString("x2"));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        internal IEnumerable<ITranslatableObject> AllTranslatableObjects
        {
            get
            {
                return Enumerable.Repeat(Model as ITranslatableObject, 1)
                    .Concat(Model.Tables)
                    .Concat(Model.AllMeasures)
                    .Concat(Model.AllColumns)
                    .Concat(Model.AllHierarchies)
                    .Concat(Model.AllLevels)
                    .Concat(Model.Perspectives);
            }
        }

        internal IEnumerable<ITabularPerspectiveObject> AllPerspectiveObjects
        {
            get
            {
                return Model.Tables.OfType<ITabularPerspectiveObject>()
                    .Concat(Model.AllMeasures)
                    .Concat(Model.AllColumns)
                    .Concat(Model.AllHierarchies);
            }
        }

        /// <summary>
        /// Saves the model to the specified folder using the specified serialize options.
        /// </summary>
        private void SaveFolder(string path, SerializeOptions options)
        {
            if(options.LocalTranslations || options.LocalPerspectives)
            {
                // Create local annotations with translations / perspective options for relevant items
                if(options.LocalTranslations)
                {
                    // Loop through all translatable objects and provide the translation in the annotation:
                    foreach (var item in AllTranslatableObjects) item.SaveTranslations();

                    // Store the cultures (without translations) as an annotation on the model:
                    Model.SetAnnotation("TabularEditor_Cultures", Model.Cultures.ToJson(), false);
                }

                if (options.LocalPerspectives)
                {
                    // Loop through all perspective objects and provide the perspective membership in the annotation:
                    foreach (var item in AllPerspectiveObjects) item.SavePerspectives();

                    // Store the perspectives (without members) as an annotation on the model:
                    Model.SetAnnotation("TabularEditor_Perspectives", Model.Perspectives.ToJson(), false);
                }
            }

            var json = Serializer.SerializeDB(options);
            var jobj = JObject.Parse(json);

            var model = jobj["model"] as JObject;
            var dataSources = options.Levels.Contains("Data Sources") ? PopArray(model, "dataSources") : null;
            var tables = options.Levels.Contains("Tables") ? PopArray(model, "tables") : null;
            var relationships = options.Levels.Contains("Relationships") ? PopArray(model, "relationships") : null;
            var cultures = options.Levels.Contains("Translations") || options.LocalTranslations ? PopArray(model, "cultures") : null;
            var perspectives = options.Levels.Contains("Perspectives") || options.LocalPerspectives ? PopArray(model, "perspectives") : null;
            var roles = options.Levels.Contains("Roles") ? PopArray(model, "roles") : null;

            CurrentFiles = new HashSet<string>();
            WriteIfChanged(path + "\\database.json", jobj.ToString(Formatting.Indented));

            if (relationships != null) OutArray(path, "relationships", relationships, options);
            if (perspectives != null && !options.LocalPerspectives) OutArray(path, "perspectives", perspectives, options);
            if (cultures != null && !options.LocalTranslations) OutArray(path, "cultures", cultures, options);
            if (dataSources != null) OutArray(path, "dataSources", dataSources, options);
            if (roles != null) OutArray(path, "roles", roles, options);

            if (tables != null)
            {
                int n = 0;
                foreach (JObject t in tables)
                {
                    var columns = options.Levels.Contains("Tables/Columns") ? PopArray(t, "columns") : null;
                    var partitions = options.Levels.Contains("Tables/Partitions") ? PopArray(t, "partitions") : null;
                    var measures = options.Levels.Contains("Tables/Measures") ? PopArray(t, "measures") : null;
                    var hierarchies = options.Levels.Contains("Tables/Hierarchies") ? PopArray(t, "hierarchies") : null;
                    var annotations = options.Levels.Contains("Tables/Annotations") ? PopArray(t, "annotations") : null;

                    var tableName = Sanitize(t["name"].ToString());
                    var tablePath = path + "\\tables\\" + (options.PrefixFilenames ? n.ToString("D3") + " " : "") + tableName;

                    var p = tablePath + "\\" + tableName + ".json";
                    var fi = new FileInfo(p);
                    if (!fi.Directory.Exists) fi.Directory.Create();
                    WriteIfChanged(p, t.ToString(Formatting.Indented));

                    if (measures != null) OutArray(tablePath, "measures", measures, options);
                    if (columns != null) OutArray(tablePath, "columns", columns, options);
                    if (hierarchies != null) OutArray(tablePath, "hierarchies", hierarchies, options);
                    if (partitions != null) OutArray(tablePath, "partitions", partitions, options);
                    if (annotations != null) OutArray(tablePath, "annotations", annotations, options);

                    n++;
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

        private void OutArray(string path, string arrayName, JArray array, SerializeOptions options)
        {
            int n = 0;
            foreach (var t in array)
            {
                var p = path + "\\" + arrayName + "\\" + (options.PrefixFilenames ? n.ToString("D3") + " " : "") + Sanitize(t["name"].ToString()) + ".json";
                var fi = new FileInfo(p);
                if (!fi.Directory.Exists) fi.Directory.Create();
                WriteIfChanged(p, t.ToString(Formatting.Indented));
                n++;
            }
        }

        private JArray PopArray(JObject obj, string arrayName)
        {
            var result = obj[arrayName] as JArray;
            obj.Remove(arrayName);
            return result;
        }

        public event ObjectChangingEventHandler ObjectChanging;
        public event ObjectChangedEventHandler ObjectChanged;
        public event ObjectDeletingEventHandler ObjectDeleting;
        public event ObjectDeletedEventHandler ObjectDeleted;

        internal void DoObjectDeleting(TabularObject obj, ref bool cancel)
        {
            var e = new ObjectDeletingEventArgs(obj);
            ObjectDeleting?.Invoke(this, e);
            cancel = e.Cancel;
        }
        internal void DoObjectDeleted(TabularObject obj)
        {
            var e = new ObjectDeletedEventArgs(obj);
            ObjectDeleted?.Invoke(this, e);
        }

        internal void DoObjectChanging(TabularObject obj, string propertyName, object newValue, ref bool cancel)
        {
            var e = new ObjectChangingEventArgs(obj, propertyName, newValue);
            ObjectChanging?.Invoke(this, e);
            cancel = e.Cancel;
        }
        internal void DoObjectChanged(TabularObject obj, string propertyName, object oldValue, object newValue)
        {
            var e = new ObjectChangedEventArgs(obj, propertyName, oldValue, newValue);
            ObjectChanged?.Invoke(this, e);
        }

        internal static List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>> GetObjectsNotReady(TOM.Database database)
        {
            var result = new List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>>();

            // Find partitions that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Partitions).Where(p => p.State != TOM.ObjectState.Ready)
                    .Select(p => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(p, p.State))
                    );

            // Find calculated columns that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Columns.OfType<TOM.CalculatedColumn>()).Where(c => c.State != TOM.ObjectState.Ready)
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
            var errorList = new List<IErrorMessageObject>();

            Tree.ClearFolderErrors();

            foreach (var t in database.Model.Tables)
            {
                errorList.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));
                errorList.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));
                errorList.AddRange(t.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));

                var table = (WrapperLookup[t] as Table);
                
                table?.CheckChildrenErrors();
                WrapperLookup.Values.OfType<IExpressionObject>().ToList().ForEach(i => i.NeedsValidation = false);
            }
            if (errorList.Count > 0 || Errors?.Count > 0)
            {
                Errors = errorList;
            }
        }

        private bool _disableUpdates = false;

        /// <summary>
        /// Begins a batch update
        /// </summary>
        /// <param name="undoName"></param>
        public void BeginUpdate(string undoName)
        {
            if (_disableUpdates) return;

            Tree.BeginUpdate();
            if(!string.IsNullOrEmpty(undoName)) UndoManager.BeginBatch(undoName);
        }

        /// <summary>
        /// Ends the latest batch update (can never be called more times than BeginUpdate).
        /// </summary>
        /// <param name="undoable"></param>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public int EndUpdate(bool undoable = true, bool rollback = false)
        {
            if (_disableUpdates) return 0;

            var actionCount = 0;
            if(undoable || rollback) actionCount = UndoManager.EndBatch(rollback);
            Tree.EndUpdate();

            if (!InsideTransaction) DoUpdateComplete();

            return actionCount;
        }

        internal bool EoB_BuildDependencyTree = false;
        internal bool InsideTransaction { get { return Tree.UpdateLocks > 0; } }

        /// <summary>
        /// Triggers when a batch of operations has completed.
        /// If any end-of-batch flags were set during the batch, these will be triggered now
        /// and the flags will be reset.
        /// </summary>
        private void DoUpdateComplete()
        {
            if (EoB_BuildDependencyTree) { FormulaFixup.BuildDependencyTree(); EoB_BuildDependencyTree = false; }
        }

        /// <summary>
        /// Ends all batch updates in progress.
        /// </summary>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public int EndUpdateAll(bool rollback = false)
        {
            var actionCount = 0;
            while (UndoManager.BatchDepth > 0)
            {
                actionCount = UndoManager.EndBatch(rollback);
            }
            while (Tree.UpdateLocks > 0)
            {
                Tree.EndUpdate();
            }
            return actionCount;
        }


        internal void UpdateObject(ITabularObject obj)
        {

            Tree.OnNodesChanged(obj);
        }
        internal void UpdateObjectName(ITabularNamedObject obj)
        {
            Tree.OnNodeNameChanged(obj);
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

        private TabularTree _tree;
        public TabularTree Tree {
            get
            {
                if(_tree == null)
                {
                    _tree = new NullTree();
                }
                return _tree;
            }
            set
            {
                _tree = value;
            }
        }
    }
}
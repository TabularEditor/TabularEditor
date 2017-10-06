using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json.Linq;
using TabularEditor.TextServices;
using Antlr4.Runtime;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Utils;

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

    public delegate void ObjectChangingEventHandler(object sender, ObjectChangingEventArgs e);
    public delegate void ObjectChangedEventHandler(object sender, ObjectChangedEventArgs e);
    public delegate void ObjectDeletingEventHandler(object sender, ObjectDeletingEventArgs e);

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

    public enum SaveFormat
    {
        /// <summary>
        /// Saves only the Model Schema as a Model.bim file
        /// </summary>
        ModelSchemaOnly,

#if CL1400
        /// <summary>
        /// Saves the Model Schema to an existing .pbit (Power BI Template) file
        /// </summary>
        PowerBiTemplate,
#endif

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

    public class SerializeOptions
    {
        public SerializeOptions Clone()
        {
            SerializeOptions other = (SerializeOptions)MemberwiseClone();
            other.Levels = new HashSet<string>(Levels);
            return other;
        }

        public static SerializeOptions Default
        {
            get
            {
                var so = new SerializeOptions();
                so.Levels = new HashSet<string> {
                    "Data Sources",
                    "Perspectives",
                    "Relationships",
                    "Roles",
                    "Tables",
                    "Tables/Columns",
                    "Tables/Hierarchies",
                    "Tables/Measures",
                    "Tables/Partitions",
                    "Translations"
                };
                return so;
            }
        }
        public static SerializeOptions PowerBi
        {
            get
            {
                return new SerializeOptions()
                {
                    IgnoreInferredObjects = false,
                    IgnoreInferredProperties = false,
                    IgnoreTimestamps = false
                };
            }
        }
        public bool IgnoreInferredObjects = true;
        public bool IgnoreInferredProperties = true;
        public bool IgnoreTimestamps = true;
        public bool SplitMultilineStrings = true;
        public bool PrefixFilenames = false;
        public bool LocalTranslations = false;
        public bool LocalPerspectives = false;

        public HashSet<string> Levels = new HashSet<string>(); 
    }

    public static class DependencyHelper
    {
        static public void AddDep(this IDAXExpressionObject target, IDaxObject dependsOn, int fromChar, int toChar, bool fullyQualified)
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
            if (database.CompatibilityLevel < 1200) throw new InvalidOperationException("Tabular Databases of compatibility level 1100 or 1103 are not supported in Tabular Editor.");
            UndoManager = new UndoFramework.UndoManager(this);
            Actions = new TabularCommonActions(this);
            Model = Model.CreateFromMetadata(database.Model);
            Model.Database = new Database(database);
            Model.LoadChildObjects();
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
                return SourceType == ModelSourceType.Pbit && Settings.PBIFeaturesOnly;
            }
        }

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

#if CL1400
            if (fi.Exists && fi.Extension == ".pbit")
            {
                data = PowerBIHelper.LoadDatabaseFromPbitFile(path);
                SourceType = ModelSourceType.Pbit;
                Source = path;
            }
            else
#endif
                        
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
        /// <param name="databaseName"></param>
        public TabularModelHandler(string serverName, string databaseName, TabularModelHandlerSettings settings = null)
        {
            Settings = settings ?? TabularModelHandlerSettings.Default;

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
            Model.SetAnnotation("TabularEditor_SerializeOptions", JsonConvert.SerializeObject(options), false);

            switch (format)
            {
                case SaveFormat.ModelSchemaOnly:
                    SaveFile(path, options);
                    break;
#if CL1400
                case SaveFormat.PowerBiTemplate:
                    SavePbit(path);
                    break;
#endif
                case SaveFormat.TabularEditorFolder:
                    SaveFolder(path, options);
                    break;

                case SaveFormat.VisualStudioProject:
                    // TODO
                    throw new NotImplementedException();
                    // break;
            }
        }

#if CL1400
        private void SavePbit(string fileName)
        {
            if (SourceType != ModelSourceType.Pbit)
            {
                Status = "Save failed!";
                throw new InvalidOperationException("Tabular Editor cannot currently convert an Analysis Services Tabular model to a Power BI Template. Please choose a different save format.");
            }

            var fi = new FileInfo(fileName);

            if (!fi.Exists)
            {
                Status = "Save failed!";
                throw new InvalidOperationException("Tabular Editor cannot save a model as a new .pbit file. Please choose an existing .pbit file or save the model as a .bim file.");
            }

            var dbcontent = Serializer.SerializeDB(SerializeOptions.PowerBi);

            // Save to .pbit file:
            PowerBIHelper.SaveDatabaseToPbitFile(fileName, dbcontent);

            Status = "File saved.";
            if (!IsConnected) UndoManager.SetCheckpoint();
        }
#endif

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
                // TODO: Deleting a column with IsKey = true, then undoing, then saving causes an error... Check if this is still the case.
                database.Model.SaveChanges();

                AttachCalculatedTableMetadata();

                // If reattaching Calculated Table metadata caused local changes, let's save the model again:
                if (database.Model.HasLocalChanges) database.Model.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = "Save to DB error!";
                throw ex;
            }
            finally
            {
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

            foreach(var ctc in Model.Tables.OfType<CalculatedTable>().Where(ctc => ctc.NeedsValidation).SelectMany(t => t.Columns.OfType<CalculatedTableColumn>()).ToList())
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
            foreach (var ct in Model.Tables.OfType<CalculatedTable>().Where(ctc => ctc.NeedsValidation))
            {
                ct.Columns.CreateChildrenFromMetadata();
                Tree.OnStructureChanged(ct);
            }

            foreach (var ctcbackup in CTCMetadataBackup)
            {
                if (Model.Tables.Contains(ctcbackup.TableName))
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
            WriteIfChanged(path + "\\database.json", jobj.ToString(Newtonsoft.Json.Formatting.Indented));

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
                    WriteIfChanged(p, t.ToString(Newtonsoft.Json.Formatting.Indented));

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
                WriteIfChanged(p, t.ToString(Newtonsoft.Json.Formatting.Indented));
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

        internal void DoObjectDeleting(TabularObject obj, ref bool cancel)
        {
            var e = new ObjectDeletingEventArgs(obj);
            ObjectDeleting?.Invoke(this, e);
            cancel = e.Cancel;
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

            foreach (var t in database.Model.Tables)
            {
                errorList.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));
                errorList.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));
                errorList.AddRange(t.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)).Select(obj => GetWrapperObject(obj) as IErrorMessageObject));

                var table = (WrapperLookup[t] as Table);
                
                table?.CheckChildrenErrors();
                WrapperLookup.Values.OfType<IDAXExpressionObject>().ToList().ForEach(i => i.NeedsValidation = false);
            }
            if (errorList.Count > 0 || Errors?.Count > 0)
            {
                Errors = errorList;
            }
        }

        /// <summary>
        /// Begins a batch update
        /// </summary>
        /// <param name="undoName"></param>
        public void BeginUpdate(string undoName)
        {
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
            var actionCount = 0;
            if(undoable || rollback) actionCount = UndoManager.EndBatch(rollback);
            Tree.EndUpdate();

            return actionCount;
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
                    _tree = new NullTree(Model);
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
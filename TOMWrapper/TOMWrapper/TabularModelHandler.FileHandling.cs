using Microsoft.AnalysisServices.Tabular.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Serialization;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class TabularModelHandler
    {
        private static string GetDatasetFromPbip(string pbipFileOrFolder)
        {
            var file = new FileInfo(pbipFileOrFolder);
            var jPbip = JObject.Parse(File.ReadAllText(pbipFileOrFolder));
            var reportPaths = (jPbip["artifacts"] as JArray).OfType<JObject>().Where(j => j.ContainsKey("report")).Select(j => j["report"]["path"].ToObject<string>()).ToList();
            var datasetFiles = new List<string>();
            if (reportPaths.Count == 0) throw new Exception("The PBIP project folder does not contain any reports.");
            foreach (var reportPath in reportPaths)
            {
                var reportFile = Path.Combine(file.DirectoryName, reportPath, "definition.pbir");
                if (File.Exists(reportFile))
                {
                    var jPbir = JObject.Parse(File.ReadAllText(reportFile));
                    if (jPbir["datasetReference"]?["byPath"]?["path"]?.ToObject<string>() is string datasetPath)
                    {
                        var datasetFile = Path.GetFullPath(Path.Combine(file.DirectoryName, reportPath, datasetPath, "model.bim"));
                        if (File.Exists(datasetFile))
                        {
                            datasetFiles.Add(datasetFile);
                        }
                        else
                        {
                            throw new FileNotFoundException(datasetFile);
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException(reportFile);
                }
            }
            if (datasetFiles.Count == 0)
            {
                // Manually search directories:
                datasetFiles.AddRange(Directory.EnumerateFiles(file.DirectoryName, "*.bim", SearchOption.AllDirectories));
            }
            if (datasetFiles.Count == 0) throw new Exception("The PBIP project folder does not contain any datasets.");
            if (datasetFiles.Count > 1) throw new Exception("The PBIP project folder contains multiple datasets. Please open the .bim file directly.");

            return datasetFiles[0];
        }

        /// <summary>
        /// Loads an Analysis Services tabular database (Compatibility Level 1200 or newer) from a file
        /// or folder.
        /// </summary>
        /// <param name="path"></param>
        public TabularModelHandler(string path, TabularModelHandlerSettings settings = null): this(settings)
        {
            _disableUpdates = true;

            var file = new FileInfo(path);

            // If the file is a .pbip, find the actual dataset:
            if (file.Exists && file.Extension.EqualsI(".pbip"))
            {
                path = GetDatasetFromPbip(path);
                file = new FileInfo(path);
            }
            else if (Directory.Exists(path))
            {
                var pbipFiles = Directory.EnumerateFiles(path, "*.pbip", SearchOption.TopDirectoryOnly).ToList();
                if (pbipFiles.Count == 1)
                {
                    path = GetDatasetFromPbip(pbipFiles[0]);
                    file = new FileInfo(path);
                }
                else if (pbipFiles.Count > 1) throw new Exception("The PBIP project folder contains multiple .pbip files. Please open the .bim file directly.");
            }

            // If the file extension is .pbit, assume Power BI template:
            if (file.Exists && file.Extension.EqualsI(".pbit")) LoadPowerBiTemplateFile(path);

            // TMDL:
            else if (file.Exists && (file.Extension.EqualsI(".tmdl") || file.Extension.EqualsI(".tmd")))
            {
                var databaseTmdlExists = File.Exists(Path.Combine(file.DirectoryName, "database.tmdl"));
                LoadTMDL(path, useModelDeserialization: !databaseTmdlExists);
            }
            else if (Directory.Exists(path) && File.Exists(Path.Combine(path, "database.tmdl"))) LoadTMDL(Path.Combine(path, "database.tmdl"), useModelDeserialization: false);
            else if (Directory.Exists(path) && File.Exists(Path.Combine(path, "database.tmd"))) LoadTMDL(Path.Combine(path, "database.tmd"), useModelDeserialization: false);
            else if (Directory.Exists(path) && File.Exists(Path.Combine(path, "model.tmdl"))) LoadTMDL(Path.Combine(path, "model.tmdl"), useModelDeserialization: true);
            else if (Directory.Exists(path) && File.Exists(Path.Combine(path, "model.tmd"))) LoadTMDL(Path.Combine(path, "model.tmd"), useModelDeserialization: true);

            // If the file name is "database.json" or path is a directory, assume Split Model:
            else if ((file.Exists && file.Name.EqualsI("database.json")) || Directory.Exists(path)) LoadSplitModelFiles(path);

            // In any other case, assume this is just a regular Model.bim file:
            else LoadModelFile(path);

            UndoManager.Suspend();
            Model.ClearTabularEditorAnnotations();
            _disableUpdates = false;

            UndoManager.Resume();
            PowerBIGovernance.UpdateGovernanceMode(path);
        }

        private void LoadSplitModelFiles(string path)
        {
            // Change 'path' to point to a directory, in case it doesn't do that already:
            if (!Directory.Exists(path)) path = new FileInfo(path).DirectoryName;

            SourceType = ModelSourceType.Folder;
            Source = path;

            var json = SplitModelSerializer.CombineFolderJson(path);
            InitModelFromJson(json);

            // After model initialization, let's make sure we restore translations/relationships/perspectives, in case they were serialized as annotations:
           // if (SerializeOptions.LocalTranslations) Model.RestoreTranslationsFromAnnotations();
            //if (SerializeOptions.LocalRelationships) Model.RestoreRelationshipsFromAnnotations();
            //if (SerializeOptions.LocalPerspectives) Model.RestorePerspectivesFromAnnotations();

        }

        private void LoadPowerBiTemplateFile(string path)
        {
            SourceType = ModelSourceType.Pbit;
            Source = path;

            pbit = new PowerBiTemplate(path);
            InitModelFromJson(pbit.ModelJson);
        }

        private void LoadTMDL(string path, bool useModelDeserialization)
        {
            SourceType = ModelSourceType.TMDL;
            if (File.Exists(path)) path = new FileInfo(path).DirectoryName;
            Source = path;

            try
            {
                if (useModelDeserialization)
                {
                    var model = TOM.TmdlSerializer.DeserializeModelFromFolder(path);
                    database = model.Database as TOM.Database;
                }
                else
                {
                    database = TOM.TmdlSerializer.DeserializeDatabaseFromFolder(path);
                }
                database.DetermineCompatibilityMode();
                Status = "Model loaded successfully.";
                Init();

                _serializeOptions = SerializeOptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.GetType().Name} encountered while deserializing TMDL: {ex.Message}");
            }
        }

        private void LoadModelFile(string path)
        {
            SourceType = ModelSourceType.File;
            Source = path;

            var json = File.ReadAllText(path);
            InitModelFromJson(json);
        }

        private void InitModelFromJson(string json)
        {
            try
            {
                var mode = PowerBiCompatibilityModeHelper.IsPbiCompatibilityMode(json) ? Microsoft.AnalysisServices.CompatibilityMode.PowerBI : Microsoft.AnalysisServices.CompatibilityMode.AnalysisServices;
                database = TOM.JsonSerializer.DeserializeDatabase(json, mode: mode);
                database.CompatibilityMode = mode;
                Status = "Model loaded successfully.";
                Init();

                _serializeOptions = SerializeOptions;
            }
            catch(Exception ex)
            {
                throw new Exception($"Unable to load Tabular Model (Compatibility Level 1200+) from {Source}. Error: " + ex.Message);
            }
        }

        private const string ANN_SERIALIZEOPTIONS = "TabularEditor_SerializeOptions";

        // When flag is set, Tabular Editor should treat the model as a Direct Lake model:
        internal const string ANN_DIRECTLAKE = "TabularEditor_DirectLake";

        public bool HasSerializeOptions =>
            Model.GetAnnotation(ANN_SERIALIZEOPTIONS) != null;

        private SerializeOptions _serializeOptions = SerializeOptions.Default;
        public SerializeOptions SerializeOptions
        {
            get
            {
                var annotatedSerializeOptions = Model.GetAnnotation(ANN_SERIALIZEOPTIONS);
                if (annotatedSerializeOptions != null)
                {
                    try
                    {
                        _serializeOptions = JsonConvert.DeserializeObject<SerializeOptions>(annotatedSerializeOptions);
                    }
                    catch { }
                }

                return _serializeOptions;
            }
            set
            {
                _serializeOptions = value;
                SetSerializeOptions(_serializeOptions, true);
            }
        }

        private void SetSerializeOptions(SerializeOptions options, bool undoable)
        {
            if (options == null)
                Model.RemoveAnnotation(ANN_SERIALIZEOPTIONS, undoable);
            else
                Model.SetAnnotation(ANN_SERIALIZEOPTIONS, JsonConvert.SerializeObject(options), undoable);
        }

        public void Save(string path, SaveFormat format, SerializeOptions options, bool useAnnotatedSerializeOptions = false, bool resetCheckpoint = false, bool restoreSerializationOptions = true)
        {
            var overrideDatabaseName = options?.DatabaseNameOverride;
            _disableUpdates = true;
            bool hasOptions = HasSerializeOptions;
            SerializeOptions optionsBackup = SerializeOptions.Default;

            if (useAnnotatedSerializeOptions)
            {
                // This is invoked when clicking "Save" - always take whatever annotations are currently on the model,
                // and use those when serializing.
                options = SerializeOptions;
            }
            else
            {
                // This is invoked when clicking "Save As" or "Save To Folder", or when the model is being saved. Here,
                // we don't want to change the annotation on the loaded model - but we still want to apply a (possible different)
                // annotation to the file that's being saved:
                optionsBackup = SerializeOptions;
                SetSerializeOptions(options, false);
            }

            options.DatabaseNameOverride = overrideDatabaseName?.Replace(TOMWrapper.Database.InvalidNameCharacters, '_');

            try
            {
                switch (format)
                {
                    case SaveFormat.ModelSchemaOnly:
                        if (options != SerializeOptions.Default) SerializeOptions = options;
                        SaveFile(path, options);
                        break;
                    case SaveFormat.PowerBiTemplate:
                        SavePbit(path);
                        break;
                    case SaveFormat.TabularEditorFolder:
                        Model.SaveToFolder(path, options);
                        break;

                    case SaveFormat.TMDL:
                        SaveTmdl(path, options);
                        break;
                }

                if (resetCheckpoint) UndoManager.SetCheckpoint();
                Status = format == SaveFormat.TabularEditorFolder ? "Model saved." : "File saved.";
            }
            catch
            {
                throw;
            }
            finally
            {

                if (!useAnnotatedSerializeOptions && restoreSerializationOptions)
                {
                    // Restore serialization options within the currently loaded model:
                    SetSerializeOptions(hasOptions ? optionsBackup : null, false);
                }
                _disableUpdates = false;
            }
        }

        private void SavePbit(string fileName)
        {
            if (SourceType != ModelSourceType.Pbit || pbit == null)
            {
                Status = "Save failed!";
                throw new InvalidOperationException("Tabular Editor cannot currently convert an Analysis Services Tabular model to a Power BI Template. Please choose a different save format.");
            }

            var dbcontent = Serializer.SerializeDB(SerializeOptions.PowerBi, true);

            // Save to .pbit file:
            pbit.ModelJson = dbcontent;
            pbit.SaveAs(fileName);
        }

        private void SaveTmdl(string path, SerializeOptions options)
        {
            var db = Model.Database.TOMDatabase;
            db.AddTabularEditorTag();

            if (options == null) options = new SerializeOptions();

            var tmdlFormattingOptions = new TOM.Serialization.MetadataFormattingOptionsBuilder(TOM.Serialization.MetadataSerializationStyle.Tmdl)
                .WithBaseIndentationLevel(options.TmdlOptions.BaseIndentationLevel)
                .WithCasingStyle(options.TmdlOptions.CasingStyle)
                .WithEncoding(options.TmdlOptions.GetEncoding())
                .WithNewLineStyle(options.TmdlOptions.NewLineStyle);
            tmdlFormattingOptions = (options.TmdlOptions.SpacesIndentation <= 0 ? tmdlFormattingOptions.WithTabsIndentationMode() : tmdlFormattingOptions.WithSpacesIndentationMode(options.TmdlOptions.SpacesIndentation));

            var tmdlOptionsBuilder = new TOM.Serialization.MetadataSerializationOptionsBuilder(TOM.Serialization.MetadataSerializationStyle.Tmdl)
                .WithFormattingOptions(tmdlFormattingOptions.GetOptions())
                .WithExpressionTrimStyle(options.TmdlOptions.ExpressionTrimStyle);
            var tmdlOptions = (options.IncludeSensitive ? tmdlOptionsBuilder.WithRestrictedInformation() : tmdlOptionsBuilder.WithoutRestrictedInformation()).GetOptions();

            TOM.TmdlSerializer.SerializeDatabaseToFolder(Database, tmdlOptionsBuilder.GetOptions(), path);

            if (!options.TmdlOptions.IncludeRefs)
            {
                // The TMDL serializer always includes refs in the model.tmdl file. But these can cause merge conflicts when multiple developers
                // work on the model in parallel. As the refs are only used to indicate the metadata ordering of model-level objects such as tables,
                // perspectives and roles, and since the order of these objects carries no semantic meaning, we provide the option in Tabular Editor
                // to output a model.tmdl file without these refs.
                //
                // In order to do that, we need to separately serialize the model.tmdl file without children, and then manually append metadata for
                // children that are not serialized as separate files by the TMDL serializer (such as annotations, query groups and extended properties).
                // This way, we end up with a model.tmdl file that contains no refs, but still contains all metadata.
                var tmdlOptionsNoChild = tmdlOptionsBuilder.WithoutChildrenMetadata().GetOptions();

                var modelTmdlSb = new StringBuilder();
                modelTmdlSb.Append(TOM.TmdlSerializer.SerializeObject(Database.Model, tmdlOptionsNoChild, qualifyObject: false));

                if (Database.CompatibilityLevel >= 1400) SerializeCollectionTmdl(modelTmdlSb, Database.Model.ExtendedProperties, tmdlOptions);
                if (Database.CompatibilityLevel >= 1480) SerializeCollectionTmdl(modelTmdlSb, Database.Model.QueryGroups, tmdlOptions);
                SerializeCollectionTmdl(modelTmdlSb, Database.Model.Annotations, tmdlOptions);

                try
                {
                    File.WriteAllText(Path.Combine(path, "model.tmdl"), modelTmdlSb.ToString(), options.TmdlOptions.GetEncoding());
                }
                catch
                {
                    // Silently fail - worst case, we leave the model.tmdl generated by the TMDL serializer as-is.
                }
            }
        }

        private void SerializeCollectionTmdl<T>(StringBuilder builder, TOM.NamedMetadataObjectCollection<T, TOM.Model> collection, TOM.Serialization.MetadataSerializationOptions options) where T: TOM.NamedMetadataObject
        {
            foreach(var obj in collection)
            {
                builder.Append(TOM.TmdlSerializer.SerializeObject(obj, options, qualifyObject: true));
            }
        }

        private void SaveFile(string fileName, SerializeOptions options)
        {
            var dbcontent = Serializer.SerializeDB(options, true);
            var jObject = JObject.Parse(dbcontent);

            jObject["name"] = Model.Database?.Name ?? "SemanticModel";
            if (Model.Database != null)
            {
                if (!Model.Database.Name.EqualsI(Model.Database.ID)) jObject["id"] = Model.Database.ID;
                else if (jObject["id"] != null) jObject["id"].Remove();
            }

            if (options.DatabaseNameOverride != null)
            {
                jObject["name"] = options.DatabaseNameOverride;
                if (jObject["id"] != null) jObject["id"].Remove();
            }

            dbcontent = jObject.ToString(Formatting.Indented);
            (new FileInfo(fileName)).Directory.Create();

            // Save to Model.bim:
            File.WriteAllText(fileName, dbcontent);
        }
    }
}

extern alias json;

using json::Newtonsoft.Json;
using json::Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Serialization;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class TabularModelHandler
    {
        /// <summary>
        /// Loads an Analysis Services tabular database (Compatibility Level 1200 or newer) from a file
        /// or folder.
        /// </summary>
        /// <param name="path"></param>
        public TabularModelHandler(string path, TabularModelHandlerSettings settings = null)
        {
            _disableUpdates = true;

            Singleton = this;
            Settings = settings ?? TabularModelHandlerSettings.Default;

            var file = new FileInfo(path);

            // If the file extension is .pbit, assume Power BI template:
            if (file.Exists && file.Extension.EqualsI(".pbit")) LoadPowerBiTemplateFile(path);

            // If the file name is "database.json" or path is a directory, assume Split Model:
            else if ((file.Exists && file.Name.EqualsI("database.json")) || Directory.Exists(path)) LoadSplitModelFiles(path);

            // In any other case, assume this is just a regular Model.bim file:
            else LoadModelFile(path);

            Model.ClearTabularEditorAnnotations();
            _disableUpdates = false;

            UndoManager.Enabled = true;
            PowerBIGovernance.UpdateGovernanceMode(this);
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
                database = TOM.JsonSerializer.DeserializeDatabase(json);
                CompatibilityLevel = database.CompatibilityLevel;
                Status = "Model loaded succesfully.";
                Init();

                _serializeOptions = SerializeOptions;
            }
            catch(Exception ex)
            {
                throw new Exception($"Unable to load Tabular Model (Compatibility Level 1200+) from {Source}. Error: " + ex.Message);
            }
        }

        private const string ANN_SERIALIZEOPTIONS = "TabularEditor_SerializeOptions";

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

                    case SaveFormat.VisualStudioProject:
                        // TODO
                        throw new NotImplementedException();
                        // break;
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

            var dbcontent = Serializer.SerializeDB(SerializeOptions.PowerBi);

            // Save to .pbit file:
            pbit.ModelJson = dbcontent;
            pbit.SaveAs(fileName);
        }

        private void SaveFile(string fileName, SerializeOptions options)
        {
            var dbcontent = Serializer.SerializeDB(options);
            var jObject = JObject.Parse(dbcontent);

            jObject["name"] = Model.Database?.Name ?? "SemanticModel";
            if (Model.Database != null)
            {
                if (!Model.Database.Name.EqualsI(Model.Database.ID)) jObject["id"] = Model.Database.ID;
                else if (jObject["id"] != null) jObject["id"].Remove();
            }

            dbcontent = jObject.ToString(Formatting.Indented);
            (new FileInfo(fileName)).Directory.Create();

            // Save to Model.bim:
            File.WriteAllText(fileName, dbcontent);
        }
    }
}

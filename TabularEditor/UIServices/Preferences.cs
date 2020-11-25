extern alias json;

using System;
using json.Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper.Utils;
using json::Newtonsoft.Json.Converters;
using TabularEditor.TOMWrapper.Serialization;
using System.Windows.Forms;

namespace TabularEditor.UIServices
{
    public enum SearchResultOption
    {
        ByParent = 1,
        ByChild = 2,
        Flat = 3
    }

    public class Preferences
    {
        #region Serializable properties
        public bool CheckForUpdates = false;
        public bool SkipPatchUpdates = false;
        public bool CollectTelemetry = true;
        public bool FormulaFixup = true;
        public bool UseSemicolonsAsSeparators = false;
        public int DaxFormatterRequestTimeout = 5000;
        public bool DaxFormatterSkipSpaceAfterFunctionName = false;
        public bool AllowUnsupportedPBIFeatures = false;
        public bool BackgroundBpa = true;
        public bool ChangeDetectionOnLocalServers = true;
        public string BackupLocation = string.Empty;
        public bool AnnotateDeploymentMetadata = false;

        // TODO: Handle backwards compatibility
        public bool IgnoreInferredObjects = true;
        public bool IgnoreInferredProperties = true;
        public bool IgnoreTimestamps = true;
        public bool SplitMultilineStrings = true;

        public bool ProxyUseSystem = true;
        public string ProxyAddress = string.Empty;
        public string ProxyUser = string.Empty;

        public bool UsePowerQueryPartitionsByDefault = false;

        /// <summary>
        /// Stores an encrypted version of the user password. Use the string Decrypt() extension method to decrypt.
        /// </summary>
        public string ProxyPasswordEncrypted = string.Empty;

        public string ScriptCompilerDirectoryPath = string.Empty;
        public string ScriptCompilerOptions = string.Empty;

        #region Deprecated
        // Deprecated
        public bool? SaveToFolder_IgnoreInferredObjects = null;
        public bool? SaveToFolder_IgnoreInferredProperties = null;
        public bool? SaveToFolder_IgnoreTimestamps = null;
        public bool? SaveToFolder_SplitMultilineStrings = null;
        public bool ShouldSerializeSaveToFolder_IgnoreInferredObjects() { return false; }
        public bool ShouldSerializeSaveToFolder_IgnoreInferredProperties() { return false; }
        public bool ShouldSerializeSaveToFolder_IgnoreTimestamps() { return false; }
        public bool ShouldSerializeSaveToFolder_SplitMultilineStrings() { return false; }

        // Deprecated
        public bool? SaveToFile_IgnoreInferredObjects = null;
        public bool? SaveToFile_IgnoreInferredProperties = null;
        public bool? SaveToFile_IgnoreTimestamps = null;
        public bool? SaveToFile_SplitMultilineStrings = null;
        public bool ShouldSerializeSaveToFile_IgnoreInferredObjects() { return false; }
        public bool ShouldSerializeSaveToFile_IgnoreInferredProperties() { return false; }
        public bool ShouldSerializeSaveToFile_IgnoreTimestamps() { return false; }
        public bool ShouldSerializeSaveToFile_SplitMultilineStrings() { return false; }
        #endregion

        public bool SaveToFolder_PrefixFiles = false;
        public bool SaveToFolder_LocalRelationships = false;
        public bool SaveToFolder_LocalPerspectives = false;
        public bool SaveToFolder_LocalTranslations = false;


        public bool Copy_IncludeTranslations = false;
        public bool Copy_IncludePerspectives = true;
        public bool Copy_IncludeRLS = true;
        public bool Copy_IncludeOLS = true;

        public bool View_DisplayFolders = true;
        public bool View_HiddenObjects = false;
        public bool View_AllObjectTypes = true;
        public bool View_SortAlphabetically = true;
        public bool View_Measures = true;
        public bool View_Columns = true;
        public bool View_Hierarchies = true;
        public bool View_Partitions = true;
        public bool View_MetadataInformation = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public SearchResultOption View_SearchResults = SearchResultOption.ByChild;

        public HashSet<string> SaveToFolder_Levels = new HashSet<string>() {
                    "Data Sources",
                    "Perspectives",
                    "Relationships",
                    "Roles",
                    "Tables",
                    "Tables/Columns",
                    "Tables/Hierarchies",
                    "Tables/Measures",
                    "Tables/Partitions",
                    "Tables/Calculation Items",
                    "Translations"
                };

        public List<ColumnPreferences> View_ColumnPreferences = new List<ColumnPreferences>();
        #endregion

        #region Serialization functionality
        public static Preferences Default
        {
            get
            {
                return new Preferences();
            }
        }


        [JsonIgnore]
        public bool IsLoaded = false;
        [JsonIgnore]
        public bool BackupOnSave { get { return !string.IsNullOrWhiteSpace(BackupLocation); } }

        public static readonly string PREFERENCES_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\Preferences.json";
        public static readonly string PREFERENCES_PATH_OLD = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TabularEditor\Preferences.json";
        private static Preferences _current = null;
        public static Preferences Current
        {
            get
            {
                if (_current == null) Load();
                return _current;
            }
        }

        public static Preferences Load(string json)
        {
            var result = JsonConvert.DeserializeObject<Preferences>(json, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });

            // For backwards compatibility:
            if (result.SaveToFile_IgnoreInferredObjects.HasValue)       result.IgnoreInferredObjects    = result.SaveToFile_IgnoreInferredObjects.Value;
            if (result.SaveToFolder_IgnoreInferredObjects.HasValue)     result.IgnoreInferredObjects    = result.SaveToFolder_IgnoreInferredObjects.Value;
            if (result.SaveToFile_IgnoreInferredProperties.HasValue)    result.IgnoreInferredProperties = result.SaveToFile_IgnoreInferredProperties.Value;
            if (result.SaveToFolder_IgnoreInferredProperties.HasValue)  result.IgnoreInferredProperties = result.SaveToFolder_IgnoreInferredProperties.Value;
            if (result.SaveToFile_IgnoreTimestamps.HasValue)            result.IgnoreTimestamps         = result.SaveToFile_IgnoreTimestamps.Value;
            if (result.SaveToFolder_IgnoreTimestamps.HasValue)          result.IgnoreTimestamps         = result.SaveToFolder_IgnoreTimestamps.Value;
            if (result.SaveToFile_SplitMultilineStrings.HasValue)       result.SplitMultilineStrings    = result.SaveToFile_SplitMultilineStrings.Value;
            if (result.SaveToFolder_SplitMultilineStrings.HasValue)     result.SplitMultilineStrings    = result.SaveToFolder_SplitMultilineStrings.Value;

            if ((result.SaveToFile_IgnoreInferredObjects.HasValue    && result.SaveToFolder_IgnoreInferredObjects.HasValue    && result.SaveToFile_IgnoreInferredObjects.Value    != result.SaveToFolder_IgnoreInferredObjects.Value) ||
                (result.SaveToFile_IgnoreInferredProperties.HasValue && result.SaveToFolder_IgnoreInferredProperties.HasValue && result.SaveToFile_IgnoreInferredProperties.Value != result.SaveToFolder_IgnoreInferredProperties.Value) ||
                (result.SaveToFile_IgnoreTimestamps.HasValue         && result.SaveToFolder_IgnoreTimestamps.HasValue         && result.SaveToFile_IgnoreTimestamps.Value         != result.SaveToFolder_IgnoreTimestamps.Value) ||
                (result.SaveToFile_SplitMultilineStrings.HasValue    && result.SaveToFolder_SplitMultilineStrings.HasValue    && result.SaveToFile_SplitMultilineStrings.Value    != result.SaveToFolder_SplitMultilineStrings.Value))
                MessageBox.Show("Tabular Editor no longer has two sets of serialization settings for file/folder serialization respectively. Please review your settings under File > Preferences > Serialization.");

            return result;
        }

        public static void Load()
        {
            _current = Default;
            try
            {
                string path;
                if (File.Exists(PREFERENCES_PATH)) path = PREFERENCES_PATH;
                else if (File.Exists(PREFERENCES_PATH_OLD)) path = PREFERENCES_PATH_OLD;
                else return;

                var json = File.ReadAllText(PREFERENCES_PATH, Encoding.Default);
                _current = Load(json);
                _current.IsLoaded = true;
            }
            catch
            {
            }
            
        }

        private Preferences()
        {
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                (new FileInfo(PREFERENCES_PATH)).Directory.Create();
                File.WriteAllText(PREFERENCES_PATH, json, Encoding.Default);
                IsLoaded = true;
            }
            catch (IOException ex)
            {
                // Should only raise exception when several instances of Tabular Editor are closed simultaneously
                // In that case - first instance closed, wins. All others silently absorb the IOException.
            }
        }
        #endregion
    }

    public class ColumnPreferences
    {
        public string Name;
        public int Width;
        public bool Visible;
    }

    public static class PreferencesConverter
    {
        static public TabularEditor.TOMWrapper.TabularModelHandlerSettings GetSettings(this Preferences value)
        {
            return new TOMWrapper.TabularModelHandlerSettings {
                AutoFixup = value.FormulaFixup,
                PBIFeaturesOnly = !value.AllowUnsupportedPBIFeatures,
                ChangeDetectionLocalServers = value.ChangeDetectionOnLocalServers,
                UsePowerQueryPartitionsByDefault = value.UsePowerQueryPartitionsByDefault
            };
        }

        static public SerializeOptions GetSerializeOptions(this Preferences value)
        {
            var serializeOptions = new SerializeOptions()
            {
                IgnoreInferredObjects = value.IgnoreInferredObjects,
                IgnoreInferredProperties = value.IgnoreInferredProperties,
                IgnoreTimestamps = value.IgnoreTimestamps,
                SplitMultilineStrings = value.SplitMultilineStrings,
            };

            serializeOptions.PrefixFilenames = value.SaveToFolder_PrefixFiles;
            serializeOptions.LocalPerspectives = value.SaveToFolder_LocalPerspectives;
            serializeOptions.LocalTranslations = value.SaveToFolder_LocalTranslations;
            serializeOptions.LocalRelationships = value.SaveToFolder_LocalRelationships;
            serializeOptions.Levels = new HashSet<string>(value.SaveToFolder_Levels);

            return serializeOptions;
        }
    }
}

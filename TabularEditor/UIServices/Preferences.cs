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

namespace TabularEditor.UIServices
{
    public enum SearchResultOption
    {
        Flat,
        ByParent,
        ByChild
    }

    public class Preferences
    {
        #region Serializable properties
        public bool CheckForUpdates = false;
        public bool CollectTelemetry = true;
        public bool FormulaFixup = true;
        public bool AllowUnsupportedPBIFeatures = false;
        public string BackupLocation = string.Empty;

        public bool SaveToFolder_IgnoreInferredObjects = true;
        public bool SaveToFolder_IgnoreInferredProperties = true;
        public bool SaveToFolder_IgnoreTimestamps = true;
        public bool SaveToFolder_SplitMultilineStrings = true;
        public bool SaveToFolder_PrefixFiles = false;

        public bool SaveToFolder_LocalPerspectives = false;
        public bool SaveToFolder_LocalTranslations = false;

        public bool SaveToFile_IgnoreInferredObjects = true;
        public bool SaveToFile_IgnoreInferredProperties = true;
        public bool SaveToFile_IgnoreTimestamps = true;
        public bool SaveToFile_SplitMultilineStrings = true;

        public bool Copy_IncludeTranslations = true;
        public bool Copy_IncludePerspectives = true;
        public bool Copy_IncludeRLS = true;
        public bool Copy_IncludeOLS = true;

        public HashSet<string> Scripting_UsingNamespaces = new HashSet<string>();

        public bool View_DisplayFolders = true;
        public bool View_HiddenObjects = false;
        public bool View_AllObjectTypes = true;
        public bool View_SortAlphabetically = true;
        public bool View_Measures = true;
        public bool View_Columns = true;
        public bool View_Hierarchies = true;
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
                if (_current != null) return _current;
                _current = Preferences.Default;
                try
                {
                    if (File.Exists(PREFERENCES_PATH))
                    {
                        var json = File.ReadAllText(PREFERENCES_PATH, Encoding.Default);
                        _current = JsonConvert.DeserializeObject<Preferences>(json, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
                        _current.IsLoaded = true;
                    }
                    // Below for backwards compatibility with older versions of Tabular Editor, storing the preferences file in %ProgramData%:
                    else if (File.Exists(PREFERENCES_PATH_OLD))
                    {
                        var json = File.ReadAllText(PREFERENCES_PATH_OLD, Encoding.Default);
                        _current = JsonConvert.DeserializeObject<Preferences>(json);
                        _current.IsLoaded = true;
                    }
                }
                catch (Exception ex) { }
                return _current;
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
                PBIFeaturesOnly = !value.AllowUnsupportedPBIFeatures
            };
        }

        static public SerializeOptions GetSerializeOptions(this Preferences value, bool SaveToFolder)
        {
            if (SaveToFolder)
            {
                return new SerializeOptions
                {
                    IgnoreInferredObjects = value.SaveToFolder_IgnoreInferredObjects,
                    IgnoreInferredProperties = value.SaveToFolder_IgnoreInferredProperties,
                    IgnoreTimestamps = value.SaveToFolder_IgnoreTimestamps,
                    SplitMultilineStrings = value.SaveToFolder_SplitMultilineStrings,
                    PrefixFilenames = value.SaveToFolder_PrefixFiles,
                    LocalPerspectives = value.SaveToFolder_LocalPerspectives,
                    LocalTranslations = value.SaveToFolder_LocalTranslations,
                    Levels = new HashSet<string>(value.SaveToFolder_Levels)
                };
            } else
            {
                return new SerializeOptions
                {
                    IgnoreInferredObjects = value.SaveToFile_IgnoreInferredObjects,
                    IgnoreInferredProperties = value.SaveToFile_IgnoreInferredProperties,
                    IgnoreTimestamps = value.SaveToFile_IgnoreTimestamps,
                    SplitMultilineStrings = value.SaveToFile_SplitMultilineStrings
                };
            }
        }
    }
}

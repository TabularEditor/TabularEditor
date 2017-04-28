using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TabularEditor.UIServices
{
    public class Preferences
    {
        #region Serializable properties
        public bool CheckForUpdates = false;
        public bool FormulaFixup = true;
        public string BackupLocation = string.Empty;

        public bool SaveToFolder_IgnoreInferredObjects = true;
        public bool SaveToFolder_IgnoreInferredProperties = true;
        public bool SaveToFolder_IgnoreTimestamps = true;
        public bool SaveToFolder_SplitMultilineStrings = true;
        public bool SaveToFolder_PrefixFiles = false;

        public HashSet<string> SaveToFolder_Levels = new HashSet<string>(); 
        #endregion

        #region Serialization functionality
        public static Preferences Default
        {
            get {
                var prefs = new Preferences();
                prefs.SaveToFolder_Levels = new HashSet<string>() {
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
                return prefs;
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
                        _current = JsonConvert.DeserializeObject<Preferences>(json);
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
                catch { }
                return _current;
            }
        }

        private Preferences()
        {
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            (new FileInfo(PREFERENCES_PATH)).Directory.Create();
            File.WriteAllText(PREFERENCES_PATH, json, Encoding.Default);
            IsLoaded = true;
        }
        #endregion
    }

    public static class PreferencesSerializerOptions
    {
        static public TabularEditor.TOMWrapper.SerializeOptions GetSerializeOptions(this Preferences value)
        {
            return new TOMWrapper.SerializeOptions
            {
                IgnoreInferredObjects = value.SaveToFolder_IgnoreInferredObjects,
                IgnoreInferredProperties = value.SaveToFolder_IgnoreInferredProperties,
                IgnoreTimestamps = value.SaveToFolder_IgnoreTimestamps,
                SplitMultilineStrings = value.SaveToFolder_SplitMultilineStrings,
                PrefixFilenames = value.SaveToFolder_PrefixFiles,
                Levels = new HashSet<string>(value.SaveToFolder_Levels)
            };
        }
    }
}

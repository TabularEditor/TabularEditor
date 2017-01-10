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
        public string BackupLocation = string.Empty;
        #endregion

        #region Serialization functionality
        [JsonIgnore]
        public bool IsLoaded = false;
        [JsonIgnore]
        public bool BackupOnSave { get { return !string.IsNullOrWhiteSpace(BackupLocation); } }

        public static readonly string PREFERENCES_PATH = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TabularEditor\Preferences.json";
        private static Preferences _current = null;
        public static Preferences Current
        {
            get
            {
                if (_current != null) return _current;
                _current = new Preferences();
                try
                {
                    var json = File.ReadAllText(PREFERENCES_PATH, Encoding.Default);
                    _current = JsonConvert.DeserializeObject<Preferences>(json);
                    _current.IsLoaded = true;
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
            File.WriteAllText(PREFERENCES_PATH, json, Encoding.Default);
            IsLoaded = true;
        }
        #endregion
    }
}

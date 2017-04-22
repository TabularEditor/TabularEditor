using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
    public class RecentFiles
    {
        public List<string> RecentHistory = new List<string>();

        [JsonIgnore]
        static readonly string RECENTFILES_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\RecentFiles.json";
        private static RecentFiles _current = null;
        public static RecentFiles Current
        {
            get
            {
                if (_current != null) return _current;
                if (File.Exists(RECENTFILES_PATH))
                {
                    try
                    {
                        var json = File.ReadAllText(RECENTFILES_PATH);
                        _current = JsonConvert.DeserializeObject<RecentFiles>(json);
                    }
                    catch { }
                }
                if (_current == null) _current = new RecentFiles();
                return _current;
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Current, Formatting.Indented);
            (new FileInfo(RECENTFILES_PATH)).Directory.Create();
            File.WriteAllText(RECENTFILES_PATH, json);
        }

        public static void Add(string fileName)
        {
            if(!Current.RecentHistory.Contains(fileName, StringComparer.InvariantCultureIgnoreCase))
                Current.RecentHistory.Add(fileName);
        }
    }
}

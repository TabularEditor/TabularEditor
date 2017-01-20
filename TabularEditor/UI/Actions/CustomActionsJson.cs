using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TabularEditor.UI.Actions
{
    public class CustomActionsJson
    {
        public static CustomActionsJson LoadFromJson(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath, Encoding.Default);
            return JsonConvert.DeserializeObject<CustomActionsJson>(json);
        }

        public void SaveToJson(string jsonPath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            (new FileInfo(jsonPath)).Directory.Create();
            File.WriteAllText(jsonPath, json);
        }

        public CustomActionJson[] Actions;
    }

    public class CustomActionJson
    {
        public string Name;
        public string Enabled;
        public string Execute;
        public string Tooltip;
    }
}

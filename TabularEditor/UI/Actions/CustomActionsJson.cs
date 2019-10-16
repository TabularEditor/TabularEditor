extern alias json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using json.Newtonsoft.Json;
using System.IO;
using json.Newtonsoft.Json.Converters;

namespace TabularEditor.UI.Actions
{
    public class CustomActionsJson
    {
        public static CustomActionsJson LoadFromJson(string jsonPath)
        {
            try
            {
                var json = File.ReadAllText(jsonPath, Encoding.UTF8);
                var result = JsonConvert.DeserializeObject<CustomActionsJson>(json);
                result.Actions = result.Actions.OrderBy(act => act.Name).ToArray();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load custom actions from file '{0}'. Error message: {1}", jsonPath, e.Message);
                return new CustomActionsJson() { Actions = new CustomActionJson[0] };
            }
        }

        public void SaveToJson(string jsonPath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            (new FileInfo(jsonPath)).Directory.Create();
            File.WriteAllText(jsonPath, json, Encoding.UTF8);
        }

        public CustomActionJson[] Actions;
    }

    public class CustomActionJson
    {
        public string Name;
        public string Enabled;
        public string Execute;
        [JsonIgnore]
        public string CleansedCode;
        public string Tooltip;
        [JsonConverter(typeof(StringEnumConverter))]
        public Context ValidContexts = Context.SingularObjects;
    }
}

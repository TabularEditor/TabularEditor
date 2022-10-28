using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;

namespace TabularEditor.UI.Actions
{
    public class MacrosJson
    {
        public static MacrosJson LoadFromJson(string jsonPath)
        {
            try
            {
                var json = File.ReadAllText(jsonPath, Encoding.UTF8);
                var result = JsonConvert.DeserializeObject<MacrosJson>(json);
                result.Actions = result.Actions.OrderBy(act => act.Name).ToArray();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load macros from file '{0}'. Error message: {1}", jsonPath, e.Message);
                return new MacrosJson() { Actions = new MacroJson[0] };
            }
        }

        public void SaveToJson()
        {
            var jsonPath = ScriptEngine.MacrosJsonPath;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            (new FileInfo(jsonPath)).Directory.Create();
            File.WriteAllText(jsonPath, json, Encoding.UTF8);
            
            try
            {
                if (File.Exists(ScriptEngine.CustomActionsJsonPath))
                    File.Delete(ScriptEngine.CustomActionsJsonPath);
            }
            catch { }
        }

        public MacroJson[] Actions;
    }

    public class MacroJson
    {
        public string Name;
        public string Enabled;
        public string Execute;
        public string Tooltip;
        [JsonConverter(typeof(StringEnumConverter))]
        public Context ValidContexts = Context.SingularObjects;
    }
}

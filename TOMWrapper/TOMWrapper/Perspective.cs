extern alias json;

using json.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Perspective
    {
    }

    public partial class PerspectiveCollection
    {
        public class SerializedPerspective
        {
            public string Name;
            public string Description;
            public Dictionary<string, string> Annotations;
        }

        public string ToJson()
        {
            // TODO: We should really use the TOM JsonSerializer here instead...

            return JsonConvert.SerializeObject(this.Select(p => 
            new SerializedPerspective {
                Name = p.Name,
                Description = p.Description,
                Annotations = p.Annotations.Keys.ToDictionary(k => k, k => p.GetAnnotation(k))
            }).OrderBy(p => p.Name).ToArray());
        }

        public void FromJson(string json)
        {
            var serializedPerspectives = JsonConvert.DeserializeObject<SerializedPerspective[]>(json);

            foreach(var p in serializedPerspectives)
            {
                var perspective = Handler.Model.AddPerspective(p.Name);
                perspective.Description = p.Description;
                if(p.Annotations != null) foreach (var k in p.Annotations.Keys) perspective.SetAnnotation(k, p.Annotations[k]);
            }
        }
    }
}

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
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.Select(p => new SerializedPerspective { Name = p.Name, Description = p.Description }).OrderBy(p => p.Name).ToArray());
        }

        public void FromJson(string json)
        {
            var perspectives = JsonConvert.DeserializeObject<SerializedPerspective[]>(json);

            foreach(var p in perspectives)
            {
                Handler.Model.AddPerspective(p.Name).Description = p.Description;
            }
        }
    }
}

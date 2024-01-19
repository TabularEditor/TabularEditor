using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TabularEditor.TOMWrapper
{
    public partial class Perspective
    {
    }

    public partial class PerspectiveCollection
    {
        internal class SerializedPerspective
        {
            public string Name;
            public string Description;
            public Dictionary<string, string> Annotations;
            public Dictionary<string, string> ExtendedProperties;
            public bool ShouldSerializeExtendedProperties() => ExtendedProperties != null && ExtendedProperties.Count > 0;
        }

        internal string ToJson()
        {
            // TODO: We should really use the TOM JsonSerializer here instead...

            return JsonConvert.SerializeObject(this.Select(p => 
            new SerializedPerspective {
                Name = p.Name,
                Description = p.Description,
                Annotations = p.Annotations.Keys.ToDictionary(k => k, k => p.GetAnnotation(k)),
                ExtendedProperties = p.Handler.CompatibilityLevel >= 1400 ? p.ExtendedProperties.Keys.ToDictionary(k => k, k => p.GetExtendedProperty(k)) : null
            }).OrderBy(p => p.Name).ToArray());
        }
    }
}

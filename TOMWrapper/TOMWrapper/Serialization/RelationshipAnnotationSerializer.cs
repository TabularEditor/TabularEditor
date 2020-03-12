extern alias json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;
using json::Newtonsoft.Json.Linq;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class RelationshipAnnotationSerializer
    {
        public static void StoreRelationshipsAsAnnotations(this Model model)
        {
            foreach (var table in model.Tables) StoreRelationshipsAsAnnotations(table);
        }

        public static void StoreRelationshipsAsAnnotations(Table table)
        {
            table.SetAnnotation(AnnotationHelper.ANN_RELATIONSHIPS, table.GetRelationshipsJson(json.Newtonsoft.Json.Formatting.Indented), false);
        }

        public static string GetRelationshipsJson(this Table table, json.Newtonsoft.Json.Formatting format)
        {
            JArray rels = new JArray();
            foreach (var rel in table.Model.Relationships.Where(r => r.FromTable == table)
                .OrderBy(r => r.FromColumn.Name).ThenBy(r => r.ToColumn.DaxObjectFullName))
            {
                var json = TOM.JsonSerializer.SerializeObject(rel.MetadataObject, new TOM.SerializeOptions() { IgnoreInferredObjects = true, IgnoreInferredProperties = true, IgnoreTimestamps = true });
                var jObj = JObject.Parse(json);
                jObj.Remove("name");
                rels.Add(jObj);
            }
            return rels.ToString(format);
        }        
    }
}

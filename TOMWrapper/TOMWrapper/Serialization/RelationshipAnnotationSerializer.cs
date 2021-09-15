using System;
using System.Linq;
using TOM = Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json.Linq;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class RelationshipAnnotationSerializer
    {
        public static void StoreRelationshipsAsAnnotations(this Model model)
        {
            bool hasRelationshipVariations = model.AllColumns.Any(v => v.Variations.Any(v2 => v2.Relationship != null));
            if(model.Relationships.OfType<SingleColumnRelationship>().Any(r => r.FromColumn == null || r.ToColumn == null))
            {
                throw new SerializationException("One or more relationships are incomplete (FromColumn or ToColumn not set).");
            }
            
            foreach (var table in model.Tables) StoreRelationshipsAsAnnotations(table, hasRelationshipVariations);
        }

        public static void StoreRelationshipsAsAnnotations(Table table, bool hasRelationshipVariations)
        {
            table.SetAnnotation(AnnotationHelper.ANN_RELATIONSHIPS, table.GetRelationshipsJson(Newtonsoft.Json.Formatting.Indented, hasRelationshipVariations), false);
        }

        public static string GetRelationshipsJson(this Table table, Newtonsoft.Json.Formatting format, bool hasRelationshipVariations)
        {
            JArray rels = new JArray();
            foreach (var rel in table.Model.Relationships.Where(r => r.FromTable == table)
                .OrderBy(r => r.FromColumn.Name).ThenBy(r => r.ToColumn.DaxObjectFullName))
            {
                var json = TOM.JsonSerializer.SerializeObject(rel.MetadataObject, new TOM.SerializeOptions() { IgnoreInferredObjects = true, IgnoreInferredProperties = true, IgnoreTimestamps = true });
                var jObj = JObject.Parse(json);
                // Only remove relationship name if model does not use variations:
                if(!hasRelationshipVariations) jObj.Remove("name");
                rels.Add(jObj);
            }
            return rels.ToString(format);
        }        
    }

    public class SerializationException: Exception
    {
        public SerializationException(string message): base(message)
        {

        }
    }
}

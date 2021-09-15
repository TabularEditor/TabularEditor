using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TabularEditor.TOMWrapper.Linguistics
{
    public static class SynonymHelper
    {
        public static string GetSynonyms(TabularNamedObject tabularObject, Culture culture)
        {
            if (culture.ContentType != ContentType.Json || string.IsNullOrEmpty(culture.Content)) return null;
            try
            {
                var jEntities = JObject.Parse(culture.Content)["Entities"] as JObject;
                if (jEntities == null) return null;
                var jEntity = FindEntity(tabularObject, jEntities);
                if (jEntity == null) return null;
                var terms = GetTerms(jEntity);
                return string.Join(", ", terms.Where(t => t.Properties.State != State.Deleted && t.Properties.State != State.Suggested).Select(t => t.Name));
            }
            catch
            {
                return null;
            }
        }

        public static void SetSynonyms(TabularNamedObject tabularObject, Culture culture, string synonyms)
        {
            if (culture.ContentType != ContentType.Json || string.IsNullOrEmpty(culture.Content))
                throw new Exception($"Can't set synonyms since culture '{culture.Name}' does not contain a Linguistics Schema.");

            try
            {
                var lsdl = JObject.Parse(culture.Content);
                var jEntities = lsdl["Entities"] as JObject;
                var jEntity = FindEntity(tabularObject, jEntities) ?? CreateEntity(tabularObject, jEntities);
                if (jEntity == null) throw new Exception($"Not able to set synonyms for this object.");
                SetTerms(jEntity, synonyms);
                tabularObject.Handler.UndoManager.Suspend();
                culture.Content = lsdl.ToString(Formatting.None);
                tabularObject.Handler.UndoManager.Resume();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to set synonym terms for this object: " + ex.Message);
            }
        }

        private static JObject CreateEntity(TabularNamedObject tabularObject, JObject jEntities)
        {
            JObject entity = null;
            switch(tabularObject)
            {
                case Table table:
                    entity = JObject.FromObject(new { Binding = new { ConceptualEntity = table.Name }, State = "Generated", Terms = new object[0] });
                    jEntities.Add(CleanName(table.Name), entity);
                    break;

                case Measure measure:
                case Column column:
                    var tto = tabularObject as ITabularTableObject;
                    entity = JObject.FromObject(new { Binding = new { ConceptualEntity = tto.Table.Name, ConceptualProperty = tto.Name }, State = "Generated", Terms = new object[0] });
                    jEntities.Add($"{CleanName(tto.Table.Name)}.{CleanName(tto.Name)}", entity);
                    break;

                case Hierarchy hierarchy:
                    entity = JObject.FromObject(new { Binding = new { ConceptualEntity = hierarchy.Table.Name, Hierarchy = hierarchy.Name }, State = "Generated", Terms = new object[0] });
                    jEntities.Add($"{CleanName(hierarchy.Table.Name)}.{CleanName(hierarchy.Name)}", entity);
                    break;

                case Level level:
                    var h = level.Hierarchy;
                    entity = JObject.FromObject(new { Binding = new { ConceptualEntity = h.Table.Name, Hierarchy = h.Name, HierarchyLevel = level.Name }, State = "Generated", Terms = new object[0] });
                    jEntities.Add($"{CleanName(h.Table.Name)}.{CleanName(h.Name)}.{CleanName(level.Name)}", entity);
                    break;
            }

            return entity;
        }

        private static string CleanName(string name)
        {
            return name.Replace("\"", "").Replace("\\", "").Replace(",", "").Replace(".", "_").Replace(" ", "_");
        }

        private static void SetTerms(JObject jEntity, string synonyms)
        {
            var existingTerms = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            var jTerms = jEntity["Terms"] as JArray;

            foreach (JObject jTerm in jTerms)
            {
                foreach(var kvp in jTerm)
                {
                    var jTermProp = kvp.Value as JObject;
                    var termProp = jTermProp.ToObject<TermProperties>();
                    if (termProp.State != State.Deleted && termProp.State != State.Suggested)
                        existingTerms.Add(kvp.Key, jTermProp);
                }
            }

            var newTerms = new HashSet<string>(synonyms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
            
            // New terms to be added:
            foreach(var newTerm in newTerms.Except(existingTerms.Keys))
            {
                var jTerm = new JObject();
                jTerm.Add(newTerm, JObject.FromObject(new { LastModified = DateTime.Now }));
                jTerms.Add(jTerm);
            }

            // Existing terms to be deleted:
            foreach(var existingTerm in existingTerms.Keys.Except(newTerms))
            {
                existingTerms[existingTerm]["State"] = "Deleted";
            }
        }

        private static IEnumerable<Term> GetTerms(JObject jEntity)
        {
            var jTerms = jEntity["Terms"] as JArray;
            foreach(JObject jTerm in jTerms)
            {
                foreach(var kvp in jTerm)
                {
                    yield return new Term { Name = kvp.Key, Properties = (kvp.Value as JObject).ToObject<TermProperties>() };
                }
            }
        }

        private static JObject FindEntity(TabularNamedObject tabularObject, JObject jEntities)
        {
            foreach (var entity in jEntities)
            {
                var jEntity = entity.Value as JObject;
                switch (tabularObject)
                {
                    case Table table: if (IsTableEntity(table, jEntity)) return jEntity; break;
                    case Measure measure: if (IsTableObjectEntity(measure, jEntity)) return jEntity; break;
                    case Column column: if (IsTableObjectEntity(column, jEntity)) return jEntity; break;
                    case Hierarchy hierarchy: if (IsHierarchyEntity(hierarchy, jEntity)) return jEntity; break;
                    case Level level: if (IsLevelEntity(level, jEntity)) return jEntity; break;
                    default: return null;
                }
            }
            return null;
        }
        private static bool IsTableEntity(Table table, JObject jEntity)
        {
            var jBinding = GetBinding(jEntity);
            return jBinding != null && jBinding.Count == 1 && jBinding["ConceptualEntity"] is JValue jConceptualEntity && jConceptualEntity.ToString() == table.Name;
        }
        private static bool IsTableObjectEntity(ITabularTableObject tableObject, JObject jEntity)
        {
            var jBinding = GetBinding(jEntity);
            return jBinding != null && jBinding.Count == 2
                && jBinding["ConceptualEntity"] is JValue jConceptualEntity && jConceptualEntity.ToString() == tableObject.Table.Name
                && jBinding["ConceptualProperty"] is JValue jConceptualProperty && jConceptualProperty.ToString() == tableObject.Name;
        }
        private static bool IsHierarchyEntity(Hierarchy hierarchy, JObject jEntity)
        {
            var jBinding = GetBinding(jEntity);
            return jBinding != null && jBinding.Count == 2
                && jBinding["ConceptualEntity"] is JValue jConceptualEntity && jConceptualEntity.ToString() == hierarchy.Table.Name
                && jBinding["Hierarchy"] is JValue jHierarchy && jHierarchy.ToString() == hierarchy.Name;
        }
        private static bool IsLevelEntity(Level level, JObject jEntity)
        {
            var jBinding = GetBinding(jEntity);
            return jBinding != null && jBinding.Count == 3
                && jBinding["ConceptualEntity"] is JValue jConceptualEntity && jConceptualEntity.ToString() == level.Hierarchy.Table.Name
                && jBinding["Hierarchy"] is JValue jHierarchy && jHierarchy.ToString() == level.Hierarchy.Name
                && jBinding["HierarchyLevel"] is JValue jLevel && jLevel.ToString() == level.Name;
        }
        private static JObject GetBinding(JObject jEntity)
        {
            return jEntity["Binding"] as JObject ?? jEntity["Definition"]?["Binding"] as JObject;
        }
    }

    public class Term
    {
        public string Name { get; set; }
        public TermProperties Properties { get; set; }
    }

    public class TermProperties
    {
        public State State { get; set; } = State.Authored;
        public bool ShouldSerializeState() => State != State.Authored;

        public TermPropertiesType? Type { get; set; }
        public bool ShouldSerializeType() => Type != null;

        public double Weight { get; set; } = 1.0;
        public bool ShouldSerializeWeight() => Weight != 1.0;

        public DateTime? LastModified { get; set; }
        public bool ShouldSerializeLastModified() => LastModified.HasValue;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum State
    {
        Authored,
        Generated,
        Suggested,
        Deleted,
        Default,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TermPropertiesType
    {
        Noun,
        Verb,
        Adjective,
        Preposition,
    }
}

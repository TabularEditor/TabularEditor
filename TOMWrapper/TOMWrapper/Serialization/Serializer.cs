using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Collections;
using Newtonsoft.Json.Serialization;

namespace TabularEditor.TOMWrapper.Serialization
{
    public static class Serializer
    {
        internal static string TypeToJson(Type type)
        {
            return type.Name.Pluralize().ToLower();
        }

        internal static string SerializeCultures(Model model, IEnumerable<Culture> cultures)
        {
            var referenceCulture = ReferenceCulture.Create(model.Handler.Database);
            string json;

            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            using (var sw = new StringWriter())
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    jw.WriteStartObject();
                    jw.WritePropertyName("referenceCulture");
                    jw.WriteRawValue(JsonConvert.SerializeObject(referenceCulture, settings));
                    jw.WritePropertyName("cultures");
                    jw.WriteStartArray();
                    foreach (var culture in cultures)
                    {
                        jw.WriteRawValue(TOM.JsonSerializer.SerializeObject(culture.MetadataObject));
                    }
                    jw.WriteEndArray();
                    jw.WriteEndObject();

                    json = sw.ToString();
                }
            }

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        }

        public static string SerializeObjects(IEnumerable<TabularObject> objects 
            , bool includeTranslations = true
            , bool includePerspectives = true
            , bool includeRLS = true
            , bool includeOLS = true
            , bool includeInstanceID = false
            )
        {
            var model = objects.FirstOrDefault()?.Model;
            if (model == null) return "[]";

            if (includeTranslations) foreach (var obj in objects.OfType<IInternalTranslatableObject>()) obj.SaveTranslations(true);
            if (includePerspectives) foreach (var obj in objects.OfType<IInternalTabularPerspectiveObject>()) obj.SavePerspectives(true);
            if (includeRLS) foreach (var obj in objects.OfType<Table>()) obj.SaveRLS();
            if (includeOLS && model.Handler.CompatibilityLevel >= 1400)
            {
                foreach (var obj in objects.OfType<Table>()) obj.SaveOLS(true);
                foreach (var obj in objects.OfType<Column>()) obj.SaveOLS();
            }

            var byType = objects.GroupBy(obj => obj.GetType(), obj => TOM.JsonSerializer.SerializeObject(obj.MetadataObject));

            using (var sw = new StringWriter())
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    jw.WriteStartObject();

                    if (includeInstanceID)
                    {
                        jw.WritePropertyName("InstanceID");
                        jw.WriteValue(model.Handler.InstanceID);
                    }

                    foreach (var type in byType)
                    {
                        jw.WritePropertyName(TypeToJson(type.Key));
                        jw.WriteStartArray();
                        foreach (var obj in type)
                        {
                            jw.WriteRawValue(obj);
                        }
                        jw.WriteEndArray();
                    }

                    jw.WriteEndObject();
                }

                foreach (var obj in objects.OfType<IInternalAnnotationObject>()) obj.ClearTabularEditorAnnotations();

                return sw.ToString();
            }
        }

        /// <summary>
        /// An ObjectJsonContainer is a special JSON structure used to serialize several different types of
        /// TabularObjects at once. The structure simply consists of a JSON object that holds one or more
        /// arrays of the various types of objects. The key of each array is the lowercase pluralized name
        /// of the type held by the array.
        /// </summary>
        public static ObjectJsonContainer ParseObjectJsonContainer(string json)
        {
            json = json.Trim();
            if (!(json.StartsWith("{") && json.EndsWith("}"))) return null; // Expect a JSON object
            JObject jObj;

            try
            {
                jObj = JObject.Parse(json);
            }
            catch (JsonReaderException jex)
            {
                return null;
            }

            return new ObjectJsonContainer(jObj);
        }

        /*public static IList<TabularObject> DeserializeObjects(string json)
        {
            json = json.Trim();
            if (!(json.StartsWith("{") && json.EndsWith("}"))) return null; // Expect a JSON object
            JObject jObj;

            try
            {
                jObj = JObject.Parse(json);
            }
            catch (JsonReaderException jex)
            {
                return null;
            }

            var result = new List<TabularObject>();

            foreach (var type in ObjectMetadata.Creatable)
            {
                var jArr = jObj[TypeToJson(type)] as JArray;
                if(jArr != null)
                {
                    var tomType = ObjectMetadata.ToTOM(type);
                    foreach (JObject tomObj in jArr)
                    {
                        var tomJson = tomObj.ToString();
                        var tom = TOM.JsonSerializer.DeserializeObject(tomType, tomJson);
                        var wrapperObj = ObjectMetadata.CreateFromMetadata(tom) as TabularObject;
                        wrapperObj.SerializedFrom = tomObj;
                        result.Add(wrapperObj);
                    }
                }
            }

            return result.Count > 0 ? result : null;
        }*/

        private const string ANN_SAVESENSITIVE = "TabularEditor_SaveSensitive";

        public static string SerializeDB(SerializeOptions options, bool includeTabularEditorTag)
        {
            var db = TabularModelHandler.Singleton.Database;
            if (includeTabularEditorTag)
                db.AddTabularEditorTag();
            else
                db.RemoveTabularEditorTag();

            // Remove object translations with no objects assigned:
            var nullTrans = db.Model.Cultures.SelectMany(c => c.ObjectTranslations).Where(ot => ot.Object == null).ToList();
            if(nullTrans.Count > 0)
            {
                foreach(var ot in nullTrans)
                {
                    ot.Culture.ObjectTranslations.Remove(ot);
                }
            }

            var serializedDB =
                TOM.JsonSerializer.SerializeDatabase(db,
                new TOM.SerializeOptions()
                {
                    IgnoreInferredObjects = options.IgnoreInferredObjects,
                    IgnoreTimestamps = options.IgnoreTimestamps,
                    IgnoreInferredProperties = options.IgnoreInferredProperties,
                    SplitMultilineStrings = options.SplitMultilineStrings,
                    IncludeRestrictedInformation = db.Model.Annotations.Contains(ANN_SAVESENSITIVE) && db.Model.Annotations[ANN_SAVESENSITIVE].Value == "1"
                });

            // Hack: Remove \r characters from multiline strings in the BIM:
            // "1 + 2\r", -> "1 + 2",
            if(options.SplitMultilineStrings) serializedDB = serializedDB.Replace("\\r\",\r\n", "\",\r\n");
            if(options.IgnoreLineageTags)
            {
                var jObject = JObject.Parse(serializedDB);
                var lineageTags = jObject.Descendants().OfType<JProperty>().Where(p => p.Name == "lineageTag").ToList();
                foreach (var lineageTag in lineageTags) lineageTag.Remove();
                serializedDB = jObject.ToString();
            }

            return serializedDB;
        }

#region Individual object deserialization
        // TODO: Below code could maybe be auto-generated, as we know from reflection which properties
        // hold references to other objects, that needs to be looked up.
        public static Level DeserializeLevel(JObject json, Hierarchy target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Level>(json.ToString(Formatting.None));
            tom.Name = target.Levels.GetNewName(tom.Name);
            tom.Column = target.Table.MetadataObject.Columns[json.Value<string>("column")];

            var level = Level.CreateFromMetadata(target, tom);

            return level;
        }

        public static CalculatedColumn DeserializeCalculatedColumn(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.CalculatedColumn>(json.ToString(Formatting.None));
            tom.Name = target.Columns.GetNewName(tom.Name);

            if (json["sortByColumn"] != null)
            {
                var srcColumnName = json.Value<string>("sortByColumn");
                if (target.MetadataObject.Columns.ContainsName(srcColumnName))
                    tom.SortByColumn = target.MetadataObject.Columns[srcColumnName];
            }

            var column = CalculatedColumn.CreateFromMetadata(target, tom);

            return column;
        }

        public static DataColumn DeserializeDataColumn(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.DataColumn>(json.ToString(Formatting.None));
            tom.Name = target.Columns.GetNewName(tom.Name);

            if (json["sortByColumn"] != null)
            {
                var srcColumnName = json.Value<string>("sortByColumn");
                if (target.MetadataObject.Columns.ContainsName(srcColumnName))
                    tom.SortByColumn = target.MetadataObject.Columns[srcColumnName];
            }

            var column = DataColumn.CreateFromMetadata(target, tom);

            return column;
        }

        public static Measure DeserializeMeasure(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Measure>(json.ToString(Formatting.None));
            tom.Name = target.Measures.GetNewName(tom.Name);

            var measure = Measure.CreateFromMetadata(target, tom);

            return measure;
        }

        public static Hierarchy DeserializeHierarchy(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Hierarchy>(json.ToString(Formatting.None));
            tom.Name = target.Hierarchies.GetNewName(tom.Name);
            for (var i = 0; i < tom.Levels.Count; i++)
            {
                var srcColumnName = json["levels"][i].Value<string>("column");
                if (!target.MetadataObject.Columns.ContainsName(srcColumnName))
                    tom.Levels[i].Column = target.MetadataObject.Columns.First(c => c.Type != TOM.ColumnType.RowNumber);
            }

            var hierarchy = Hierarchy.CreateFromMetadata(target, tom);

            return hierarchy;
        }

        public static Partition DeserializePartition(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None));
            tom.Name = target.Partitions.GetNewName(tom.Name);
            if(tom.Source is TOM.QueryPartitionSource)
            {
                (tom.Source as TOM.QueryPartitionSource).DataSource = target.MetadataObject.Model.DataSources[json["source"].Value<string>("dataSource")];
            }

            var partition = Partition.CreateFromMetadata(target, tom);

            return partition;
        }

        public static MPartition DeserializeMPartition(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None));
            tom.Name = target.Partitions.GetNewName(tom.Name);
            var partition = MPartition.CreateFromMetadata(target, tom);
            return partition;
        }

        public static PolicyRangePartition DeserializePolicyRangePartition(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None));
            tom.Name = target.Partitions.GetNewName(tom.Name);
            var partition = PolicyRangePartition.CreateFromMetadata(target, tom);
            return partition;
        }
        public static EntityPartition DeserializeEntityPartition(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None));
            tom.Name = target.Partitions.GetNewName(tom.Name);
            var partition = EntityPartition.CreateFromMetadata(target, tom);
            return partition;
        }

        public static CalculatedTable DeserializeCalculatedTable(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None));
            tom.Name = model.Tables.GetNewName(tom.Name);

            // Make sure all measures in the table still have model-wide unique names:
            foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewName(model, m.Name);

            var table = CalculatedTable.CreateFromMetadata(model, tom);

            return table;
        }

        public static Table DeserializeTable(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None));
            tom.Name = model.Tables.GetNewName(tom.Name);

            // Make sure all measures in the table still have model-wide unique names:
            foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewName(model, m.Name);

            var table = Table.CreateFromMetadata(model, tom);

            return table;
        }

        public static SingleColumnRelationship DeserializeSingleColumnRelationship(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.SingleColumnRelationship>(json.ToString(Formatting.None));
            if(model.Relationships.TOM_ContainsName(tom.Name))
                tom.Name = Guid.NewGuid().ToString();

            var relationship = SingleColumnRelationship.CreateFromMetadata(model, tom);

            return relationship;
        }

        public static NamedExpression DeserializeNamedExpression(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.NamedExpression>(json.ToString(Formatting.None));
            tom.Name = model.Expressions.GetNewName(tom.Name);

            var expr = NamedExpression.CreateFromMetadata(model, tom);
            model.Expressions.Add(expr);

            return expr;
        }
        public static ModelRole DeserializeModelRole(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.ModelRole>(json.ToString(Formatting.None));
            tom.Name = model.Roles.GetNewName(tom.Name);

            var role = ModelRole.CreateFromMetadata(model, tom);

            return role;
        }

        public static Perspective DeserializePerspective(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Perspective>(json.ToString(Formatting.None));
            tom.Name = model.Perspectives.GetNewName(tom.Name);

            var tomModel = model.MetadataObject;
            foreach(var pt in tom.PerspectiveTables.ToList())
            {
                if (tomModel.Tables.Contains(pt.Name))
                {
                    var tomTable = tomModel.Tables[pt.Name];
                    foreach (var pc in pt.PerspectiveColumns.ToList()) if (!tomTable.Columns.Contains(pc.Name)) pt.PerspectiveColumns.Remove(pc.Name);
                    foreach (var pm in pt.PerspectiveMeasures.ToList()) if (!tomTable.Measures.Contains(pm.Name)) pt.PerspectiveMeasures.Remove(pm.Name);
                    foreach (var ph in pt.PerspectiveHierarchies.ToList()) if (!tomTable.Hierarchies.Contains(ph.Name)) pt.PerspectiveHierarchies.Remove(ph.Name);
                }
                else
                    tom.PerspectiveTables.Remove(pt.Name);
            }

            var perspective = Perspective.CreateFromMetadata(model, tom);


            return perspective;
        }

        public static Culture DeserializeCulture(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Culture>(json.ToString(Formatting.None));
            tom.Name = model.Cultures.GetNewName();
            var culture = Culture.CreateFromMetadata(model, tom);

            return culture;
        }
        public static ProviderDataSource DeserializeProviderDataSource(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.ProviderDataSource>(json.ToString(Formatting.None));
            tom.Name = model.DataSources.GetNewName(tom.Name);

            var dataSource = ProviderDataSource.CreateFromMetadata(model, tom);

            return dataSource;
        }

        public static StructuredDataSource DeserializeStructuredDataSource(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.StructuredDataSource>(json.ToString(Formatting.None));
            tom.Name = model.DataSources.GetNewName(tom.Name);

            var dataSource = StructuredDataSource.CreateFromMetadata(model, tom);
            return dataSource;
        }

        public static CalculationItem DeserializeCalculationItem(JObject json, CalculationGroupTable calculationGroupTable)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.CalculationItem>(json.ToString(Formatting.None));
            tom.Name = calculationGroupTable.CalculationItems.GetNewName(tom.Name);
            tom.Ordinal = calculationGroupTable.CalculationItems.Any(i => i.Ordinal != -1) ? calculationGroupTable.CalculationItems.Max(i => i.Ordinal) + 1 : -1;

            var calculationItem = CalculationItem.CreateFromMetadata(calculationGroupTable.CalculationGroup, tom);
            return calculationItem;
        }

        public static CalculationGroupTable DeserializeCalculationGroupTable(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None));
            tom.Name = model.Tables.GetNewName(tom.Name);

            var calculationGroupTable = CalculationGroupTable.CreateFromMetadata(model, tom);
            return calculationGroupTable;
        }
        #endregion
    }

    public static class ObjectJsonContainerHelper
    {
        public static IEnumerable<JObject> Get<T>(this ObjectJsonContainer container)
        {
            if (!container.ContainsKey(typeof(T))) yield break;
            foreach (var obj in container[typeof(T)]) yield return obj;
        }
    }


    public class ObjectJsonContainer : IReadOnlyDictionary<Type, JObject[]>
    {
        private Dictionary<Type, JObject[]> Dict;

        public Guid InstanceID = Guid.Empty;

        internal ObjectJsonContainer(JObject jObj)
        {
            InstanceID = jObj["InstanceID"] != null ? Guid.Parse(jObj["InstanceID"].ToString()) : Guid.Empty;
            Dict = ObjectMetadata.Creatable.Where(t => jObj[Serializer.TypeToJson(t)] != null)
                .ToDictionary(t => t, t => (jObj[Serializer.TypeToJson(t)] as JArray).Cast<JObject>().ToArray());
        }

        public JObject[] this[Type key]
        {
            get
            {
                return Dict[key];
            }
        }

        public int Count
        {
            get
            {
                return Dict.Count;
            }
        }

        public bool CanPaste(TabularModelHandler handler)
        {
            return Dict != null && Dict.Count > 0 && Dict.Keys.All(type => handler.PowerBIGovernance.AllowCreate(type));
        }

        public IEnumerable<Type> Keys
        {
            get
            {
                return Dict.Keys;
            }
        }

        public IEnumerable<JObject[]> Values
        {
            get
            {
                return Dict.Values;
            }
        }

        public bool ContainsKey(Type key)
        {
            return Dict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<Type, JObject[]>> GetEnumerator()
        {
            return Dict.GetEnumerator();
        }

        public bool TryGetValue(Type key, out JObject[] value)
        {
            return Dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

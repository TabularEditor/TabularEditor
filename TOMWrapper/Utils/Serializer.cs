using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Collections;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class Serializer
    {
        internal static string TypeToJson(Type type)
        {
            return type.Name.Pluralize().ToLower();
        }

        public static string SerializeObjects(IEnumerable<TabularObject> objects 
            , bool includeTranslations = true
            , bool includePerspectives = true
            , bool includeRLS = true
#if CL1400
            , bool includeOLS = true
#endif
            )
        {
            if (includeTranslations) foreach (var obj in objects.OfType<ITranslatableObject>()) obj.SaveTranslations(true);
            if (includePerspectives) foreach (var obj in objects.OfType<ITabularPerspectiveObject>()) obj.SavePerspectives(true);
            if (includeRLS) foreach (var obj in objects.OfType<Table>()) obj.SaveRLS();
            if (includeOLS)
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

                foreach (var obj in objects.OfType<IAnnotationObject>()) obj.ClearTabularEditorAnnotations();

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

        public static IList<TabularObject> DeserializeObjects(string json)
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
        }

        public static string SerializeDB(SerializeOptions options)
        {
            return TOM.JsonSerializer.SerializeDatabase(TabularModelHandler.Singleton.Database,
                new TOM.SerializeOptions()
                {
                    IgnoreInferredObjects = options.IgnoreInferredObjects,
                    IgnoreTimestamps = options.IgnoreTimestamps,
                    IgnoreInferredProperties = options.IgnoreInferredProperties,
                    SplitMultilineStrings = options.SplitMultilineStrings
                });
        }

        #region Individual object deserialization
        // TODO: Below code could maybe be auto-generated, as we know from reflection which properties
        // hold references to other objects, that needs to be looked up.
        public static Level DeserializeLevel(JObject json, Hierarchy target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Level>(json.ToString());
            tom.Name = target.Levels.GetNewName(tom.Name);
            tom.Column = target.Table.MetadataObject.Columns[json.Value<string>("column")];

            var level = Level.CreateFromMetadata(tom, false);
            target.Levels.Add(level);
            level.InitFromMetadata();

            return level;
        }

        public static CalculatedColumn DeserializeCalculatedColumn(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.CalculatedColumn>(json.ToString());
            tom.Name = target.Columns.GetNewName(tom.Name);
            tom.SortByColumn = json["sortByColumn"] != null ? target.MetadataObject.Columns[json.Value<string>("sortByColumn")] : null;

            var column = CalculatedColumn.CreateFromMetadata(tom, false);
            target.Columns.Add(column);
            column.InitFromMetadata();

            return column;
        }

        public static DataColumn DeserializeDataColumn(JObject json, Table target)
        {
#if CL1400
            if (TabularModelHandler.Singleton.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;
#endif

            var tom = TOM.JsonSerializer.DeserializeObject<TOM.DataColumn>(json.ToString());
            tom.Name = target.Columns.GetNewName(tom.Name);
            tom.SortByColumn = json["sortByColumn"] != null ? target.MetadataObject.Columns[json.Value<string>("sortByColumn")] : null;

            var column = DataColumn.CreateFromMetadata(tom, false);
            target.Columns.Add(column);
            column.InitFromMetadata();

            return column;
        }

        public static Measure DeserializeMeasure(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Measure>(json.ToString());
            tom.Name = target.Measures.GetNewName(tom.Name);

            var measure = Measure.CreateFromMetadata(tom, false);
            target.Measures.Add(measure);
            measure.InitFromMetadata();

            return measure;
        }

        public static Hierarchy DeserializeHierarchy(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Hierarchy>(json.ToString());
            tom.Name = target.Hierarchies.GetNewName(tom.Name);
            for(var i = 0; i < tom.Levels.Count; i++) tom.Levels[i].Column = target.MetadataObject.Columns[json["levels"][i].Value<string>("column")];

            var hierarchy = Hierarchy.CreateFromMetadata(tom, false);
            target.Hierarchies.Add(hierarchy);
            hierarchy.InitFromMetadata();

            return hierarchy;
        }

        public static Partition DeserializePartition(JObject json, Table target)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Partition>(json.ToString());
            tom.Name = target.Partitions.GetNewName(tom.Name);
            if(tom.Source is TOM.QueryPartitionSource)
            {
                (tom.Source as TOM.QueryPartitionSource).DataSource = target.MetadataObject.Model.DataSources[json["source"].Value<string>("dataSource")];
            }

            var partition = Partition.CreateFromMetadata(tom, false);
            target.Partitions.Add(partition);
            partition.InitFromMetadata();

            return partition;
        }

        public static CalculatedTable DeserializeCalculatedTable(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Table>(json.ToString());
            tom.Name = model.Tables.GetNewName(tom.Name);

            var table = CalculatedTable.CreateFromMetadata(tom, false);
            model.Tables.Add(table);
            table.InitFromMetadata();
            table.InitOLSIndexer();
            table.InitRLSIndexer();

            return table as CalculatedTable;
        }

        public static Table DeserializeTable(JObject json, Model model)
        {
            var tom = TOM.JsonSerializer.DeserializeObject<TOM.Table>(json.ToString());
            tom.Name = model.Tables.GetNewName(tom.Name);
            foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewMeasureName(m.Name);

            var table = Table.CreateFromMetadata(tom, false);
            model.Tables.Add(table);
            table.InitFromMetadata();
            table.InitOLSIndexer();
            table.InitRLSIndexer();

            return table;
        }
        #endregion
    }

    public class ObjectJsonContainer : IReadOnlyDictionary<Type, JObject[]>
    {
        private Dictionary<Type, JObject[]> Dict;

        internal ObjectJsonContainer(JObject jObj)
        {
            Dict = ObjectMetadata.Creatable.ToDictionary(t => t, t => 
             jObj[Serializer.TypeToJson(t)] == null ? new JObject[0] :
            (jObj[Serializer.TypeToJson(t)] as JArray).Cast<JObject>().ToArray());
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

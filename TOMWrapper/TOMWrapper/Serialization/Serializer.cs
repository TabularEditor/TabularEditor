using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace TabularEditor.TOMWrapper.Serialization;

internal class Serializer
{
    public bool Replace { get; set; } = false;
    private const string ANN_SAVESENSITIVE = "TabularEditor_SaveSensitive";

    public static JArray SortArray(JObject parentObject, string arrayProperty, string sortProperty = "name")
    {
        if (parentObject[arrayProperty] is JArray orgArray)
        {
            var newArray = new JArray(orgArray.OrderBy(k => k.Value<string>(sortProperty)));
            parentObject[arrayProperty] = newArray;
            return newArray;
        }
        return null;
    }

    public static void SortModelArrays(JObject jModel, SerializeOptions options, bool folderSerialization)
    {
        if (!options.SortArrays) return;

        if (!options.LocalPerspectives || !folderSerialization)
        {
            var perspectives = options.Levels.Contains("Perspectives") && folderSerialization ? jModel["perspectives"] as JArray : SortArray(jModel, "perspectives");
            if (perspectives != null)
                foreach (JObject perspective in perspectives)
                {
                    var perspectiveTables = SortArray(perspective, "tables");
                    if (perspectiveTables != null)
                        foreach (JObject perspectiveTable in perspectiveTables)
                        {
                            SortArray(perspectiveTable, "columns");
                            SortArray(perspectiveTable, "measures");
                            SortArray(perspectiveTable, "hierarchies");
                        }
                }
        }

        var roles = options.Levels.Contains("Roles") && folderSerialization ? jModel["roles"] as JArray : SortArray(jModel, "roles");
        if (roles != null)
            foreach (JObject role in roles)
            {
                var tablePermissions = SortArray(role, "tablePermissions");
                if (tablePermissions != null)
                    foreach (JObject tablePermission in tablePermissions)
                        SortArray(tablePermission, "columnPermissions");
            }
    }

    internal static void OverrideDataSources(JObject model, bool includeSensitive, Dictionary<string, DataSourceProperties> dataSourceOverrides)
    {
        if (dataSourceOverrides != null && dataSourceOverrides.Count > 0)
        {
            var dataSources = model?["dataSources"] as JArray;
            if (dataSources == null || dataSources.Count == 0) return;
            foreach (JObject dataSource in dataSources) OverrideDataSource(dataSource, includeSensitive, dataSourceOverrides);
        }
    }

    private static void OverrideDataSource(JObject dataSource, bool includeSensitive, Dictionary<string, DataSourceProperties> dataSourceOverrides)
    {
        if (!dataSourceOverrides.TryGetValue(dataSource.Value<string>("name"), out var sourceOverride)) return;

        if (dataSource.Value<string>("type") == "structured" && dataSource["credential"] is JObject credential)
        {
            if (!string.IsNullOrEmpty(sourceOverride.Username)) credential["Username"] = sourceOverride.Username;
            if (!string.IsNullOrEmpty(sourceOverride.PrivacySetting)) credential["PrivacySetting"] = sourceOverride.PrivacySetting;
            if (includeSensitive)
            {
                if (!string.IsNullOrEmpty(sourceOverride.Password)) credential["Password"] = sourceOverride.Password;
                if (!string.IsNullOrEmpty(sourceOverride.AccountKey)) credential["Key"] = sourceOverride.AccountKey;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(sourceOverride.Username)) dataSource["account"] = sourceOverride.Username;
            if (!string.IsNullOrEmpty(sourceOverride.Password)) dataSource["password"] = sourceOverride.Password;
            if (includeSensitive)
                if (!string.IsNullOrEmpty(sourceOverride.ConnectionString)) dataSource["connectionString"] = sourceOverride.ConnectionString;
        }
    }

    internal static string TypeToJson(Type type) => type.Name.Pluralize().ToLower();

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
                foreach (var culture in cultures) jw.WriteRawValue(TOM.JsonSerializer.SerializeObject(culture.MetadataObject));
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

        if (includeTranslations) foreach (var obj in objects.OfType<ITranslatableObject>()) obj.SaveTranslations(true);
        if (includePerspectives) foreach (var obj in objects.OfType<ITabularPerspectiveObject>()) obj.SavePerspectives(true);
        if (includeRLS) foreach (var obj in objects.OfType<Table>()) obj.SaveRLS();
        if (includeOLS && model.Handler.CompatibilityLevel >= 1400)
        {
            foreach (var obj in objects.OfType<Table>()) obj.SaveOLS(true);
            foreach (var obj in objects.OfType<Column>()) obj.SaveOLS();
        }

        var byType = objects.GroupBy(obj => obj.GetType(), obj => TOM.JsonSerializer.SerializeObject(obj.MetadataObject, null, obj.Handler.CompatibilityLevel, obj.Handler.Database.CompatibilityMode));

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
                    foreach (var obj in type) jw.WriteRawValue(obj);
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
        catch (JsonReaderException)
        {
            return null;
        }

        return new ObjectJsonContainer(jObj);
    }

    private static void RemoveNullTranslations(TOM.Culture culture)
    {
        foreach (var ot in culture.ObjectTranslations.Where(ot => ot.Object == null).ToList()) culture.ObjectTranslations.Remove(ot);
    }

    private static void RemoveNullTranslations(IEnumerable<TOM.Culture> cultures)
    {
        foreach (var culture in cultures) RemoveNullTranslations(culture);
    }

    public static string SerializeDB(TabularModelHandler handler, SerializeOptions options, bool includeTabularEditorTag)
    {
        var db = handler.Database;
        if (includeTabularEditorTag)
            db.AddTabularEditorTag();
        else
            db.RemoveTabularEditorTag();

        // Remove object translations with no objects assigned:
        RemoveNullTranslations(db.Model.Cultures);

        var serializeOptions = new TOM.SerializeOptions
        {
            IgnoreInferredObjects = options.IgnoreInferredObjects,
            IgnoreTimestamps = options.IgnoreTimestamps,
            IgnoreInferredProperties = options.IgnoreInferredProperties,
            SplitMultilineStrings = options.SplitMultilineStrings,
            IncludeRestrictedInformation = (db.Model.Annotations.Contains(ANN_SAVESENSITIVE) && db.Model.Annotations[ANN_SAVESENSITIVE].Value == "1") || options.IncludeSensitive
        };
        var serializedDB = TOM.JsonSerializer.SerializeDatabase(db, serializeOptions);

        // Hack: Remove \r characters from multiline strings in the BIM:
        // "1 + 2\r", -> "1 + 2",
        if (options.SplitMultilineStrings) serializedDB = serializedDB.Replace("\\r\",\r\n", "\",\r\n");

        var jObject = JObject.Parse(serializedDB);
        if (options.IgnoreLineageTags)
        {
            var lineageTags = jObject.Descendants().OfType<JProperty>().Where(p => p.Name == "lineageTag").ToList();
            foreach (var lineageTag in lineageTags) lineageTag.Remove();
        }
        // Remove privacy settings if specified:
        foreach (var obj in jObject.DescendantsAndSelf().OfType<JObject>().ToList())
            if (options.IgnorePrivacySettings)
                if (obj.ContainsKey("PrivacySetting")) obj.Remove("PrivacySetting");

        if (options.IgnoreIncrementalRefreshPartitions)
        {
            var partitionCollections = jObject.Descendants().OfType<JArray>().Where(o => o.Parent is JProperty jProp && jProp.Name == "partitions").ToList();
            foreach (var partitionCollection in partitionCollections)
                foreach (var partition in partitionCollection.OfType<JObject>().ToList())
                    if (partition.Has("mode", "import") && partition["source"] is JObject sourceObj && sourceObj.Has("type", "policyRange"))
                        partition.Remove();
        }
        if (!serializeOptions.IncludeRestrictedInformation)
            // Remove sensitive properties entirely (since TOM.JsonSerializer.SerializeDatabase would have asterisk'ed their values)
            foreach (var jDataSource in jObject.SelectTokens("model.dataSources[*]").OfType<JObject>())
                foreach (var jSensitive in jDataSource.Descendants().OfType<JProperty>().Where(p => p.Name.IsOneOf("key", "password", "pwd", "secret", "connectionString")).ToList())
                    if (jSensitive.Name.EqualsI("connectionString"))
                    {
                        // For connection strings, parse the string and remove the password property within the string:
                        var connectionString = jSensitive.Value.Value<string>();
                        try
                        {
                            var csb = new DbConnectionStringBuilder();
                            csb.ConnectionString = connectionString;
                            if (csb.TryGet(new[] { "password", "pwd", "secret", "key" }, out var propertyKey, out _))
                            {
                                csb.Remove(propertyKey);
                                connectionString = csb.ConnectionString;
                            }
                        }
                        catch { }

                        // Last resort if code above didn't manage to get rid of the password property:
                        jSensitive.Value = connectionString.Replace("********", "");
                    }
                    else // For other sensitive properties, remove the JSON property entirely:
                    {
                        jSensitive.Remove();
                    }

        serializedDB = jObject.ToString();

        return serializedDB;
    }

    #region Individual object deserialization

    // TODO: Below code could maybe be auto-generated, as we know from reflection which properties
    // hold references to other objects, that needs to be looked up.
    public Level DeserializeLevel(JObject json, Hierarchy target)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Level>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Levels.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Levels.GetNewName(tom.Name);
        tom.Column = target.Table.MetadataObject.Columns[json.Value<string>("column")];

        var level = Level.CreateFromMetadata(target, tom);

        return level;
    }

    public CalculatedColumn DeserializeCalculatedColumn(JObject json, Table target)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.CalculatedColumn>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Columns.FindByName(tom.Name)?.Delete();
        else
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

    public DataColumn DeserializeDataColumn(JObject json, Table target)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.DataColumn>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Columns.FindByName(tom.Name)?.Delete();
        else
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

    public Measure DeserializeMeasure(JObject json, Table target)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Measure>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Measures.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Measures.GetNewName(tom.Name);

        var measure = Measure.CreateFromMetadata(target, tom);

        return measure;
    }

    public Calendar DeserializeCalendar(JObject json, Table target)
    {
        EnsureCalendarColumnsExistOrRemoved(json, target);
        var tom = ObjectSerializer.DeserializeObject<TOM.Calendar>(json.ToString(Formatting.None), target.Model);
        if (Replace)
            target.Calendars.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Calendars.GetNewName(tom.Name);

        var calendar = Calendar.CreateFromMetadata(target, tom);
        return calendar;
    }

    public Hierarchy DeserializeHierarchy(JObject json, Table target)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Hierarchy>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Hierarchies.FindByName(tom.Name)?.Delete();
        else
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

    public Partition DeserializePartition(JObject json, Table target)
    {
        EnsureQueryGroupCreatedOrRemoved(json, target.Model);
        var tom = ObjectSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Partitions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Partitions.GetNewName(tom.Name);
        if (tom.Source is TOM.QueryPartitionSource) (tom.Source as TOM.QueryPartitionSource).DataSource = target.MetadataObject.Model.DataSources[json["source"].Value<string>("dataSource")];

        var partition = Partition.CreateFromMetadata(target, tom);

        return partition;
    }

    public MPartition DeserializeMPartition(JObject json, Table target)
    {
        EnsureQueryGroupCreatedOrRemoved(json, target.Model);
        var tom = ObjectSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Partitions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Partitions.GetNewName(tom.Name);
        var partition = MPartition.CreateFromMetadata(target, tom);
        return partition;
    }

    public PolicyRangePartition DeserializePolicyRangePartition(JObject json, Table target)
    {
        EnsureQueryGroupCreatedOrRemoved(json, target.Model);
        var tom = ObjectSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Partitions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Partitions.GetNewName(tom.Name);
        var partition = PolicyRangePartition.CreateFromMetadata(target, tom);
        return partition;
    }

    public EntityPartition DeserializeEntityPartition(JObject json, Table target)
    {
        EnsureQueryGroupCreatedOrRemoved(json, target.Model);
        EnsureExpressionSourceExistsOrRemoved(json["source"], target.Model);
        var tom = ObjectSerializer.DeserializeObject<TOM.Partition>(json.ToString(Formatting.None), target.Model);
        if(Replace)
            target.Partitions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = target.Partitions.GetNewName(tom.Name);
        var partition = EntityPartition.CreateFromMetadata(target, tom);
        return partition;
    }

    public CalculatedTable DeserializeCalculatedTable(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None), model);
        var tableOrgName = tom.Name;
        if(Replace)
            model.Tables.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Tables.GetNewName(tom.Name);

        // Make sure all measures in the table still have model-wide unique names:
        foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewName(model, m.Name);

        var table = CalculatedTable.CreateFromMetadata(model, tom);
        if (tableOrgName != tom.Name) table.SetAnnotation(TableRenamedAnnotation, tableOrgName, false);

        return table;
    }

    internal const string TableRenamedAnnotation = "TE3_TableRenamedFrom";

    public Table DeserializeTable(JObject json, Model model)
    {
        if (json["partitions"] is JArray jPartitions)
        {
            foreach (var jPartition in jPartitions)
            {
                EnsureQueryGroupCreatedOrRemoved(jPartition, model);
                EnsureExpressionSourceExistsOrRemoved(jPartition["source"], model);
            }
        }

        var tom = ObjectSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None), model);
        var tableOrgName = tom.Name;
        if(Replace)
            model.Tables.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Tables.GetNewName(tom.Name);

        // Make sure all measures in the table still have model-wide unique names:
        foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewName(model, m.Name);

        var table = Table.CreateFromMetadata(model, tom);
        if (tableOrgName != tom.Name) table.SetAnnotation(TableRenamedAnnotation, tableOrgName, false);

        return table;
    }

    private static void EnsureCalendarColumnsExistOrRemoved(JObject calendarSource, Table target)
    {
        var ccg = calendarSource["calendarColumnGroups"] as JArray;
        if (ccg == null) return;
        foreach (var jToken in ccg)
        {
            if (jToken is not JObject jObj) continue;
            var columns = jObj["columns"] as JArray ?? jObj["associatedColumns"] as JArray;
            if (columns != null)
            {
                foreach (var jColumn in columns.ToList())
                {
                    if (jColumn is JValue { Value: string columnName } && !target.Columns.Contains(columnName))
                    {
                        jColumn.Remove();
                    }
                }
            }
        }
    }

    private static void EnsureExpressionSourceExistsOrRemoved(JToken objectWithExpressionSource, Model model)
    {
        if (objectWithExpressionSource is JObject jObject)
        {
            if (jObject["expressionSource"]?.Value<string>() is { } expressionSource &&
                !string.IsNullOrEmpty(expressionSource))
            {
                if (model.Database.CompatibilityLevel < 1400
                    || model.Expressions.FindByName(expressionSource) == null)
                {
                    jObject.Remove("expressionSource");
                }
            }
        }
    }

    private static void EnsureQueryGroupCreatedOrRemoved(JToken objectWithQueryGroup, Model model)
    {
        // If the provided object contains a 'queryGroup' property, we ensure that the destination model
        // contains a Query Group with that name. If the destination model does not support query groups,
        // remove the 'queryGroup' property from the JObject, to avoid having invalid references in the
        // object tree.
        if (objectWithQueryGroup is JObject jObject)
        {
            if (jObject["queryGroup"]?.Value<string>() is { } queryGroup && !string.IsNullOrEmpty(queryGroup))
            {
                if (model.Database.CompatibilityLevel >= QueryGroup.RequiredCompatibilityLevel)
                {
                    if (model.QueryGroups.FindByName(queryGroup) == null)
                        model.AddQueryGroup(queryGroup);
                }
                else
                    jObject.Remove("queryGroup");
            }
        }
    }

    public SingleColumnRelationship DeserializeSingleColumnRelationship(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.SingleColumnRelationship>(json.ToString(Formatting.None), model);
        if(Replace)
        {
            var existing = model.Relationships.FirstOrDefault(r => r.Name == tom.Name);
            existing?.Delete();
        }
        else if (model.Relationships.TOM_ContainsName(tom.Name))
            tom.Name = Guid.NewGuid().ToString();

        var relationship = SingleColumnRelationship.CreateFromMetadata(model, tom);

        return relationship;
    }

    public NamedExpression DeserializeNamedExpression(JObject json, Model model)
    {
        EnsureQueryGroupCreatedOrRemoved(json, model);
        EnsureExpressionSourceExistsOrRemoved(json, model);
        var tom = ObjectSerializer.DeserializeObject<TOM.NamedExpression>(json.ToString(Formatting.None), model);
        if(Replace)
            model.Expressions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Expressions.GetNewName(tom.Name);

        var expr = NamedExpression.CreateFromMetadata(model, tom);
        model.Expressions.Add(expr);

        return expr;
    }

    public ModelRole DeserializeModelRole(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.ModelRole>(json.ToString(Formatting.None), model);
        if(Replace)
            model.Roles.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Roles.GetNewName(tom.Name);

        var role = ModelRole.CreateFromMetadata(model, tom);

        return role;
    }

    public Function DeserializeFunction(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Function>(json.ToString(Formatting.None), model);
        if (Replace)
            model.Functions.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Functions.GetNewName(tom.Name);

        var role = Function.CreateFromMetadata(model, tom);

        return role;
    }

    public Perspective DeserializePerspective(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Perspective>(json.ToString(Formatting.None), model);
        if (Replace)
            model.Perspectives.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Perspectives.GetNewName(tom.Name);

        var tomModel = model.MetadataObject;
        foreach (var pt in tom.PerspectiveTables.ToList())
            if (tomModel.Tables.Contains(pt.Name))
            {
                var tomTable = tomModel.Tables[pt.Name];
                foreach (var pc in pt.PerspectiveColumns.ToList()) if (!tomTable.Columns.Contains(pc.Name)) pt.PerspectiveColumns.Remove(pc.Name);
                foreach (var pm in pt.PerspectiveMeasures.ToList()) if (!tomTable.Measures.Contains(pm.Name)) pt.PerspectiveMeasures.Remove(pm.Name);
                foreach (var ph in pt.PerspectiveHierarchies.ToList()) if (!tomTable.Hierarchies.Contains(ph.Name)) pt.PerspectiveHierarchies.Remove(ph.Name);
            }
            else
            {
                tom.PerspectiveTables.Remove(pt.Name);
            }

        var perspective = Perspective.CreateFromMetadata(model, tom);


        return perspective;
    }

    public Culture DeserializeCulture(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Culture>(json.ToString(Formatting.None), model);
        RemoveNullTranslations(tom);
        if(Replace)
            model.Cultures.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Cultures.GetNewName();
        var culture = Culture.CreateFromMetadata(model, tom);

        return culture;
    }

    public ProviderDataSource DeserializeProviderDataSource(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.ProviderDataSource>(json.ToString(Formatting.None), model);
        if(Replace)
            model.DataSources.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.DataSources.GetNewName(tom.Name);

        var dataSource = ProviderDataSource.CreateFromMetadata(model, tom);

        return dataSource;
    }

    public StructuredDataSource DeserializeStructuredDataSource(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.StructuredDataSource>(json.ToString(Formatting.None), model);
        if(Replace)
            model.DataSources.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.DataSources.GetNewName(tom.Name);

        var dataSource = StructuredDataSource.CreateFromMetadata(model, tom);
        return dataSource;
    }

    public CalculationItem DeserializeCalculationItem(JObject json, CalculationGroupTable calculationGroupTable)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.CalculationItem>(json.ToString(Formatting.None), calculationGroupTable.Model);
        if(Replace)
            calculationGroupTable.CalculationItems.FindByName(tom.Name)?.Delete();
        else
            tom.Name = calculationGroupTable.CalculationItems.GetNewName(tom.Name);
        tom.Ordinal = calculationGroupTable.CalculationItems.Any(i => i.Ordinal != -1) ? calculationGroupTable.CalculationItems.Max(i => i.Ordinal) + 1 : -1;

        var calculationItem = CalculationItem.CreateFromMetadata(calculationGroupTable.CalculationGroup, tom);
        return calculationItem;
    }

    public CalculationGroupTable DeserializeCalculationGroupTable(JObject json, Model model)
    {
        var tom = ObjectSerializer.DeserializeObject<TOM.Table>(json.ToString(Formatting.None), model);
        var tableOrgName = tom.Name;
        if(Replace)
            model.Tables.FindByName(tom.Name)?.Delete();
        else
            tom.Name = model.Tables.GetNewName(tom.Name);

        // Make sure all measures in the table still have model-wide unique names:
        foreach (var m in tom.Measures.ToList()) m.Name = MeasureCollection.GetNewName(model, m.Name);

        var calculationGroupTable = CalculationGroupTable.CreateFromMetadata(model, tom);
        if (tableOrgName != tom.Name) calculationGroupTable.SetAnnotation(TableRenamedAnnotation, tableOrgName, false);
        return calculationGroupTable;
    }

    #endregion
}

internal static class ObjectJsonContainerHelper
{
    public static IEnumerable<JObject> Get<T>(this ObjectJsonContainer container)
    {
        if (!container.ContainsKey(typeof(T))) yield break;
        foreach (var obj in container[typeof(T)]) yield return obj;
    }
}


public class ObjectJsonContainer: IReadOnlyDictionary<Type, JObject[]>
{
    private readonly Dictionary<Type, JObject[]> Dict;

    public Guid InstanceID = Guid.Empty;

    internal ObjectJsonContainer(JObject jObj)
    {
        InstanceID = jObj["InstanceID"] != null ? Guid.Parse(jObj["InstanceID"].ToString()) : Guid.Empty;
        Dict = ObjectMetadata.Creatable.Where(t => jObj[Serializer.TypeToJson(t)] != null)
            .ToDictionary(t => t, t => (jObj[Serializer.TypeToJson(t)] as JArray).Cast<JObject>().ToArray());
    }

    internal ObjectJsonContainer(IDictionary<Type, JObject[]> objects)
    {
        Dict = new Dictionary<Type, JObject[]>(objects);
    }

    public JObject[] this[Type key] => Dict[key];

    public int Count => Dict.Count;

    public IEnumerable<Type> Keys => Dict.Keys;

    public IEnumerable<JObject[]> Values => Dict.Values;

    public bool ContainsKey(Type key) => Dict.ContainsKey(key);

    public IEnumerator<KeyValuePair<Type, JObject[]>> GetEnumerator() => Dict.GetEnumerator();

    public bool TryGetValue(Type key, out JObject[] value) => Dict.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool CanPaste(TabularModelHandler handler)
    {
        return Dict != null && Dict.Count > 0 && Dict.Keys.All(type => handler.PowerBIGovernance.AllowCreate(type));
    }
}

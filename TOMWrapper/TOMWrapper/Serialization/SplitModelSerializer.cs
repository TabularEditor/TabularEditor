using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class SplitModelSerializer
    {
        class FileWriter
        {
            private HashSet<string> CurrentFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            private HashSet<char> InvalidFileChars = new HashSet<char>(Path.GetInvalidFileNameChars());

            /// <summary>
            /// Sanitize a string to turn it into a valid file name
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            private string Sanitize(string fileName)
            {
                var sb = new StringBuilder();
                foreach (var c in fileName)
                {
                    if (InvalidFileChars.Contains(c))
                    {
                        sb.Append("%");
                        sb.Append(((byte)c).ToString("x2"));
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }

            /// <summary>
            /// Saves the model to the specified folder using the specified serialize options.
            /// </summary>
            public void SaveToFolder(Model model, string path, SerializeOptions options)
            {
                if (options.LocalTranslations) model.StoreTranslationsAsAnnotations();
                if (options.LocalPerspectives) model.StorePerspectivesToAnnotations();
                if (options.LocalRelationships) model.StoreRelationshipsAsAnnotations();

                var json = Serializer.SerializeDB(options, false);
                var jobj = JObject.Parse(json);

                jobj["name"] = model.Database?.Name ?? "SemanticModel";
                if (model.Database != null)
                {
                    if (!model.Database.Name.EqualsI(model.Database.ID)) jobj["id"] = model.Database.ID;
                    else if (jobj["id"] != null) jobj["id"].Remove();
                }

                if (options.DatabaseNameOverride != null)
                {
                    jobj["name"] = options.DatabaseNameOverride;
                    if (jobj["id"] != null) jobj["id"].Remove();
                }

                var jModel = jobj["model"] as JObject;
                var dataSources = options.Levels.Contains("Data Sources") ? PopArray(jModel, "dataSources") : null;
                var expressions = options.Levels.Contains("Shared Expressions") ? PopArray(jModel, "expressions") : null;
                var tables = options.Levels.Contains("Tables") ? PopArray(jModel, "tables") : null;
                var relationships = options.Levels.Contains("Relationships") || options.LocalRelationships ? PopArray(jModel, "relationships") : null;
                var cultures = options.Levels.Contains("Translations") || options.LocalTranslations ? PopArray(jModel, "cultures") : null;
                var perspectives = options.Levels.Contains("Perspectives") || options.LocalPerspectives ? PopArray(jModel, "perspectives") : null;
                var roles = options.Levels.Contains("Roles") ? PopArray(jModel, "roles") : null;

                WriteIfChanged(path + "\\database.json", jobj.ToString(Formatting.Indented));

                if (relationships != null && !options.LocalRelationships) OutArray(path, "relationships", relationships, options);
                if (perspectives != null && !options.LocalPerspectives) OutArray(path, "perspectives", perspectives, options);
                if (cultures != null && !options.LocalTranslations) OutArray(path, "cultures", cultures, options);
                if (dataSources != null) OutArray(path, "dataSources", dataSources, options);
                if (expressions != null) OutArray(path, "expressions", expressions, options);
                if (roles != null) OutArray(path, "roles", roles, options);

                if (tables != null)
                {
                    int n = 0;
                    foreach (JObject t in tables)
                    {
                        var table = model.Tables[t["name"].ToString()];

                        var columns = options.Levels.Contains("Tables/Columns") ? PopArray(t, "columns") : null;
                        var partitions = options.Levels.Contains("Tables/Partitions") ? PopArray(t, "partitions") : null;
                        var measures = options.Levels.Contains("Tables/Measures") ? PopArray(t, "measures") : null;
                        var hierarchies = options.Levels.Contains("Tables/Hierarchies") ? PopArray(t, "hierarchies") : null;
                        var annotations = options.Levels.Contains("Tables/Annotations") ? PopArray(t, "annotations") : null;
                        var calculationItems = options.Levels.Contains("Tables/Calculation Items") ? PopArray(t, "calculationGroup.calculationItems") : null;

                        var tableName = Sanitize(table.Name);
                        var tablePath = path + "\\tables\\" + (options.PrefixFilenames ? n.ToString("D3") + " " : "") + tableName;

                        var p = tablePath + "\\" + tableName + ".json";
                        var fi = new FileInfo(p);
                        if (!fi.Directory.Exists) fi.Directory.Create();
                        WriteIfChanged(p, t.ToString(Formatting.Indented));

                        if (measures != null) OutArray(tablePath, "measures", measures, options);
                        if (columns != null) OutArray(tablePath, "columns", columns, options);
                        if (hierarchies != null) OutArray(tablePath, "hierarchies", hierarchies, options);
                        if (partitions != null) OutArray(tablePath, "partitions", partitions, options);
                        if (annotations != null) OutArray(tablePath, "annotations", annotations, options);
                        if (calculationItems != null) OutArray(tablePath, "calculationItems", calculationItems, options);

                        n++;
                    }
                }

                RemoveUnusedFiles(path + "\\tables", CurrentFiles);
                RemoveUnusedFiles(path + "\\relationships", CurrentFiles);
                RemoveUnusedFiles(path + "\\perspectives", CurrentFiles);
                RemoveUnusedFiles(path + "\\cultures", CurrentFiles);
                RemoveUnusedFiles(path + "\\dataSources", CurrentFiles);
                RemoveUnusedFiles(path + "\\expressions", CurrentFiles);
                RemoveUnusedFiles(path + "\\roles", CurrentFiles);
            }

            private void RemoveUnusedFiles(string path, HashSet<string> currentFiles, bool recursive = true)
            {
                if (!Directory.Exists(path)) return;
                foreach (var f in Directory.GetFiles(path, "*.json"))
                {
                    if (!currentFiles.Contains(f, StringComparer.InvariantCultureIgnoreCase))
                        File.Delete(f);
                }

                foreach (var d in Directory.GetDirectories(path))
                {
                    RemoveUnusedFiles(d, currentFiles);
                    if (!Directory.EnumerateFileSystemEntries(d).Any()) Directory.Delete(d);
                }
            }


            /// <summary>
            /// Writes textual data to a file, but only if the file does not already contain the exact same text.
            /// Automatically creates a directory for the file, if it doesn't already exist.
            /// </summary>
            private void WriteIfChanged(string path, string content)
            {
                CurrentFiles.Add(path);
                var fi = new FileInfo(path);
                if (!fi.Directory.Exists) fi.Directory.Create();
                else if (fi.Exists)
                {
                    var s = File.ReadAllText(path);
                    if (content.Equals(s, StringComparison.InvariantCulture)) return;
                }
                File.WriteAllText(path, content);
            }

            private void OutArray(string path, string arrayName, JArray array, SerializeOptions options)
            {
                int n = 0;
                foreach (var t in array)
                {
                    var p = path + "\\" + arrayName + "\\" + (options.PrefixFilenames ? n.ToString("D3") + " " : "") + Sanitize(t["name"].ToString()) + ".json";
                    var fi = new FileInfo(p);
                    if (!fi.Directory.Exists) fi.Directory.Create();
                    WriteIfChanged(p, t.ToString(Formatting.Indented));
                    n++;
                }
            }

            private JArray PopArray(JObject obj, string arrayPath)
            {
                var objPath = arrayPath.Split('.');
                for(int i = 0; i < objPath.Length - 1; i++)
                {
                    obj = obj[objPath[i]] as JObject;
                    if (obj == null) return null;
                }

                var arrayName = objPath.Last();

                var result = obj[arrayName] as JArray;
                obj.Remove(arrayName);
                return result;
            }
        }
        public static void SaveToFolder(this Model model, string path, SerializeOptions options)
        {
            var serializer = new FileWriter();
            serializer.SaveToFolder(model, path, options);
        }


        public static string CombineFolderJson(string path)
        {
            if (!File.Exists(path + "\\database.json")) throw new FileNotFoundException("This folder does not contain a database.json file");

            var jobj = JObjectParse(path + "\\database.json");
            var model = jobj["model"] as JObject;

            JArray annotatedRelationships = new JArray();

            InArray(path, "dataSources", model);
            InArray(path, "expressions", model);
            if (Directory.Exists(path + "\\tables"))
            {
                var tables = new JArray();
                foreach (var tablePath in Directory.GetDirectories(path + "\\tables"))
                {
                    var filesInTableFolder = Directory.GetFiles(tablePath, "*.json");
                    if (filesInTableFolder.Length != 1) throw new FileNotFoundException(string.Format("Folder '{0}' is expected to contain exactly one .json file.", tablePath));
                    var tableFile = filesInTableFolder[0];

                    var table = JObjectParse(tableFile);
                    InArray(tablePath, "columns", table);
                    InArray(tablePath, "partitions", table);
                    InArray(tablePath, "measures", table);
                    InArray(tablePath, "hierarchies", table);
                    InArray(tablePath, "annotations", table);
                    InArray(tablePath, "calculationGroup.calculationItems", table);

                    tables.Add(table);
                }
                model.Add("tables", tables);
            }
            InArray(path, "relationships", model);
            InArray(path, "cultures", model);
            InArray(path, "perspectives", model);
            InArray(path, "roles", model);

            ResolveAnnotations(model);

            return jobj.ToString();
        }

        private static void ResolveAnnotations(JObject model)
        {
            // Relationships:
            var relationships = new JArray();
            foreach (var table in model.Enum("tables")) GetAnnotatedRelationships(table, relationships);
            if (relationships.Count > 0) model["relationships"] = relationships;

            // Perspectives:
            var perspectivesJson = model.GetAnnotation(AnnotationHelper.ANN_PERSPECTIVES, true);
            if (perspectivesJson != null) model["perspectives"] = ConvertPerspectivesJson(perspectivesJson);

            // Cultures:
            var culturesJson = model.GetAnnotation(AnnotationHelper.ANN_CULTURES, true);
            if (culturesJson != null) model["cultures"] = ConvertCulturesJson(culturesJson);

            // Perspective memberships:
            foreach (var table in model.Enum("tables")) ResolveTablePerspective(table, model);

            // Translations:
            if (model["cultures"] != null) ResolveTranslations(model);
        }

        private static void ResolveTranslations(JObject model)
        {
            ApplyAllTranslations(model, c => GetOrCreateModelTranslation(model, c));
            foreach (var perspective in model.Enum("perspectives")) ApplyAllTranslations(perspective, c => GetOrCreatePerspectiveTranslation(model, c, (string)perspective["name"]));
            foreach (var table in model.Enum("tables"))
            {
                var tableName = (string)table["name"];
                ApplyAllTranslations(table, c => GetOrCreateTableTranslation(model, c, tableName));
                foreach (var measure in table.Enum("measures")) ApplyAllTranslations(measure, c => GetOrCreateMeasureTranslation(model, c, tableName, (string)measure["name"]));
                foreach (var column in table.Enum("columns")) ApplyAllTranslations(column, c => GetOrCreateColumnTranslation(model, c, tableName, (string)column["name"]));
                foreach (var hierarchy in table.Enum("hierarchies"))
                {
                    var hierarchyName = (string)hierarchy["name"];
                    ApplyAllTranslations(hierarchy, c => GetOrCreateHierarchyTranslation(model, c, tableName, hierarchyName));
                    foreach (var level in hierarchy.Enum("levels")) ApplyAllTranslations(level, c => GetOrCreateLevelTranslation(model, c, tableName, hierarchyName, (string)level["name"]));
                }
            }
        }

        private static JObject GetOrCreatePerspectiveTranslation(JObject model, string cultureName, string perspectiveName)
        {
            var modelTran = GetOrCreateModelTranslation(model, cultureName);
            return modelTran.GetOrCreateArrayObj("perspectives", perspectiveName);
        }
        private static JObject GetOrCreateModelTranslation(JObject model, string cultureName)
        {
            var culture = GetOrCreateCulture(model, cultureName);
            var translations = culture["translations"] as JObject;
            if(translations == null)
            {
                translations = new JObject();
                culture["translations"] = translations;
            }
            var modelTran = translations["model"] as JObject;
            if(modelTran == null)
            {
                modelTran = new JObject();
                modelTran["name"] = model["name"] == null ? "Model" : (string)model["name"];
                translations["model"] = modelTran;
            }
            return modelTran;
        }
        private static JObject GetOrCreateTableTranslation(JObject model, string cultureName, string tableName)
        {
            var modelTran = GetOrCreateModelTranslation(model, cultureName);
            return modelTran.GetOrCreateArrayObj("tables", tableName);
        }
        private static JObject GetOrCreateColumnTranslation(JObject model, string cultureName, string tableName, string columnName)
        {
            var tableTran = GetOrCreateTableTranslation(model, cultureName, tableName);
            return tableTran.GetOrCreateArrayObj("columns", columnName);
        }
        private static JObject GetOrCreateMeasureTranslation(JObject model, string cultureName, string tableName, string measureName)
        {
            var tableTran = GetOrCreateTableTranslation(model, cultureName, tableName);
            return tableTran.GetOrCreateArrayObj("measures", measureName);
        }
        private static JObject GetOrCreateHierarchyTranslation(JObject model, string cultureName, string tableName, string hierarchyName)
        {
            var tableTran = GetOrCreateTableTranslation(model, cultureName, tableName);
            return tableTran.GetOrCreateArrayObj("hierarchies", hierarchyName);
        }
        private static JObject GetOrCreateLevelTranslation(JObject model, string cultureName, string tableName, string hierarchyName, string levelName)
        {
            var hierarchyTran = GetOrCreateHierarchyTranslation(model, cultureName, tableName, hierarchyName);
            return hierarchyTran.GetOrCreateArrayObj("levels", levelName);
        }
        private static JObject GetOrCreateCulture(JObject model, string cultureName)
        {
            return model.GetOrCreateArrayObj("cultures", cultureName);
        }
        private static JObject GetOrCreateArrayObj(this JObject baseObject, string arrayName, string objectName)
        {
            var array = baseObject.Sub(arrayName);
            var result = array.OfType<JObject>().FirstOrDefault(j => j["name"] != null && (string)j["name"] == objectName);
            if (result == null)
            {
                result = new JObject();
                result["name"] = objectName;
                array.Add(result);
            }
            return result;
        }

        private static void ApplyTranslations(string annotatedTranslationJson, string translatedProperty, Func<string, JObject> translation)
        {
            if (annotatedTranslationJson[0] == '[')
            {
                var jTranArr = JArray.Parse(annotatedTranslationJson);
                foreach (var item in jTranArr)
                {
                    translation((string)item["Key"])[translatedProperty] = (string)item["Value"];
                }
            }
            else
            {
                var jTran = JObject.Parse(annotatedTranslationJson);
                foreach (var prop in jTran.Properties())
                {
                    translation(prop.Name)[translatedProperty] = (string)prop.Value;
                }
            }
        }
        private static void ApplyAllTranslations(JObject translatableObject, Func<string, JObject> translation)
        {
            var translatedNamesJson = translatableObject.GetAnnotation(AnnotationHelper.ANN_NAMES, true);
            var translatedDescriptionsJson = translatableObject.GetAnnotation(AnnotationHelper.ANN_DESCRIPTIONS, true);
            var translatedDisplayFoldersJson = translatableObject.GetAnnotation(AnnotationHelper.ANN_DISPLAYFOLDERS, true);

            if (translatedNamesJson != null) ApplyTranslations(translatedNamesJson, "translatedCaption", translation);
            if (translatedDescriptionsJson != null) ApplyTranslations(translatedDescriptionsJson, "translatedDescription", translation);
            if (translatedDisplayFoldersJson != null) ApplyTranslations(translatedDisplayFoldersJson, "translatedDisplayFolder", translation);
        }

        /// <summary>
        /// Converts an array of culture names into an equivalent TOM representation of the Culture
        /// </summary>
        /// <returns></returns>
        private static JArray ConvertCulturesJson(string culturesAnnotationJson)
        {
            var cultures = JsonConvert.DeserializeObject<IEnumerable<string>>(culturesAnnotationJson);
            var result = new JArray();
            foreach(var culture in cultures)
            {
                var cult = new JObject();
                cult["name"] = culture;
                cult["translations"] = new JObject();
                result.Add(cult);
            }
            return result;
        }

        /// <summary>
        /// Converts a string representing an array of <see cref="PerspectiveCollection.SerializedPerspective"/> objects into an equivalent TOM representation
        /// </summary>
        /// <returns></returns>
        private static JArray ConvertPerspectivesJson(string perspectivesAnnotationJson)
        {
            var perspectives = JsonConvert.DeserializeObject<PerspectiveCollection.SerializedPerspective[]>(perspectivesAnnotationJson);
            var result = new JArray();
            foreach(var perspective in perspectives)
            {
                var obj = new JObject();
                obj["name"] = perspective.Name;
                obj["description"] = perspective.Description;
                if(perspective.Annotations.Count > 0)
                {
                    var anns = new JArray();
                    foreach(var kvp in perspective.Annotations)
                    {
                        var ann = new JObject();
                        ann["name"] = kvp.Key;
                        ann["value"] = kvp.Value;
                        anns.Add(ann);
                    }
                    obj["annotations"] = anns;
                }
                result.Add(obj);
            }
            return result;
        }

        private static HashSet<string> StringToHashSet(string jsonStringArray)
        {
            if (jsonStringArray == null) return new HashSet<string>();
            return new HashSet<string>(JsonConvert.DeserializeObject<IEnumerable<string>>(jsonStringArray), StringComparer.InvariantCultureIgnoreCase);
        }

        private static void ResolveTablePerspective(JObject table, JObject model)
        {
            var inPerspectives = StringToHashSet(table.GetAnnotation(AnnotationHelper.ANN_INPERSPECTIVE, true));
            if (inPerspectives.Count == 0) return;

            bool any = false;

            var perspectiveTableMap = new List<Tuple<string, JObject>>();

            foreach(var perspective in model.Enum("perspectives"))
            {
                var perspectiveName = (string)perspective["name"];
                if (inPerspectives.Contains(perspectiveName))
                {
                    var perspectiveTable = new JObject();
                    perspectiveTable["name"] = table["name"];
                    perspective.Sub("tables").Add(perspectiveTable);
                    perspectiveTableMap.Add(Tuple.Create(perspectiveName, perspectiveTable));

                    any = true;
                }
            }
            if (!any) return;

            ResolveObjectPerspective(table, perspectiveTableMap, "measures");
            ResolveObjectPerspective(table, perspectiveTableMap, "columns");
            ResolveObjectPerspective(table, perspectiveTableMap, "hierarchies");
        }

        private static void ResolveObjectPerspective(JObject table, List<Tuple<string, JObject>> perspectiveTableMap, string collectionName)
        {
            foreach(var obj in table.Enum(collectionName))
            {
                var inPerspectives = StringToHashSet(obj.GetAnnotation(AnnotationHelper.ANN_INPERSPECTIVE, true));
                foreach (var p in perspectiveTableMap)
                {
                    if (inPerspectives.Contains(p.Item1))
                    {
                        var pObj = new JObject();
                        pObj["name"] = obj["name"];
                        p.Item2.Sub(collectionName).Add(pObj);
                    }
                }
            }
        }

        private static string GetAnnotation(this JObject obj, string annotationName, bool removeIfFound = false)
        {
            if (!(obj["annotations"] is JArray annotations))
                return null;
            var annotation = annotations.OfType<JObject>().FirstOrDefault(j => (string)j["name"] == annotationName);
            if (annotation == null)
                return null;

            var value = annotation["value"];
            if (removeIfFound) annotation.Remove();

            if (value.Type == JTokenType.String)
                return (string)value;
            else if (value.Type == JTokenType.Array) 
                return string.Join("\r\n", (value as JArray).Select(j => (string)j).ToArray());
            else 
                throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or creates the specified array
        /// </summary>
        /// <param name="baseObject"></param>
        /// <param name="arrayProperty"></param>
        /// <returns></returns>
        private static JArray Sub(this JObject baseObject, string arrayProperty)
        {
            if (!(baseObject[arrayProperty] is JArray array))
            {
                array = new JArray();
                baseObject.Add(arrayProperty, array);
            }
            return array;
        }

        /// <summary>
        /// Enumerates all JObjects of the specified JArray (provided it exists)
        /// </summary>
        /// <param name="baseObject"></param>
        /// <param name="arrayProperty"></param>
        /// <returns></returns>
        private static IEnumerable<JObject> Enum(this JObject baseObject, string arrayProperty)
        {
            if (baseObject[arrayProperty] is JArray array)
                return array.OfType<JObject>();
            else
                return Enumerable.Empty<JObject>();
        }
        private static IEnumerable<JObject> Enum(this JArray baseArray)
        {
            return baseArray.OfType<JObject>();
        }

        private static void GetAnnotatedRelationships(JObject table, JArray relationships)
        {
            var relationshipJson = table.GetAnnotation(AnnotationHelper.ANN_RELATIONSHIPS, true);
            if (relationshipJson == null) return;

            var annotatedRelationships = JArray.Parse(relationshipJson);
            foreach (var relationship in annotatedRelationships)
                relationships.Add(relationship);
        }

        private static void InArray(string path, string arrayPath, JObject baseObject)
        {
            var objPath = arrayPath.Split('.');
            var arrayName = objPath.Last();

            var array = new JArray();
            if (Directory.Exists(path + "\\" + arrayName))
            {
                foreach (var file in Directory.GetFiles(path + "\\" + arrayName, "*.json").OrderBy(n => n))
                {
                    array.Add(JObjectParse(file));
                }

                for(int i = 0; i < objPath.Length - 1; i++)
                {
                    baseObject = baseObject[objPath[i]] as JObject;
                }

                if (baseObject[arrayName] is JArray existingArray)
                    existingArray.Merge(array);
                else
                    baseObject.Add(arrayName, array);
            }
        }

        private static JObject JObjectParse(string path)
        {
            try
            {
                return JObject.Parse(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to load Tabular Model (Compatibility Level 1200+) from { path }.\r\nError:\r\n{ ex.GetType() } - { ex.Message }", ex);
            }
        }
    }
}

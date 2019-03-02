extern alias json;

using json::Newtonsoft.Json;
using json::Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public void SaveToFolder(Model model, string path, SerializeOptions options, string replaceId = null)
            {
                if (options.LocalTranslations) model.StoreTranslationsAsAnnotations();
                if (options.LocalPerspectives) model.StorePerspectivesToAnnotations();
                if (options.LocalRelationships) model.StoreRelationshipsAsAnnotations();

                var json = Serializer.SerializeDB(options);
                var jobj = JObject.Parse(json);

                if(replaceId != null)
                {
                    jobj["name"] = replaceId;
                    if (jobj["id"] != null) jobj["id"].Remove();
                }

                var jModel = jobj["model"] as JObject;
                var dataSources = options.Levels.Contains("Data Sources") ? PopArray(jModel, "dataSources") : null;
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
        public static void SaveToFolder(this Model model, string path, SerializeOptions options, string replaceId = null)
        {
            var serializer = new FileWriter();
            serializer.SaveToFolder(model, path, options, replaceId);
        }


        public static string CombineFolderJson(string path)
        {
            if (!File.Exists(path + "\\database.json")) throw new FileNotFoundException("This folder does not contain a database.json file");

            var jobj = JObject.Parse(File.ReadAllText(path + "\\database.json"));
            var model = jobj["model"] as JObject;

            InArray(path, "dataSources", model);
            if (Directory.Exists(path + "\\tables"))
            {
                var tables = new JArray();
                foreach (var tablePath in Directory.GetDirectories(path + "\\tables"))
                {
                    var filesInTableFolder = Directory.GetFiles(tablePath, "*.json");
                    if (filesInTableFolder.Length != 1) throw new FileNotFoundException(string.Format("Folder '{0}' is expected to contain exactly one .json file.", tablePath));
                    var tableFile = filesInTableFolder[0];

                    var table = JObject.Parse(File.ReadAllText(tableFile));
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

            return jobj.ToString();
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
                    array.Add(JObject.Parse(File.ReadAllText(file)));
                }

                for(int i = 0; i < objPath.Length - 1; i++)
                {
                    baseObject = baseObject[objPath[i]] as JObject;
                }

                baseObject.Add(arrayName, array);
            }
        }
    }

}

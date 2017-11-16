using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Scripting
{
    public static class PropertySerializer
    {
        private static string GetTsvForObject(TabularObject obj, string properties)
        {
            var props = properties.Split(',');
            var sb = new StringBuilder();
            sb.Append(obj.GetObjectPath());
            foreach (var prop in props)
            {
                sb.Append('\t');
                var pInfo = obj.GetType().GetProperty(prop);
                if (pInfo != null)
                {
                    var pValue = pInfo.GetValue(obj);
                    if (pValue == null)
                        continue;
                    else if (pValue is TabularObject)
                        // Improve GetObjectPath to always provide unique path, and create corresponding method to resolve a path
                        sb.Append((pValue as TabularObject).GetObjectPath());
                    else
                        sb.Append(pValue.ToString().Replace("\n", "\\n").Replace("\t", "\\t"));
                }
            }
            return sb.ToString();
        }

        [ScriptMethod]
        // TODO: Provide more formatting options
        public static string ExportProperties(this IEnumerable<ITabularNamedObject> objects, string properties = "Name,Description,Expression,FormatString,DataType")
        {
            var sb = new StringBuilder();
            sb.Append("Object\t");
            sb.Append(properties.Replace(",", "\t"));
            foreach (var obj in objects.OfType<TabularObject>())
            {
                // Only certain types of objects can have their properties exported:
                // TODO: Change this to a HashSet lookup
                switch(obj.ObjectType)
                {
                    case ObjectType.Table:
                    case ObjectType.Partition:
                    case ObjectType.DataSource:
                    case ObjectType.Expression:
                    case ObjectType.Column:
                    case ObjectType.Role:
                    case ObjectType.Model:
                    case ObjectType.Hierarchy:
                    case ObjectType.Level:
                    case ObjectType.Measure:
                    case ObjectType.KPI:
                    case ObjectType.Relationship:
                        break;
                    default:
                        continue;
                }

                sb.Append("\n");
                sb.Append(GetTsvForObject(obj, properties));
            }
            return sb.ToString();
        }

        [ScriptMethod]
        public static void ImportProperties(string tsvData)
        {
            var rows = tsvData.Split('\n');
            var properties = string.Join(",", rows[0].Split('\t').Skip(1).ToArray());
            foreach (var row in rows.Skip(1))
            {
                if (!string.IsNullOrWhiteSpace(row))
                    AssignTsvToObject(row, properties);
            }
        }

        private static void AssignTsvToObject(string propertyValues, string properties)
        {
            var props = properties.Split(',');
            var values = propertyValues.Split('\t').Select(v => v.Replace("\\n", "\n").Replace("\\t", "\t")).ToArray();
            var obj = ResolveObjectPath(values[0]);
            if (obj == null) return;

            for (int i = 0; i < props.Length; i++) {
                var pInfo = obj.GetType().GetProperty(props[i]);

                // Consider only properties that exist, and have a public setter:
                if (pInfo == null || !pInfo.CanWrite || !pInfo.GetSetMethod(true).IsPublic) continue;

                var pValue = values[i + 1]; // This is shifted by 1 since the first column is the Object path
                if (typeof(TabularObject).IsAssignableFrom(pInfo.PropertyType))
                {
                    // Object references need to be resolved:
                    var pValueObj = ResolveObjectPath(pValue);
                    pInfo.SetValue(obj, pValueObj);
                }
                else if (pInfo.PropertyType.IsEnum)
                {
                    // Value is conerted from string to an enum type:
                    pInfo.SetValue(obj, Enum.Parse(pInfo.PropertyType, pValue));
                }
                else {
                    // Value is converted directly from string to the type of the property:
                    pInfo.SetValue(obj, Convert.ChangeType(pValue, pInfo.PropertyType));
                }
            }
        }

        private static TabularObject ResolveObjectPath(string path)
        {
            var parts = path.Split('.');
            var model = UI.UIController.Current.Handler?.Model;
            if (model == null) return null;
            TabularObject obj = model;
            foreach (var part in parts)
            {
                if (part == "Model") continue;
                if (obj is Model)
                {
                    obj = model.Tables[part];
                    continue;
                }
                if (obj is Table)
                {
                    obj = (obj as Table).GetChildren().OfType<TabularNamedObject>().FirstOrDefault(c => c.Name == part);
                    continue;
                }
                if (obj is Hierarchy)
                {
                    obj = (obj as Hierarchy).Levels.FirstOrDefault(l => l.Name == part);
                    continue;
                }
                obj = null;
                break;
            }
            return obj;
        }
    }
}

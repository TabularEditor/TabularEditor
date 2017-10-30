using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Utils
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

        // TODO: Provide more formatting options
        public static string ExportProperties(this IEnumerable<ITabularNamedObject> objects, string properties = "Name,Description,Expression,FormatString,DataType")
        {
            var sb = new StringBuilder();
            sb.Append("Object\t");
            sb.Append(properties.Replace(",", "\t"));
            foreach (var obj in objects.OfType<TabularObject>())
            {
                sb.Append("\n");
                sb.Append(GetTsvForObject(obj, properties));
            }
            return sb.ToString();
        }
    }
}

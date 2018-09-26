using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public static class TabularObjectHelper
    {
        public static string GetName(this ITabularObject obj)
        {
            if ((obj is ITabularNamedObject)) return (obj as ITabularNamedObject)?.Name;

            var tryRlsFilterExpr = obj as RLSFilterExpression;
            if(tryRlsFilterExpr != null) return tryRlsFilterExpr.Role.Name;

            return obj.GetTypeName();
        }

        public static void CopyTranslationsFrom(this ITranslatableObject target, ITranslatableObject src)
        {
            target.TranslatedNames.CopyFrom(src.TranslatedNames);
            target.TranslatedDescriptions.CopyFrom(src.TranslatedDescriptions);
            if (target is IFolderObject && src is IFolderObject)
            {
                ((IFolderObject)target).TranslatedDisplayFolders.CopyFrom(((IFolderObject)src).TranslatedDisplayFolders);
            }
        }

        public static string GetLinqPath(this ITabularNamedObject obj)
        {
            switch (obj.ObjectType)
            {
                case ObjectType.KPI:
                    return (obj as KPI).Measure.GetLinqPath() + ".KPI";
                case ObjectType.Model:
                    return "Model";
                case ObjectType.Column:
                    return string.Format("({0}.Columns[\"{1}\"] as {2})", (obj as ITabularTableObject).Table.GetLinqPath(), obj.Name, obj.GetType().Name);
                case ObjectType.Measure:
                    return string.Format("{0}.Measures[\"{1}\"]", (obj as ITabularTableObject).Table.GetLinqPath(), obj.Name);
                case ObjectType.Hierarchy:
                    return string.Format("{0}.Hierarchies[\"{1}\"]", (obj as ITabularTableObject).Table.GetLinqPath(), obj.Name);
                case ObjectType.Partition:
                    return string.Format("({0}.Partitions[\"{1}\"] as {2})", (obj as ITabularTableObject).Table.GetLinqPath(), obj.Name, obj.GetType().Name);
                case ObjectType.Table:
                    return string.Format("{0}.Tables[\"{1}\"]", obj.Model.GetLinqPath(), obj.Name);
                case ObjectType.Level:
                    return string.Format("{0}.Levels[\"{1}\"]", (obj as Level).Hierarchy.GetLinqPath(), obj.Name);
                case ObjectType.Perspective:
                    return string.Format("{0}.Perspectives[\"{1}\"]", obj.Model.GetLinqPath(), obj.Name);
                case ObjectType.Culture:
                    return string.Format("{0}.Cultures[\"{1}\"]", obj.Model.GetLinqPath(), obj.Name);
                case ObjectType.DataSource:
                    return string.Format("({0}.DataSources[\"{1}\"] as {2})", obj.Model.GetLinqPath(), obj.Name, obj.GetType().Name);
                case ObjectType.Relationship:
                    return string.Format("({0}.Relationships[{1}] as {2})", obj.Model.GetLinqPath(), obj.MetadataIndex, obj.GetType().Name);
                case ObjectType.Role:
                    return string.Format("{0}.Roles[\"{1}\"]", obj.Model.GetLinqPath(), obj.Name);
                case ObjectType.Expression:
                    return string.Format("{0}.Expressions[\"{1}\"]", obj.Model.GetLinqPath(), obj.Name);
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetObjectPath(this TOM.MetadataObject obj)
        {
            var name = (obj is TOM.Model) ? "Model" : (obj as TOM.NamedMetadataObject)?.Name ?? obj.ObjectType.ToString();

            if (obj.Parent != null)
                return obj.Parent.GetObjectPath() + "." + name;
            else
                return name;
        }

        public static string GetObjectPath(this TabularObject obj)
        {
            return GetObjectPath(obj.MetadataObject);
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string Pluralize(this string str)
        {
            if (str.EndsWith("y")) return str.Substring(0, str.Length - 1) + "ies";
            else if (!str.EndsWith("data")) return str + "s";
            else return str;
        }

        public static string GetTypeName(this ObjectType objType, bool plural = false)
        {
            if (objType == ObjectType.Culture) return "Translation" + (plural ? "s" : "");

            var result = SplitCamelCase(objType.ToString());
            return plural ? result.Pluralize() : result;
        }

        public static string GetTypeName(this ITabularObject obj, bool plural = false)
        {
            if (obj is CalculatedTable) return "Calculated Table" + (plural ? "s" : "");
            if (obj is DataColumn) return "Column" + (plural ? "s" : "");
            if (obj is CalculatedColumn) return "Calculated Column" + (plural ? "s" : "");
            if (obj is CalculatedTableColumn) return "Calculated Table Column" + (plural ? "s" : "");
            if (obj is StructuredDataSource) return "Data Source (Power Query)";
            if (obj is ProviderDataSource) return "Data Source (Legacy)";
            if (obj is MPartition) return "Partition (Power Query)";
            if (obj is Partition) return "Partition (Legacy)";
            else return obj.ObjectType.GetTypeName(plural);
        }

        public static string GetTypeName(this Type type, bool plural = false)
        {
            var n = type.Name.SplitCamelCase();
            return plural ? n.Pluralize() : n;
        }

        public static string GetName(this ITabularNamedObject obj, Culture culture)
        {
            // Translatable objects must take culture into account for their name:
            if (obj is ITranslatableObject && culture != null)
            {
                var name = (obj as ITranslatableObject).TranslatedNames[culture];

                // Return base name if there was no translated name:
                if (string.IsNullOrEmpty(name)) name = obj.Name;
                return name;
            }

            // Other objects simply use their name:
            return obj.Name;
        }

        public static bool SetName(this ITabularNamedObject obj, string newName, Culture culture)
        {
            if (obj is ITranslatableObject && culture != null)
            {
                var tObj = obj as ITranslatableObject;
                tObj.TranslatedNames[culture] = newName;
                return true;
            }

            if (string.IsNullOrEmpty(newName)) return false;
            obj.Name = newName;
            return true;
        }
    }

}

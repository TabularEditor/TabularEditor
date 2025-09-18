using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public static class TabularObjectHelper
    {
        public static TOM.MetadataObject GetMetadataObject(this ITabularObject obj)
        {
            return (obj as TabularObject)?.MetadataObject;
        }

        public static TOM.Model GetMetadataObject(this Model model)
        {
            return model.MetadataObject;
        }

        public static string GetName(this ITabularObject obj)
        {
            if ((obj is ITabularNamedObject)) return (obj as ITabularNamedObject)?.Name;

            if (obj is TablePermission tp) return tp.Role.Name;

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

        public static string LinqEscape(string name)
        {
            return name.Escape();
        }

        private static string Escape(this string name)
        {
            return name.Replace(@"\", @"\\").Replace("\"", "\\\"");
        }

        private static string GetLinqPath(this TabularObject obj)
        {
            switch (obj)
            {
                case KPI kpi:
                    return kpi.Measure.GetLinqPath() + ".KPI";
                case Model _:
                    return "Model";
                case Column column:
                    return string.Format("{0}.Columns[\"{1}\"]", column.Table.GetLinqPath(), column.Name.Escape());
                case Measure measure:
                    return string.Format("{0}.Measures[\"{1}\"]", measure.Table.GetLinqPath(), measure.Name.Escape());
                case Hierarchy hierarchy:
                    return string.Format("{0}.Hierarchies[\"{1}\"]", hierarchy.Table.GetLinqPath(), hierarchy.Name.Escape());
                case Partition partition:
                    return string.Format("{0}.Partitions[\"{1}\"]", partition.Table.GetLinqPath(), partition.Name);
                case Calendar calendar:
                    return string.Format("{0}.Calendars[\"{1}\"]", calendar.Table.GetLinqPath(), calendar.Name);
                case Table table:
                    return string.Format("Model.Tables[\"{0}\"]", table.Name.Escape());
                case Level level:
                    return string.Format("{0}.Levels[\"{1}\"]", level.Hierarchy.GetLinqPath(), level.Name.Escape());
                case Perspective perspective:
                    return string.Format("{0}.Perspectives[\"{1}\"]", obj.Model.GetLinqPath(), perspective.Name.Escape());
                case Function function:
                    return string.Format("{0}.Functions[\"{1}\"]", obj.Model.GetLinqPath(), function.Name.Escape());
                case Culture culture:
                    return string.Format("{0}.Cultures[\"{1}\"]", obj.Model.GetLinqPath(), culture.Name.Escape());
                case DataSource ds:
                    return string.Format("{0}.DataSources[\"{1}\"]", obj.Model.GetLinqPath(), ds.Name, obj.GetType().Name.Escape());
                case Relationship rel:
                    return string.Format("{0}.Relationships[{1}]", obj.Model.GetLinqPath(), rel.MetadataIndex, obj.GetType().Name.Escape());
                case ModelRole role:
                    return string.Format("{0}.Roles[\"{1}\"]", obj.Model.GetLinqPath(), role.Name.Escape());
                case NamedExpression expression:
                    return string.Format("{0}.Expressions[\"{1}\"]", obj.Model.GetLinqPath(), expression.Name.Escape());
                case TablePermission tp:
                    return string.Format("{0}.TablePermissions[\"{1}\"]", tp.Role.GetLinqPath(), tp.Table.Name.Escape());
                case CalculationItem ci:
                    return string.Format("{0}.CalculationItems[\"{1}\"]", ci.CalculationGroupTable.GetLinqPath(true), ci.Name.Escape());
                case Variation variation:
                    return string.Format("{0}.Variations[\"{1}\"]", variation.Column.GetLinqPath(), variation.Name.Escape());
                case CalculationGroup cg:
                    return string.Format("{0}.CalculationGroup", cg.Table.GetLinqPath(true));
                case AlternateOf altOf:
                    return string.Format("{0}.AlternateOf", altOf.Column.GetLinqPath());
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetLinqPath(this TabularObject obj, bool explicitCast = true)
        {
            if (explicitCast)
            {
                switch (obj)
                {
                    case Column _:
                    case Partition _:
                    case Table _:
                    case DataSource _:
                    case Relationship _:
                        return $"({GetLinqPath(obj)} as {obj.GetType().Name})";
                    default:
                        return GetLinqPath(obj);
                }
            }
            else
                return GetLinqPath(obj);
        }

        private static string GetObjectPathTableObject(string collectionName, TOM.NamedMetadataObject obj)
        {
            var name = QuotePath(obj.Name);

            if (obj.Parent != null)
                return obj.Parent.GetObjectPath() + "." + collectionName + "." + name;
            else
                return name;
        }

        public static string GetObjectPath(this TOM.MetadataObject obj)
        {
            switch (obj.ObjectType)
            {
                case TOM.ObjectType.Model:
                    return "Model";
                case TOM.ObjectType.Measure:
                    return GetObjectPathTableObject("M", obj as TOM.NamedMetadataObject);
                case TOM.ObjectType.Table:
                    return GetObjectPathTableObject("T", obj as TOM.NamedMetadataObject);
                case TOM.ObjectType.Column:
                    return GetObjectPathTableObject("C", obj as TOM.NamedMetadataObject);
                case TOM.ObjectType.Hierarchy:
                    return GetObjectPathTableObject("H", obj as TOM.NamedMetadataObject);
                case TOM.ObjectType.Level:
                    var level = obj as TOM.Level;
                    return GetObjectPathTableObject("H", level.Hierarchy) + "." + level.Name;
                case TOM.ObjectType.KPI:
                    return GetObjectPathTableObject("M", (obj as TOM.KPI).Measure) + ".KPI";
                case TOM.ObjectType.Variation:
                    return GetObjectPathTableObject("C", (obj as TOM.Variation).Column) + ".Variations." + QuotePath((obj as TOM.Variation).Name);
                case TOM.ObjectType.Relationship:
                case TOM.ObjectType.DataSource:
                case TOM.ObjectType.Role:
                case TOM.ObjectType.Expression:
                case TOM.ObjectType.Perspective:
                case TOM.ObjectType.Function:
                case TOM.ObjectType.Culture:
                case TOM.ObjectType.QueryGroup:
                    return obj.ObjectType.ToString() + "." + QuotePath((obj as TOM.NamedMetadataObject).Name);
                case TOM.ObjectType.Partition:
                    return GetObjectPathTableObject("P", (obj as TOM.Partition));
                case TOM.ObjectType.Calendar:
                    return GetObjectPathTableObject("CA", obj as TOM.Calendar);
                case TOM.ObjectType.CalendarColumnGroup:
                    if (obj is TOM.TimeUnitColumnAssociation assoc)
                    {
                        return GetObjectPath(assoc.Calendar) + "." + assoc.TimeUnit;
                    }
                    else
                    {
                        var ccg = obj as TOM.TimeRelatedColumnGroup;
                        return GetObjectPath(ccg.Calendar) + "." + ccg.Calendar.CalendarColumnGroups.IndexOf(ccg);
                    }
                case TOM.ObjectType.RoleMembership:
                    var mrm = obj as TOM.ModelRoleMember;
                    return GetObjectPath(mrm.Role) + "." + mrm.Name;
                case TOM.ObjectType.CalculationGroup:
                    var cg = obj as TOM.CalculationGroup;
                    return GetObjectPath(cg.Table) + ".CalculationGroup";
                case TOM.ObjectType.TablePermission:
                    var tp = obj as TOM.TablePermission;
                    return GetObjectPath(tp.Role) + "." + tp.Table.Name;
                case TOM.ObjectType.CalculationItem:
                    var ci = obj as TOM.CalculationItem;
                    return GetObjectPath(ci.CalculationGroup.Table) + ".CI." + QuotePath(ci.Name);
                case TOM.ObjectType.AlternateOf:
                    var ao = obj as TOM.AlternateOf;
                    return GetObjectPath(ao.Column) + ".AlternateOf";
                default:
                    throw new NotSupportedException($"Cannot create reference for object of type {obj.ObjectType}.");
            }

        }

        private static string QuotePath(string name)
        {
            return name.Contains(".") ? $"[{name}]" : name;
        }

        public static string GetObjectPath(this TabularObject obj)
        {
            return GetObjectPath(obj.MetadataObject);
        }

        public static TabularObject ResolveObject(Model model, string path)
        {
            var parts = path.Split('.');

            var partsFixed = new List<string>();

            // Objects that have "." in their name, will be enclosed by square brackets. So let's traverse the array
            // and concatenate any parts between a set of square brackets:
            string partFraction = null;
            foreach (var p in parts)
            {
                if (partFraction == null)
                {
                    if (p.StartsWith("["))
                    {
                        if (p.EndsWith("]"))
                        {
                            partFraction = p.Substring(1, p.Length - 2);
                            partsFixed.Add(partFraction.ToLower());
                            partFraction = null;
                        }
                        else
                            partFraction = p.Substring(1);
                    }
                    else
                        partsFixed.Add(p.ToLower());
                }
                else
                {
                    if (p.EndsWith("]"))
                    {
                        partFraction += "." + p.Substring(0, p.Length - 1);
                        partsFixed.Add(partFraction.ToLower());
                        partFraction = null;
                    }
                    else
                        partFraction += "." + p;
                }
            }
            parts = partsFixed.ToArray();

            if (model == null || parts.Length == 0) return null;
            if (parts.Length == 1 && parts[0] == "model") return model;
            switch (parts[0])
            {
                case "model":
                    if (parts[1] == "t")
                    {
                        var table = model.Tables.FindByName(parts[2]);
                        if (parts.Length == 3 || table == null) return table;
                        if (parts.Length == 4 && parts[3] == "calculationgroup" && table is CalculationGroupTable cgt2)
                            return cgt2.CalculationGroup;
                        if (parts.Length >= 5)
                        {
                            if (parts[3] == "p")
                            {
                                return table.Partitions.FindByName(parts[4]);
                            }
                            if (parts[3] == "m")
                            {
                                var measure = table.Measures.FindByName(parts[4]);
                                if (parts.Length == 6 && parts[5] == "kpi") return measure?.KPI;
                                else return measure;
                            }
                            if (parts[3] == "c")
                            {
                                var column = table.Columns.FindByName(parts[4]);
                                if (parts.Length == 7 && parts[5] == "variations") return column?.Variations?.FindByName(parts[6]);
                                else if (parts.Length == 6 && parts[5] == "alternateof") return column?.AlternateOf;
                                else return column;
                            }
                            if (parts[3] == "h")
                            {
                                var hierarchy = table.Hierarchies.FindByName(parts[4]);
                                if (parts.Length == 6) return hierarchy?.Levels.FindByName(parts[5]);
                                else return hierarchy;
                            }
                            if (parts[3].EqualsI("ca"))
                            {
                                var calendar = table.Calendars.FindByName(parts[4]);
                                if (parts.Length == 5) return calendar;
                                if (int.TryParse(parts[5], out var columnGroupIndex))
                                    return calendar.CalendarColumnGroups[columnGroupIndex];
                                else return calendar.CalendarColumnGroups[(TimeUnit)Enum.Parse(typeof(TimeUnit), parts[5])];
                            }
                            if (parts[3] == "ci" && table is CalculationGroupTable cgt && parts.Length == 5)
                            {
                                return cgt?.CalculationItems.FindByName(parts[4]);
                            }
                        }
                    }
                    else
                    {
                        // Fallback to the old path format: "Model.Dates.Date":
                        var table = model.Tables.FindByName(parts[1]);
                        if (parts.Length == 2 || table == null) return table;
                        if (table.Columns.FindByName(parts[2]) is Column c) return c;
                        if (table.Measures.FindByName(parts[2]) is Measure m) return m;
                        if (table.Hierarchies.FindByName(parts[2]) is Hierarchy h)
                        {
                            if (parts.Length == 3) return h;
                            return h.Levels.FindByName(parts[3]);
                        }
                    }
                    return null;

                case "relationship": return model.Relationships.FindByName(parts[1]);
                case "datasource": return model.DataSources.FindByName(parts[1]);
                case "role":
                    if (parts.Length == 3) // TablePermission:
                        return model.Roles.FindByName(parts[1])?.TablePermissions.FindByName(parts[2]);
                    else // Role:
                        return model.Roles.FindByName(parts[1]);
                case "expression": return model.Expressions.FindByName(parts[1]);
                case "perspective": return model.Perspectives.FindByName(parts[1]);
                case "functions": return model.Functions.FindByName(parts[1]);
                case "culture": return model.Cultures.FindByName(parts[1]);

                default:
                    return null;
            }
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

        public static string GetTypeName<T>(bool plural = false) where T : TabularObject
        {
            var result = SplitCamelCase(typeof(T).Name.ToString());
            if (result == "Culture") result = "Translation";
            return plural ? result.Pluralize() : result;
        }

        public static string GetTypeName(this ITabularObject obj, bool plural = false)
        {
            if (obj is DataColumn) return "Column" + (plural ? "s" : "");
            if (obj is StructuredDataSource) return "Data Source (Power Query)";
            if (obj is ProviderDataSource) return "Data Source (Legacy)";
            if (obj is PolicyRangePartition p4) return $"Partition (Policy Range)";
            if (obj is EntityPartition p3) return $"Partition (DQ over AS)";
            if (obj is MPartition p1) return $"Partition (M - {p1.GetMode()})";
            if (obj is Partition p2) return $"Partition (Legacy - {p2.GetMode()})";
            if (obj is CalculationGroupTable cgt) return $"Calculation Group Table" + (plural ? "s" : "");
            if (obj is CalculatedTable ct) return $"Calculated Table" + (plural ? "s" : "");
            if (obj is Table t) return $"Table ({t.GetMode()})";
            else return obj.ObjectType.GetTypeName(plural);
        }

        public static ModeType GetMode(this Partition partition)
        {
            return partition.Mode == ModeType.Default ? (partition.Model?.DefaultMode ?? ModeType.Import) : partition.Mode;
        }
        public static string GetMode(this Table table)
        {
            if (table.Partitions.FirstOrDefault() is EntityPartition) return "DQ over AS";
            var p1 = table.Partitions.FirstOrDefault()?.GetMode() ?? ModeType.Import;
            return table.Partitions.All(p => p.GetMode() == p1) ? p1.ToString() : "Hybrid";
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

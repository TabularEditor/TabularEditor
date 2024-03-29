﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Scripting
{
    public static class PropertySerializer
    {
        private static readonly HashSet<ObjectType> SerializableObjectTypes = new HashSet<ObjectType>
        {
            ObjectType.Table,
            ObjectType.Partition,
            ObjectType.DataSource,
            ObjectType.Expression,
            ObjectType.Column,
            ObjectType.Role,
            ObjectType.Model,
            ObjectType.Hierarchy,
            ObjectType.Level,
            ObjectType.Measure,
            ObjectType.KPI,
            ObjectType.Relationship,
            ObjectType.Perspective,
            ObjectType.CalculationGroup,
            ObjectType.CalculationGroupTable,
            ObjectType.CalculationItem
        };

        private class Property
        {
            public Property(string name, string key, bool isIndexer)
            {
                Name = name;
                Key = key;
                IsIndexer = isIndexer;
            }

            public string Name { get; }
            public string Key { get; }
            public bool IsIndexer { get; }
            public Type IndexerValueType { get; set; }
            public override string ToString()
            {
                if (IsIndexer)
                    return $"{ Name }[{ Key }]";

                return Name;
            }
        }

        private static Property[] ParseProperties(string header)
        {
            var regex = new Regex("(.*)\\[([^()]*)\\]$", RegexOptions.IgnoreCase);

            var properties = header.Split(',').Where((p) => !string.IsNullOrWhiteSpace(p))
                .Select((p) =>
                {
                    var match = regex.Match(p.Trim());

                    return new Property
                    (
                        name: match.Success ? match.Groups[1].Value : p.Trim(),
                        key: match.Success ? match.Groups[2].Value : null,
                        isIndexer: match.Success
                    );
                });

            return properties.ToArray();
        }

        private static Property[] ExpandProperties(Property[] properties, IEnumerable<TabularObject> objects)
        {
            var expandedProperties = new List<Property>();

            foreach (var property in properties)
            {
                if (property.IsIndexer)
                {
                    expandedProperties.Add(property);
                }
                else
                {
                    var expandedKeys = new List<string>();
                    var isIndexer = false;

                    foreach (var tabularObject in objects)
                    {
                        var value = tabularObject.GetType().GetProperty(property.Name)?.GetValue(tabularObject);
                        if (value == null) continue; // Ignore non-existing properties, as they could exist on other objects in the collection

                        if (value is ITabularObjectCollection)
                            throw new Exception($"ExportProperties error: Cannot export property {property.Name} on {tabularObject.ObjectType.GetTypeName()} \"{tabularObject.GetName()}\" since it is a collection.");

                        if (value is IExpandableIndexer indexerProperty)
                        {
                            isIndexer = true;
                            expandedKeys.AddRange(indexerProperty.Keys);
                        }
                        else if (value != null && value.GetType().IsClass && value.GetType() != typeof(string) && !(value is TabularObject))
                        {
                            throw new Exception($"ExportProperties error: Cannot export property {property.Name} on {tabularObject.ObjectType.GetTypeName()} \"{tabularObject.GetName()}\" since it is a complex object.");
                        }
                    }

                    if (isIndexer)
                        expandedProperties.AddRange(expandedKeys.Distinct().Select((k) => new Property(property.Name, key: k, isIndexer: true)));
                    else
                        expandedProperties.Add(property);
                }
            }

            return expandedProperties.ToArray();
        }

        private static string GetTsvForObject(TabularObject obj, Property[] properties)
        {
            var sb = new StringBuilder();
            sb.Append(obj.GetObjectPath());
            foreach (var prop in properties)
            {
                sb.Append('\t');
                var pInfo = obj.GetType().GetProperty(prop.Name);
                if (pInfo != null)
                {
                    var pValue = pInfo.GetValue(obj);
                    if (pValue == null)
                        continue;
                    else if (pValue is TabularObject)
                        // Improve GetObjectPath to always provide unique path, and create corresponding method to resolve a path
                        sb.Append((pValue as TabularObject).GetObjectPath());
                    else if (prop.IsIndexer && pValue is IExpandableIndexer indexer)
                    {
                        sb.Append(indexer.Keys.Contains(prop.Key) ? ToString(indexer[prop.Key]) : string.Empty);
                    }
                    else
                        sb.Append(ToString(pValue));
                }
            }
            return sb.ToString();

            string ToString(object value)
            {
                return Convert.ToString(value).Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
            }
        }

        [ScriptMethod]
        // TODO: Provide more formatting options
        public static string ExportProperties(this IEnumerable<ITabularNamedObject> objects, string properties = "Name,Description,SourceColumn,Expression,FormatString,DataType")
        {
            // Only certain types of objects can have their properties exported
            var serializableObjects = objects.OfType<TabularObject>().Where((o) => SerializableObjectTypes.Contains(o.ObjectType)).ToArray();

            var parsedProperties = ParseProperties(properties);
            var expandedProperties = ExpandProperties(parsedProperties, serializableObjects);

            var sb = new StringBuilder();
            sb.Append("Object\t");
            sb.Append(string.Join("\t", expandedProperties.Select(Convert.ToString)));

            foreach (var obj in serializableObjects)
            {
                sb.Append("\n");
                sb.Append(GetTsvForObject(obj, expandedProperties));
            }

            return sb.ToString();
        }

        [ScriptMethod]
        public static void ImportProperties(string tsvData)
        {
            var rows = tsvData.Split('\n');
            var header = string.Join(",", rows[0].Replace("\r", "").Split('\t').Skip(1).ToArray());
            var properties = ParseProperties(header);
            foreach (var row in rows.Skip(1))
            {
                if (!string.IsNullOrWhiteSpace(row))
                    AssignTsvToObject(row, properties);
            }
        }

        private static void AssignTsvToObject(string propertyValues, Property[] properties)
        {
            var values = propertyValues.Replace("\r", "").Split('\t').Select(v => v.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t")).ToArray();
            var obj = ResolveObjectPath(values[0]);
            if (obj == null) return;


            for (int i = 0; i < properties.Length; i++)
            {
                var pInfo = obj.GetType().GetProperty(properties[i].Name);
                if (pInfo == null)
                    continue;

                var pValue = values[i + 1]; // This is shifted by 1 since the first column is the Object path
                try
                {
                    if (properties[i].IsIndexer)
                    {
                        if (typeof(IExpandableIndexer).IsAssignableFrom(pInfo.PropertyType))
                        {
                            var indexer = (IExpandableIndexer)pInfo.GetValue(obj);
                            var indexerValueType = GetIndexerValueType(indexer);
                            if (indexer.Keys.Contains(properties[i].Key))
                            {
                                if (string.Empty.Equals(pValue) && indexerValueType.IsValueType) continue; // Can't set value types to null

                                if (indexerValueType.IsEnum)
                                {
                                    indexer[properties[i].Key] = Enum.Parse(indexerValueType, pValue);
                                }
                                else
                                {
                                    indexer[properties[i].Key] = string.Empty.Equals(pValue) ? null : Convert.ChangeType(pValue, indexerValueType);
                                }
                            }
                        }

                        continue;
                    }

                    // Consider only properties that exist, and have a public setter:
                    if (!pInfo.CanWrite || !pInfo.GetSetMethod(true).IsPublic) continue;

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
                    else
                    {
                        // Value is converted directly from string to the type of the property:
                        pInfo.SetValue(obj, Convert.ChangeType(pValue, pInfo.PropertyType));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception($"ImportProperties error: Unable to assign value \"{pValue}\" to property '{properties[i].ToString()}' on {obj.ObjectType.GetTypeName()} \"{obj.GetName()}\": {ex.Message}");
                }
            }
        }

        [ScriptMethod]
        public static string GetObjectPath(this TabularObject obj)
        {
            return TabularObjectHelper.GetObjectPath(obj);
        }

        [ScriptMethod]
        public static TabularObject ResolveObjectPath(string path)
        {
            var model = TabularModelHandler.Singleton.Model;
            return TabularObjectHelper.ResolveObject(model, path);
        }

        static Type GetIndexerValueType(IExpandableIndexer indexer)
        {
            // Most indexers use strings, but a few derive from GenericIndexer, in which case the
            // 2nd generic type argument dictates the value type of the indexer:

            var indexerType = indexer.GetType();
            while(indexerType != null && !indexerType.IsGenericType)
            {
                indexerType = indexerType.BaseType;
            }
            if (indexerType != null && indexerType.Name == "GenericIndexer`2")
                return indexerType.GenericTypeArguments[1];

            return typeof(string);
        }
    }
}

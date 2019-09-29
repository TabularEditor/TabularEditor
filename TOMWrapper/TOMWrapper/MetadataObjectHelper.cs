﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor;

namespace Microsoft.AnalysisServices.Tabular.Helper
{

    /// <summary>
    /// Provides various extension methods to make it easier to work with the Tabular Object Model
    /// </summary>
    internal static class MetadataObjectHelper
    {
        private static bool IsTranslatable(this NamedMetadataObject obj)
        {
            switch (obj.ObjectType)
            {
                case ObjectType.Table:
                case ObjectType.Measure:
                case ObjectType.Hierarchy:
                case ObjectType.Level:
                case ObjectType.Column:
                case ObjectType.Model:
                case ObjectType.Perspective:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetName(this NamedMetadataObject obj, Culture culture = null)
        {
            if (obj == null) return null;
            string name;
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[obj, TranslatedProperty.Caption];
                if (tran != null) name = tran.Value;
                else name = obj.Name;
            }
            else
                name = obj.Name;
            return name;
        }

        public static void SetName(this NamedMetadataObject obj, string newName, Culture culture = null)
        {
            if (culture != null)
            {
                culture.ObjectTranslations.SetTranslation(obj, TranslatedProperty.Caption, newName);
            }
            else
            {
                if (obj.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
                {
                    obj.Name = newName;
                    return;
                }

                if (obj is Measure)
                {
                    var measure = obj as Measure;

                    // Do not allow multiple measures with the same name:
                    if (measure.Model.Tables.Any(t => t.Measures.Any(m => m != obj && m.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase))))
                        throw new ArgumentException(string.Format(Messages.DuplicateMeasureName, newName));

                    // Do not allow a measure with the same name as a column in the table:
                    if (measure.Table.Columns.Any(c => c.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format(Messages.DuplicateColumnName, newName));
                }
                else if (obj is Column)
                {
                    var column = obj as Column;

                    // Do not allow a column with the same name as a measure in the table:
                    if (column.Table.Measures.Any(m => m.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format(Messages.DuplicateMeasureName, newName));

                    // Do not allow a column with the same name as another column in the table:
                    if (column.Table.Columns.Any(c => c != obj && c.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format(Messages.DuplicateColumnName, newName));
                }

                // Automatically apply name change to translations (but only if the translation is identical to the old name):
                var cultures = obj.Model?.Cultures;

                if (cultures != null && obj.IsTranslatable())
                {
                    foreach (var c in cultures)
                    {
                        var translatedName = obj.GetName(c);
                        if (!string.IsNullOrEmpty(translatedName) && translatedName.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            obj.SetName(newName, c);
                        }
                    }
                }

                if (!obj.IsRemoved) obj.Name = newName;
            }
        }    

        public static List<Column> GetDuplictedKeyColumns(this TableCollection tables)
        {
            var duplicatedKeyColumns = new List<Column>();

            foreach (var table in tables)
            {
                var keyColumns = table.Columns.Where(c => c.IsKey).ToList();
                if (keyColumns.Count > 1)
                    duplicatedKeyColumns.AddRange(keyColumns.Where(c => c.Type != ColumnType.RowNumber));
            }

            return duplicatedKeyColumns;
        }
    }
}

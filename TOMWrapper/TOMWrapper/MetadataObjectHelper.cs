using System;
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
        public static bool IsNullOrEmpty(this FormatStringDefinition value)
        {
            return value == null || string.IsNullOrEmpty(value.Expression);
        }
        public static bool IsNullOrEmpty(this CalculationGroupExpression value)
        {
            return value == null
                || (string.IsNullOrEmpty(value.Expression) && string.IsNullOrEmpty(value.Description) && value.FormatStringDefinition.IsNullOrEmpty());
        }

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

        public static string GetNameTranslation(this NamedMetadataObject obj, Culture culture = null)
        {
            if (obj == null) return null;
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[obj, TranslatedProperty.Caption];
                if (tran != null) return tran.Value;
            }
            return null;
        }

        /// <summary>
        /// Throws an exception of the provided new name is invalid for the specified object.
        /// </summary>
        public static void ValidateName(this NamedMetadataObject obj, string newName)
        {
            if (obj.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)) return;

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
            else if (obj is Calendar calendar)
            {
                // Do not allow calendar names with the same name as any other calendar or table in the model:
                if (calendar.Model.Tables.Any(t => t.Calendars.Find(newName) is { } c && c != calendar))
                    throw new ArgumentException(string.Format(Messages.DuplicateCalendarName, newName));

                if (calendar.Model.Tables.Find(newName) != null)
                    throw new ArgumentException(string.Format(Messages.DuplicateCalendarTableName, newName));

            }
        }

        public static void SetTranslation(this Culture culture, NamedMetadataObject obj, TranslatedProperty property, string translatedValue)
        {
            culture.ObjectTranslations.SetTranslation(obj, property, translatedValue);
            if (obj.Model.Database.CompatibilityLevel >= 1571 && !string.IsNullOrEmpty(translatedValue))
                culture.ObjectTranslations[obj, property].Altered = true;
        }

        public static void SetName(this NamedMetadataObject obj, string newName, Culture culture = null)
        {
            if (culture != null)
            {
                culture.SetTranslation(obj, TranslatedProperty.Caption, newName);
            }
            else
            {
                if (obj.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
                {
                    obj.Name = newName;
                    return;
                }

                // Automatically apply name change to translations (but only if the translation is identical to the old name):
                var cultures = obj.Model?.Cultures;

                if (cultures != null && obj.IsTranslatable())
                {
                    foreach (var c in cultures)
                    {
                        var translatedName = obj.GetNameTranslation(c);
                        if (!string.IsNullOrEmpty(translatedName) && translatedName.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            obj.SetName(newName, c);
                        }
                    }
                }

                if (!obj.IsRemoved) obj.Name = newName;
            }
        }    
    }
}

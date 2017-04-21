using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AnalysisServices.Tabular.Helper
{

    /// <summary>
    /// Provides various extension methods to make it easier to work with the Tabular Object Model
    /// </summary>
    public static class MetadataObjectHelper
    {
        #region Localized Name and Description handling
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

        public static void RemoveAllTranslations(this NamedMetadataObject obj)
        {
            foreach (var c in obj.Model.Cultures)
            {
                var tran = c.ObjectTranslations[obj, TranslatedProperty.Caption];
                if (tran != null) c.ObjectTranslations.Remove(tran);

                tran = c.ObjectTranslations[obj, TranslatedProperty.Description];
                if (tran != null) c.ObjectTranslations.Remove(tran);

                tran = c.ObjectTranslations[obj, TranslatedProperty.DisplayFolder];
                if (tran != null) c.ObjectTranslations.Remove(tran);
            }
        }

        public static void SetName(this NamedMetadataObject obj, string newName, Culture culture = null)
        {
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[obj, TranslatedProperty.Caption];
                if (tran == null)
                {
                    tran = new ObjectTranslation
                    {
                        Object = obj,
                        Property = TranslatedProperty.Caption
                    };
                    culture.ObjectTranslations.Add(tran);
                }
                tran.Value = newName;
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
                        throw new ArgumentException(string.Format("A measure with the name \"{0}\" already exists in the model. Choose a different name for this measure.", newName));

                    // Do not allow a measure with the same name as a column in the table:
                    if (measure.Table.Columns.Any(c => c.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format("A column with the name \"{0}\" already exists in the table. Choose a different name for this measure.", newName));
                }
                else if (obj is Column)
                {
                    var column = obj as Column;

                    // Do not allow a column with the same name as a measure in the table:
                    if (column.Table.Measures.Any(m => m.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format("A measure with the name \"{0}\" already exists in the table. Choose a different name for this column.", newName));

                    // Do not allow a column with the same name as another column in the table:
                    if (column.Table.Columns.Any(c => c != obj && c.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new ArgumentException(string.Format("A column with the name \"{0}\" already exists in the table. Choose a different name for this column.", newName));
                }

                // Automatically apply name change to translations (but only if the translation is identical to the old name):
                var cultures = obj.Model?.Cultures;

                // TODO: Find a better way to avoid translations on objects that can't be translated
                if (cultures != null && !(obj is ModelRole || obj is Partition || obj is Relationship))
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

        public static string GetDescription(this NamedMetadataObject obj, Culture culture = null)
        {
            if (obj == null) return null;
            string desc = null;
            desc = culture?.ObjectTranslations[obj, TranslatedProperty.Description]?.Value;

            if (desc == null)
            {
                if (obj is Table) return (obj as Table).Description;
                else if (obj is Measure) return (obj as Measure).Description;
                else if (obj is Column) return (obj as Column).Description;
                else if (obj is Hierarchy) return (obj as Hierarchy).Description;
                else if (obj is Level) return (obj as Level).Description;
                else if (obj is Model) return (obj as Model).Description;
            }
            return desc;
        }
        public static void SetDescription(this NamedMetadataObject obj, string description, Culture culture = null)
        {
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[obj, TranslatedProperty.Description];
                if (tran == null)
                {
                    tran = new ObjectTranslation
                    {
                        Object = obj,
                        Property = TranslatedProperty.Description
                    };
                    culture.ObjectTranslations.Add(tran);
                }
                tran.Value = description;
            }
            else
            {
                if (obj is Table) (obj as Table).Description = description;
                else if (obj is Measure) (obj as Measure).Description = description;
                else if (obj is Column) (obj as Column).Description = description;
                else if (obj is Hierarchy) (obj as Hierarchy).Description = description;
                else if (obj is Level) (obj as Level).Description = description;
                else if (obj is Model) (obj as Model).Description = description;
            }
        }
        #endregion

        #region Localized Display Folder handling
        #region Measures
        /// <summary>
        /// Gets the (translated) display folder path of a Measure. If no culture is specified,
        /// gets the default display folder. An empty folder "\" is returned as a blank string "\".
        /// </summary>
        public static string GetDisplayFolder(this Measure measure, Culture culture = null)
        {
            if (culture != null)
            {
                return culture.ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value ?? string.Empty;
            }
            else
                return measure.DisplayFolder;
        }

        public static void SetDisplayFolder(this Measure measure, string displayFolder, Culture culture = null)
        {
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[measure, TranslatedProperty.DisplayFolder];
                if (tran == null)
                {
                    tran = new ObjectTranslation
                    {
                        Object = measure,
                        Property = TranslatedProperty.DisplayFolder
                    };
                    culture.ObjectTranslations.Add(tran);
                }
                tran.Value = displayFolder;
            }
            else
                measure.DisplayFolder = displayFolder;
        }
        #endregion

        #region Columns
        /// <summary>
        /// Gets the (translated) display folder path of a Column. If no culture is specified,
        /// gets the default display folder. An empty folder "\" is returned as a blank string "\".
        /// </summary>
        public static string GetDisplayFolder(this Column column, Culture culture = null)
        {
            if (culture != null)
            {
                return culture.ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value ?? string.Empty;
            }
            else
                return column.DisplayFolder;
        }
        public static void SetDisplayFolder(this Column column, string displayFolder, Culture culture = null)
        {
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[column, TranslatedProperty.DisplayFolder];
                if (tran == null)
                {
                    tran = new ObjectTranslation
                    {
                        Object = column,
                        Property = TranslatedProperty.DisplayFolder
                    };
                    culture.ObjectTranslations.Add(tran);
                }
                tran.Value = displayFolder;
            }
            else
                column.DisplayFolder = displayFolder;
        }
        #endregion

        #region Hierarchies
        /// <summary>
        /// Gets the (translated) display folder path of a Hierarchy. If no culture is specified,
        /// gets the default display folder. An empty folder "\" is returned as a blank string "\".
        /// </summary>
        public static string GetDisplayFolder(this Hierarchy hierarchy, Culture culture = null)
        {
            if (culture != null)
            {
                return culture.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value ?? string.Empty;
            }
            else
                return hierarchy.DisplayFolder;
        }
        public static void SetDisplayFolder(this Hierarchy hierarchy, string displayFolder, Culture culture = null)
        {
            if (culture != null)
            {
                var tran = culture.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder];
                if (tran == null)
                {
                    tran = new ObjectTranslation
                    {
                        Object = hierarchy,
                        Property = TranslatedProperty.DisplayFolder
                    };
                    culture.ObjectTranslations.Add(tran);
                }
                tran.Value = displayFolder;
            }
            else
                hierarchy.DisplayFolder = displayFolder;
        }
        #endregion
        #endregion

        #region Perspective handling
        #region Table perspective handling
        public static bool InPerspective(this Table table, string perspectiveName)
        {
            return table.InPerspective(table.Model.Perspectives[perspectiveName]);
        }

        public static bool InPerspective(this Table table, Perspective perspective)
        {
            if (perspective == null) return false;
            return perspective.PerspectiveTables.ContainsName(table.Name);
        }

        public static void SetPerspective(this Table table, string perspectiveName, bool inPerspective, bool includeChildren = false)
        {
            table.SetPerspective(table.Model.Perspectives[perspectiveName], inPerspective, includeChildren);
        }
        public static void SetPerspective(this Table table, Perspective perspective, bool inPerspective, bool includeChildren = false)
        {
            var pt = perspective?.PerspectiveTables;
            if (pt == null) return;
            if (inPerspective)
            {
                if (!pt.Contains(table.Name)) pt.Add(new PerspectiveTable { Table = table });
                if (includeChildren)
                {

                }
            }
            else
            {
                if (pt.Contains(table.Name)) pt.Remove(table.Name);
            }
        }
        #endregion

        #endregion
    }
}

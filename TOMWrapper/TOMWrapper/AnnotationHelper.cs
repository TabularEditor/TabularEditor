extern alias json;

using json.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    internal static class AnnotationHelper
    {
        public static void ClearTabularEditorAnnotations(this IAnnotationObject obj, bool includeChildren = false)
        {
            obj.RemoveAnnotation("TabularEditor_TranslatedNames", false);
            obj.RemoveAnnotation("TabularEditor_TranslatedDescriptions", false);
            obj.RemoveAnnotation("TabularEditor_TranslatedDisplayFolders", false);
            obj.RemoveAnnotation("TabularEditor_InPerspective", false);
            obj.RemoveAnnotation("TabularEditor_RLS", false);
            obj.RemoveAnnotation("TabularEditor_OLS", false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IAnnotationObject>()) child.ClearTabularEditorAnnotations(true);
            }
        }

        /// <summary>
        /// Stores all translations on the current instance of an ITranslatableObject as annotations on the object.
        /// Translations can later be retrieved using the LoadTranslations() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveTranslations(this ITranslatableObject obj, bool includeChildren = false)
        {
            if (!obj.TranslatedNames.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedNames", obj.TranslatedNames.ToJson(), false);
            if (!obj.TranslatedDescriptions.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedDescriptions", obj.TranslatedDescriptions.ToJson(), false);
            if (obj is IDetailObject && !(obj as IDetailObject).TranslatedDisplayFolders.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedDisplayFolders", (obj as IDetailObject).TranslatedDisplayFolders.ToJson(), false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<ITranslatableObject>()) child.SaveTranslations(true);
            }
        }

        /// <summary>
        /// Reads any translations stored in the annotations of the current instance of an ITranslatableObject,
        /// and applies them to the model culture.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadTranslations(this ITranslatableObject obj, bool includeChildren = false)
        {
            var tn = obj.GetAnnotation("TabularEditor_TranslatedNames");
            if (tn != null) obj.TranslatedNames.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(tn));

            var td = obj.GetAnnotation("TabularEditor_TranslatedDescriptions");
            if (td != null) obj.TranslatedDescriptions.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(td));

            var tdf = obj.GetAnnotation("TabularEditor_TranslatedDisplayFolders");
            if (tdf != null && obj is IDetailObject) (obj as IDetailObject).TranslatedDisplayFolders.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(tdf));

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<ITranslatableObject>()) child.LoadTranslations(true);
            }
        }

        /// <summary>
        /// Stores all perspective membership information on the current instance of an ITabularPerspectiveObject
        /// as annotations on the object. Perspective membership can later be retrieved using the LoadPerspectives() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SavePerspectives(this ITabularPerspectiveObject obj, bool includeChildren = false)
        {
            if(obj.InPerspective.Any(ip => ip))
                obj.SetAnnotation("TabularEditor_InPerspective", obj.InPerspective.ToJson(), false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<ITabularPerspectiveObject>()) child.SavePerspectives(true);
            }
        }

        /// <summary>
        /// Reads any perspective membership information stored in the annotations of the current instance of an ITabularPerspectiveObject
        /// and applies them to the model perspectives.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadPerspectives(this ITabularPerspectiveObject obj, bool includeChildren = false)
        {
            var p = obj.GetAnnotation("TabularEditor_InPerspective");
            if (p != null) obj.InPerspective.CopyFrom(JsonConvert.DeserializeObject<string[]>(p));

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<ITabularPerspectiveObject>()) child.LoadPerspectives(true);
            }
        }

        /// <summary>
        /// Stores all RLS information on the current table as annotations.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveRLS(this Table obj)
        {
            if (obj.RowLevelSecurity.Any(rls => !string.IsNullOrEmpty(rls)))
                obj.SetAnnotation("TabularEditor_RLS", obj.RowLevelSecurity.ToJson(), false);
        }
        public static void LoadRLS(this Table obj)
        {
            var p = obj.GetAnnotation("TabularEditor_RLS");
            if (p != null) obj.RowLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(p));
        }

#if CL1400
        /// <summary>
        /// Stores all OLS information on the current table as annotations.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveOLS(this Table obj, bool includeChildren = false)
        {
            if (obj.ObjectLevelSecurity.Any(ols => ols != Microsoft.AnalysisServices.Tabular.MetadataPermission.Default))
                obj.SetAnnotation("TabularEditor_OLS", obj.ObjectLevelSecurity.ToJson(), false);

            if (includeChildren)
            {
                foreach (var child in obj.Columns) child.SaveOLS();
            }
        }

        public static void LoadOLS(this Table obj, bool includeChildren = false)
        {
            var p = obj.GetAnnotation("TabularEditor_OLS");
            if (p != null) obj.ObjectLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, Microsoft.AnalysisServices.Tabular.MetadataPermission>>(p));

            if (includeChildren)
            {
                foreach (var child in obj.Columns) child.LoadOLS();
            }
        }

        /// <summary>
        /// Stores all OLS information on the current column as annotations.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveOLS(this Column obj)
        {
            if (obj.ObjectLevelSecurity.Any(ols => ols != Microsoft.AnalysisServices.Tabular.MetadataPermission.Default))
                obj.SetAnnotation("TabularEditor_OLS", obj.ObjectLevelSecurity.ToJson(), false);
        }
        public static void LoadOLS(this Column obj)
        {
            var p = obj.GetAnnotation("TabularEditor_OLS");
            if (p != null) obj.ObjectLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, Microsoft.AnalysisServices.Tabular.MetadataPermission>>(p));
        }
#endif
    }
}

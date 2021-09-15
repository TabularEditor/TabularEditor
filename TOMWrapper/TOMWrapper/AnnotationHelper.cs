using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    internal static class AnnotationHelper
    {
        public const string ANN_NAMES = "TabularEditor_TranslatedNames";
        public const string ANN_DESCRIPTIONS = "TabularEditor_TranslatedDescriptions";
        public const string ANN_DISPLAYFOLDERS = "TabularEditor_TranslatedDisplayFolders";
        public const string ANN_INPERSPECTIVE = "TabularEditor_InPerspective";
        public const string ANN_RLS = "TabularEditor_RLS";
        public const string ANN_OLS = "TabularEditor_OLS";

        public const string ANN_RELATIONSHIPS = "TabularEditor_Relationships";
        public const string ANN_CULTURES = "TabularEditor_Cultures";
        public const string ANN_PERSPECTIVES = "TabularEditor_Perspectives";

        public static bool CheckFlag(this IAnnotationObject obj, string annotationName)
        {
            if (!obj.HasAnnotation(annotationName))
                return false;

            var value = obj.GetAnnotation(annotationName);

            if (value == "0" || value.EqualsI("false") || value.EqualsI("no"))
                return false;

            return true;
        }

        /// <summary>
        /// Removes all annotations from the model, that are used by Tabular Editor to serialize metadata in a way different from the TOM. For example,
        /// to store object translations as annotations on the object instead of elsewhere in the TOM tree. All these annotations can be recreated from
        /// the TOM.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeChildren"></param>
        public static void ClearTabularEditorAnnotations(this IInternalAnnotationObject obj, bool includeChildren = false)
        {
            obj.RemoveAnnotation(ANN_NAMES, false);
            obj.RemoveAnnotation(ANN_DESCRIPTIONS, false);
            obj.RemoveAnnotation(ANN_DISPLAYFOLDERS, false);
            obj.RemoveAnnotation(ANN_INPERSPECTIVE, false);
            obj.RemoveAnnotation(ANN_RLS, false);
            obj.RemoveAnnotation(ANN_OLS, false);

            obj.RemoveAnnotation(ANN_RELATIONSHIPS, false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IInternalAnnotationObject>()) child.ClearTabularEditorAnnotations(true);
            }
        }

        public static void ClearTabularEditorAnnotations(this Model model)
        {
            model.RemoveAnnotation(ANN_PERSPECTIVES, false);
            model.RemoveAnnotation(ANN_CULTURES, false);
            foreach (var table in model.Tables) table.ClearTabularEditorAnnotations(true);
        }

        /// <summary>
        /// Stores all perspective membership information on the current instance of an ITabularPerspectiveObject
        /// as annotations on the object. Perspective membership can later be retrieved using the LoadPerspectives() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SavePerspectives(this IInternalTabularPerspectiveObject obj, bool includeChildren = false)
        {
            if(obj.InPerspective.Any(ip => ip))
                obj.SetAnnotation(ANN_INPERSPECTIVE, obj.InPerspective.ToJson(), false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IInternalTabularPerspectiveObject>()) child.SavePerspectives(true);
            }
        }

        /// <summary>
        /// Reads any perspective membership information stored in the annotations of the current instance of an ITabularPerspectiveObject
        /// and applies them to the model perspectives.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadPerspectives(this IInternalTabularPerspectiveObject obj, bool includeChildren = false)
        {
            var p = obj.GetAnnotation(ANN_INPERSPECTIVE);
            if (p != null)
            {
                obj.InPerspective.CopyFrom(JsonConvert.DeserializeObject<string[]>(p));
                obj.RemoveAnnotation(ANN_INPERSPECTIVE, true);
            }

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IInternalTabularPerspectiveObject>()) child.LoadPerspectives(true);
            }
        }

        /// <summary>
        /// Stores all RLS information on the current table as annotations.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveRLS(this Table obj)
        {
            if (obj.RowLevelSecurity.Any(rls => !string.IsNullOrEmpty(rls)))
                obj.SetAnnotation(ANN_RLS, obj.RowLevelSecurity.ToJson(), false);
        }
        public static void LoadRLS(this Table obj)
        {
            var p = obj.GetAnnotation(ANN_RLS);
            if (p != null)
            {
                obj.RowLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(p));
                obj.RemoveAnnotation(ANN_RLS);
            }
        }

        /// <summary>
        /// Stores all OLS information on the current table as annotations.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveOLS(this Table obj, bool includeChildren = false)
        {
            if (obj.ObjectLevelSecurity.Any(ols => ols != MetadataPermission.Default))
                obj.SetAnnotation(ANN_OLS, obj.ObjectLevelSecurity.ToJson(), false);

            if (includeChildren)
            {
                foreach (var child in obj.Columns) child.SaveOLS();
            }
        }

        public static void LoadOLS(this Table obj, bool includeChildren = false)
        {
            var p = obj.GetAnnotation(ANN_OLS);
            if (p != null)
            {
                obj.ObjectLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, MetadataPermission>>(p));
                obj.RemoveAnnotation(ANN_OLS);
            }

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
            if (obj.ObjectLevelSecurity.Any(ols => ols != MetadataPermission.Default))
                obj.SetAnnotation(ANN_OLS, obj.ObjectLevelSecurity.ToJson(), false);
        }
        public static void LoadOLS(this Column obj)
        {
            var p = obj.GetAnnotation(ANN_OLS);
            if (p != null)
            {
                obj.ObjectLevelSecurity.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, MetadataPermission>>(p));
                obj.RemoveAnnotation(ANN_OLS);
            }
        }
    }
}

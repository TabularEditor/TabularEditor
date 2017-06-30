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
        /// <summary>
        /// Stores all translations on the current instance of an ITranslatableObject as annotations on the object.
        /// Translations can later be retrieved using the LoadTranslations() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveTranslations(this ITranslatableObject obj)
        {
            if (!obj.TranslatedNames.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedNames", obj.TranslatedNames.ToJson(), false);
            if (!obj.TranslatedDescriptions.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedDescriptions", obj.TranslatedDescriptions.ToJson(), false);
            if (obj is IDetailObject)
                obj.SetAnnotation("TabularEditor_TranslatedDisplayFolders", (obj as IDetailObject).TranslatedDisplayFolders.ToJson(), false);
        }

        /// <summary>
        /// Reads any translations stored in the annotations of the current instance of an ITranslatableObject,
        /// and applies them to the model culture.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadTranslations(this ITranslatableObject obj)
        {
            var tn = obj.GetAnnotation("TabularEditor_TranslatedNames");
            if (tn != null) obj.TranslatedNames.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(tn));

            var td = obj.GetAnnotation("TabularEditor_TranslatedDescriptions");
            if (td != null) obj.TranslatedDescriptions.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(td));

            var tdf = obj.GetAnnotation("TabularEditor_TranslatedDisplayFolders");
            if (tdf != null && obj is IDetailObject) (obj as IDetailObject).TranslatedDisplayFolders.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(tdf));
        }

        /// <summary>
        /// Stores all perspective membership information on the current instance of an ITabularPerspectiveObject
        /// as annotations on the object. Perspective membership can later be retrieved using the LoadPerspectives() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SavePerspectives(this ITabularPerspectiveObject obj)
        {
            obj.SetAnnotation("TabularEditor_InPerspective", obj.InPerspective.ToJson(), false);
        }

        /// <summary>
        /// Reads any perspective membership information stored in the annotations of the current instance of an ITabularPerspectiveObject
        /// and applies them to the model perspectives.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadPerspectives(this ITabularPerspectiveObject obj)
        {
            var p = obj.GetAnnotation("TabularEditor_InPerspective");
            if (p != null) obj.InPerspective.CopyFrom(JsonConvert.DeserializeObject<string[]>(p));
        }
    }
}

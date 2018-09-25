extern alias json;

using json::Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class TranslationAnnotationSerializer
    {
        internal static IEnumerable<ITranslatableObject> GetAllTranslatableObjects(this Model model)
        {
                return Enumerable.Repeat(model as ITranslatableObject, 1)
                    .Concat(model.Tables)
                    .Concat(model.AllMeasures)
                    .Concat(model.AllColumns)
                    .Concat(model.AllHierarchies)
                    .Concat(model.AllLevels)
                    .Concat(model.Perspectives);
        }

        public static void StoreTranslationsAsAnnotations(this Model model)
        {
            // Loop through all translatable objects and provide the translation in the annotation:
            foreach (var item in model.GetAllTranslatableObjects()) item.SaveTranslations();

            // Store the cultures (without translations) as an annotation on the model:
            model.SetAnnotation("TabularEditor_Cultures", model.Cultures.ToJson(), false);
        }

        public static void RestoreTranslationsFromAnnotations(this Model model)
        {
            var translationsJson = model.GetAnnotation("TabularEditor_Cultures");
            if (translationsJson != null)
            {
                model.Cultures.FromJson(translationsJson);
                foreach (var item in model.GetAllTranslatableObjects()) item.LoadTranslations();
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
            if (obj is IFolderObject && !(obj as IFolderObject).TranslatedDisplayFolders.IsEmpty)
                obj.SetAnnotation("TabularEditor_TranslatedDisplayFolders", (obj as IFolderObject).TranslatedDisplayFolders.ToJson(), false);

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
            if (tdf != null && obj is IFolderObject) (obj as IFolderObject).TranslatedDisplayFolders.CopyFrom(JsonConvert.DeserializeObject<Dictionary<string, string>>(tdf));

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<ITranslatableObject>()) child.LoadTranslations(true);
            }
        }
    }
}

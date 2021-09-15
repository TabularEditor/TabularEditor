using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class TranslationAnnotationSerializer
    {
        internal static IEnumerable<IInternalTranslatableObject> GetAllTranslatableObjects(this Model model)
        {
            return Enumerable.Repeat(model as IInternalTranslatableObject, 1)
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
            model.SetAnnotation(AnnotationHelper.ANN_CULTURES, model.Cultures.ToJson(), false);
        }

        /// <summary>
        /// Stores all translations on the current instance of an ITranslatableObject as annotations on the object.
        /// Translations can later be retrieved using the LoadTranslations() extension method.
        /// </summary>
        /// <param name="obj"></param>
        public static void SaveTranslations(this IInternalTranslatableObject obj, bool includeChildren = false)
        {
            if (!obj.TranslatedNames.IsEmpty)
                obj.SetAnnotation(AnnotationHelper.ANN_NAMES, obj.TranslatedNames.ToJson(), false);
            if (!obj.TranslatedDescriptions.IsEmpty)
                obj.SetAnnotation(AnnotationHelper.ANN_DESCRIPTIONS, obj.TranslatedDescriptions.ToJson(), false);
            if (obj is IFolderObject && !(obj as IFolderObject).TranslatedDisplayFolders.IsEmpty)
                obj.SetAnnotation(AnnotationHelper.ANN_DISPLAYFOLDERS, (obj as IFolderObject).TranslatedDisplayFolders.ToJson(), false);

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IInternalTranslatableObject>()) child.SaveTranslations(true);
            }
        }

        /// <summary>
        /// Reads any translations stored in the annotations of the current instance of an ITranslatableObject,
        /// and applies them to the model culture.
        /// </summary>
        /// <param name="obj"></param>
        public static void LoadTranslations(this IInternalTranslatableObject obj, bool includeChildren = false)
        {
            var tn = obj.GetAnnotation(AnnotationHelper.ANN_NAMES);
            if (tn != null)
            {
                obj.TranslatedNames.CopyFrom(DeserializeTranslations(tn));
                obj.RemoveAnnotation(AnnotationHelper.ANN_NAMES, false);
            }

            var td = obj.GetAnnotation(AnnotationHelper.ANN_DESCRIPTIONS);
            if (td != null)
            {
                obj.TranslatedDescriptions.CopyFrom(DeserializeTranslations(td));
                obj.RemoveAnnotation(AnnotationHelper.ANN_DESCRIPTIONS, false);
            }

            var tdf = obj.GetAnnotation(AnnotationHelper.ANN_DISPLAYFOLDERS);
            if (tdf != null && obj is IFolderObject)
            {
                (obj as IFolderObject).TranslatedDisplayFolders.CopyFrom(DeserializeTranslations(tdf));
                obj.RemoveAnnotation(AnnotationHelper.ANN_DISPLAYFOLDERS, false);
            }

            if (includeChildren && obj is ITabularObjectContainer)
            {
                foreach (var child in (obj as ITabularObjectContainer).GetChildren().OfType<IInternalTranslatableObject>()) child.LoadTranslations(true);
            }
        }

        private static Dictionary<string, string> DeserializeTranslations(string json)
        {
            // Build 2.7.6853.27686 unfortunately had a bug that would cause annotated translations to be serialized as an
            // array of KeyValuePairs, instead of as a pure dictionary. What that means, is that the generated JSON would
            // look like:
            //
            // [{"Key":"en-US","Value":"Address"},{"Key":"da-DK","Value":"Adresse"}]
            //
            // instead of:
            //
            // {"en-US":"Address","da-DK":"Adresse"}
            //
            // So when deserializing, we must be able to handle both:
            try
            {
                // First, let's try the pure dictionary deserialization:
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
                // If that fails, let's try deserializing as a list of KeyValuePairs:
                return JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }
}

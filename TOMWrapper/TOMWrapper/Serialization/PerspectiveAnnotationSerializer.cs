using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class PerspectiveAnnotationSerializer
    {
        public static IEnumerable<ITabularPerspectiveObject> GetAllPerspectiveObjects(this Model model)
        {
                return model.Tables.OfType<ITabularPerspectiveObject>()
                    .Concat(model.AllMeasures)
                    .Concat(model.AllColumns)
                    .Concat(model.AllHierarchies);
        }

        public static void RestorePerspectivesFromAnnotations(this Model model)
        {
            var perspectivesJson = model.GetAnnotation("TabularEditor_Perspectives");
            if (perspectivesJson != null)
            {
                model.Perspectives.FromJson(perspectivesJson);
                foreach (var table in model.Tables) table.LoadPerspectives(true);
            }
        }

        public static void StorePerspectivesToAnnotations(this Model model)
        {
            // Loop through all perspective objects and provide the perspective membership in the annotation:
            foreach (var item in model.GetAllPerspectiveObjects()) item.SavePerspectives();

            // Store the perspectives (without members) as an annotation on the model:
            model.SetAnnotation("TabularEditor_Perspectives", model.Perspectives.ToJson(), false);

        }
    }
}

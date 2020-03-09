using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal static class PerspectiveAnnotationSerializer
    {

        public static IEnumerable<IInternalTabularPerspectiveObject> GetAllPerspectiveObjects(this Model model)
        {
                return model.Tables.OfType<IInternalTabularPerspectiveObject>()
                    .Concat(model.AllMeasures)
                    .Concat(model.AllColumns)
                    .Concat(model.AllHierarchies);
        }

        public static void StorePerspectivesToAnnotations(this Model model)
        {
            // Loop through all perspective objects and provide the perspective membership in the annotation:
            foreach (var item in model.GetAllPerspectiveObjects()) item.SavePerspectives();

            // Store the perspectives (without members) as an annotation on the model:
            model.SetAnnotation(AnnotationHelper.ANN_PERSPECTIVES, model.Perspectives.ToJson(), false);
        }
    }
}

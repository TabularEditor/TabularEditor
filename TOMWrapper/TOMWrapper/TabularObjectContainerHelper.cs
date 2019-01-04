using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public static class TabularObjectContainerHelper
    {
        public static IEnumerable<ITabularNamedObject> GetChildrenRecursive(this ITabularObjectContainer container, bool includeSelf)
        {
            if (includeSelf) yield return container;

            foreach (var child in container.GetChildren())
            {
                yield return child;
                if(child is ITabularObjectContainer toc)
                {
                    foreach (var c in toc.GetChildrenRecursive(false)) yield return c;
                }
            }
        }
    }
}

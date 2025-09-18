using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Various helper (extension) methods, for working with <see cref="ITabularObjectContainer"/> objects.
    /// </summary>
    public static class TabularObjectContainerHelper
    {
        /// <summary>
        /// Returns a list of all children of the specified container, including all children of child containers.
        /// </summary>
        /// <param name="container">The object to iterate</param>
        /// <param name="includeSelf">Indicates whether the current object should be included in the iteration output</param>
        /// <returns></returns>
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

        [Localizable(false)]
        internal static string GetNewName(this IEnumerable<string> existingNames, string prefix, bool caseSensitive = false)
        {
            var testName = prefix;
            var suffix = 0;
            var names = new HashSet<string>(existingNames, caseSensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            // Loop to determine if prefix + suffix is already in use - break, when we find a name
            // that's not being used anywhere:
            while (names.Contains(testName))
            {
                suffix++;
                testName = prefix + " " + suffix;
            }
            return testName;
        }

        [Localizable(false)]
        internal static string GetNewName(this IEnumerable<ITabularNamedObject> objectCollection, string prefix, params IEnumerable<ITabularNamedObject>[] additionalObjectCollections)
        {
            return GetNewName(objectCollection.Select(o => o.Name).Concat(additionalObjectCollections.SelectMany(o => o).Select(o => o.Name)), prefix);
        }
    }
}

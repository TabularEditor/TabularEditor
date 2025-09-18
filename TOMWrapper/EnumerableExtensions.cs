using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor
{
    internal static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            var i = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) return i;
                i++;
            }
            return -1;
        }
    }
}

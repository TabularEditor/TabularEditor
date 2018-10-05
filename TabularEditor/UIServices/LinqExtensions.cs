using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
    public static class LinqExtensions
    {
        public static int FirstIndexOf<T>(this IEnumerable<T> items, Predicate<T> where)
        {
            var i = 0;
            foreach(var item in items)
            {
                if (where(item)) return i;
                i++;
            }
            return -1;
        }
    }
}

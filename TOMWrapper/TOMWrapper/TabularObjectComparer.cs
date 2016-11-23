using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public class TabularObjectComparer : IComparer<ITabularNamedObject>, IComparer
    {
        public int Compare(object x, object y)
        {
            return Compare(x as ITabularNamedObject, y as ITabularNamedObject);
        }
        
        public int Compare(ITabularNamedObject x, ITabularNamedObject y)
        {
            var c = x.ObjectType.CompareTo(y.ObjectType);
            if (c == 0)
            {
                if (x.ObjectType == ObjectType.Level && y.ObjectType == ObjectType.Level)
                    return (x as Level).Ordinal.CompareTo((y as Level).Ordinal);

                return string.Compare(x.Name, y.Name, true);
            }
            return c;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    internal static class CollectionHelper
    {
        public static List<T> AsList<T, TBaseParentProp, TBaseParent>(
            this TOM.MetadataObjectCollection<TBaseParent, TBaseParentProp> collection, Func<TBaseParent, T> objSelector)
            where T: TabularNamedObject
            where TBaseParent: TOM.MetadataObject
            where TBaseParentProp: TOM.MetadataObject
        {
            if (collection == null || collection.Count == 0) return new List<T>();
            return collection.Select(objSelector).ToList();
        }
    }
}

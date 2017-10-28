using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class Scripter
    {
        /// <summary>
        /// Scripts the entire database
        /// </summary>
        /// <returns></returns>
        public static string ScriptCreateOrReplace()
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(TabularModelHandler.Singleton.Database);
        }

        public static string ScriptAlter(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptAlter(obj.MetadataObject);
        }

        public static string ScriptCreate(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptCreate(obj.MetadataObject);
        }
        public static string ScriptDelete(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptDelete(obj.MetadataObject);
        }
        public static string ScriptMergePartitions(IList<Partition> obj)
        {
            return TOM.JsonScripter.ScriptMergePartitions(obj.First().MetadataObject, obj.Skip(1).Select(p => p.MetadataObject));
        }


        /// <summary>
        /// Scripts the object
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static string ScriptCreateOrReplace(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(obj.MetadataObject);
        }

        public static string ScriptTranslations(IEnumerable<Culture> translations)
        {
            return Serializer.SerializeObjects(translations);
        }

        public static string ScriptProcess(TabularNamedObject obj, RefreshType refreshType = RefreshType.Automatic)
        {
            return TOM.JsonScripter.ScriptRefresh(obj.MetadataObject, (TOM.RefreshType)refreshType);
        }
        public static string ScriptRefresh(IEnumerable<TabularNamedObject> objects, RefreshType refreshType = RefreshType.Automatic)
        {
            return TOM.JsonScripter.ScriptRefresh(objects.Select(o => o.MetadataObject).ToList(), (TOM.RefreshType)refreshType);
        }
    }
}

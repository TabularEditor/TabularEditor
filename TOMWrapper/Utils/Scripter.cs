using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.Utils
{
    /// <summary>
    /// Various methods for scripting TOM objects into TMSL
    /// </summary>
    public static class Scripter
    {
        /// <summary>
        /// Scripts the entire database as a CreateOrReplace
        /// </summary>
        /// <returns></returns>
        public static string ScriptCreateOrReplace()
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(TabularModelHandler.Singleton.Database);
        }

        /// <summary>
        /// Scripts an Alter TMSL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ScriptAlter(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptAlter(obj.MetadataObject);
        }

        /// <summary>
        /// Scripts a Create TMSL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ScriptCreate(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptCreate(obj.MetadataObject);
        }

        /// <summary>
        /// Scripts a Delete TMSL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ScriptDelete(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptDelete(obj.MetadataObject);
        }

        /// <summary>
        /// Scripts a Merge Partition TMSL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ScriptMergePartitions(IList<Partition> obj)
        {
            return TOM.JsonScripter.ScriptMergePartitions(obj.First().MetadataObject, obj.Skip(1).Select(p => p.MetadataObject));
        }

        /// <summary>
        /// Scripts the object as a CreateOrReplace TMSL
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ScriptCreateOrReplace(TabularNamedObject obj)
        {
            return TOM.JsonScripter.ScriptCreateOrReplace(obj.MetadataObject);
        }

        /// <summary>
        /// Scripts the translation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public static string ScriptTranslations(Model model, IEnumerable<Culture> translations)
        {
            return Serializer.SerializeCultures(model, translations);
        }

        /// <summary>
        /// Generates a Refresh TMSL script for the specified object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="refreshType"></param>
        /// <returns></returns>
        public static string ScriptRefresh(TabularNamedObject obj, RefreshType refreshType = RefreshType.Automatic)
        {
            return TOM.JsonScripter.ScriptRefresh(obj.MetadataObject, (TOM.RefreshType)refreshType);
        }

        /// <summary>
        /// Generates a Refresh TMSL script for the specified objects
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="refreshType"></param>
        /// <returns></returns>
        public static string ScriptRefresh(IEnumerable<TabularNamedObject> objects, RefreshType refreshType = RefreshType.Automatic)
        {
            if (objects.Count() == 1 && objects.First() is Model)
            {
                return TOM.JsonScripter.ScriptRefresh((objects.First().MetadataObject as TOM.Model).Database, (TOM.RefreshType)refreshType);
            }
            else
            {
                return TOM.JsonScripter.ScriptRefresh(objects.Where(o => o is Table || o is Partition).Select(o => o.MetadataObject).ToList(), (TOM.RefreshType)refreshType);
            }
        }
    }
}

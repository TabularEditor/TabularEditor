using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    static public class FolderHelper
    {
        public static string PathFromFullPath(string path)
        {
            var i = path.IndexOf('\\');
            if (i < 0) return "";
            return path.Substring(i).TrimFolder();
        }

        public static ITabularNamedObject GetContainer(this ITabularNamedObject obj, bool useFolders = true)
        {
            var tree = TabularModelHandler.Singleton.Tree;

            if (obj is Model) return null;
            if (obj is Partition p) return p.Table.Partitions;
            if (obj is KPI kpi) return kpi.Measure;
            if (obj is CalculationItem ci) return ci.CalculationGroupTable.CalculationItems;

            if(tree.Options.HasFlag(LogicalTreeOptions.AllObjectTypes))
            {
                if (obj is DataSource) return LogicalGroups.Singleton.DataSources;
                if (obj is Relationship) return LogicalGroups.Singleton.Relationships;
                if (obj is ModelRole) return LogicalGroups.Singleton.Roles;
                if (obj is Perspective) return LogicalGroups.Singleton.Perspectives;
                if (obj is Culture) return LogicalGroups.Singleton.Translations;
                if (obj is Table) return LogicalGroups.Singleton.Tables;
                if (obj is NamedExpression) return LogicalGroups.Singleton.Expressions;
            }

            if (obj is IFolderObject)
            {
                var dObj = obj as IFolderObject;
                if (useFolders)
                {
                    var path = ((tree.Culture == null || obj is Folder) ? dObj.DisplayFolder : dObj.TranslatedDisplayFolders[tree.Culture]).TrimFolder();
                    if (string.IsNullOrEmpty(path)) return dObj.Table;
                    Folder result;
                    if (dObj.Table.FolderCache.TryGetValue(path, out result)) return result;
                }
                return dObj.Table;
            }

            return obj.Model;
        }

        public static IEnumerable<Folder> GetFolderStack(this IFolderObject folderObject, Culture culture)
        {
            var folder = folderObject.GetFolder(culture);
            while(folder != null && folder.Name != "")
            {
                yield return folder;
                folder = folder.GetFolder(culture);
            }
        }

        public static Folder GetFolder(this IFolderObject folderObject, Culture culture)
        {
            var df = folderObject.GetDisplayFolder(culture);
            if (df == null) return null;
            Folder folder;
            return folderObject.Table.FolderCache.TryGetValue(df, out folder) ? folder : null;
        }

        public static string GetDisplayFolder(this IFolderObject folderObject, Culture culture)
        {
            if (culture == null) return folderObject.DisplayFolder.TrimFolder();
            if(folderObject is Folder f)
            {
                return f.DisplayFolder;
            }
            var folder = folderObject.TranslatedDisplayFolders[culture];
            return folder == null ? "" : folder.TrimFolder();
        }
        public static void SetDisplayFolder(this IFolderObject folderObject, string newFolderName, Culture culture)
        {
            if (folderObject is Folder)
            {
                var folder = folderObject as Folder;
                folder.DisplayFolder = newFolderName;
            }
            else
            {
                if (culture == null) folderObject.DisplayFolder = newFolderName;
                else folderObject.TranslatedDisplayFolders[culture] = newFolderName;
            }
        }

        public static string TrimFolder(this string folderPath)
        {
            return folderPath.Trim('\\');
        }

        public static string ConcatPath(this string path, string additionalPath)
        {
            return (path.TrimFolder() + "\\" + additionalPath.TrimFolder()).TrimFolder();
        }

        public static string ConcatPath(this IEnumerable<string> pathBits)
        {
            return string.Join("\\", pathBits);
        }

        public static int Level(this string path)
        {
            return string.IsNullOrEmpty(path) ? 0 : (path.Count(c => c == '\\') + 1);
        }
    }
}

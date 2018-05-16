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

        public static string GetFullPath(ITabularNamedObject obj)
        {
            if (obj is TabularNamedObject) return (obj as TabularNamedObject).Name;
            else if (obj is Folder) return (obj as Folder).FullPath;
            else throw new ArgumentException("Argument must be of type Table or Folder.", "obj");
        }

        public static ITabularNamedObject GetContainer(this ITabularNamedObject obj)
        {
            var tree = TabularModelHandler.Singleton.Tree;

            if (obj is Model) return null;
            if (obj is Partition) return (obj as Partition).Table.Partitions;
            if (obj is KPI) return (obj as KPI).Measure;

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
                var path = ((tree.Culture == null || obj is Folder) ? dObj.DisplayFolder : dObj.TranslatedDisplayFolders[tree.Culture]).TrimFolder();
                if (string.IsNullOrEmpty(path)) return dObj.Table;
                var fullPath = dObj.Table.Name.ConcatPath(path);
                Folder result;
                if (tree.FolderCache.TryGetValue(fullPath, out result)) return result;
                return dObj.Table;
            }

            return obj.Model;
        }

        public static string GetDisplayFolder(this IFolderObject folderObject, Culture culture)
        {
            if (culture == null) return folderObject.DisplayFolder.TrimFolder();
            return folderObject.TranslatedDisplayFolders[culture]?.TrimFolder() ?? "";
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

        public static bool HasAncestor(this IFolderObject child, ITabularNamedObject ancestor, Culture culture)
        {
            string ancestorPath = GetFullPath(ancestor);

            return (child.Table.Name.ConcatPath(child.GetDisplayFolder(culture)) + "\\").StartsWith(ancestorPath + "\\");
        }

        public static bool HasParent(this IFolderObject child, ITabularNamedObject parent, Culture culture)
        {
            string parentPath = GetFullPath(parent);

            return child.Table.Name.ConcatPath(child.GetDisplayFolder(culture)) == parentPath;
        }

        public static int Level(this string path)
        {
            return string.IsNullOrEmpty(path) ? 0 : (path.Count(c => c == '\\') + 1);
        }
    }
}

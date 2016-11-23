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
            if (obj is Table) return (obj as Table).Name;
            else if (obj is Folder) return (obj as Folder).FullPath;
            else throw new ArgumentException("Argument must be of type Table or Folder.", "obj");
        }

        public static IDetailObjectContainer GetContainer(this IDetailObject obj)
        {
            var tree = obj.Table.Handler.Tree;
            var path = ((tree.Culture == null || obj is Folder) ? obj.DisplayFolder : obj.TranslatedDisplayFolders[tree.Culture]).TrimFolder();
            if (string.IsNullOrEmpty(path)) return obj.Table;
            var fullPath = obj.Table.Name.ConcatPath(path);
            Folder result;
            if (tree.FolderTree.TryGetValue(fullPath, out result)) return result;
            return obj.Table;
        }

        public static string GetDisplayFolder(this IDetailObject folderObject, Culture culture)
        {
            if (culture == null) return folderObject.DisplayFolder.TrimFolder();
            return folderObject.TranslatedDisplayFolders[culture]?.TrimFolder() ?? "";
        }
        public static void SetDisplayFolder(this IDetailObject folderObject, string newFolderName, Culture culture)
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

        public static bool HasAncestor(this IDetailObject child, ITabularNamedObject ancestor, Culture culture)
        {
            string ancestorPath = GetFullPath(ancestor);

            return (child.Table.Name.ConcatPath(child.GetDisplayFolder(culture)) + "\\").StartsWith(ancestorPath + "\\");
        }

        public static bool HasParent(this IDetailObject child, ITabularNamedObject parent, Culture culture)
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

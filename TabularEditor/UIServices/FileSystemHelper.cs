using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UIServices
{
    public static class FileSystemHelper
    {
        public static bool IsDirectoryWritable(string dirPath)
        {
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(perm: new FileIOPermission(FileIOPermissionAccess.Write, dirPath));
            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        /// <summary>
        /// Returns the directory of the provided filePath.
        ///  - If <paramref name="filePath"/> is a directory, it is returned.
        ///  - If <paramref name="filePath"/> is a file, its parent directory is returned.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string DirectoryFromPath(string filePath)
        {
            if (filePath != null)
            {
                if (Directory.Exists(filePath))
                    return filePath;

                return (new FileInfo(filePath)).DirectoryName;
            }
            return null;
        }

        /// <summary>
        /// Constructs an absolute path from a base path and a path which can be absolute (in which case it is returned as-is) or relative with respect to the base path.
        /// </summary>
        public static string GetAbsolutePath(string basePath, string relativeOrAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(basePath)) throw new ArgumentException("basePath must be specified");
            if (string.IsNullOrWhiteSpace(relativeOrAbsolutePath)) throw new System.IO.FileNotFoundException();

            var isAbsolutePath = relativeOrAbsolutePath.StartsWith(@"\\") || relativeOrAbsolutePath.StartsWith("/") || (char.IsLetter(relativeOrAbsolutePath[0]) && relativeOrAbsolutePath[1] == ':');

            return isAbsolutePath ? relativeOrAbsolutePath : Path.GetFullPath(basePath.ConcatPath(relativeOrAbsolutePath));
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException("toPath");
            }

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}

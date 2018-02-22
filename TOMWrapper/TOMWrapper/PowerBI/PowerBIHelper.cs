using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.PowerBI
{
    internal static class PowerBIHelper
    {

        public static string LoadDatabaseFromPbitFile(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                using (var za = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    var modelEntry = za.Entries.FirstOrDefault(e => e.Name == "DataModelSchema");
                    if (modelEntry != null)
                    {
                        using (var sr = new StreamReader(modelEntry.Open(), Encoding.Unicode))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                    else
                        throw new Exception();
                }
            }
            catch
            {
                throw new InvalidOperationException("This file is not a valid PBIX / PBIT file.");
            }
        }

        public static void SaveDatabaseToPbitFile(string path, string db)
        {
            using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
            using (var za = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                var modelEntry = za.Entries.FirstOrDefault(e => e.Name == "DataModelSchema");
                if (modelEntry != null)
                {
                    var modelEntryName = modelEntry.FullName;
                    modelEntry.Delete();
                    modelEntry = za.CreateEntry(modelEntryName);
                    var unicodeNoBom = new UnicodeEncoding(false, false);
                    using (var sw = new StreamWriter(modelEntry.Open(), unicodeNoBom))
                    {
                        sw.Write(db);
                    }
                }
                else
                    throw new InvalidOperationException("This file is not a valid PBIX / PBIT file.");
            }
        }
    }
}

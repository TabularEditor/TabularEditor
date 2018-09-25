using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.PowerBI
{
    /// <summary>
    /// Represents a Power BI Template (.pbit) file. Constructor loads the file into memory. Provides properties for
    /// getting and setting the data model json, as well as saving the content back to a .pbit file.
    /// </summary>
    internal class PowerBiTemplate
    {
        MemoryStream fileData = new MemoryStream();
        public PowerBiTemplate(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    fs.CopyTo(fileData);
                    fileData.Seek(0, SeekOrigin.Begin);
                    using (var za = new ZipArchive(fileData, ZipArchiveMode.Read, true))
                    {
                        var modelEntry = za.Entries.FirstOrDefault(e => e.Name == "DataModelSchema");
                        if (modelEntry != null)
                        {
                            using (var sr = new StreamReader(modelEntry.Open(), Encoding.Unicode, true, 1024, true))
                            {
                                _modelJson = sr.ReadToEnd();
                            }
                        }
                        else
                            throw new Exception();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("This file is not a valid PBIX / PBIT file.");
            }
        }

        private string _modelJson;
        public string ModelJson {
            get
            {
                return _modelJson;
            }
            set
            {
                SetModelJson(value);
                _modelJson = value;
            }
        }

        private void SetModelJson(string modelJson)
        {
            try
            {
                fileData.Seek(0, SeekOrigin.Begin);
                using (var za = new ZipArchive(fileData, ZipArchiveMode.Update, true))
                {
                    var modelEntry = za.Entries.FirstOrDefault(e => e.Name == "DataModelSchema");
                    if (modelEntry != null)
                    {
                        var modelEntryName = modelEntry.FullName;
                        modelEntry.Delete();
                        modelEntry = za.CreateEntry(modelEntryName);
                        var unicodeNoBom = new UnicodeEncoding(false, false);
                        using (var sw = new StreamWriter(modelEntry.Open(), unicodeNoBom, 1024, true))
                        {
                            sw.Write(modelJson);
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

        public void SaveAs(string path)
        {
            fileData.Seek(0, SeekOrigin.Begin);
            using (var fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0);
                fileData.CopyTo(fs);
            }
        }
    }
}
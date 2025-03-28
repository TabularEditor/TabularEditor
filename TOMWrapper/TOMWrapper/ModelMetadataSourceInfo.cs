using Newtonsoft.Json.Linq;
using System.IO;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Provides additional metadata about a model, including its source and type.
    /// </summary>
    public record ModelMetadataSourceInfo
    {
        /// <summary>
        /// A string representing the file path of the model metadata source (if loaded from disk), or the connection string (if loaded through the XMLA endpoint)
        /// </summary>
        [IntelliSense("A string representing the file path of the model metadata source (if loaded from disk), or the connection string (if loaded through the XMLA endpoint)")]
        public string Source { get; }

        /// <summary>
        /// Specifies which format the model metadata source is using (TMSL, TMDL, etc.) or if the model metadata was loaded through an XMLA endpoint
        /// </summary>
        [IntelliSense("Specifies which format the model metadata source is using (TMSL, TMDL, etc.) or if the model metadata was loaded through an XMLA endpoint")]
        public ModelSourceType SourceType { get; }

        /// <summary>
        /// If the model metadata is loaded from a PBIP folder structure, this object will hold additional information parsed from the .platform file in that folder
        /// </summary>
        [IntelliSense("If the model metadata is loaded from a PBIP folder structure, this object will hold additional information parsed from the .platform file in that folder")]
        public PbipInfo Pbip { get; }

        internal ModelMetadataSourceInfo(string source, ModelSourceType sourceType, PbipInfo pbip = null)
        {
            Source = source;
            SourceType = sourceType;
            Pbip = pbip;
        }

        public override string ToString()
        {
            return Source + " (" + SourceType + ")";
        }
    }

    /// <summary>
    /// Provides additional metadata about a PBIP folder structure, parsed from the .platform file in that folder.
    /// </summary>
    public class PbipInfo
    {
        /// <summary>
        /// This is the name of the folder in which the model metadata source resides
        /// </summary>
        [IntelliSense("This is the name of the folder in which the model metadata source resides")]
        public string Name { get; }
        /// <summary>
        /// The DisplayName property as specified in the .platform file
        /// </summary>
        [IntelliSense("The DisplayName property as specified in the .platform file")]
        public string DisplayName { get; }
        /// <summary>
        /// The LogicalId property as specified in the .platform file
        /// </summary>
        [IntelliSense("The LogicalId property as specified in the .platform file")]
        public string LogicalId { get; }
        /// <summary>
        /// The root folder of the PBIP folder structure, corresponding to the Power BI / Fabric workspace root folder
        /// </summary>
        [IntelliSense("The root folder of the PBIP folder structure, corresponding to the Power BI / Fabric workspace root folder")]
        public string RootFolder { get; }

        private PbipInfo(string name, string displayName, string logicalId, string rootFolder)
        {
            Name = name;
            DisplayName = displayName;
            LogicalId = logicalId;
            RootFolder = rootFolder;
        }

        internal static PbipInfo GetFromPath(string path)
        {
            const string dirSuffix = ".SemanticModel";
            const string platformFile = ".platform";

            var fi = new FileInfo(path);
            var dir = fi.Directory;
            while (dir != null && !dir.Name.EndsWith(dirSuffix))
            {
                dir = dir.Parent;
            }
            if (dir == null || !File.Exists(Path.Combine(dir.FullName, platformFile)))
                return null;

            var name = dir.Name;
            var rootFolder = dir.Parent?.FullName;
            string displayName = null;
            string logicaId = null;
            try
            {
                var platformJson = File.ReadAllText(Path.Combine(dir.FullName, platformFile));
                var platform = JObject.Parse(platformJson);
                displayName = platform["metadata"]["displayName"].ToString();
                logicaId = platform["config"]["logicalId"].ToString();
            }
            catch
            {
                // silently fail if we can't parse the json
            }

            return new PbipInfo(name, displayName, logicaId, rootFolder);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

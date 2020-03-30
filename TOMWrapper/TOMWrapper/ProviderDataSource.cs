using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class ProviderDataSource
    {
        /// <summary>
        /// Replaces any occurence of the specified "keyword" with the specified "replaceWith", in the
        /// ConnectionString property of this data source.
        /// </summary>
        public void SetPlaceholder(string keyword, string replaceWith)
        {
            ConnectionString = ConnectionString.Replace(keyword, replaceWith, StringComparison.InvariantCultureIgnoreCase);
        }
        protected override void Init()
        {
            if (MetadataObject.ImpersonationMode == TOM.ImpersonationMode.Default)
                MetadataObject.ImpersonationMode = TOM.ImpersonationMode.ImpersonateServiceAccount;

            if (ConnectionString.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                ExtractMashupMetadata();
            }
            base.Init();
        }

        private void ExtractMashupMetadata()
        {
            // This method looks for the keywords "Mashup" or "PowerBI" in the connection string. If the
            // connection string contains one of these, the actual MQuery definition making up the connection
            // can be found inside a Base64 encoded Zip stream.

            var cs = new System.Data.OleDb.OleDbConnectionStringBuilder(this.ConnectionString);
            if (cs.Provider.IndexOf("Mashup", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                cs.Provider.IndexOf("PowerBI", StringComparison.InvariantCultureIgnoreCase) >= 0) {
                IsPowerBIMashup = true;

                string mashupBase64;
                if (cs.ContainsKey("Mashup")) mashupBase64 = cs["Mashup"].ToString();
                else mashupBase64 = cs["Extended Properties"].ToString();
                Location = cs.ContainsKey("Location") ? cs["Location"].ToString() : null ;

                var mashupZip = Convert.FromBase64String(mashupBase64);
                var stream = new MemoryStream(mashupZip);
                var archive = new ZipArchive(stream);
                foreach (var entry in archive.Entries)
                {
                    if (entry.Name.EndsWith(".m", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MQuery = new StreamReader(entry.Open()).ReadToEnd();
                    }
                }
            }
        }

        public override string Name
        {
            set
            {
                base.Name = value;
            }
            get
            {
                return IsPowerBIMashup && !string.IsNullOrEmpty(Location) ? Location : base.Name;
            }
        }

        [Browsable(false)]
        public bool IsPowerBIMashup { get; private set; }
        [Browsable(false)]
        public string Location { get; private set; }
        
        [DisplayName("M Query"),Category("Power BI Source Details")]
        public string MQuery { get; private set; }
        [DisplayName("Source ID"), Category("Power BI Source Details")]
        public string SourceID { get { return MetadataObject.Name; } }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "MQuery":
                case "SourceID":
                    return IsPowerBIMashup;
                default: return true;
            }
        }
    }
}

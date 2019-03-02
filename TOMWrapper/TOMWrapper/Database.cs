using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Database
    {
        [Browsable(false)]
        public TOM.Database TOMDatabase { get; private set; }

        internal Database(Microsoft.AnalysisServices.Core.Database tomDatabase)
        {
            var db = tomDatabase as TOM.Database;
            TOMDatabase = db;
        }

        public override string ToString()
        {
            return TOMDatabase?.Server == null ? "(Metadata loaded from file)" : ServerName + "." + Name;
        }
        
        public string Name
        {
            get { return TOMDatabase?.Name; }
        }
        public string ID
        {
            get
            {
                return TOMDatabase?.ID;
            }
        }
        [DisplayName("Compatibility Level")]
        public int? CompatibilityLevel
        {
            get
            {
                return TOMDatabase?.CompatibilityLevel;
            }
        }

        public long? Version
        {
            get
            {
                return TOMDatabase?.Version;
            }
        }

        [DisplayName("Last Processed")]
        public DateTime? LastProcessed
        {
            get
            {
                return TOMDatabase?.LastProcessed;
            }
        }

        [DisplayName("Last Update")]
        public DateTime? LastUpdate
        {
            get
            {
                return TOMDatabase?.LastUpdate;
            }
        }

        [DisplayName("Last Schema Update")]
        public DateTime? LastSchemaUpdate
        {
            get
            {
                return TOMDatabase?.LastSchemaUpdate;
            }
        }

        [DisplayName("Created Timestamp")]
        public DateTime? CreatedTimestamp
        {
            get
            {
                return TOMDatabase?.CreatedTimestamp;
            }
        }

        [DisplayName("Server Name")]
        public string ServerName
        {
            get
            {
                return TOMDatabase?.Server?.Name;
            }
        }
        [DisplayName("Server Version")]
        public string ServerVersion
        {
            get
            {
                var s = TOMDatabase?.Server;
                if (s == null) return null;
                return string.Format("{0} ({1}) {2}",s.ProductName, s.ProductLevel, s.Version);
            }
        }
    }
}

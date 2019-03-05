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
    public sealed class Database: ITabularObject
    {
        [Browsable(false)]
        public TOM.Database TOMDatabase { get; private set; }

        private Model _model;

        internal Database(Model model, Microsoft.AnalysisServices.Core.Database tomDatabase)
        {
            var db = tomDatabase as TOM.Database;
            TOMDatabase = db;
            _model = model;
            _name = tomDatabase.Name;
            _id = tomDatabase.ID;
        }

        public override string ToString()
        {
            return TOMDatabase?.Server == null ? "(Metadata loaded from file)" : ServerName + "." + Name;
        }

        private string _name;
        private string _id;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void DoPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Description("The name of the deployed database. Changing this has no effect on an already deployed database.")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_id == _name) _id = value;
                _name = value;
                DoPropertyChanged("Name");
            }
        }
        [Description("The ID of the deployed database. Changing this has no effect on an already deployed database.")]
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                DoPropertyChanged("ID");
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

        [Browsable(false)]
        public ObjectType ObjectType => ObjectType.Database;

        [Browsable(false)]
        public Model Model => _model;

        [Browsable(false)]
        public bool IsRemoved => false;
    }
}

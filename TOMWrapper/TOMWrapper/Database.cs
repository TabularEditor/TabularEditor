extern alias json;

using json::Newtonsoft.Json.Linq;
using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Database : ITabularObject, INotifyPropertyChanged, INotifyPropertyChanging, IDynamicPropertyObject
    {
        [Browsable(false)]
        public TOM.Database TOMDatabase { get; private set; }

        private Model _model;
        private TabularModelHandler Handler => _model.Handler;

        internal Database(Model model, Microsoft.AnalysisServices.Core.Database tomDatabase)
        {
            var db = tomDatabase as TOM.Database;
            TOMDatabase = db;
            _model = model;
            _name = tomDatabase.Name;
            _id = tomDatabase.ID;
            _compatibilityLevel = tomDatabase.CompatibilityLevel;
        }

        public bool NameModified => _name != TOMDatabase.Name;
        public bool IdModified => _id != TOMDatabase.ID;
        public bool CompatibilityLevelModified => _compatibilityLevel != TOMDatabase.CompatibilityLevel;

        public override string ToString()
        {
            return TOMDatabase?.Server == null ? "(Metadata loaded from file)" : ServerName + "." + Name;
        }

        private string _name;
        private string _id;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Handler.DoObjectChanged(this, propertyName, oldValue, newValue);
        }

        /// <summary>
        /// Called before a property is changed on an object. Derived classes can control how the change is handled.
        /// Throw ArgumentException within this method, to display an error message in the UI.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        /// <param name="newValue">New value assigned to the property.</param>
        /// <param name="undoable">Return false if automatic undo of the property change is not needed.</param>
        /// <param name="cancel">Return true if the property change should not apply.</param>
        void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (!Handler.PowerBIGovernance.AllowEditProperty(ObjectType, propertyName))
            {
                cancel = true;
                return;
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            if (cancel) return;
            Handler.DoObjectChanging(this, propertyName, newValue, ref cancel);
        }

        [Description("The name of the deployed database. Changing this has no effect on an already deployed database.")]
        public string Name
        {
            get { return _name; }
            set
            {
                var oldValue = Name;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.NAME, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.NAME, oldValue, value));
                _name = value;
                OnPropertyChanged(Properties.NAME, oldValue, value);
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
                var oldValue = ID;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.ID, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.ID, oldValue, value));
                _id = value;
                OnPropertyChanged(Properties.ID, oldValue, value);
            }
        }

        private readonly int[] validCompatibilityLevels = new[] { 1200, 1400, 1450, 1455, 1465, 1470, 1500 };
        private bool IsValidCompatibilityLevel(int compatibilityLevel)
        {
            return validCompatibilityLevels.Contains(compatibilityLevel);
        }

        public bool Browsable(string propertyName)
        {
            if (!Handler.PowerBIGovernance.VisibleProperty(ObjectType.Database, propertyName)) return false;
            return true;
        }

        public bool Editable(string propertyName)
        {
            if (!Handler.PowerBIGovernance.AllowEditProperty(ObjectType.Database, propertyName)) return false;
            return true;
        }

        private int _compatibilityLevel;

        [DisplayName("Compatibility Level")]
        public int CompatibilityLevel
        {
            get
            {
                return _compatibilityLevel;
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

    internal static partial class Properties
    {
        public const string COMPATIBILITYLEVEL = "CompatibilityLevel";
        public const string ID = "Id";
    }
}

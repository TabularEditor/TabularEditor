using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TypeConverter(typeof(DynamicPropertyConverter))]
    public sealed class Database : ITabularObject, INotifyPropertyChanged, INotifyPropertyChanging, IDynamicPropertyObject
    {
        public static readonly char[] InvalidNameCharacters = new[] { '.', ',', ';', '\'', '`', ':', '/', '\\', '*', '|', '?', '"', '&', '%', '$', '!', '+', '=', '(', ')', '[', ']', '{', '}', '<', '>' };

        [Browsable(false)]
        public TOM.Database TOMDatabase { get; private set; }

        private Model _model;
        private TabularModelHandler Handler => _model.Handler;

        internal Database(Model model, Microsoft.AnalysisServices.Core.Database tomDatabase)
        {
            Debug.Assert(tomDatabase != null);
            var db = tomDatabase as TOM.Database;
            TOMDatabase = db;
            _model = model;

            orgName = tomDatabase.Name;
            orgID = tomDatabase.ID;
            orgCompatibilityLevel = tomDatabase.CompatibilityLevel;
            orgCompatibilityMode = tomDatabase.CompatibilityMode;
            orgDescription = tomDatabase.Description;
            orgVisible = tomDatabase.Visible;
        }

        private string orgName;
        private string orgID;
        private string orgDescription;
        private int orgCompatibilityLevel;
        private Microsoft.AnalysisServices.CompatibilityMode orgCompatibilityMode;
        private bool orgVisible;

        internal bool HasLocalChanges 
            => TOMDatabase.Name != orgName 
            || TOMDatabase.ID != orgID
            || TOMDatabase.CompatibilityLevel != orgCompatibilityLevel
            || TOMDatabase.CompatibilityMode != orgCompatibilityMode
            || TOMDatabase.Description != orgDescription
            || TOMDatabase.Visible != orgVisible;

        /// <summary>
        /// Executes the specified TMSL script against the connected instance of Analysis Services.
        /// </summary>
        [IntelliSense("Executes the specified TMSL script against the connected instance of Analysis Services.")]
        public void ExecuteCommand(string tmsl)
        {
            if (TOMDatabase?.Server == null) throw new NotSupportedException("Cannot execute queries when not connected to Analysis Services");

            var xmla = new XElement("Statement", tmsl).ToString();
            var results = TOMDatabase.Server.Execute(xmla);
            if (results != null && results.ContainsErrors)
            {
                var errorMessage = results[0].Messages.OfType<Microsoft.AnalysisServices.XmlaError>().FirstOrDefault()?.Description;
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Executes the specified DAX query against the connected database and returns an AmoDataReader object that enumerates the result
        /// </summary>
        [IntelliSense("Executes the specified DAX query against the connected database and returns an AmoDataReader object that enumerates the result.")]
        public IDataReader ExecuteReader(string dax)
        {
            if (TOMDatabase?.Server == null) throw new NotSupportedException("Cannot execute queries when not connected to Analysis Services");

            var xmla = new XElement("Statement", dax).ToString();
            CloseReader();
            LastOpenedReader = TOMDatabase.Server.ExecuteReader(
                xmla,
                out Microsoft.AnalysisServices.XmlaResultCollection results,
                new Dictionary<string, string> { { "Catalog", TOMDatabase.Name } });
            if (results != null && results.ContainsErrors)
            {
                var errorMessage = results[0].Messages.OfType<Microsoft.AnalysisServices.XmlaError>().FirstOrDefault()?.Description;
                throw new DaxQueryException(errorMessage);
            }
            return LastOpenedReader;
        }

        private Microsoft.AnalysisServices.AmoDataReader LastOpenedReader;

        internal void CloseReader()
        {
            if (LastOpenedReader != null && !LastOpenedReader.IsClosed)
                LastOpenedReader.Close();
        }

        /// <summary>
        /// Executes the specified DAX query against the connected database and returns a data set containing the result
        /// </summary>
        [IntelliSense("Executes the specified DAX query against the connected database and returns a data set containing the result")]
        public DataSet ExecuteDax(string dax)
        {
            var dataSet = new DataSet();
            using (var reader = ExecuteReader(dax) as AmoDataReader)
            {
                var adapter = new AmoDataAdapter(reader);
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        /// <summary>
        /// Evaluates the specified DAX expression against the connected database and returns a data table or scalar value containing the result
        /// </summary>
        [IntelliSense("Evaluates the specified DAX expression against the connected database and returns a data table or scalar value containing the result")]
        public object EvaluateDax(string dax)
        {
            try
            {
                var ds = ExecuteDax($"EVALUATE {dax}");
                if (ds.Tables.Count == 1)
                {
                    var table = ds.Tables[0];
                    if (table.Columns.Count == 1 && table.Rows.Count == 1)
                        return table.Rows[0][0];
                    else
                        return table;
                }
                else
                    return null;
            }
            catch (DaxQueryException)
            {
                using (var reader = ExecuteReader($"EVALUATE ROW(\"Value\", {dax})"))
                {
                    if (!reader.Read()) return null;
                    return reader.IsDBNull(0) ? null : reader.GetValue(0);
                }
            }
        }

        public override string ToString()
        {
            return TOMDatabase?.Server == null ? "(Metadata loaded from file)" : ServerName + "." + Name;
        }

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
            get => TOMDatabase.Name;
            set
            {
                var oldValue = Name;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.NAME, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.NAME, oldValue, value));
                TOMDatabase.Name = value;
                OnPropertyChanged(Properties.NAME, oldValue, value);
            }
        }
        [Description("The ID of the deployed database. Changing this has no effect on an already deployed database.")]
        public string ID
        {
            get => TOMDatabase.ID;
            set
            {
                var oldValue = ID;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.ID, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.ID, oldValue, value));
                TOMDatabase.ID = value;
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
                return TOMDatabase.CompatibilityLevel;
            }
            set
            {
                var oldValue = CompatibilityLevel;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.COMPATIBILITYLEVEL, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.COMPATIBILITYLEVEL, oldValue, value));
                TOMDatabase.CompatibilityLevel = value;
                OnPropertyChanged(Properties.COMPATIBILITYLEVEL, oldValue, value);
            }
        }
        [DisplayName("Compatibility Mode")]
        public CompatibilityMode CompatibilityMode
        {
            get => (CompatibilityMode)TOMDatabase.CompatibilityMode;
            set
            {
                var oldValue = CompatibilityMode;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.COMPATIBILITYMODE, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.COMPATIBILITYMODE, oldValue, value));
                TOMDatabase.CompatibilityMode = (Microsoft.AnalysisServices.CompatibilityMode)value;
                OnPropertyChanged(Properties.COMPATIBILITYMODE, oldValue, value);
            }
        }

        public string Description
        {
            get => TOMDatabase.Description;
            set
            {
                var oldValue = Description;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DESCRIPTION, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.DESCRIPTION, oldValue, value));
                TOMDatabase.Description = value;
                OnPropertyChanged(Properties.DESCRIPTION, oldValue, value);
            }
        }

        public bool Visible
        {
            get => TOMDatabase.Visible;
            set
            {
                var oldValue = Visible;
                if (oldValue == value) return;
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.VISIBLE, value, ref undoable, ref cancel);
                if (cancel) return;
                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedNonMetadataObjectAction(this, Properties.VISIBLE, oldValue, value));
                TOMDatabase.Visible = value;
                OnPropertyChanged(Properties.VISIBLE, oldValue, value);
            }
        }

        public long? Version
        {
            get
            {
                return TOMDatabase.Version;
            }
        }

        public string EstimatedSize
        {
            get
            {
                return (TOMDatabase.EstimatedSize / 1024.0 / 1024.0).ToString("#,##0.00") + " MB";
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
                if(s.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI)
                {
                    var productName = s.ServerLocation == ServerLocation.OnPremise ? "Power BI Desktop" : "Power BI Service";
                    return $"{productName} (AS build {s.Version})";

                }
                if (!string.IsNullOrEmpty(s.ProductLevel))
                {
                    return $"{s.ProductName} {s.ProductLevel} (AS build {s.Version})";
                }
                else
                {
                    return $"{s.ProductName} (AS build {s.Version})";
                }
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
        public const string COMPATIBILITYMODE = "CompatibilityMode";
        public const string COMPATIBILITYLEVEL = "CompatibilityLevel";
        public const string ID = "Id";
        public const string VISIBLE = "Visible";
    }

    //
    // Summary:
    //     An enumeration of the compatibility modes supported by the various AnalysisServices
    //     services.
    public enum CompatibilityMode
    {
        //
        // Summary:
        //     Unknown Mode.
        Unknown = 0,
        //
        // Summary:
        //     Basic AnalysisServices mode - used on SSAS and AAS.
        AnalysisServices = 1,
        //
        // Summary:
        //     Power BI mode.
        PowerBI = 2,
        //
        // Summary:
        //     Excel mode.
        Excel = 4
    }

    public class DaxQueryException: Exception
    {
        public DaxQueryException(string message): base(message) { }
    }
}

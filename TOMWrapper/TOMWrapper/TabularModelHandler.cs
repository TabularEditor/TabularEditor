﻿extern alias json;

using System;
using System.Collections.Generic;
using System.Linq;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.Utils;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class TabularModelHandler : IDisposable
    {
        internal Guid InstanceID = Guid.NewGuid();
        public const string PROP_HASUNSAVEDCHANGES = "HasUnsavedChanges";
        public const string PROP_ISCONNECTED = "IsConnected";
        public const string PROP_STATUS = "Status";
        public const string PROP_ERRORS = "Errors";

        private TabularModelHandlerSettings _settings = TabularModelHandlerSettings.Default;
        public TabularModelHandlerSettings Settings
        {
            get
            { return _settings; }
            set
            {
                if (_settings != null) _settings.PropertyChanged -= _settings_PropertyChanged;
                _settings = value;
                if (_settings != null) _settings.PropertyChanged += _settings_PropertyChanged;

                UpdateSettings();
            }
        }

        private void UpdateSettings()
        {
            PowerBIGovernance.UpdateGovernanceMode();
            _tree?.OnStructureChanged();
            
            if(trace != null)
            {
                if (_settings.ChangeDetectionLocalServers && !trace.IsStarted)
                    trace.Start();
                else if (!_settings.ChangeDetectionLocalServers && trace.IsStarted)
                    trace.Stop();
            }
        }

        private void _settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateSettings();
        }

        public UndoManager UndoManager { get; private set; }
        public TabularCommonActions Actions { get; private set; }

        private TOM.Server server = null;
        private TOM.Database database;

        public static void Cleanup()
        {
            ExternalChangeTrace.Cleanup();
        }

        public Model Model { get; private set; }
        public TOM.Database Database { get { return database; } }
        public string ConnectionInfo => IsConnected && database?.Server != null ? database.Server.Name : "(No connection)";
        public int CompatibilityLevel => database.CompatibilityLevel;
        public bool PbiMode => Database?.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI;
        public bool IsPbiDesktop => PowerBIGovernance.IsPBIDesktop(database);
        /// <summary>
        /// Applys translation from a JSON string.
        /// </summary>
        /// <param name="culturesJson"></param>
        /// <param name="overwriteExisting"></param>
        /// <param name="ignoreInvalid"></param>
        /// <returns>False if ignoreInvalid is set to false and an invalid object is encountered</returns>
        public bool ImportTranslations(string culturesJson, bool overwriteExisting, bool ignoreInvalid)
        {
            BeginUpdate("Import translations");
            var result = TabularCultureHelper.ImportTranslations(culturesJson, Model, overwriteExisting, !ignoreInvalid);

            // Rolls back translation changes if an error were encountered
            EndUpdate(true, !result);

            return result;
        }

        private void Init()
        {
            UndoManager = new UndoManager(this);
            UndoManager.Suspend();
            Actions = new TabularCommonActions(this);
            Model = Model.CreateFromMetadata(database.Model);
            Model.Database = new Database(Model, database);
            //CheckErrors();

            FormulaFixup.BuildDependencyTree();
            UndoManager.Resume();
        }
        public TabularObject GetWrapperObject(TOM.MetadataObject obj) { return WrapperLookup[obj]; }
        internal readonly Dictionary<TOM.MetadataObject, TabularObject> WrapperLookup = new Dictionary<TOM.MetadataObject, TabularObject>();

        public ModelSourceType SourceType { get; private set; }
        public string Source { get; private set; }

        private TabularModelHandler(TabularModelHandlerSettings settings)
        {
            this.PowerBIGovernance = new PowerBIGovernance(this);
            Settings = settings ?? TabularModelHandlerSettings.Default;
            Singleton = this;
        }

        /// <summary>
        /// Creates a new blank Tabular Model
        /// </summary>
        public TabularModelHandler(int compatibilityLevel = 1200, TabularModelHandlerSettings settings = null, bool pbiDatasetModel = false): this(settings)
        {
            server = null;

            database = new TOM.Database("SemanticModel") { CompatibilityLevel = compatibilityLevel,
                CompatibilityMode = pbiDatasetModel ? Microsoft.AnalysisServices.CompatibilityMode.PowerBI : Microsoft.AnalysisServices.CompatibilityMode.AnalysisServices };

            database.Model = new TOM.Model();
            if (pbiDatasetModel) database.Model.DefaultPowerBIDataSourceVersion = TOM.PowerBIDataSourceVersion.PowerBI_V3;

             SourceType = ModelSourceType.File;
            Source = "Model.bim";

            Status = "Successfully created new model.";
            Init();
            
            PowerBIGovernance.UpdateGovernanceMode();
        }
        internal PowerBIGovernance PowerBIGovernance { get; }

        private PowerBiTemplate pbit;

        private string serverName;

        private readonly string applicationName = "TabularEditor-" + Guid.NewGuid().ToString("D");
        private readonly ExternalChangeTrace trace;

        public event EventHandler<ExternalChangeEventArgs> OnExternalChange;
        public event EventHandler<ProgressReportEventArgs> OnProgressReport;

        private void XEventCallback(TOM.TraceEventArgs eventArgs)
        {
            switch(eventArgs.EventClass)
            {
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportBegin:
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportCurrent:
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportEnd:
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportError:
                    OnProgressReport?.Invoke(this, new ProgressReportEventArgs(eventArgs));
                    break;

                case Microsoft.AnalysisServices.TraceEventClass.CommandEnd:
                    if (!Settings.ChangeDetectionLocalServers) return;

                    if (skipEvent) break;

                    var ext = new ExternalChangeEventArgs(eventArgs);
                    if(OnExternalChange != null)
                    {
                        skipEvent = true;
                        OnExternalChange.BeginInvoke(this, ext, EndExternalChangeEvent, ext);
                    }
                    break;
            }
        }

        private void EndExternalChangeEvent(IAsyncResult result)
        {
            var eventArgs = result.AsyncState as ExternalChangeEventArgs;
            
            skipEvent = false;
        }

        private bool skipEvent = false;

        public void RefreshTom()
        {
            database.Model.Sync(new TOM.SyncOptions { DiscardLocalChanges = true });
            database.Refresh();
            Version = database.Version;

            Tree.BeginUpdate();
            Model.Reinit();
            Tree.RebuildFolderCache();
            Tree.EndUpdate();
            Tree.OnStructureChanged();

            UndoManager.Clear();
			UndoManager.SetCheckpoint();
        }

        public static void Log(string additionalMessage, Exception ex)
        {
            MessageLogger?.Invoke(additionalMessage + "\r\n    Exception type: " + ex.GetType().Name + "\r\n    Exception message: " + ex.Message + "\r\n    Stack trace: " + ex.StackTrace);
        }
        public static void Log(string message)
        {
#if DEBUG
            if (MessageLogger == null) Debug.WriteLine(message);
#endif
            MessageLogger?.Invoke(message);
        }
        public static Action<string> MessageLogger = null;

        /// <summary>
        /// Connects to a SQL Server 2016 Analysis Services instance and loads a tabular model
        /// from one of the deployed databases on the instance.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        public TabularModelHandler(string serverName, string databaseName, TabularModelHandlerSettings settings = null): this(settings)
        {
            this.serverName = serverName;
            _disableUpdates = true;

            server = new TOM.Server();

            var connectionString = TabularConnection.GetConnectionString(serverName, applicationName);
            server.Connect(connectionString);

            if (databaseName == null)
            {
                if (server.Databases.Count >= 1) database = server.Databases[0];
                else throw new InvalidOperationException("This instance does not contain any databases, or the user does not have access.");
            }
            else
            {
                database = server.Databases.FindByName(databaseName);
                if (database == null)
                    database = server.Databases[databaseName];
            }
            if (CompatibilityLevel < 1200) throw new InvalidOperationException("Only databases with Compatibility Level 1200 or higher can be loaded in Tabular Editor.");

            SourceType = ModelSourceType.Database;
            Source = database.Server.Name + "." + database.Name;

            Status = "Connected successfully.";
            Version = database.Version;
            Init();
            UndoManager.Suspend();

            Model.ClearTabularEditorAnnotations();

            _disableUpdates = false;
            UndoManager.Resume();
            PowerBIGovernance.UpdateGovernanceMode();
            CheckErrors();

            try
            {
                ExternalChangeTrace.Cleanup();
                trace = new ExternalChangeTrace(database, applicationName, XEventCallback);
                if (Settings.ChangeDetectionLocalServers) trace.Start();
            }
            catch (Exception ex)
            {
                Log("Exception while configuring AS trace: " + ex.Message);
            }
        }

        public void CleanOrphanedTraces()
        {
            trace?.CleanOrphanedTraces();
        }

        public int GetOrphanedTraceCount()
        {
            return trace == null ? 0 : trace.GetOrphanedTraceCount();
        }

        private static TabularModelHandler _singleton;
        internal static TabularModelHandler Singleton
        {
            get => _singleton;
            set
            {
                _singleton = value;
            }
        }

        public bool IsConnected { get { return Version != -1; } }
        public long Version { get; private set; } = -1;

        public bool HasUnsavedChanges
        {
            get
            {
                return !UndoManager.AtCheckpoint;
            }
        }

        private string _status;
        public string Status { get { return _status; } set { _status = value; } }

        internal HashSet<IErrorMessageObject> _errors { get; private set; } = new HashSet<IErrorMessageObject>();
        public IReadOnlyCollection<IErrorMessageObject> Errors => _errors;

        internal static List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>> GetObjectsNotReady(TOM.Database database)
        {
            var result = new List<Tuple<TOM.NamedMetadataObject, TOM.ObjectState>>();

            // Find partitions that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Partitions).Where(p => p.State != TOM.ObjectState.Ready)
                    .Select(p => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(p, p.State))
                    );

            // Find calculated columns that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Columns.OfType<TOM.CalculatedColumn>()).Where(c => c.State != TOM.ObjectState.Ready)
                    .Select(c => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(c, c.State))
                );

            // Find calculated columns that are not in the "Ready" state:
            result.AddRange(
                    database.Model.Tables.SelectMany(t => t.Columns.OfType<TOM.DataColumn>()).Where(c => c.State != TOM.ObjectState.Ready)
                    .Select(c => new Tuple<TOM.NamedMetadataObject, TOM.ObjectState>(c, c.State))
                );

            return result;
        }

        internal static List<Tuple<TOM.NamedMetadataObject, string>> CheckErrors(TOM.Database database)
        {
            var result = new List<Tuple<TOM.NamedMetadataObject, string>>();
            foreach (var t in database.Model.Tables)
            {
                result.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)).Select(m => new Tuple<TOM.NamedMetadataObject, string>(m, m.ErrorMessage)));
                if (database.CompatibilityLevel >= 1400) result.AddRange(t.Measures.Where(m => !string.IsNullOrEmpty(m.DetailRowsDefinition?.ErrorMessage)).Select(m => new Tuple<TOM.NamedMetadataObject, string>(m, "Detail rows expression: " + m.DetailRowsDefinition.ErrorMessage)));
                result.AddRange(t.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(c => new Tuple<TOM.NamedMetadataObject, string>(c, c.ErrorMessage)));
                result.AddRange(t.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)).Select(p => new Tuple<TOM.NamedMetadataObject, string>(p, p.ErrorMessage)));
                if(database.CompatibilityLevel >= 1470 && t.CalculationGroup != null)
                {
                    result.AddRange(t.CalculationGroup.CalculationItems.Where(ci => !string.IsNullOrEmpty(ci.ErrorMessage)).Select(ci => new Tuple<TOM.NamedMetadataObject, string>(ci, ci.ErrorMessage)));
                    result.AddRange(t.CalculationGroup.CalculationItems.Where(ci => !string.IsNullOrEmpty(ci.FormatStringDefinition?.ErrorMessage)).Select(ci => new Tuple<TOM.NamedMetadataObject, string>(ci, "Format string expression: " + ci.FormatStringDefinition.ErrorMessage)));
                }
            }
            foreach(var r in database.Model.Roles)
            {
                result.AddRange(r.TablePermissions.Where(tp => !string.IsNullOrEmpty(tp.ErrorMessage)).Select(tp => new Tuple<TOM.NamedMetadataObject, string>(tp, tp.ErrorMessage)));
            }
            return result;
        }

        private void CheckErrors()
        {
            var errorList = new List<IErrorMessageObject>();

            Tree.ClearFolderErrors();

            foreach (var t in database.Model.Tables)
            {
                var table = GetWrapperObject(t) as Table;
                table.ClearError();
                errorList.AddRange(table.Measures.Where(m => !string.IsNullOrEmpty(m.ErrorMessage)));
                errorList.AddRange(table.Columns.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)));
                errorList.AddRange(table.Partitions.Where(p => !string.IsNullOrEmpty(p.ErrorMessage)));
                if (CompatibilityLevel >= 1470 && table is CalculationGroupTable cgt)
                    errorList.AddRange(cgt.CalculationItems.Where(ci => !string.IsNullOrEmpty(ci.ErrorMessage)));

                WrapperLookup.Values.OfType<IExpressionObject>().ToList().ForEach(i => i.ResetModifiedState());
            }
            errorList.AddRange(Model.Roles.Where(r => r.ErrorMessage != null));
            if (errorList.Count > 0 || Errors?.Count > 0)
            {
                _errors = new HashSet<IErrorMessageObject>(errorList);
            }

            foreach(var errObj in errorList)
            {
                if (errObj is IFolderObject fo)
                {
                    var parentFolder = fo.GetFolder(Tree.Culture);
                    if (parentFolder != null) parentFolder.AddError(fo);
                    else if(fo is ITabularTableObject tto) tto.Table.AddError(fo);
                }
                else if (errObj is CalculationItem ci)
                {
                    var parentTable = ci.CalculationGroupTable;
                    parentTable.AddError(ci);
                }
            }
        }

        private bool _disableUpdates = false;
        

        /// <summary>
        /// Begins a batch update
        /// </summary>
        /// <param name="undoName"></param>
        public void BeginUpdate(string undoName)
        {
            if (_disableUpdates) return;

            EoB_PostponeOperations = true;

            Tree.BeginUpdate();
            if(!string.IsNullOrEmpty(undoName)) UndoManager.BeginBatch(undoName);
        }

        /// <summary>
        /// Ends the latest batch update (can never be called more times than BeginUpdate).
        /// </summary>
        /// <param name="undoable"></param>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public int EndUpdate(bool undoable = true, bool rollback = false)
        {
            if (_disableUpdates) return 0;

            var actionCount = 0;
            if(undoable || rollback) actionCount = UndoManager.EndBatch(rollback);

            if (Tree.UpdateLocks == 1) EoB_PostponeOperations = false;
            if (Tree.UpdateLocks == 1 && EoB_RequireRebuildDependencyTree)
            {
                FormulaFixup.BuildDependencyTree();
                EoB_RequireRebuildDependencyTree = false;
            }

            // This takes care of reducing the UpdateLocks counter, and notifying the UI when we reach zero:
            Tree.EndUpdate();

            return actionCount;
        }

        internal bool EoB_RequireRebuildDependencyTree = false;
        internal bool EoB_PostponeOperations = false;
        public bool UpdateInProgress => EoB_PostponeOperations;

        /// <summary>
        /// Ends all batch updates in progress.
        /// </summary>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public int EndUpdateAll(bool rollback = false)
        {
            var actionCount = 0;
            while (UndoManager.BatchDepth > 0)
            {
                actionCount = UndoManager.EndBatch(rollback);
            }

            if (EoB_RequireRebuildDependencyTree) { EoB_PostponeOperations = false; FormulaFixup.BuildDependencyTree(); EoB_RequireRebuildDependencyTree = false; }

            while (Tree.UpdateLocks > 0)
            {
                Tree.EndUpdate();
            }
            return actionCount;
        }


        internal void UpdateObject(ITabularObject obj)
        {

            Tree.OnNodesChanged(obj);
        }
        internal void UpdateObjectName(ITabularNamedObject obj)
        {
            Tree.OnNodeNameChanged(obj);
        }
 
        internal void UpdateFolders(Table table)
        {
            Tree.RebuildFolderCacheForTable(table);
            Tree.OnStructureChanged(table);
        }
        internal void UpdateLevels(Hierarchy hierarchy)
        {
            Tree.OnStructureChanged(hierarchy);
        }

        public void Dispose()
        {
            if (server != null)
            {
                server.Dispose();
            }
        }

        private TabularTree _tree;
        public TabularTree Tree {
            get
            {
                if(_tree == null)
                {
                    _tree = new NullTree(this);
                }
                return _tree;
            }
            set
            {
                _tree = value;
            }
        }
    }

    public class ProgressReportEventArgs
    {
        public long IntegerData { get; }
        public string ObjectName { get; }
        public string ObjectPath { get; }
        public string ObjectReference { get; }

        public ProgressEventType EventType { get; }

        public ProgressReportEventArgs(TOM.TraceEventArgs xEvent)
        {
            switch(xEvent.EventClass)
            {
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportBegin:
                    EventType = ProgressEventType.Begin;
                    break;
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportCurrent:
                    EventType = ProgressEventType.Current;
                    break;
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportEnd:
                    EventType = ProgressEventType.End;
                    break;
                case Microsoft.AnalysisServices.TraceEventClass.ProgressReportError:
                    EventType = ProgressEventType.Error;
                    break;
            }

            this.IntegerData = xEvent.IntegerData;
            this.ObjectName = xEvent.ObjectName;
            this.ObjectPath = xEvent.ObjectPath;
            this.ObjectReference = xEvent.ObjectReference;
        }
    }

    public class ExternalChangeEventArgs
    {
        public string TextData { get; }

        internal ExternalChangeEventArgs(TOM.TraceEventArgs xEvent)
        {
            this.TextData = xEvent.TextData;
        }
    }

    public enum ProgressEventType
    {
        Begin,
        Current,
        End,
        Error
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;
using AS = Microsoft.AnalysisServices;
using System.IO;
using System.Xml;

namespace TabularEditor.TOMWrapper
{
    internal class ExternalChangeTrace
    {
        private TOM.Trace trace;
        private readonly TOM.Database database;
        private readonly string databaseNameWhenTraceStarted;
        private TOM.Server server => database.Server;
        private readonly string applicationName;
        private readonly Action<TOM.TraceEventArgs> onExternalChangeCallback;

        private static List<TOM.Trace> tabularEditorSessionTraces = new List<TOM.Trace>();

        private static void CleanupTrace(TOM.Trace trace)
        {
            if (trace.IsStarted)
                trace.Stop();

            if (trace.Parent != null)
            {
                trace.Drop();
            }
            trace.Dispose();
        }
        public static void Cleanup()
        {
            lock (tabularEditorSessionTraces)
            {
                try
                {
                    foreach (var trace in tabularEditorSessionTraces)
                    {
                        CleanupTrace(trace);
                    }
                    tabularEditorSessionTraces.Clear();
                    TabularModelHandler.Log("Analysis Services trace cleanup completed");
                }
                catch (Exception ex)
                {
                    TabularModelHandler.Log("Error occurred during trace cleanup: " + ex.Message);
                }
            }
        }

        public void Stop()
        {
            if (trace == null || !trace.IsStarted) return;

            trace.Stop();
            TabularModelHandler.Log("Analysis Services trace stopped");
        }

        private string currentTraceName;
        private const string traceNamePrefix = "TabularEditor-";

        public int GetOrphanedTraceCount()
        {
            server.Refresh();
            return server.Traces.OfType<TOM.Trace>().Count(t => t.Name.StartsWith(traceNamePrefix) && t.Name != currentTraceName);
        }

        public void CleanOrphanedTraces()
        {
            server.Refresh();
            foreach(var trace in server.Traces.OfType<TOM.Trace>().Where(t => t.Name.StartsWith(traceNamePrefix) && t.Name != currentTraceName)
                .ToList())
            {
                CleanupTrace(trace);
            }
        }

        private void Configure()
        {
            if (server == null) return;
            if (!server.ConnectionInfo.Server.EqualsI("localhost")) return;

            try
            {
                currentTraceName = traceNamePrefix + Guid.NewGuid().ToString("D");
                this.trace = server.Traces.Add(currentTraceName);
                tabularEditorSessionTraces.Add(this.trace);

                TOM.TraceEvent tEvent;
                tEvent = this.trace.Events.Add(AS.TraceEventClass.CommandEnd);
                tEvent.Columns.Add(AS.TraceColumn.EventSubclass);
                tEvent.Columns.Add(AS.TraceColumn.Success);
                tEvent.Columns.Add(AS.TraceColumn.TextData);
                tEvent.Columns.Add(AS.TraceColumn.ApplicationName);
                tEvent.Columns.Add(AS.TraceColumn.DatabaseName);

                tEvent = this.trace.Events.Add(AS.TraceEventClass.ProgressReportCurrent);
                tEvent.Columns.Add(AS.TraceColumn.IntegerData);
                tEvent.Columns.Add(AS.TraceColumn.CurrentTime);
                tEvent.Columns.Add(AS.TraceColumn.ObjectName);
                tEvent.Columns.Add(AS.TraceColumn.ObjectPath);
                tEvent.Columns.Add(AS.TraceColumn.DatabaseName);

                this.trace.OnEvent += Trace_OnEvent;
                this.trace.Update();
                TabularModelHandler.Log("Analysis Services trace configured on localhost");
            }
            catch (Exception ex)
            {
                this.trace = null;
                TabularModelHandler.Log("Exception while configuring Analysis Services trace", ex);
            }
        }

        public void Start()
        {
            try
            {
                if (trace == null)
                    Configure();

                if (trace != null)
                {
                    if (!trace.IsStarted)
                    {
                        trace.Start();
                        TabularModelHandler.Log("Analysis Services trace started");
                    }
                }
            }
            catch (Exception ex)
            {
                TabularModelHandler.Log("Exception while starting Analysis Services trace", ex);
            }
        }

        public bool IsStarted => trace != null && trace.IsStarted;

        public ExternalChangeTrace(TOM.Database database, string applicationName, Action<TOM.TraceEventArgs> onExternalChangeCallback)
        {
            this.applicationName = applicationName;
            this.onExternalChangeCallback = onExternalChangeCallback;
            this.database = database;
            this.databaseNameWhenTraceStarted = database.Name;
        }

        private static readonly AS.TraceEventSubclass[] changeSubClasses =
            new[]
            {
                AS.TraceEventSubclass.Create,
                AS.TraceEventSubclass.TabularCreate,
                AS.TraceEventSubclass.Alter,
                AS.TraceEventSubclass.TabularAlter,
                AS.TraceEventSubclass.Delete,
                AS.TraceEventSubclass.TabularDelete,
                AS.TraceEventSubclass.TabularRename,
                AS.TraceEventSubclass.Batch,
                AS.TraceEventSubclass.CommitTransaction
            };

        private bool IsChangeSubClass(AS.TraceEventSubclass eventSubClass)
        {
            return Array.IndexOf(changeSubClasses, eventSubClass) >= 0;
        }

        private void Trace_OnEvent(object sender, TOM.TraceEventArgs e)
        {
            if (e.Success != AS.TraceEventSuccess.Success) return;
            if (!IsChangeSubClass(e.EventSubclass)) return;
            if (e.DatabaseName != databaseNameWhenTraceStarted) return;
            if (e.ApplicationName == this.applicationName) return;
            if (!(e.EventSubclass == AS.TraceEventSubclass.CommitTransaction || IsChangeXmla(e.TextData))) return;

            this.onExternalChangeCallback(e);
        }
        
        private const string XMLA_ALTER = "Alter";
        private const string XMLA_CREATE = "Create";
        private const string XMLA_DELETE = "Delete";

        private bool IsChangeXmla(string xmla)
        {
            // Returns true if the xmla string contains an XMLA ALTER, CREATE or DELETE statement.
            using (StringReader xmlaStringReader = new StringReader(xmla))
            using (XmlReader xmlaReader = XmlReader.Create(xmlaStringReader))
            {
                while (xmlaReader.Read())
                {
                    if (xmlaReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlaReader.Name.Equals(XMLA_ALTER, StringComparison.OrdinalIgnoreCase)
                            || xmlaReader.Name.Equals(XMLA_CREATE, StringComparison.OrdinalIgnoreCase)
                            || xmlaReader.Name.Equals(XMLA_DELETE, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}

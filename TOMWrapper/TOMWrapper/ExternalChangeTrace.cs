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
    // TODO: This doesn't work. Let's try to use QueryableXEventData instead...
    internal class ExternalChangeTrace
    {
        private TOM.Trace trace;
        private readonly TOM.Server server;
        private readonly string applicationName;
        private readonly Action<TOM.TraceEventArgs> onExternalChangeCallback;

        private static List<TOM.Trace> sessionTraces = new List<TOM.Trace>();
        public static void Cleanup()
        {
            lock (sessionTraces)
            {
                foreach (var trace in sessionTraces)
                {
                    if (trace.IsStarted)
                        trace.Stop();
                    if(trace.Parent != null)
                    {
                        trace.Drop();
                    }
                    trace.Dispose();
                }
                sessionTraces.Clear();
            }
        }

        public void Stop()
        {
            if (trace == null || !trace.IsStarted) return;

            trace.Stop();
        }

        public void Start()
        {
            if (!server.ConnectionInfo.Server.EqualsI("localhost")) return;

            try
            {

                if (trace != null)
                {
                    if (!trace.IsStarted)
                        trace.Start();

                    return;
                }

                this.trace = server.Traces.Add("TabularEditor-" + Guid.NewGuid().ToString("D"));
                sessionTraces.Add(this.trace);

                TOM.TraceEvent tEvent;
                tEvent = this.trace.Events.Add(AS.TraceEventClass.CommandEnd);
                tEvent.Columns.Add(AS.TraceColumn.EventSubclass);
                tEvent.Columns.Add(AS.TraceColumn.Success);
                tEvent.Columns.Add(AS.TraceColumn.TextData);
                tEvent.Columns.Add(AS.TraceColumn.ApplicationName);

                tEvent = this.trace.Events.Add(AS.TraceEventClass.ProgressReportCurrent);
                tEvent.Columns.Add(AS.TraceColumn.IntegerData);
                tEvent.Columns.Add(AS.TraceColumn.CurrentTime);
                tEvent.Columns.Add(AS.TraceColumn.ObjectName);
                tEvent.Columns.Add(AS.TraceColumn.ObjectPath);
                tEvent.Columns.Add(AS.TraceColumn.DatabaseName);

                this.trace.OnEvent += Trace_OnEvent;
                this.trace.Update();
                this.trace.Start();
            }
            catch
            {
                this.trace = null;
            }
        }

        public ExternalChangeTrace(TOM.Server server, string applicationName, Action<TOM.TraceEventArgs> onExternalChangeCallback)
        {
            this.applicationName = applicationName;
            this.onExternalChangeCallback = onExternalChangeCallback;
            this.server = server;

            Start();
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
                AS.TraceEventSubclass.Batch
            };

        private bool IsChangeSubClass(int eventSubClass)
        {
            return Array.IndexOf(changeSubClasses, eventSubClass) >= 0;
        }

        private void Trace_OnEvent(object sender, TOM.TraceEventArgs e)
        {
            if (e.Success != AS.TraceEventSuccess.Success) return;
            if (!changeSubClasses.Contains(e.EventSubclass)) return;
            if (e.ApplicationName == this.applicationName) return;
            if (!IsChangeXmla(e.TextData)) return;

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

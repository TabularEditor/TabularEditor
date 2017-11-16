using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.UI;

namespace TabularEditor.UIServices
{
    public class UsageTelemetry
    {
        public string ServerName { get; set; }
        public string ServerEdition { get; set; }
        public string ServerType { get; set; }
        public string ServerMode { get; set; }
        public string ServerLocation { get; set; }
        public string ServerVersion { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseCompatibilityLevel { get; set; }

        private UIController ui;

        private UsageTelemetry(UIController ui)
        {
            this.ui = ui;
        }

        private void PopulateServerType()
        {
            switch (ui.Handler.SourceType)
            {
                case ModelSourceType.Pbit: ServerType = ".pbit file"; break;
                case ModelSourceType.File: ServerType = ".bim file"; break;
                case ModelSourceType.Folder: ServerType = ".bim exploded"; break;
                case ModelSourceType.Database:
                    switch (ui.LocalInstanceType)
                    {
                        case EmbeddedInstanceType.None: ServerType = "SSAS"; break;
                        case EmbeddedInstanceType.Devenv: ServerType = "SSDT Workspace"; break;
                        case EmbeddedInstanceType.PowerBI: ServerType = "PBI Desktop"; break;
                    }
                    break;
            }
        }

        private void PopulateServerInfo()
        {
            if(ui.Handler.SourceType == ModelSourceType.Database)
            {
                var server = ui.Handler.Database.Server;

                // One-way encryption of server name to avoid sending sensitive data
                ServerName = Crypto.SHA256(server.Name);

                ServerEdition = server.Edition.ToString();
                ServerMode = server.ServerMode.ToString();
                ServerLocation = server.ServerLocation.ToString();
                ServerVersion = server.Version;
            }
        }

        private void PopulateDatabaseInfo()
        {
            string name = ui.Handler.Database.Name;
            if (name == "SemanticModel") name = ui.Handler.Source;

            // One-way encryption of database name to avoid sending sensitive data:
            DatabaseName = Crypto.SHA256(name);

            DatabaseCompatibilityLevel = ui.Handler.Database.CompatibilityLevel.ToString();
        }

        public static UsageTelemetry Collect()
        {
            // Do not collect telemetry data if not allowed according to preferences:
            if (!Preferences.Current.CollectTelemetry) return null;

            var telemetry = new UsageTelemetry(UIController.Current);

            if (UIController.Current.Handler != null)
            {
                telemetry.PopulateServerType();
                telemetry.PopulateServerInfo();
                telemetry.PopulateDatabaseInfo();
            }

            return telemetry;
        }
    }
}

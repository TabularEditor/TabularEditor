using System;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public static class TabularModelHandlerExtension
    {
        public static bool IsPBIDesktop(this TOM.Database database)
        {
            return database.GetServerType() == AsServerType.PBIDesktop;
        }
        public static bool IsAzure(this Microsoft.AnalysisServices.Core.Server server)
        {
            switch (server.GetServerType())
            {
                case AsServerType.AzureAS:
                case AsServerType.PBIService:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetServerTypeName(this TOM.Database database)
        {
            switch (database.GetServerType())
            {
                case AsServerType.PBIDesktop: return "Power BI Desktop";
                case AsServerType.PBIService: return "Power BI Service";
                case AsServerType.PBIReportServer: return "Power BI Report Server";
                case AsServerType.SSAS: return "SQL Server Analysis Services";
                case AsServerType.AzureAS: return "Azure Analysis Services";
                default:
                    return "Not connected";
            }
        }
        public static AsServerType GetServerType(this Microsoft.AnalysisServices.Core.Server server)
        {
            if (server.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI)
            {
                if (server.ServerLocation == Microsoft.AnalysisServices.ServerLocation.OnPremise)
                {
                    if (server.ServerMode == Microsoft.AnalysisServices.ServerMode.SharePoint)
                    {
                        return AsServerType.PBIDesktop;
                    }
                    else
                    {
                        return AsServerType.PBIReportServer;
                    }
                }
                else
                {
                    return AsServerType.PBIService;
                }
            }
            else
            {
                return server.ConnectionString.IndexOf("asazure://", StringComparison.OrdinalIgnoreCase) >= 0
                    || server.ConnectionInfo.Server.EndsWith("/xmla", StringComparison.OrdinalIgnoreCase)
                    || server.ServerLocation == Microsoft.AnalysisServices.ServerLocation.Azure ? AsServerType.AzureAS : AsServerType.SSAS;
            }
        }

        public static AsServerType GetServerType(this TOM.Database database)
        {
            var server = database.Server;
            if (server == null) return AsServerType.NotConnected;

            return server.GetServerType();
        }
    }

    public enum AsServerType
    {
        NotConnected,
        SSAS,
        AzureAS,
        PBIDesktop,
        PBIReportServer,
        PBIService
    }

}
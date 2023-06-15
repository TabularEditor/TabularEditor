using Newtonsoft.Json;
using System.IO;
using System.Linq;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    static class PowerBiCompatibilityModeHelper
    {
        private static readonly int[] analysisServicesStandardCompatLevels = new[]
        {
            1200,
            1400,
            1500,
            1600
        };

        public static bool IsPbiCompatibilityMode(string tomJson)
        {
            // Use PBI CompatibilityMode when model is one of the non-standard CL's, or if V3 metadata is enabled:
            using (var reader = new JsonTextReader(new StringReader(tomJson)))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        switch ((string)reader.Value)
                        {
                            case "compatibilityLevel":
                                reader.Read();
                                if (!analysisServicesStandardCompatLevels.Contains((int)((long)reader.Value))) return true;
                                break;
                            case "defaultPowerBIDataSourceVersion":
                                reader.Read();
                                if ((string)reader.Value == "powerBI_V3") return true;
                                break;
                            default:
                                if (ObjectMetadata.PbiOnlyProperties.Contains((string)reader.Value)) return true;
                                break;
                        }
                    }
                }
            }
            return false;
        }

        public static void DetermineCompatibilityMode(this TOM.Database database)
        {
            if (database.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.Unknown)
            {
                database.CompatibilityMode = IsPbiCompatibilityMode(database) ? Microsoft.AnalysisServices.CompatibilityMode.PowerBI : Microsoft.AnalysisServices.CompatibilityMode.AnalysisServices;
            }
        }

        public static bool IsPbiCompatibilityMode(TOM.Database database)
        {
            if (database.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI) return true;
            if (!analysisServicesStandardCompatLevels.Contains(database.CompatibilityLevel)) return true;
            if (database.Model.DefaultPowerBIDataSourceVersion == TOM.PowerBIDataSourceVersion.PowerBI_V3) return true;
            foreach (var table in database.Model.Tables)
            {
                if (table.Sets != null && table.Sets.Count > 0) return true;
                if (table.Columns.Any(c => c.RelatedColumnDetails != null)) return true;
            }
            return false;
        }
    }
}

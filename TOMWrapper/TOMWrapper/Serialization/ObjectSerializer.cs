using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.Serialization;

internal static class ObjectSerializer
{
    public static T DeserializeObject<T>(string json, Model model) where T : TOM.MetadataObject => TOM.JsonSerializer.DeserializeObject<T>(json, null, model.Database.CompatibilityLevel, (Microsoft.AnalysisServices.CompatibilityMode)model.Database.CompatibilityMode);
}

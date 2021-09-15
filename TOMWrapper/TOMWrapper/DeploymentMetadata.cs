using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TabularEditor.TOMWrapper
{
    public class DeploymentMetadata
    {
        [IntelliSense("Domain and username for the user performing the latest deployment of this model.")]
        public string User { get; set; }
        [IntelliSense("Date and time of the latest deployment for this model.")]
        public DateTime Time { get; set; }
        [IntelliSense("Name of the client machine from which the latest deployment was performed.")]
        public string ClientMachine { get; set; }
        [IntelliSense("Specifies how the deployment was performed.")]
        public DeploymentModeMetadata DeploymentMode {get;set;}
        [IntelliSense("Build number of Tabular Editor used for deployment.")]
        public string TabularEditorBuild { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeploymentModeMetadata
    {
        WizardUI,
        SaveUI,
        CLI
    }
}

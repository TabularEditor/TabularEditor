using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UIServices
{
    public static class DeploymentMetadataHelper
    {

        public static void UpdateDeploymentMetadata(this Model model, DeploymentModeMetadata deploymentMode)
        {
            model.DeploymentMetadata = Preferences.Current.AnnotateDeploymentMetadata ?
                new DeploymentMetadata
                {
                    ClientMachine = Environment.MachineName,
                    DeploymentMode = deploymentMode,
                    TabularEditorBuild = UpdateService.CurrentBuild.ToString(),
                    Time = DateTime.Now,
                    User = Environment.UserDomainName + "\\" + Environment.UserName
                } : null;
            model.UpdateDeploymentMetadata();
        }
    }
}

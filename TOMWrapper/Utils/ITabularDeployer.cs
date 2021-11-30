using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Utils
{

    public interface ITabularDeployer
    {
        DeploymentResult Deploy(Microsoft.AnalysisServices.Tabular.Database db, string targetConnectionString, string targetDatabaseName, DeploymentOptions options, CancellationToken cancellationToken);
        void SaveModelMetadataBackup(string connectionString, string targetDatabaseName, string backupFilePath);
    }
}

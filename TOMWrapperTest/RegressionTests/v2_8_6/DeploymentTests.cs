using System.Linq;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TOMWrapperTest.RegressionTests.v2_8_6
{
    [TestClass]
    public class DeploymentTests
    {
        [TestMethod]
        public void TestNewDeploymentNoErrors()
        {
            var newDbName = "AdventureWorks_UT_New1";
            var server = new TOM.Server();
            server.Connect("localhost");
            if(server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();

            var handler = new TabularModelHandler("AdventureWorks.bim");

            var results = TabularDeployer.Deploy(handler, "localhost", newDbName);

            Assert.AreEqual(0, results.Issues.Count);
            Assert.AreNotEqual(0, results.Unprocessed.Count);
            Assert.AreEqual(0, results.Warnings.Count);

            server.Refresh();
            if (server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();
        }

        [TestMethod]
        public void TestNewDeploymentMeasureError()
        {
            var newDbName = "AdventureWorks_UT_New2";
            var server = new TOM.Server();
            server.Connect("localhost");
            if (server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();

            var handler = new TabularModelHandler("AdventureWorks.bim");
            handler.Model.Tables["Employee"].AddMeasure("ErrorTest", "xxx");

            var results = TabularDeployer.Deploy(handler, "localhost", newDbName);

            Assert.AreNotEqual(0, results.Issues.Count);
            Assert.AreNotEqual(0, results.Unprocessed.Count);
            Assert.AreEqual(0, results.Warnings.Count);

            server.Refresh();
            if (server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();
        }

        [TestMethod]
        public void TestNewDeploymentCalcColumnError()
        {
            var newDbName = "AdventureWorks_UT_New3";
            var server = new TOM.Server();
            server.Connect("localhost");
            if (server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();

            var handler = new TabularModelHandler("AdventureWorks.bim");
            handler.Model.Tables["Employee"].AddCalculatedColumn("ErrorTest", "xxx");

            var results = TabularDeployer.Deploy(handler, "localhost", newDbName);

            Assert.AreNotEqual(0, results.Issues.Count);
            Assert.AreNotEqual(0, results.Unprocessed.Count);
            Assert.AreNotEqual(0, results.Warnings.Count);

            server.Refresh();
            if (server.Databases.ContainsName(newDbName)) server.Databases[newDbName].Drop();
        }

        [TestMethod]
        public void TestExistingDeployment()
        {
            var s = new Server();
            s.Connect("localhost");
            if(s.Databases.ContainsName("AdventureWorks_X"))
            {
                s.Databases["AdventureWorks_X"].Drop();
            }
            s.Disconnect();

            var orgModel = new TabularModelHandler("AdventureWorks2.bim");
            TabularDeployer.Deploy(orgModel, "localhost", "AdventureWorks_X");
            

            var modifiedModel = new TabularModelHandler("AdventureWorks.bim");
            foreach (var p in modifiedModel.Model.Perspectives.ToList()) p.Delete();
            modifiedModel.Model.Tables["Product"].Delete();

            TabularDeployer.Deploy(modifiedModel, "localhost", "AdventureWorks_X");

            TabularDeployer.Deploy(orgModel, "localhost", "AdventureWorks_X");
        }
    }
}

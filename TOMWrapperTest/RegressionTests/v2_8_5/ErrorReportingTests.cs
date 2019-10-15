using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest.RegressionTests.v2_8_5
{
    [TestClass]
    public class ErrorReportingTests
    {
        [TestMethod]
        public void DeploymentErrorMessages()
        {
            var handler = new TabularModelHandler("AdventureWorks.bim");
            var options = new DeploymentOptions { DeployMode = DeploymentMode.CreateOrAlter };
            var result = TabularDeployer.Deploy(handler, "localhost", "AdventureWorks_RegTest_2_8_5", options);

            Assert.AreEqual(0, result.Issues.Count);

            var errorMeasure = handler.Model.Tables[0].AddMeasure("ErrorMeasure", "SYNTAX ERROR");

            result = TabularDeployer.Deploy(handler, "localhost", "AdventureWorks_RegTest_2_8_5", options);

            Assert.AreEqual(1, result.Issues.Count);

            errorMeasure.Delete();
            result = TabularDeployer.Deploy(handler, "localhost", "AdventureWorks_RegTest_2_8_5", options);
            Assert.AreEqual(0, result.Issues.Count);
        }
    }
}

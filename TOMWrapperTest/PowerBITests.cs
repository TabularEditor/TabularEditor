using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.PowerBI;

namespace TOMWrapperTest
{
    [TestClass]
    public class PowerBITests
    {
        [TestMethod]
        public void ReadOnlyMode()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1465.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = true };

            Assert.AreEqual(PowerBIGovernanceMode.ReadOnly, handler.PowerBIGovernance.GovernanceMode);
        }

        [TestMethod]
        public void RestrictedMode()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1520v3.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = true };

            Assert.AreEqual(PowerBIGovernanceMode.V3Restricted, handler.PowerBIGovernance.GovernanceMode);
        }
    }
}

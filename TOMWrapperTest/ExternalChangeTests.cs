using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest
{
    [TestClass]
    public class ExternalChangeTests
    {
        [TestMethod]
        public void UpdateTomTests()
        {
            Ensure1500ModelExists();
            var handler = new TabularModelHandler(Constants.AasServerName, "TomWrapperTest1500");
            var cg1 = handler.Model.AddCalculationGroup("CG1");
            var ci1 = cg1.AddCalculationItem("CI1", "SELECTEDMEASURE()");
            Assert.AreEqual(handler.Model, ci1.Model);

            handler.SaveDB();
            handler.RefreshTom();

            cg1 = handler.Model.CalculationGroups.First();
            ci1 = (handler.Model.Tables["CG1"] as CalculationGroupTable).CalculationItems[0];
            Assert.AreEqual(cg1, ci1.Parent.Table);
            Assert.AreEqual(handler.Model, cg1.Model);
            Assert.AreEqual(handler.Model, ci1.Model);

            var jsonRep = Serializer.SerializeObjects(cg1.CalculationItems);
            var objContainer = Serializer.ParseObjectJsonContainer(jsonRep);
            Assert.IsTrue(objContainer[typeof(CalculationItem)].Length > 0, "JsonContainer empty after serializing calc items (this will prevent CTRL+C operations on calc items)");

            cg1.Delete();
            handler.UndoManager.Undo();
            
            cg1 = handler.Model.CalculationGroups.First();
            ci1 = (handler.Model.Tables["CG1"] as CalculationGroupTable).CalculationItems[0];
            Assert.AreEqual(cg1, ci1.Parent.Table);
            Assert.AreEqual(handler.Model, cg1.Model);
            Assert.AreEqual(handler.Model, ci1.Model);
        }

        private void Ensure1500ModelExists()
        {
            var testModel = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1500);
            TabularDeployer.Deploy(testModel, Constants.AasServerName, "TomWrapperTest1500", new DeploymentOptions { DeployMode = DeploymentMode.CreateOrAlter });
        }
    }
}

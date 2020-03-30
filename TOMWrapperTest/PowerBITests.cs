using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public void ReadOnlyModePBit()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1465.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = true };

            Assert.AreEqual(PowerBIGovernanceMode.ReadOnly, handler.PowerBIGovernance.GovernanceMode);

            // In read-only mode, we shouldn't be able to modify any property at all...
            foreach(var obj in handler.Model.GetChildrenRecursive(true).OfType<TabularObject>())
            {
                // Get public properties:
                var type = obj.GetType();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
                foreach (var prop in props)
                    Assert.IsFalse(obj.Editable(prop), $"'{prop}' is editable on {obj.GetTypeName()} '{obj.GetName()}'");

            }
        }

        [TestMethod]
        public void RestrictedModePbit()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1520v3.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = true };

            var model = handler.Model;

            Assert.AreEqual(PowerBIGovernanceMode.V3Restricted, handler.PowerBIGovernance.GovernanceMode);

            // In restricted mode, some properties can be set while others can't. We don't test everything here,
            // as that is left to the PowerBIGovernanceTests class. Just a few samples of things that should and
            // shouldn't be allowed:

            // All of this should be OK:
            var newMeasure = model.Tables["Customer"].AddMeasure("Added Measure", "123");
            Assert.IsTrue(model.Tables["Customer"].Measures.Contains(newMeasure));
            newMeasure.Expression = "456"; Assert.AreEqual("456", newMeasure.Expression);
            newMeasure.Description = "This is a test measure"; Assert.AreEqual("This is a test measure", newMeasure.Description);
            newMeasure.DisplayFolder = "DF"; Assert.AreEqual("DF", newMeasure.DisplayFolder);
            newMeasure.Name = "Renamed Measure"; Assert.AreEqual("Renamed Measure", newMeasure.Name);
            newMeasure.Delete();
            Assert.IsFalse(model.Tables["Customer"].Measures.Contains(newMeasure));

            // Most things related to columns cannot be changed:
            Assert.ThrowsException<PowerBIGovernanceException>(() => model.Tables["Customer"].AddDataColumn("New Column"));
            var column = model.Tables["Customer"].Columns["AddressLine1"];
            column.Name = "Renamed"; Assert.AreEqual("AddressLine1", column.Name);
            column.DataType = DataType.Int64; Assert.AreEqual(DataType.String, column.DataType);
            Assert.IsFalse(column.Browsable("Annotations"));

            // A couple of properties are allowed on a column, though:
            column.Description = "New desc"; Assert.AreEqual("New desc", column.Description);
            column.DisplayFolder = "New DF"; Assert.AreEqual("New DF", column.DisplayFolder);
            column.IsHidden = true; Assert.IsTrue(column.IsHidden);
        }

        [TestMethod]
        public void UnrestrictedPbit1()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1465.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = false };

            Assert.AreEqual(PowerBIGovernanceMode.Unrestricted, handler.PowerBIGovernance.GovernanceMode);

            // In unrestricted mode, everything should be possible...
            foreach (var obj in handler.Model.GetChildrenRecursive(true).OfType<TabularObject>())
            {
                // Get public properties:
                var type = obj.GetType();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.SetMethod != null).Select(p => p.Name);
                foreach (var prop in props)
                {
                    if (prop == Properties.NAME && obj is SingleColumnRelationship) continue;

                    Assert.IsTrue(obj.Editable(prop), $"'{prop}' not editable on {obj.GetTypeName()} '{obj.GetName()}'");
                }
            }
        }

        [TestMethod]
        public void UnrestrictedPbit2()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1520v3.pbit");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = false };

            Assert.AreEqual(PowerBIGovernanceMode.Unrestricted, handler.PowerBIGovernance.GovernanceMode);
        }

        [TestMethod]
        public void UnrestrictedModeBimFile()
        {
            var handler = new TabularModelHandler("TestData\\AdventureWorks.bim");
            handler.Settings = new TabularModelHandlerSettings { PBIFeaturesOnly = true };

            Assert.AreEqual(PowerBIGovernanceMode.Unrestricted, handler.PowerBIGovernance.GovernanceMode);
        }
    }
}

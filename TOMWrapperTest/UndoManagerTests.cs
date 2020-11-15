using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TOMWrapperTest;

namespace TabularEditor.TOMWrapper.Tests
{
    [TestClass()]
    public class UndoManagerTests
    {
        [TestMethod()]
        public void BatchActionTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");

            Assert.AreEqual(0, handler.UndoManager.BatchDepth);
            Assert.AreEqual(0, handler.UndoManager.UndoSize);

            handler.BeginUpdate("Batch 1");
            handler.BeginUpdate("Batch 1.1");
            handler.BeginUpdate("Batch 1.1.1");

            Assert.AreEqual(3, handler.UndoManager.UndoSize);
            Assert.AreEqual(3, handler.UndoManager.BatchDepth);

            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 1"); // 3 actions (BeginBatch - AddMeasure - EndBatch)
            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 2"); // 3 actions (BeginBatch - AddMeasure - EndBatch)

            Assert.AreEqual(5, handler.UndoManager.UndoSize);
            Assert.AreEqual(3, handler.UndoManager.BatchDepth);

            handler.EndUpdate(); // Batch 1.1.1
            Assert.AreEqual(6, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.EndUpdate(); // Batch 1.1
            Assert.AreEqual(7, handler.UndoManager.UndoSize);
            Assert.AreEqual(1, handler.UndoManager.BatchDepth);

            handler.BeginUpdate("Batch 1.2");
            Assert.AreEqual(8, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 3"); // 4 actions (BeginBatch - AddMeasure - EndBatch)
            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 4"); // 4 actions (BeginBatch - AddMeasure - EndBatch)
            Assert.AreEqual(10, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.EndUpdate(true, true); // Rollback Batch 1.2
            Assert.AreEqual(7, handler.UndoManager.UndoSize);
            Assert.AreEqual(1, handler.UndoManager.BatchDepth);

            handler.EndUpdate(true, true); // Rollback Batch 1
            Assert.AreEqual(0, handler.UndoManager.BatchDepth);
            Assert.AreEqual(0, handler.UndoManager.UndoSize);
        }

        [TestMethod]
        public void UndoRedoCopyPerspectiveTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var serializedCopy = Serializer.SerializeObjects(new[] { model.Perspectives["Test Perspective 1"] });
            var parsedCopy = Serializer.ParseObjectJsonContainer(serializedCopy);
            handler.Actions.InsertObjects(parsedCopy);
            Assert.AreEqual(3, model.Perspectives.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(2, model.Perspectives.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(3, model.Perspectives.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(2, model.Perspectives.Count);

            // Using undo/redo on a copied perspective object causes a TOMInternalException crash on the call to TOM .Clone
            // We use a workaround with serializing to/from JSON instead of calling .Clone, but we keep this test method
            // so we can go back to .Clone in case this issue is fixed later on.

            handler.UndoManager.Redo();
            Assert.AreEqual(3, model.Perspectives.Count);
        }

        [TestMethod]
        public void UndoRedoCopyCultureTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var serializedCopy = Serializer.SerializeObjects(new[] { model.Cultures["da-DK"] });
            var parsedCopy = Serializer.ParseObjectJsonContainer(serializedCopy);
            model.Cultures["da-DK"].Delete();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.Actions.InsertObjects(parsedCopy);
            Assert.AreEqual(2, model.Cultures.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(2, model.Cultures.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(2, model.Cultures.Count);
        }
    }
}

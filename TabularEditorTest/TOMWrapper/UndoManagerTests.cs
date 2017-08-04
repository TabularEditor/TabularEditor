using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Tests
{
    [TestClass()]
    public class UndoManagerTests
    {
        [TestMethod()]
        public void BatchActionTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);

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
    }
}

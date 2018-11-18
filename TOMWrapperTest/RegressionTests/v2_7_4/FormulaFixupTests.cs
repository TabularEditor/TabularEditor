using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class FormulaFixupTests
    {
        [TestMethod]
        public void BatchRenameMeasuresFixupTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddTable();
            var m1 = t1.AddMeasure("M1", "1");
            var m2 = t1.AddMeasure("M2", "2");
            var m3 = t1.AddMeasure("M3", "[M1]+[M2]");

            handler.BeginUpdate("Batch rename");
            m1.Name = "M1X";
            m2.Name = "M2X";
            handler.EndUpdate();

            Assert.AreEqual("[M1X]+[M2X]", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("[M1]+[M2]", m3.Expression);

            handler.BeginUpdate("Batch rename");
            m2.Name = "M2X";
            m1.Name = "M1X";
            handler.EndUpdate();

            Assert.AreEqual("[M1X]+[M2X]", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("[M1]+[M2]", m3.Expression);
        }

        [TestMethod]
        public void BatchRenameColumnsFixupTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddTable("T1");
            var c1 = t1.AddDataColumn("C1");
            var c2 = t1.AddDataColumn("C2");
            var m3 = t1.AddMeasure("M3", "SUM('T1'[C1])+SUM('T1'[C2])");

            handler.BeginUpdate("Batch rename");
            c1.Name = "C1X";
            c2.Name = "C2X";
            handler.EndUpdate();

            Assert.AreEqual("SUM('T1'[C1X])+SUM('T1'[C2X])", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("SUM('T1'[C1])+SUM('T1'[C2])", m3.Expression);

            handler.BeginUpdate("Batch rename");
            c2.Name = "C2X";
            c1.Name = "C1X";
            handler.EndUpdate();

            Assert.AreEqual("SUM('T1'[C1X])+SUM('T1'[C2X])", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("SUM('T1'[C1])+SUM('T1'[C2])", m3.Expression);
        }

        [TestMethod]
        public void BatchRenameTablesFixupTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddTable("T1");
            var t2 = model.AddTable("T2");
            var m3 = t1.AddMeasure("M3", "COUNTROWS(T1)+COUNTROWS(T2)");

            handler.BeginUpdate("Batch rename");
            t1.Name = "T1X";
            t2.Name = "T2X";
            handler.EndUpdate();

            Assert.AreEqual("COUNTROWS('T1X')+COUNTROWS('T2X')", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("COUNTROWS(T1)+COUNTROWS(T2)", m3.Expression);

            handler.BeginUpdate("Batch rename");
            t2.Name = "T2X";
            t1.Name = "T1X";
            handler.EndUpdate();

            Assert.AreEqual("COUNTROWS('T1X')+COUNTROWS('T2X')", m3.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual("COUNTROWS(T1)+COUNTROWS(T2)", m3.Expression);
        }

        [TestMethod]
        public void SuddenReferenceTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddTable("T1");
            var c1 = t1.AddDataColumn("C1");
            var m1 = t1.AddMeasure("M1", "SUM('T1'[C1X])");

            Assert.AreEqual(0, m1.DependsOn.Columns.Count());
            Assert.AreEqual(0, c1.ReferencedBy.Measures.Count());
            c1.Name = "C1X";
            Assert.AreEqual(1, m1.DependsOn.Columns.Count());
            Assert.AreSame(c1, m1.DependsOn.Columns.First());
            Assert.AreEqual(1, c1.ReferencedBy.Measures.Count());
            Assert.AreEqual(m1, c1.ReferencedBy.Measures.First());

            handler.UndoManager.Undo();
            Assert.AreEqual(0, m1.DependsOn.Columns.Count());
            Assert.AreEqual(0, c1.ReferencedBy.Measures.Count());
        }
    }
}

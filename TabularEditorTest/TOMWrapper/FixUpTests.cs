using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TabularEditor.TOMWrapper.Tests
{
    [TestClass]
    public class FixUpTests
    {
        [TestMethod]
        public void TestColumnFixup()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.AutoFixup = true;

            var sw = new Stopwatch();
            sw.Start();

            var m0 = handler.Model.Tables["Date"].AddMeasure("Test 0", "[Test 1]+[Test 2]");

            var m1 = handler.Model.Tables["Date"].AddMeasure("Test 1", "COUNT(Date[Date])");
            var m2 = handler.Model.Tables["Date"].AddMeasure("Test 2", "COUNT('Date'[Date])");
            var m3 = handler.Model.Tables["Date"].AddMeasure("Test 3", "COUNT([Date])");

            var m4 = handler.Model.Tables["Date"].AddMeasure("Test 4", "[Test 1]+[Test 2]");

            handler.UndoManager.SetCheckpoint();

            handler.Model.Tables["Date"].Columns["Date"].Name = "x";

            Assert.AreEqual("COUNT(Date[x])", m1.Expression);
            Assert.AreEqual("COUNT('Date'[x])", m2.Expression);
            Assert.AreEqual("COUNT([x])", m3.Expression);

            handler.Model.Tables["Date"].Name = "y";

            Assert.AreEqual("COUNT(Date[x])", m1.Expression); // Unaffected by table name change, since Date[x] is not a valid column reference ("Date" is a reserved word).
            Assert.AreEqual("COUNT('y'[x])", m2.Expression);
            Assert.AreEqual("COUNT([x])", m3.Expression);

            handler.Model.Tables["y"].Name = "x y";

            Assert.AreEqual("COUNT('x y'[x])", m2.Expression);
            Assert.AreEqual("COUNT([x])", m3.Expression);

            handler.Model.Tables["x y"].Name = "Date";

            Assert.AreEqual("COUNT('Date'[x])", m2.Expression);
            Assert.AreEqual("COUNT([x])", m3.Expression);

            m1.Name = "Test X";
            Assert.AreEqual("[Test X]+[Test 2]", m4.Expression);
            Assert.AreEqual("[Test X]+[Test 2]", m0.Expression);

            handler.UndoManager.Rollback(true);

            Assert.AreEqual("COUNT(Date[Date])", m1.Expression);
            Assert.AreEqual("COUNT('Date'[Date])", m2.Expression);
            Assert.AreEqual("COUNT([Date])", m3.Expression);

            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 200);
            Console.WriteLine("Fixup test completed in {0} ms", sw.ElapsedMilliseconds);
        }
    }
}

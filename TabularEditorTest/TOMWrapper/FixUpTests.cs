using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper.Tests
{
    [TestClass]
    public class FixUpTests
    {
        private void FixUpTest(TabularModelHandler tmh, IDaxDependantObject obj, Utils.DAXProperty property, bool isFullyQualified, Table tableRef, IDaxObject objRef)
        {
            var baseExpression = obj.GetDAX(property);

            if (isFullyQualified)
            {
                Assert.AreEqual(2, obj.DependsOn.Count);

                Assert.AreSame(tableRef, obj.DependsOn[0]);
                var references = obj.DependsOn[tableRef];
                Assert.AreEqual(1, references.Count);
                Assert.AreEqual(property, references[0].property);
                Assert.IsTrue(references[0].fullyQualified);

                Assert.AreSame(objRef, obj.DependsOn[1]);
                references = obj.DependsOn[objRef];
                Assert.AreEqual(1, references.Count);
                Assert.AreEqual(property, references[0].property);
                Assert.IsTrue(references[0].fullyQualified);
            }
            else
            {
                Assert.AreEqual(1, obj.DependsOn.Count);

                Assert.AreSame(objRef, obj.DependsOn[0]);
                var references = obj.DependsOn[objRef];
                Assert.AreEqual(1, references.Count);
                Assert.AreEqual(property, references[0].property);
                Assert.IsTrue(references[0].fullyQualified);
            }

            var expression = baseExpression;

            var oldObjName = objRef.Name;
            objRef.Name = oldObjName + " Renamed";
            var expr1 = expression = expression.Replace(oldObjName, objRef.Name);
            Assert.AreEqual(expression, obj.GetDAX(property));

            var oldTableName = tableRef.Name;
            tableRef.Name = oldTableName + " Renamed";
            var expr2 = expression = expression.Replace(oldTableName, tableRef.Name);
            Assert.AreEqual(expression, obj.GetDAX(property));

            oldObjName = objRef.Name;
            objRef.Name = "b b";
            var expr3 = expression = expression.Replace(oldObjName, objRef.Name);
            Assert.AreEqual(expression, obj.GetDAX(property));

            oldTableName = tableRef.Name;
            tableRef.Name = "a a";
            expression = baseExpression.Replace(oldTableName, tableRef.Name);
            Assert.AreEqual(expression, obj.GetDAX(property));

            tmh.UndoManager.Undo();
            Assert.AreEqual(expr3, obj.GetDAX(property));

            tmh.UndoManager.Undo();
            Assert.AreEqual(expr2, obj.GetDAX(property));

            tmh.UndoManager.Undo();
            Assert.AreEqual(expr1, obj.GetDAX(property));

            tmh.UndoManager.Undo();
            Assert.AreEqual(baseExpression, obj.GetDAX(property));
        }

        [TestMethod]
        public void FixUpUnqualifiedMeasureTest()
        {
            var tmh = ObjectHandlingTests.CreateTestModel();
            var model = tmh.Model;
            var t1 = model.Tables["Test Table 1"];
            var ref1 = t1.Measures["Measure 1"];
            var m1 = t1.AddMeasure("m1", "2*[Measure 1]+1");

            FixUpTest(tmh, m1, DAXProperty.Expression, false, t1, ref1);
        }

        [TestMethod]
        public void FixUpFullyQualifiedColumnTest()
        {
            var tmh = ObjectHandlingTests.CreateTestModel();
            var model = tmh.Model;
            var t1 = model.Tables["Test Table 1"];
            var c1 = t1.Columns["Column 1"];
            var m1 = t1.AddMeasure("m1", "SUM('Test Table 1'[Column 1])");

            FixUpTest(tmh, m1, DAXProperty.Expression, true, t1, c1);
        }


        [TestMethod]
        public void FixupVariousColumnTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            handler.Tree = new MockTree(handler.Model);
            handler.Settings.AutoFixup = true;

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

        [TestMethod]
        public void ImplicitReferenceTest_CL1200()
        {
            var tmh = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1200);
            var model = tmh.Model;

            var t1 = model.Tables["Test Table 1"];
            var m1 = t1.AddMeasure("m1", "COUNTROWS([Column 1])");
            Dax_SingleRefAssert(m1, t1.Columns["Column 1"]);

            var m2 = t1.AddMeasure("m2", "[m1]+[m1]");
            Dax_SingleRefAssert(m2, m1);
        }

        [TestMethod]
        public void FixupExplicitNameTest_CL1400()
        {
            var model = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1400).Model;

            var t1 = model.Tables["Test Table 1"];
            var dax = "DISTINCTCOUNT('Test Table 1'[Column 1])";
            var m1 = t1.AddMeasure("m1", dax);

            t1.Name = "Renamed";
            dax = dax.Replace("'Test Table 1'", "'Renamed'");

            Assert.AreEqual(dax, m1.Expression);
        }

        [TestMethod]
        public void FixupExplicitNameFromInvalidTest_CL1400()
        {
            var model = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1400).Model;

            var t1 = model.Tables["Test Table 1"];
            var m1 = t1.AddMeasure("m1", "DISTINCTCOUNT(Renamed[Column 1])");

            t1.Name = "Renamed";
            t1.Name = "Test Table 1";

            Assert.AreEqual("DISTINCTCOUNT('Test Table 1'[Column 1])", m1.Expression);
        }

        [TestMethod]
        public void FixupTableNameTest_CL1400()
        {
            var tmh = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1400);
            var model = tmh.Model;

            var t1 = model.Tables["Test Table 1"];

            var dax = "COUNTROWS('Test Table 1')";

            var m1 = t1.AddMeasure("m1", dax);
            var m2 = t1.AddMeasure("m2"); m2.DetailRowsExpression = dax;
            var ct1 = model.AddCalculatedTable("ct1", dax);
            var t2 = model.AddTable("t2"); t2.DefaultDetailRowsExpression = dax;
            var c1 = t1.AddCalculatedColumn("c1", dax);
            var k1 = t1.AddMeasure("mk1").AddKPI(); k1.StatusExpression = dax;
            var k2 = t1.AddMeasure("mk2").AddKPI(); k2.TargetExpression = dax;
            var k3 = t1.AddMeasure("mk3").AddKPI(); k3.TrendExpression = dax;

            Assert.AreEqual(dax, m1.Expression);
            Assert.AreEqual(dax, m2.DetailRowsExpression);
            Assert.AreEqual(dax, ct1.Expression);
            Assert.AreEqual(dax, c1.Expression);
            Assert.AreEqual(dax, t2.DefaultDetailRowsExpression);
            Assert.AreEqual(dax, k1.StatusExpression);
            Assert.AreEqual(dax, k2.TargetExpression);
            Assert.AreEqual(dax, k3.TrendExpression);

            t1.Name = "Renamed";
            dax = dax.Replace("'Test Table 1'", "'Renamed'");

            Assert.AreEqual(dax, m1.Expression);
            Assert.AreEqual(dax, m2.DetailRowsExpression);
            Assert.AreEqual(dax, ct1.Expression);
            Assert.AreEqual(dax, c1.Expression);
            Assert.AreEqual(dax, t2.DefaultDetailRowsExpression);
            Assert.AreEqual(dax, k1.StatusExpression);
            Assert.AreEqual(dax, k2.TargetExpression);
            Assert.AreEqual(dax, k3.TrendExpression);
        }

        [TestMethod]
        public void FixupTableNameTest_CL1200()
        {
            var tmh = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1200);
            var model = tmh.Model;

            var t1 = model.Tables["Test Table 1"];

            var dax = "COUNTROWS('Test Table 1')";

            var m1 = t1.AddMeasure("m1", dax);
            var ct1 = model.AddCalculatedTable("ct1", dax);
            var c1 = t1.AddCalculatedColumn("c1", dax);
            var k1 = t1.AddMeasure("mk1").AddKPI(); k1.StatusExpression = dax;
            var k2 = t1.AddMeasure("mk2").AddKPI(); k2.TargetExpression = dax;
            var k3 = t1.AddMeasure("mk3").AddKPI(); k3.TrendExpression = dax;

            Assert.AreEqual(dax, m1.Expression);
            Assert.AreEqual(dax, ct1.Expression);
            Assert.AreEqual(dax, c1.Expression);
            Assert.AreEqual(dax, k1.StatusExpression);
            Assert.AreEqual(dax, k2.TargetExpression);
            Assert.AreEqual(dax, k3.TrendExpression);

            t1.Name = "Renamed";
            dax = dax.Replace("'Test Table 1'", "'Renamed'");

            Assert.AreEqual(dax, m1.Expression);
            Assert.AreEqual(dax, ct1.Expression);
            Assert.AreEqual(dax, c1.Expression);
            Assert.AreEqual(dax, k1.StatusExpression);
            Assert.AreEqual(dax, k2.TargetExpression);
            Assert.AreEqual(dax, k3.TrendExpression);
        }

        [TestMethod]
        public void AddReferenceTest_CL1200()
        {
            var tmh = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1200);
            var model = tmh.Model;

            var t1 = model.Tables["Test Table 1"];

            Dax_SingleRefAssert(t1.AddMeasure("m1", "'Test Table 1'"), t1);
            Dax_SingleRefAssert(t1.AddMeasure("m2", "COUNTROWS('Test Table 1')"), t1);
            Dax_SingleRefAssert(t1.AddCalculatedColumn("cc1", "'Test Table 1'"), t1);
            Dax_SingleRefAssert(t1.AddCalculatedColumn("cc2", "COUNTROWS('Test Table 1')"), t1);

            var c1 = t1.Columns["Column 1"];

            Dax_ExplicitColumnRefAssert(t1.AddMeasure("m1", "'Test Table 1'[Column 1]"), c1);
            Dax_ExplicitColumnRefAssert(t1.AddMeasure("m2", "COUNTROWS('Test Table 1'[Column 1])"), c1);
            Dax_ExplicitColumnRefAssert(t1.AddCalculatedColumn("cc1", "'Test Table 1'[Column 1]"), c1);
            Dax_ExplicitColumnRefAssert(t1.AddCalculatedColumn("cc2", "COUNTROWS('Test Table 1'[Column 1])"), c1);

            var imp = t1.AddMeasure("m1", "[Column 1]");

            var k1 = t1.Measures[0].AddKPI(); k1.StatusExpression = "'Test Table 1'";
            var k2 = t1.Measures[0].AddKPI(); k2.TargetExpression = "'Test Table 1'";
            var k3 = t1.Measures[0].AddKPI(); k3.TrendExpression = "'Test Table 1'";
            Dax_SingleRefAssert(k1, t1);
            Dax_SingleRefAssert(k2, t1);
            Dax_SingleRefAssert(k3, t1);
        }

        private void Dax_SingleRefAssert(IDaxDependantObject dependant, IDaxObject reference)
        {
            Assert.AreEqual(1, dependant.DependsOn.Count);
            Assert.AreSame(reference, dependant.DependsOn[0]);
            Assert.IsTrue(reference.ReferencedBy.Contains(dependant));
        }

        private void Dax_ExplicitColumnRefAssert(IDaxDependantObject dependant, Column reference)
        {
            // If a column is explicitly referenced, we expect to have two references (one for the column, one for the table):
            Assert.AreEqual(2, dependant.DependsOn.Count);
            Assert.IsTrue(dependant.DependsOn.ContainsKey(reference));
            Assert.IsTrue(dependant.DependsOn.ContainsKey(reference.Table));
            Assert.IsTrue(reference.ReferencedBy.Contains(dependant));
        }
    }
}

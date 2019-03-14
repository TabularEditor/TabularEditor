using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    [TestClass]
    public class GetPathTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var handler = new TabularModelHandler(1470);
            var tree = new TabularUITree(handler);
            var model = handler.Model;

            var t1 = model.AddTable("Table 1");
            t1.AddDataColumn("Column 1");
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            cg1.Field.Name = "Field";
            var ci1 = cg1.AddCalculationItem("Item 1");

            tree.Options = LogicalTreeOptions.Default;

            Assert.AreEqual(5, tree.GetPath(ci1).FullPath.Length);
            Assert.IsTrue(
                tree.GetPath(ci1).FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.Field, ci1 }));

            cg1.Field.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(7, tree.GetPath(ci1).FullPath.Length);

        }
    }
}

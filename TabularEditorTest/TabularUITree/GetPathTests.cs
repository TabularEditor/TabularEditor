using System;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    [TestClass]
    public class GetPathTests
    {
        [TestMethod]
        public void TestWithDisplayFolders()
        {
            var handler = new TabularModelHandler(1470);
            var tree = new TabularUITree(handler);
            var model = handler.Model;

            tree.Options = LogicalTreeOptions.Default;

            var t1 = model.AddTable("Table 1");
            t1.AddDataColumn("Column 1");
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            cg1.NameField.Name = "Field";
            var ci1 = cg1.AddCalculationItem("Item 1");

            var cg1Path = tree.GetPath(cg1);
            var ci1Path = tree.GetPath(ci1);

            Assert.AreEqual(5, ci1Path.FullPath.Length);
            Assert.IsTrue(
                ci1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.NameField, ci1 }));

            Assert.AreEqual(1, tree.GetChildren(cg1Path).Count());

            cg1.NameField.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(7, tree.GetPath(ci1).FullPath.Length);

        }

        [TestMethod]
        public void TestWithoutDisplayFolders()
        {
            var handler = new TabularModelHandler(1470);
            var tree = new TabularUITree(handler);
            var model = handler.Model;

            tree.Options = LogicalTreeOptions.Default & ~LogicalTreeOptions.DisplayFolders;

            var t1 = model.AddTable("Table 1");
            t1.AddDataColumn("Column 1");
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            cg1.NameField.Name = "Field";
            var ci1 = cg1.AddCalculationItem("Item 1");

            var cg1Path = tree.GetPath(cg1);
            var ci1Path = tree.GetPath(ci1);

            Assert.AreEqual(5, ci1Path.FullPath.Length);
            Assert.IsTrue(
                ci1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.NameField, ci1 }));

            Assert.AreEqual(1, tree.GetChildren(cg1Path).Count());

            cg1.NameField.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(5, tree.GetPath(ci1).FullPath.Length);

        }
    }
}

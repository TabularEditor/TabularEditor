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
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            var ci1 = cg1.AddCalculationItem("Item 1");
            var cc1 = cg1.AddCalculatedColumn("Column 1");

            var cg1Path = tree.GetPath(cg1);
            var ci1Path = tree.GetPath(ci1);
            var cc1Path = tree.GetPath(cc1);

            Assert.AreEqual(5, ci1Path.FullPath.Length);
            Assert.IsTrue(
                ci1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.CalculationItems, ci1 }));

            Assert.AreEqual(3, tree.GetChildren(cg1Path).Count());
            Assert.IsTrue(tree.GetChildren(cg1Path).OfType<object>().SequenceEqual(new object[] {
                cg1.CalculationItems,
                cg1.Columns["Name"],
                cc1
            }));

            Assert.IsTrue(
                cc1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cc1 }));

            cc1.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(6, tree.GetPath(cc1).FullPath.Length);
        }

        [TestMethod]
        public void TestWithDisplayFoldersShowHidden()
        {
            var handler = new TabularModelHandler(1470);
            var tree = new TabularUITree(handler);
            var model = handler.Model;

            tree.Options = LogicalTreeOptions.Default | LogicalTreeOptions.ShowHidden;

            var t1 = model.AddTable("Table 1");
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            var ci1 = cg1.AddCalculationItem("Item 1");
            var cc1 = cg1.AddCalculatedColumn("Column 1");

            var cg1Path = tree.GetPath(cg1);
            var ci1Path = tree.GetPath(ci1);
            var cc1Path = tree.GetPath(cc1);

            Assert.AreEqual(5, ci1Path.FullPath.Length);
            Assert.IsTrue(
                ci1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.CalculationItems, ci1 }));

            Assert.AreEqual(4, tree.GetChildren(cg1Path).Count());
            Assert.IsTrue(tree.GetChildren(cg1Path).OfType<object>().SequenceEqual(new object[] {
                cg1.CalculationItems,
                cg1.Columns["Name"],
                cg1.Columns["Ordinal"],
                cc1
            }));

            Assert.IsTrue(
                cc1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cc1 }));

            cc1.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(6, tree.GetPath(cc1).FullPath.Length);
        }

        [TestMethod]
        public void TestWithoutDisplayFolders()
        {
            var handler = new TabularModelHandler(1470);
            var tree = new TabularUITree(handler);
            var model = handler.Model;

            tree.Options = LogicalTreeOptions.Default & ~LogicalTreeOptions.DisplayFolders;

            var t1 = model.AddTable("Table 1");
            t1.AddMeasure("Measure 1");

            var cg1 = model.AddCalculationGroup("Calc Group 1");
            var ci1 = cg1.AddCalculationItem("Item 1");
            var cc1 = cg1.AddCalculatedColumn("Column 1");

            var cg1Path = tree.GetPath(cg1);
            var ci1Path = tree.GetPath(ci1);
            var cc1Path = tree.GetPath(cc1);

            Assert.AreEqual(5, ci1Path.FullPath.Length);
            Assert.IsTrue(
                ci1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cg1.CalculationItems, ci1 }));

            Assert.AreEqual(3, tree.GetChildren(cg1Path).Count());
            Assert.IsTrue(tree.GetChildren(cg1Path).OfType<object>().SequenceEqual(new object[] {
                cg1.CalculationItems,
                cg1.Columns["Name"],
                cc1
            }));

            Assert.IsTrue(
                cc1Path.FullPath.SequenceEqual(
                    new object[] { model, model.Groups.Tables, cg1, cc1 }));

            cc1.DisplayFolder = "Folder\\Subfolder";

            Assert.AreEqual(4, tree.GetPath(cc1).FullPath.Length);

        }
    }
}

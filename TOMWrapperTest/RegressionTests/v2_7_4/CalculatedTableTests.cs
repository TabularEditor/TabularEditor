using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class CalculatedTableTests
    {
        [TestMethod]
        public void ColumnsAndDataTypeNotVisibleInTreeTest()
        {
            var handler = new TabularModelHandler("localhost", "TestModel");
            handler.Tree.Options = 0; // Hide everything
            var model = handler.Model;

            var table = model.AddCalculatedTable();
            table.Expression = "ROW(\"Int64\", 1, \"String\", \"ABC\", \"Double\", 1.0)";

            handler.SaveDB();

            var itemsFromTree = handler.Tree.GetChildren(table).ToList();
            Assert.AreEqual(0, itemsFromTree.Count);

            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(DataType.Int64, table.Columns["Int64"].DataType);
            Assert.AreEqual(DataType.String, table.Columns["String"].DataType);
            Assert.AreEqual(DataType.Double, table.Columns["Double"].DataType);

            handler.UndoManager.Rollback();

            handler.SaveDB();
        }

        [TestMethod]
        public void ColumnsAndDataTypeVisibleInTreeTest()
        {
            var handler = new TabularModelHandler("localhost", "TestModel");
            handler.Tree.Options = LogicalTreeOptions.Columns;
            var model = handler.Model;

            var table = model.AddCalculatedTable();
            table.Expression = "ROW(\"Int64\", 1, \"String\", \"ABC\", \"Double\", 1.0)";

            handler.SaveDB();

            var itemsFromTree = handler.Tree.GetChildren(table).ToList();
            Assert.AreEqual(3, itemsFromTree.Count);

            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(DataType.Int64, table.Columns["Int64"].DataType);
            Assert.AreEqual(DataType.String, table.Columns["String"].DataType);
            Assert.AreEqual(DataType.Double, table.Columns["Double"].DataType);

            handler.UndoManager.Rollback();

            handler.SaveDB();
        }

        [TestMethod]
        public void ColumnsAndDataTypeVisibleInTreeWithDisplayFoldersTest()
        {
            var handler = new TabularModelHandler("localhost", "TestModel");
            handler.Tree.Options = LogicalTreeOptions.Columns | LogicalTreeOptions.DisplayFolders;
            var model = handler.Model;

            var table = model.AddCalculatedTable();
            table.Expression = "ROW(\"Int64\", 1, \"String\", \"ABC\", \"Double\", 1.0)";

            handler.SaveDB();

            var itemsFromTree = handler.Tree.GetChildren(table).ToList();
            Assert.AreEqual(3, itemsFromTree.Count);

            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(DataType.Int64, table.Columns["Int64"].DataType);
            Assert.AreEqual(DataType.String, table.Columns["String"].DataType);
            Assert.AreEqual(DataType.Double, table.Columns["Double"].DataType);

            handler.UndoManager.Rollback();

            handler.SaveDB();
        }

        [TestMethod]
        public void ColumnsAndDataTypeVisibleInTreeAlphabeticalTest()
        {
            var handler = new TabularModelHandler("localhost", "TestModel");
            handler.Tree.Options = LogicalTreeOptions.Columns | LogicalTreeOptions.DisplayFolders | LogicalTreeOptions.OrderByName;
            var model = handler.Model;

            var table = model.AddCalculatedTable();
            table.Expression = "ROW(\"Int64\", 1, \"String\", \"ABC\", \"Double\", 1.0)";

            handler.SaveDB();

            var itemsFromTree = handler.Tree.GetChildren(table).ToList();
            Assert.AreEqual(3, itemsFromTree.Count);
            Assert.AreEqual("Double", itemsFromTree[0].Name);
            Assert.AreEqual("Int64", itemsFromTree[1].Name);
            Assert.AreEqual("String", itemsFromTree[2].Name);

            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(DataType.Int64, table.Columns["Int64"].DataType);
            Assert.AreEqual(DataType.String, table.Columns["String"].DataType);
            Assert.AreEqual(DataType.Double, table.Columns["Double"].DataType);

            handler.UndoManager.Rollback();

            handler.SaveDB();
        }
    }
}

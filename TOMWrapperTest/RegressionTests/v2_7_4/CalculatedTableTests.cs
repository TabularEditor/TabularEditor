using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class CalculatedTableTests
    {
        [TestMethod]
        public void ColumnsAndDataTypeTest()
        {
            var handler = new TabularModelHandler("localhost", "TestModel");
            var model = handler.Model;

            var table = model.AddCalculatedTable();
            table.Expression = "ROW(\"Int64\", 1, \"String\", \"ABC\", \"Double\", 1.0)";

            handler.SaveDB();

            Assert.AreEqual(3, table.Columns.Count);
            Assert.AreEqual(DataType.Int64, table.Columns["Int64"].DataType);
            Assert.AreEqual(DataType.String, table.Columns["String"].DataType);
            Assert.AreEqual(DataType.Double, table.Columns["Double"].DataType);

            handler.UndoManager.Rollback();

            handler.SaveDB();
        }
    }
}

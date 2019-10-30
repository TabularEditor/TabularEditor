using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest.RegressionTests.v2_8_6
{
    [TestClass]
    public class FormulaFixupTests
    {
        const string orgDax = "SUM([Gross Profit]) +\r\n SUM('Reseller Sales'[Gross Profit])";
        const string orgDaxNoCr = "SUM([Gross Profit]) +\n SUM('Reseller Sales'[Gross Profit])";


        [TestMethod]
        public void TestNewDeploymentNoErrors()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var table = model.Tables["Reseller Sales"];

            var column = table.Columns["Gross Profit"] as CalculatedColumn;
            var measure = table.Measures["Reseller Total Gross Profit"];

            measure.Expression = orgDax;
            column.Name = "GP";

            Assert.AreEqual("SUM([GP]) +\n SUM('Reseller Sales'[GP])", measure.Expression);
            handler.UndoManager.Undo();
            Assert.AreEqual(orgDaxNoCr, measure.Expression);
            handler.SaveDB();

            column.Name = "Gross Profit XXX";
            Assert.AreEqual("SUM([Gross Profit XXX]) +\n SUM('Reseller Sales'[Gross Profit XXX])", measure.Expression);
            handler.SaveDB();

            column.Name = "GP";
            Assert.AreEqual("SUM([GP]) +\n SUM('Reseller Sales'[GP])", measure.Expression);
            handler.SaveDB();

            column.Name = "Gross Profit";
            Assert.AreEqual(orgDaxNoCr, measure.Expression);
            handler.SaveDB();
        }

    }
}

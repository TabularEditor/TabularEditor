using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using System.Linq;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class DependenciesTests
    {
        /// <summary>
        /// https://github.com/otykier/TabularEditor/issues/208
        /// </summary>
        [TestMethod]
        public void DependencyWithUnderscoreInTableName()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("New_Table");
            var c1 = t1.AddCalculatedColumn("Column1", "123");
            var m1 = t1.AddMeasure("Measure1", "SUM(New_Table[Column1])");

            Assert.AreEqual(1, c1.ReferencedBy.Measures.Count());
            Assert.AreEqual(m1, c1.ReferencedBy.Measures.First());

            Assert.AreEqual(1, m1.DependsOn.Columns.Count());
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_8_7
{
    [TestClass]
    public class ClonePartitionsTests
    {
        [TestMethod]
        public void TestCloneMPartition()
        {
            // https://github.com/otykier/TabularEditor/issues/357 
            var handler = new TabularModelHandler(1500);
            var model = handler.Model;
            var ds1 = model.AddDataSource();
            var ds2 = model.AddStructuredDataSource();
            var t1 = model.AddTable();
            var p1 = t1.AddMPartition("Test", "Some M expression");

            var p2 = p1.Clone();

            Assert.IsInstanceOfType(p2, typeof(MPartition));
            Assert.AreEqual("Some M expression", (p2 as MPartition).Expression);
            p2.Delete();
        }
    }
}

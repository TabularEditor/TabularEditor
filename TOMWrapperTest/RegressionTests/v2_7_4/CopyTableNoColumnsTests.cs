using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using System.Linq;
using System.IO;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class CopyTableNoColumnsTests
    {

        /// <summary>
        /// https://github.com/otykier/TabularEditor/issues/202
        /// </summary>
        [TestMethod]
        public void CopyTable()
        {
            var handler = new TabularModelHandler();
            handler.Tree.Options |= LogicalTreeOptions.DisplayFolders;
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("NewTable");
            var m1 = t1.AddMeasure("Measure1", "123");
            var c1 = t1.AddCalculatedColumn("Column1", "123");
            var c2 = t1.AddCalculatedColumn("Column2", "123");

            var json = Serializer.SerializeObjects(Enumerable.Repeat(t1, 1));
            var ojc = Serializer.ParseObjectJsonContainer(json);
            handler.Actions.InsertObjects(ojc);

            Assert.AreEqual(2, model.Tables.Count);
            var t2 = model.Tables["NewTable 1"];

            Assert.AreEqual(3, t2.GetChildrenByFolders().Count());
        }
    }
}

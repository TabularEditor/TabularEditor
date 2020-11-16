using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TabularEditor.TOMWrapper
{
    [TestClass]
    public class ObjectCreationTests
    {
        [TestMethod]
        public void CreateTableTest()
        {
            const string tableName = "My Table";

            var tm = new TabularModelHandler();
            var t = tm.Model.AddTable(tableName);

            // Check that table was created correctly:
            Assert.AreEqual(tableName, t.Name);
            Assert.AreEqual(tm.Model, t.Parent);

            // New data source should have been created:
            Assert.AreEqual(1, tm.Model.DataSources.Count);
            Assert.AreEqual(DataSourceType.Provider, tm.Model.DataSources[0].Type);

            // Table partition should have been created:
            Assert.AreEqual(1, t.Partitions.Count);
            Assert.AreEqual("My Table", t.Partitions[0].Name);
            Assert.AreEqual(PartitionSourceType.Query, t.Partitions[0].SourceType);
            Assert.AreEqual(tm.Model.DataSources[0], t.Partitions[0].DataSource);
        }

        [TestMethod]
        public void CreateCalculatedTableTest()
        {
            const string tableName = "My CalcTable";
            const string expression = "ROW(\"Test\", 1)";

            var tm = new TabularModelHandler();
            var t = tm.Model.AddCalculatedTable(tableName, expression);

            // Check that table was created correctly:
            Assert.AreEqual(tableName, t.Name);
            Assert.AreEqual(expression, t.Expression);
            Assert.AreEqual(tm.Model, t.Parent);

            // New data source should not have been created:
            Assert.AreEqual(0, tm.Model.DataSources.Count);

            // Table partition should have been created:
            Assert.AreEqual(1, t.Partitions.Count);
            Assert.AreEqual(tableName, t.Partitions[0].Name);
            Assert.AreEqual(PartitionSourceType.Calculated, t.Partitions[0].SourceType);
            Assert.AreEqual(expression, t.Partitions[0].Expression);
        }
        
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TOMWrapperTest;

namespace TabularEditor.TOMWrapper.Tests
{
    [TestClass()]
    public class UndoManagerTests
    {
        [TestMethod()]
        public void BatchActionTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");

            Assert.AreEqual(0, handler.UndoManager.BatchDepth);
            Assert.AreEqual(0, handler.UndoManager.UndoSize);

            handler.BeginUpdate("Batch 1");
            handler.BeginUpdate("Batch 1.1");
            handler.BeginUpdate("Batch 1.1.1");

            Assert.AreEqual(3, handler.UndoManager.UndoSize);
            Assert.AreEqual(3, handler.UndoManager.BatchDepth);

            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 1"); // 3 actions (BeginBatch - AddMeasure - EndBatch)
            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 2"); // 3 actions (BeginBatch - AddMeasure - EndBatch)

            Assert.AreEqual(5, handler.UndoManager.UndoSize);
            Assert.AreEqual(3, handler.UndoManager.BatchDepth);

            handler.EndUpdate(); // Batch 1.1.1
            Assert.AreEqual(6, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.EndUpdate(); // Batch 1.1
            Assert.AreEqual(7, handler.UndoManager.UndoSize);
            Assert.AreEqual(1, handler.UndoManager.BatchDepth);

            handler.BeginUpdate("Batch 1.2");
            Assert.AreEqual(8, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 3"); // 4 actions (BeginBatch - AddMeasure - EndBatch)
            handler.Model.Tables["Reseller Sales"].AddMeasure("Test 4"); // 4 actions (BeginBatch - AddMeasure - EndBatch)
            Assert.AreEqual(10, handler.UndoManager.UndoSize);
            Assert.AreEqual(2, handler.UndoManager.BatchDepth);

            handler.EndUpdate(true, true); // Rollback Batch 1.2
            Assert.AreEqual(7, handler.UndoManager.UndoSize);
            Assert.AreEqual(1, handler.UndoManager.BatchDepth);

            handler.EndUpdate(true, true); // Rollback Batch 1
            Assert.AreEqual(0, handler.UndoManager.BatchDepth);
            Assert.AreEqual(0, handler.UndoManager.UndoSize);
        }

        [TestMethod]
        public void UndoRedoCopyPerspectiveTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var serializedCopy = Serializer.SerializeObjects(new[] { model.Perspectives["Test Perspective 1"] });
            var parsedCopy = Serializer.ParseObjectJsonContainer(serializedCopy);
            handler.Actions.InsertObjects(parsedCopy);
            Assert.AreEqual(3, model.Perspectives.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(2, model.Perspectives.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(3, model.Perspectives.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(2, model.Perspectives.Count);

            // Using undo/redo on a copied perspective object causes a TOMInternalException crash on the call to TOM .Clone
            // We use a workaround with serializing to/from JSON instead of calling .Clone, but we keep this test method
            // so we can go back to .Clone in case this issue is fixed later on.

            handler.UndoManager.Redo();
            Assert.AreEqual(3, model.Perspectives.Count);
        }

        [TestMethod]
        public void UndoRedoCopyCultureTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var serializedCopy = Serializer.SerializeObjects(new[] { model.Cultures["da-DK"] });
            var parsedCopy = Serializer.ParseObjectJsonContainer(serializedCopy);
            model.Cultures["da-DK"].Delete();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.Actions.InsertObjects(parsedCopy);
            Assert.AreEqual(2, model.Cultures.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(2, model.Cultures.Count);

            handler.UndoManager.Undo();
            Assert.AreEqual(1, model.Cultures.Count);

            handler.UndoManager.Redo();
            Assert.AreEqual(2, model.Cultures.Count);
        }

        [TestMethod]
        public void UndoDeleteColumnUsedAsLevelTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var t1 = model.Tables["Test Table 1"];
            var h1 = t1.Hierarchies["Hierarchy 1"];
            Assert.AreEqual(4, h1.Levels.Count);
            Assert.AreEqual(t1.Columns["Column 1"], h1.Levels["Level 1"].Column);

            t1.Columns["Column 1"].Delete();
            Assert.AreEqual(3, h1.Levels.Count);

            handler.UndoManager.Undo();

            Assert.AreEqual(4, h1.Levels.Count);
            Assert.AreEqual(t1.Columns["Column 1"], h1.Levels["Level 1"].Column);
        }

        [TestMethod]
        public void UndoDeleteSortByColumnTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var t1 = model.Tables["Test Table 1"];
            t1.Columns["Column 2"].SortByColumn = t1.Columns["Column 1"];

            t1.Columns["Column 2"].Delete();
            t1.Columns["Column 1"].Delete();

            handler.UndoManager.Undo();
            handler.UndoManager.Undo();
            
            Assert.AreEqual(t1.Columns["Column 1"], t1.Columns["Column 2"].SortByColumn);
        }

        [TestMethod]
        public void RecreateFromMetadataPerformanceTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel();
            var model = handler.Model;

            var t1 = model.Tables["Test Table 1"];

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                t1.Columns["Column 1"].Delete();
                handler.UndoManager.Undo();
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void TestUndoStackDepth()
        {
            var handler = ObjectHandlingTests.CreateTestModel(fileName: null, compatibilityLevel: 1200, enableUndo: false);
            var model = handler.Model;

            var t1 = model.AddTable("T1");
            var c1 = t1.AddDataColumn("c1");
            var c2 = t1.AddDataColumn("c2");
            var c3 = t1.AddDataColumn("c3");
            c1.SortByColumn = c2;
            var cc1 = t1.AddCalculatedColumn("cc1");
            t1.AddHierarchy("h1", null, c1, c2, c3);
            var m1 = t1.AddMeasure("m1");
            m1.AddKPI();

            handler.UndoManager.Enabled = true;

            t1.Delete();
            Assert.AreEqual(1, handler.UndoManager.UndoSize);

            handler.UndoManager.Undo();
            handler.UndoManager.Redo();
            handler.UndoManager.Undo();

            var json = @"{
  ""name"": ""T1"",
  ""columns"": [
    {
      ""name"": ""c1"",
      ""dataType"": ""string"",
      ""sortByColumn"": ""c2""
    },
    {
      ""name"": ""c2"",
      ""dataType"": ""string""
    },
    {
      ""name"": ""c3"",
      ""dataType"": ""string""
    },
    {
      ""type"": ""calculated"",
      ""name"": ""cc1"",
      ""dataType"": ""unknown"",
      ""isDataTypeInferred"": true
    }
  ],
  ""partitions"": [
    {
      ""name"": ""T1"",
      ""source"": {
        ""type"": ""query"",
        ""dataSource"": ""Test Datasource""
      }
    }
  ],
  ""measures"": [
    {
      ""name"": ""m1"",
      ""kpi"": {}
    }
  ],
  ""hierarchies"": [
    {
      ""name"": ""h1"",
      ""levels"": [
        {
          ""name"": ""c1"",
          ""ordinal"": 0,
          ""column"": ""c1""
        },
        {
          ""name"": ""c2"",
          ""ordinal"": 1,
          ""column"": ""c2""
        },
        {
          ""name"": ""c3"",
          ""ordinal"": 2,
          ""column"": ""c3""
        }
      ]
    }
  ]
}";
            Assert.AreEqual(json, Microsoft.AnalysisServices.Tabular.JsonSerializer.SerializeObject(t1.MetadataObject));
        }
    }
}

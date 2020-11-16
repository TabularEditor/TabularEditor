using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Serialization;
using TOMWrapperTest;

namespace TabularEditor.TOMWrapper
{
    [TestClass]
    public class ObjectHandlingTests
    {
        const int CompatibilityLevel = 1200;
        const string TestFileName = "testmodel1200.bim";
        const string TestDBName = "TestModel";
        /// <summary>
        /// Creates a simple model for unit testing.
        /// </summary>
        /// <remarks>
        /// The model will contain:
        ///   - 1 data source: "Test Datasource"
        ///   - 2 roles: "Test Role 1", "Test Role 2"
        ///   - 2 cultures: "da-DK", "en-US" (all objects translated in these)
        ///   - 2 perspectives: "Test Perspective 1", "Test Perspective 2"
        ///   - 2 tables: "Test Table 1", "Test Table 2"
        ///   - 1 calculated table: "Test CalcTable 1"
        ///   - "Test Table 1" contains:
        ///         - 3 columns, 1 calculated column
        ///         - 1 hierarchy (4 levels - one per column)
        ///         - 2 measures, 1 with kpi
        ///   - "Test Table 2" contains:
        ///         - 3 columns, 1 calculated column
        ///         - 1 hierarchy
        ///   - 1 relationship
        /// </remarks>
        /// <param name="fileName">If specified, the model will be saved to this file.</param>
        /// <param name="compatibilityLevel">Specify the compatibility level of the created model.</param>
        public static TabularModelHandler CreateTestModel(string fileName = null, int compatibilityLevel = 1200, bool enableUndo = true)
        {
            var tm = new TabularModelHandler(compatibilityLevel);
            tm.UndoManager.Enabled = enableUndo;

            var ds = tm.Model.AddDataSource("Test Datasource");
            ds.ConnectionString = "Provider=MSOLEDBSQL;Data Source=localhost;Initial Catalog=db";
            var r1 = tm.Model.AddRole("Test Role 1");
            var r2 = tm.Model.AddRole("Test Role 2");
            var c1 = tm.Model.AddTranslation("da-DK");
            var c2 = tm.Model.AddTranslation("en-US");
            var p1 = tm.Model.AddPerspective("Test Perspective 1");
            var p2 = tm.Model.AddPerspective("Test Perspective 2");

            var t1 = tm.Model.AddTable("Test Table 1");
            var t2 = tm.Model.AddTable("Test Table 2");

            // Create calculated table based on table 2:
            var t3 = tm.Model.AddCalculatedTable("Test CalcTable 1", @"DATATABLE (
    ""String Column"", STRING,
    ""Decimal Column"", CURRENCY,
    ""Int Column"", INTEGER,
    ""Date Column"", DATETIME,
    ""Boolean Column"", BOOLEAN,
    ""Double Column"", DOUBLE,
    {
        { ""Category A"", -50.1234, -5, ""2017-01-01"", TRUE, 5.134E17 }, 
        { ""Category B"", 3.1415, 2, ""2017-01-02"", FALSE, 9.761E-12 }, 
        { ""Category C"", 45.9876, 8, ""2017-01-03"", TRUE, 1.654 },
        { """", 0, 0, 0, FALSE, 0},
        { , , , , , }
    }
)");

            var c11 = t1.AddDataColumn("Column 1");
            var c12 = t1.AddDataColumn("Column 2");
            var c13 = t1.AddDataColumn("Column 3");
            var c14 = t1.AddCalculatedColumn("Column 4", "[Column 1]");
            var h1 = t1.AddHierarchy("Hierarchy 1", null, c11, c12, c13, c14);
            h1.Levels[0].Name = "Level 1";
            h1.Levels[1].Name = "Level 2";
            h1.Levels[2].Name = "Level 3";
            h1.Levels[3].Name = "Level 4";

            var c21 = t2.AddDataColumn("Column 1");
            var c22 = t2.AddDataColumn("Column 2");
            var c23 = t2.AddDataColumn("Column 3");
            var c24 = t2.AddCalculatedColumn("Column 4", "[Column 1]");
            var h2 = t2.AddHierarchy("Hierarchy 1", null, c21, c22, c23, c24);

            var r = tm.Model.AddRelationship();
            r.FromColumn = c11;
            r.ToColumn = c21;

            var m1 = t1.AddMeasure("Measure 1", "sum('Test CalcTable 1'[Decimal Column])");
            var m2 = t1.AddMeasure("Measure 2", "sum('Test CalcTable 1'[Int Column])");

            var k1 = m1.AddKPI();

            // Apply translations to all items in table 1:
            var items = t1.GetChildren().Concat(t1.AllLevels).Concat(Enumerable.Repeat(t1, 1));
            foreach (var item in items.OfType<ITranslatableObject>())
            {
                item.TranslatedNames["da-DK"] = item.Name + " DK";
                item.TranslatedNames["en-US"] = item.Name + " US";
            }

            // Include all items in table 1 in perspective:
            foreach(var item in t1.GetChildren().OfType<ITabularPerspectiveObject>()) item.InPerspective.All();

            if(!string.IsNullOrEmpty(fileName)) tm.Save(fileName, SaveFormat.ModelSchemaOnly, SerializeOptions.Default);

            return tm;
        }

        [TestMethod]
        public void HierarchyTest()
        {
            var handler = CreateTestModel();
            var t1 = handler.Model.Tables["Test Table 1"];
            var c = t1.AddDataColumn("NewLevelCol");

            var l = t1.Hierarchies["Hierarchy 1"].AddLevel(c);
            handler.UndoManager.Undo();
            Assert.IsTrue(l.IsRemoved);

            handler.UndoManager.Redo();
            Assert.IsFalse(l.IsRemoved);
            Assert.AreEqual(c, l.Column);
        }

        [TestMethod]
        public void HierarchyTest2()
        {
            var handler = CreateTestModel();
            var t1 = handler.Model.Tables["Test Table 1"];
            var c = t1.AddDataColumn("NewLevelCol");

            var l = t1.Hierarchies["Hierarchy 1"].AddLevel(c);
            handler.UndoManager.Undo();
            Assert.IsTrue(l.IsRemoved);
            handler.UndoManager.Undo();
            Assert.IsTrue(c.IsRemoved);

            handler.UndoManager.Redo();
            Assert.IsFalse(c.IsRemoved);
            handler.UndoManager.Redo();
            Assert.IsFalse(l.IsRemoved);
            Assert.AreEqual(c, l.Column);
        }

        public TabularModelHandler ResetAndConnect()
        {
            CreateTestModel(TestFileName);
            var tm = new TabularModelHandler(TestFileName);
            TabularDeployer.Deploy(tm, Constants.ServerName, TestDBName, DeploymentOptions.Full);

            return new TabularModelHandler(Constants.ServerName, TestDBName);
        }

        [TestMethod]
        public void ResetTest()
        {
            var tm = ResetAndConnect();
            tm.SaveDB();
        }

        [TestMethod]
        public void DeleteTableTest()
        {
            var tm = ResetAndConnect();

            tm.Model.Tables["Test Table 1"].Delete();
            tm.SaveDB();

            tm.UndoManager.Undo(); // Undo delete
            tm.SaveDB();
        }
    }
}

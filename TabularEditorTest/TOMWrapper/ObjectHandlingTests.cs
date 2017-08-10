using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Linq;

namespace TabularEditor.TOMWrapper
{
    [TestClass]
    public class ObjectHandlingTests
    {
        const string ServerName = @"localhost\Tabular2017";
        const string TestDBName = "testdb";
        const string TestFileName = "testmodel.bim";

        /// <summary>
        /// Creates a simple model with a few tables, measures, hierarchies, relationships, cultures and perspectives.
        /// </summary>
        /// <param name="fileName"></param>
        public void CreateTestModel(string fileName)
        {
            var tm = new TabularModelHandler(1400);

            var ds = tm.Model.AddDataSource("Test Datasource");
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
    {
        { ""Category A"", -50.1234, -5, ""2017-01-01"" }, 
        { ""Category B"", 3.1415,    2, ""2017-01-02"" }, 
        { ""Category C"", 45.9876,   8, ""2017-01-03"" }
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

            tm.SaveFile(fileName, SerializeOptions.Default);
        }

        public TabularModelHandler ResetAndConnect()
        {
            CreateTestModel(TestFileName);
            var tm = new TabularModelHandler(TestFileName);
            TabularDeployer.Deploy(tm.Database, ServerName, TestDBName, DeploymentOptions.Full);

            return new TabularModelHandler(ServerName, TestDBName);
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

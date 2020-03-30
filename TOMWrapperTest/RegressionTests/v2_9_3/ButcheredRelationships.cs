using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TOMWrapperTest.RegressionTests.v2_9_3
{
    [TestClass]
    public class ButcheredRelationships
    {
        [TestMethod]
        public void RenameTableTestWorks()
        {
            if(Directory.Exists("TestData\\DemoRenameIssue")) Directory.Delete("TestData\\DemoRenameIssue", true);
            ZipFile.ExtractToDirectory("TestData\\DemoRenameIssue.zip", "TestData");
            var handler = new TabularModelHandler("TestData\\DemoRenameIssue");
            var model = handler.Model;

            var t1 = model.Tables["Blah"];
            var t2 = model.Tables["Blah2"];
            var c1 = t1.Columns["BlahKey"];
            var c2 = t2.Columns["BlahKey"];

            // Accessing the relationships before renaming the table prevents us from reproducing the bug:
            Assert.AreEqual(1, t1.UsedInRelationships.Count());

            // Rename table:
            t1.Name = "Blah A";

            // Test relationship:
            Assert.AreEqual(1, t1.UsedInRelationships.Count());
        }

        [TestMethod]
        public void RenameTableTestFails()
        {
            if (Directory.Exists("TestData\\DemoRenameIssue")) Directory.Delete("TestData\\DemoRenameIssue", true);
            ZipFile.ExtractToDirectory("TestData\\DemoRenameIssue.zip", "TestData");
            var handler = new TabularModelHandler("TestData\\DemoRenameIssue");
            var model = handler.Model;

            var t1 = model.Tables["Blah"];
            var t2 = model.Tables["Blah2"];
            var c1 = t1.Columns["BlahKey"];
            var c2 = t2.Columns["BlahKey"];

            // Here, we deliberately rename the table before accessing the relationships. This causes the test to fail in 2.9.3.

            // Rename table:
            t1.Name = "Blah A";

            // Test relationship:
            Assert.AreEqual(1, t1.UsedInRelationships.Count());
        }
    }
}

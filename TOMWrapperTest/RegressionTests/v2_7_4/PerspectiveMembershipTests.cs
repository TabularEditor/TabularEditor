using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    /// <summary>
    /// https://github.com/otykier/TabularEditor/issues/215
    /// </summary>
    [TestClass]
    public class PerspectiveMembershipTests
    {
        [TestMethod]
        public void CheckPerspectiveMembershipAfterSave_Positive()
        {
            {
                var handler = new TabularModelHandler();
                var model = handler.Model;
                Table t1 = model.AddCalculatedTable("Table1");
                Column c1 = t1.AddCalculatedColumn("Column1");
                Column c2 = t1.AddCalculatedColumn("Column2");
                var p1 = model.AddPerspective("TestPerspective");

                c1.InPerspective[p1] = true;
                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);

                Directory.CreateDirectory("test_2_7_4_perspective_membership_3");
                handler.Save("test_2_7_4_perspective_membership_3", SaveFormat.TabularEditorFolder, SerializeOptions.Default);
            }

            {
                // Load from folder:
                var handler = new TabularModelHandler("test_2_7_4_perspective_membership_3");
                var model = handler.Model;
                var p1 = model.Perspectives["TestPerspective"];
                var t1 = model.Tables["Table1"];
                var c1 = t1.Columns["Column1"];
                var c2 = t1.Columns["Column2"];

                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);
            }
        }


        [TestMethod]
        public void CheckPerspectiveMembershipAfterSave_Negative()
        {
            {
                var handler = new TabularModelHandler();
                var model = handler.Model;
                Table t1 = model.AddCalculatedTable("Table1");
                Column c1 = t1.AddCalculatedColumn("Column1");
                Column c2 = t1.AddCalculatedColumn("Column2");
                var p1 = model.AddPerspective("TestPerspective");

                t1.InPerspective[p1] = true;
                c2.InPerspective[p1] = false;
                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);

                Directory.CreateDirectory("test_2_7_4_perspective_membership_4");
                handler.Save("test_2_7_4_perspective_membership_4", SaveFormat.TabularEditorFolder, SerializeOptions.Default);
            }

            {
                // Load from folder:
                var handler = new TabularModelHandler("test_2_7_4_perspective_membership_4");
                var model = handler.Model;
                var p1 = model.Perspectives["TestPerspective"];
                var t1 = model.Tables["Table1"];
                var c1 = t1.Columns["Column1"];
                var c2 = t1.Columns["Column2"];

                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);
            }
        }

        [TestMethod]
        public void CheckPerspectiveMembershipAfterSerialization_Positive()
        {
            {
                var handler = new TabularModelHandler();
                var model = handler.Model;
                Table t1 = model.AddCalculatedTable("Table1");
                Column c1 = t1.AddCalculatedColumn("Column1");
                Column c2 = t1.AddCalculatedColumn("Column2");
                var p1 = model.AddPerspective("TestPerspective");

                c1.InPerspective[p1] = true;
                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);

                Directory.CreateDirectory("test_2_7_4_perspective_membership_1");
                handler.Save("test_2_7_4_perspective_membership_1", SaveFormat.TabularEditorFolder, new SerializeOptions { LocalPerspectives = true });
            }

            {
                // Load from folder:
                var handler = new TabularModelHandler("test_2_7_4_perspective_membership_1");
                var model = handler.Model;
                var p1 = model.Perspectives["TestPerspective"];
                var t1 = model.Tables["Table1"];
                var c1 = t1.Columns["Column1"];
                var c2 = t1.Columns["Column2"];

                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);
            }
        }

    
        [TestMethod]
        public void CheckPerspectiveMembershipAfterSerialization_Negative()
        {
            {
                var handler = new TabularModelHandler();
                var model = handler.Model;
                Table t1 = model.AddCalculatedTable("Table1");
                Column c1 = t1.AddCalculatedColumn("Column1");
                Column c2 = t1.AddCalculatedColumn("Column2");
                var p1 = model.AddPerspective("TestPerspective");

                t1.InPerspective[p1] = true;
                c2.InPerspective[p1] = false;
                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);

                Directory.CreateDirectory("test_2_7_4_perspective_membership_2");
                handler.Save("test_2_7_4_perspective_membership_2", SaveFormat.TabularEditorFolder, new SerializeOptions { LocalPerspectives = true });
            }

            {
                // Load from folder:
                var handler = new TabularModelHandler("test_2_7_4_perspective_membership_2");
                var model = handler.Model;
                var p1 = model.Perspectives["TestPerspective"];
                var t1 = model.Tables["Table1"];
                var c1 = t1.Columns["Column1"];
                var c2 = t1.Columns["Column2"];

                Assert.AreEqual(true, t1.InPerspective[p1]);
                Assert.AreEqual(true, c1.InPerspective[p1]);
                Assert.AreEqual(false, c2.InPerspective[p1]);
            }
        }
    }
}

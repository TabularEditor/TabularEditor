using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_8_1
{
    [TestClass]
    public class DeleteObjectTests
    {
        [TestMethod]
        public void DeleteTableTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");

            Assert.IsFalse(handler.UpdateInProgress);

            handler.Model.Tables["Product"].Delete();

            Assert.IsFalse(handler.UpdateInProgress);
        }

        [TestMethod]
        public void SetOLSTest()
        {
            var handler = ObjectHandlingTests.CreateTestModel("Test", 1400);

            Assert.IsFalse(handler.UpdateInProgress);

            var role = handler.Model.Roles["Test Role 1"];
            var table = handler.Model.Tables["Test Table 1"];

            table.Columns["Column 1"].ObjectLevelSecurity[role] = MetadataPermission.None;
            Assert.AreEqual(MetadataPermission.None, role.TablePermissions["Test Table 1"].ColumnPermissions["Column 1"]);

            Assert.IsFalse(handler.UpdateInProgress);
        }
    }
}

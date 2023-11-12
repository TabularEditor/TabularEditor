using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest
{
    [TestClass]
    public class DatabaseHelperTest
    {
        [TestMethod]
        public void SetIdNameReflection()
        {
            var handler = new TabularModelHandler("TestData\\AdventureWorks.bim");
            var db = handler.Database;
            db.SetID("test-id-123");
            db.SetName("test-name-123");
            Assert.AreEqual("test-id-123", db.ID);
            Assert.AreEqual("test-name-123", db.Name);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest.RegressionTests.v2_8_5
{
    [TestClass]
    public class LockedTreeTests
    {
        [TestMethod]
        public void LockedAfterTryingToProvideDuplicateName()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var m1 = model.Tables["Employee"].AddMeasure("Hello1");
            var m2 = model.Tables["Employee"].AddMeasure("Hello2");

            Assert.ThrowsException<ArgumentException>(() => m2.Name = "Hello1");

            Assert.IsFalse(handler.UpdateInProgress);
        }
    }
}

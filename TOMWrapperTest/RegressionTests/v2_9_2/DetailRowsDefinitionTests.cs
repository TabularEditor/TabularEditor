using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_9_2
{
    [TestClass]
    public class DetailRowsDefinitionTests
    {
        [TestMethod]
        public void DetailRowsDefinitionHack()
        {
            var handler = ObjectHandlingTests.CreateTestModel(compatibilityLevel: 1500);
            var model = handler.Model;
            var t1 = model.Tables["Test Table 1"];
            t1.DefaultDetailRowsExpression = "abc";
            t1.DefaultDetailRowsExpression = null;

            var m1 = t1.Measures["Measure 1"];
            m1.DetailRowsExpression = "123";
            m1.DetailRowsExpression = null;
        }
    }
}

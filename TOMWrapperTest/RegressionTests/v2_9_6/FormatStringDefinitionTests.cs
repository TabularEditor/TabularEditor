using System;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest.RegressionTests.v2_9_5
{
    [TestClass]
    public class FormatStringDefinitionTests
    {
        
        /// <summary>
        /// See issue https://github.com/otykier/TabularEditor/issues/411
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var handler = new TabularModelHandler(1500);
            var model = handler.Model;
            var cg = model.AddCalculationGroup();
            var ci1 = cg.AddCalculationItem();
            ci1.FormatStringExpression = "\"0.00\"";
            ci1.FormatStringExpression = null;
        }
    }
}

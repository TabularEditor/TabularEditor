using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest
{
    [TestClass]
    public class DeserializerTests
    {
        [TestMethod]
        public void CompatibilityModeTest()
        {
            var handler1 = new TabularModelHandler(@"TestData\model-lineageTags.bim");
            Assert.AreEqual(Microsoft.AnalysisServices.CompatibilityMode.AnalysisServices, handler1.Database.CompatibilityMode);
            
            var handler2 = new TabularModelHandler(@"TestData\model-relatedColumnDetails.bim");
            Assert.AreEqual(Microsoft.AnalysisServices.CompatibilityMode.PowerBI, handler2.Database.CompatibilityMode);
        }
    }
}

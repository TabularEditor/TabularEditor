using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var t = typeof(Microsoft.AnalysisServices.Tabular.Measure);
            var expr = t.GetProperty("Expression");

            var x = expr.GetCustomAttributesData();

            Console.WriteLine(x.Count);
        }
    }
}

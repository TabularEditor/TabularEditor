using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TabularEditor.Dax;
using TabularEditor.Scripting;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    [TestClass]
    public class ScriptHelperTests
    {
        [TestMethod]
        public void FormatDaxDeprecatedTest()
        {
            var mockFormatter = new Mock<IDaxFormatterProxy>();
            ScriptHelper.DaxFormatter = mockFormatter.Object;

            var mockCommandLine = new Mock<ICommandLineHandler>();
            mockCommandLine.Setup(cl => cl.CommandLineMode).Returns(true);
            Program.CommandLine = mockCommandLine.Object;

            var measures = GetModelWithMeasures(5);

            var timer = DateTime.Now;
            ScriptHelper.FormatDax(measures[0].Expression);
            Assert.IsTrue((DateTime.Now - timer).TotalSeconds < 0.1, "No throttling for first few calls");

            timer = DateTime.Now;
            ScriptHelper.FormatDax(measures[1].Expression);
            Assert.IsTrue((DateTime.Now - timer).TotalSeconds < 0.1, "No throttling for first few calls");

            timer = DateTime.Now;
            ScriptHelper.FormatDax(measures[2].Expression);
            Assert.IsTrue((DateTime.Now - timer).TotalSeconds < 0.1, "No throttling for first few calls");

            timer = DateTime.Now;
            ScriptHelper.FormatDax(measures[3].Expression);
            Assert.IsTrue((DateTime.Now - timer).TotalSeconds > 1.5, "Throttling after first 3 calls");

            timer = DateTime.Now;
            ScriptHelper.FormatDax(measures[4].Expression);
            Assert.IsTrue((DateTime.Now - timer).TotalSeconds > 1.5, "Throttling after first 3 calls");

            mockCommandLine.Verify(c => c.Warning(It.IsAny<string>()), Times.Once, "Deprecation warning must have been called exactly once");
        }

        [TestMethod]
        public void FormatDaxEnumerableTest()
        {
            var mockFormatter = new Mock<MockFormatter>() { CallBase = true };
            ScriptHelper.DaxFormatter = mockFormatter.Object;

            var measures = GetModelWithMeasures(50);

            ScriptHelper.BeforeScriptExecution();

            measures.FormatDax();
            mockFormatter.Verify(c => c.FormatDaxMulti(It.Is<List<string>>(l => l.Count == 50), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

            ScriptHelper.AfterScriptExecution();
            mockFormatter.Verify(c => c.FormatDaxMulti(It.Is<List<string>>(l => l.Count == 50), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void FormatDaxBatchingTest()
        {
            var mockFormatter = new Mock<MockFormatter>() { CallBase = true };
            ScriptHelper.DaxFormatter = mockFormatter.Object;
            var measures = GetModelWithMeasures(50);

            ScriptHelper.BeforeScriptExecution();

            foreach (var m in measures) m.FormatDax();
            mockFormatter.Verify(c => c.FormatDax(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            mockFormatter.Verify(c => c.FormatDaxMulti(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);

            ScriptHelper.AfterScriptExecution();

            mockFormatter.Verify(c => c.FormatDaxMulti(It.Is<List<string>>(l => l.Count == 50), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        private List<Measure> GetModelWithMeasures(int numberOfMeasures)
        {
            var model = new TabularModelHandler().Model;
            var t1 = model.AddTable();
            return Enumerable.Repeat(0, numberOfMeasures).Select(i => t1.AddMeasure(null, "1+2")).ToList();
        }
    }

    public class MockFormatter : IDaxFormatterProxy
    {
        public virtual DaxFormatterResult FormatDax(string query, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            throw new NotImplementedException();
        }

        public virtual List<DaxFormatterResult> FormatDaxMulti(List<string> dax, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            return dax.Select(str => new DaxFormatterResult {
                FormattedDax = str,
                errors = new List<DaxFormatterError>()
            }).ToList();
        }
    }
}

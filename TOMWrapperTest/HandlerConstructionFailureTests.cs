using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest
{
    /// <summary>
    /// Regression tests for GitHub issue #1339: a failed load (bad file, unreachable server, local
    /// Power BI instance without a database, ...) must not leave a half-constructed handler as the
    /// active TabularModelHandler singleton. Otherwise, code that consults the singleton (e.g. the
    /// LogicalGroups enumeration used by the UI tree) throws a NullReferenceException afterwards.
    /// </summary>
    [TestClass]
    public class HandlerConstructionFailureTests
    {
        [TestMethod]
        public void FailedFileLoadRestoresPreviousSingleton()
        {
            var original = new TabularModelHandler(1500);
            Assert.AreSame(original, TabularModelHandler.Singleton);

            var bogusPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "Model.bim");
            AssertThrows(() => new TabularModelHandler(bogusPath));

            Assert.AreSame(original, TabularModelHandler.Singleton);
            AssertSingletonUsable(original);
        }

        [TestMethod]
        public void FailedConnectionRestoresPreviousSingleton()
        {
            var original = new TabularModelHandler(1500);
            Assert.AreSame(original, TabularModelHandler.Singleton);

            // Nothing listens on port 1 on the loopback interface, so the connection is refused immediately:
            AssertThrows(() => new TabularModelHandler("localhost:1", null));

            Assert.AreSame(original, TabularModelHandler.Singleton);
            AssertSingletonUsable(original);
        }

        [TestMethod]
        public void FailedLoadWithNoPreviousSingletonLeavesSingletonNull()
        {
            TabularModelHandler.Singleton = null;

            var bogusPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "Model.bim");
            AssertThrows(() => new TabularModelHandler(bogusPath));

            Assert.IsNull(TabularModelHandler.Singleton);
        }

        private static void AssertThrows(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                return;
            }
            Assert.Fail("Expected the handler construction to fail");
        }

        private static void AssertSingletonUsable(TabularModelHandler expected)
        {
            // This is the exact code path that crashed in issue #1339 (LogicalGroups.Groups -> Singleton.CompatibilityLevel):
            var groups = LogicalGroups.Singleton.ToList();
            Assert.IsTrue(groups.Any(g => g.Name == LogicalGroups.TABLES));
            Assert.IsTrue(groups.Any(g => g.Name == LogicalGroups.EXPRESSIONS), "CL 1500 model should expose the Shared Expressions group");
            Assert.AreSame(expected.Model, groups.First().Model);

            // Objects created after the failed load must attach to the surviving handler:
            var table = expected.Model.AddCalculatedTable("After failed load");
            Assert.AreSame(expected, table.Handler);
        }
    }
}

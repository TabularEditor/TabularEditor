using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.UIServices;

namespace TabularEditor
{
    [TestClass]
    public class UpdateServiceTests
    {
        [TestMethod]
        public void TestVersionCompareLogic()
        {
            var current = Version.Parse("2.13.41451.2342");

            // Same version:
            var check = VersionCheckResultExtension.DetermineUpdate(current, Version.Parse("2.13.41451.2342"));
            Assert.AreEqual(VersionCheckResult.NoNewVersion, check);
            Assert.IsFalse(check.UpdateAvailable());
            Assert.IsFalse(check.UpdateAvailable(true));

            // Lower version:
            check = VersionCheckResultExtension.DetermineUpdate(current, Version.Parse("2.13.41450.1253"));
            Assert.AreEqual(VersionCheckResult.NoNewVersion, check);
            Assert.IsFalse(check.UpdateAvailable());
            Assert.IsFalse(check.UpdateAvailable(true));

            // Patch available:
            check = VersionCheckResultExtension.DetermineUpdate(current, Version.Parse("2.13.42643.1064"));
            Assert.AreEqual(VersionCheckResult.PatchAvailable, check);
            Assert.IsTrue(check.UpdateAvailable());
            Assert.IsFalse(check.UpdateAvailable(true));

            // Minor available:
            check = VersionCheckResultExtension.DetermineUpdate(current, Version.Parse("2.14.3123.144"));
            Assert.AreEqual(VersionCheckResult.MinorAvailable, check);
            Assert.IsTrue(check.UpdateAvailable());
            Assert.IsTrue(check.UpdateAvailable(true));

            // Minor available:
            check = VersionCheckResultExtension.DetermineUpdate(current, Version.Parse("3.0.10433.4344"));
            Assert.AreEqual(VersionCheckResult.MajorAvailable, check);
            Assert.IsTrue(check.UpdateAvailable());
            Assert.IsTrue(check.UpdateAvailable(true));
        }
    }
}

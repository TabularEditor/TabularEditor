using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.UIServices;

namespace TabularEditor
{
    [TestClass]
    public class PreferencesTests
    {
        /// <summary>
        /// Work item #6545: UDF per-file serialization must be default-on so new folder saves pick
        /// it up. The UI default comes from Preferences.SaveToFolder_Levels (not
        /// SerializeOptions.DefaultFolder), so this guards the actual product path.
        /// </summary>
        [TestMethod]
        public void DefaultSaveToFolderLevelsIncludeFunctions()
        {
            Assert.IsTrue(Preferences.Default.SaveToFolder_Levels.Contains("Functions"));
            Assert.IsTrue(Preferences.Default.GetSerializeOptions().Levels.Contains("Functions"));
        }
    }
}

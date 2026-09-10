using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_28
{
    /// <summary>
    /// Regression tests for work item #6545: DAX UDFs (User Defined Functions) can be serialized
    /// as individual JSON files under a `functions/` subfolder when saving a model to a folder.
    /// This gives teams using Git the same per-object diffability they already have for measures,
    /// columns and hierarchies, and avoids merge conflicts when different developers edit different
    /// UDFs in parallel.
    /// </summary>
    [TestClass]
    public class UdfPerFileSerializationTests
    {
        private static string TestOutputRoot(string name) => Path.Combine("Output", "UdfPerFile_" + name);

        private static TabularModelHandler CreateHandlerWithUdfs()
        {
            var handler = new TabularModelHandler(1702);
            var model = handler.Model;
            AddFunction(model, "SimpleFunction", "() => 42");
            AddFunction(model, "DaxLib.Convert.AcresToSqm", "(acres) => acres * 4046.856");
            AddFunction(model, "DaxLib.Text.Capitalize", "(s) => UPPER(LEFT(s, 1)) & LOWER(MID(s, 2, LEN(s)))");
            return handler;
        }

        private static void AddFunction(Model model, string name, string expression)
        {
            var function = model.AddFunction(name);
            function.Expression = expression;
        }

        private static SerializeOptions FolderOptionsWithFunctions()
            => SerializeOptions.DefaultFolder;

        private static SerializeOptions FolderOptionsWithoutFunctions()
        {
            var options = SerializeOptions.DefaultFolder;
            options.Levels.Remove("Functions");
            return options;
        }

        [TestMethod]
        public void FunctionsLevelIsIncludedInDefaultFolderLevels()
        {
            Assert.IsTrue(SerializeOptions.DefaultFolder.Levels.Contains("Functions"),
                "UDF per-file serialization must be default-on so new folder saves pick it up.");
        }

        [TestMethod]
        public void SaveToFolder_WithFunctionsLevel_WritesOneFilePerUdf()
        {
            var outputDir = TestOutputRoot(nameof(SaveToFolder_WithFunctionsLevel_WritesOneFilePerUdf));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.IsTrue(Directory.Exists(functionsFolder), "functions/ subfolder should exist");

            var files = Directory.GetFiles(functionsFolder, "*.json").Select(Path.GetFileName).OrderBy(n => n).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                "DaxLib.Convert.AcresToSqm.json",
                "DaxLib.Text.Capitalize.json",
                "SimpleFunction.json"
            }, files);

            // Database.json must not contain the functions array inline anymore.
            var databaseJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "database.json")));
            Assert.IsNull(databaseJson["model"]?["functions"], "functions array should be extracted out of database.json");
        }

        [TestMethod]
        public void SaveToFolder_WithFunctionsLevel_RoundTripsSemanticallyIdentical()
        {
            var outputDir = TestOutputRoot(nameof(SaveToFolder_WithFunctionsLevel_RoundTripsSemanticallyIdentical));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var reloaded = new TabularModelHandler(Path.Combine(outputDir, "database.json"));
            Assert.AreEqual(3, reloaded.Model.Functions.Count);

            var byName = reloaded.Model.Functions.ToDictionary(f => f.Name);
            Assert.AreEqual("() => 42", byName["SimpleFunction"].Expression);
            Assert.AreEqual("(acres) => acres * 4046.856", byName["DaxLib.Convert.AcresToSqm"].Expression);
            Assert.IsTrue(byName["DaxLib.Text.Capitalize"].Expression.Contains("UPPER"));
        }

        [TestMethod]
        public void SaveToFolder_RenameUdf_RemovesOldFileAndWritesNew()
        {
            var outputDir = TestOutputRoot(nameof(SaveToFolder_RenameUdf_RemovesOldFileAndWritesNew));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var reloaded = new TabularModelHandler(Path.Combine(outputDir, "database.json"));
            reloaded.Model.Functions["SimpleFunction"].Name = "RenamedFunction";
            reloaded.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.IsFalse(File.Exists(Path.Combine(functionsFolder, "SimpleFunction.json")),
                "Old file should be removed after rename");
            Assert.IsTrue(File.Exists(Path.Combine(functionsFolder, "RenamedFunction.json")),
                "New file should be present after rename");

            // Other UDF files should be untouched.
            Assert.IsTrue(File.Exists(Path.Combine(functionsFolder, "DaxLib.Convert.AcresToSqm.json")));
            Assert.IsTrue(File.Exists(Path.Combine(functionsFolder, "DaxLib.Text.Capitalize.json")));
        }

        [TestMethod]
        public void SaveToFolder_DeleteUdf_RemovesFile()
        {
            var outputDir = TestOutputRoot(nameof(SaveToFolder_DeleteUdf_RemovesFile));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var reloaded = new TabularModelHandler(Path.Combine(outputDir, "database.json"));
            reloaded.Model.Functions["DaxLib.Convert.AcresToSqm"].Delete();
            reloaded.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.IsFalse(File.Exists(Path.Combine(functionsFolder, "DaxLib.Convert.AcresToSqm.json")),
                "Deleted UDF's file should be removed");
            Assert.IsTrue(File.Exists(Path.Combine(functionsFolder, "SimpleFunction.json")));
            Assert.IsTrue(File.Exists(Path.Combine(functionsFolder, "DaxLib.Text.Capitalize.json")));
        }

        [TestMethod]
        public void SaveToFolder_ModelWithoutUdfs_DoesNotCreateFunctionsFolder()
        {
            var outputDir = TestOutputRoot(nameof(SaveToFolder_ModelWithoutUdfs_DoesNotCreateFunctionsFolder));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = new TabularModelHandler(1702);
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            Assert.IsFalse(Directory.Exists(Path.Combine(outputDir, "functions")),
                "No functions/ subfolder should be created when the model has no UDFs.");
        }

        [TestMethod]
        public void SaveToFolder_WithoutFunctionsLevel_KeepsUdfsInlineInDatabaseJson()
        {
            // AC #7: teams that opt out (or have not opted in) via the annotation should get the
            // legacy inline layout unchanged, so existing folder-serialized models don't churn.
            var outputDir = TestOutputRoot(nameof(SaveToFolder_WithoutFunctionsLevel_KeepsUdfsInlineInDatabaseJson));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithoutFunctions());

            Assert.IsFalse(Directory.Exists(Path.Combine(outputDir, "functions")),
                "functions/ folder should not exist when the Functions level is disabled.");

            var databaseJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "database.json")));
            var inlineFunctions = databaseJson["model"]?["functions"] as JArray;
            Assert.IsNotNull(inlineFunctions, "functions array should remain inline in database.json when level is disabled.");
            Assert.AreEqual(3, inlineFunctions.Count);
        }

        [TestMethod]
        public void SaveToFolder_TogglingFunctionsLevelOff_RemovesFilesAndInlinesUdfs()
        {
            // Unchecking the "User Defined Functions (UDFs)" toggle and re-saving must move the UDFs
            // back inline into database.json and delete the per-file jsons. Note: RemoveUnusedFiles
            // never deletes the top-level subfolder itself (same behavior for all levels, and same as
            // TE3), so an empty functions/ directory may remain.
            var outputDir = TestOutputRoot(nameof(SaveToFolder_TogglingFunctionsLevelOff_RemovesFilesAndInlinesUdfs));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());
            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.IsTrue(Directory.Exists(functionsFolder));

            var reloaded = new TabularModelHandler(Path.Combine(outputDir, "database.json"));
            reloaded.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithoutFunctions());

            Assert.AreEqual(0, Directory.Exists(functionsFolder) ? Directory.GetFiles(functionsFolder, "*.json", SearchOption.AllDirectories).Length : 0,
                "All per-file UDF jsons should be removed after re-saving with the Functions level disabled.");

            var databaseJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "database.json")));
            var inlineFunctions = databaseJson["model"]?["functions"] as JArray;
            Assert.IsNotNull(inlineFunctions, "functions array should be back inline in database.json.");
            Assert.AreEqual(3, inlineFunctions.Count);
        }

        [TestMethod]
        public void SaveToFolder_NamespacedUdfsAreFlatInFunctionsFolder()
        {
            // The full DAX name (dots and all) is the filename. Namespace is a TE-only concept
            // and must not be reflected in the on-disk layout.
            var outputDir = TestOutputRoot(nameof(SaveToFolder_NamespacedUdfsAreFlatInFunctionsFolder));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = CreateHandlerWithUdfs();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, FolderOptionsWithFunctions());

            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.AreEqual(0, Directory.GetDirectories(functionsFolder).Length,
                "functions/ must not contain any subdirectories — layout is flat.");
        }

        [TestMethod]
        public void SaveWithAnnotatedOptions_WritesPerFileUdfs_WhenAnnotationEnablesFunctionsLevel()
        {
            // The TabularEditor_SerializeOptions annotation is the mechanism used to share
            // serialization preferences across team members (and with the CLI).
            var outputDir = TestOutputRoot(nameof(SaveWithAnnotatedOptions_WritesPerFileUdfs_WhenAnnotationEnablesFunctionsLevel));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = new TabularModelHandler(1702);
            AddFunction(handler.Model, "SimpleFunction", "() => 42");
            AddFunction(handler.Model, "DaxLib.Convert.AcresToSqm", "(acres) => acres * 4046.856");

            handler.SerializeOptions = SerializeOptions.DefaultFolder;
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, null, useAnnotatedSerializeOptions: true);

            var functionsFolder = Path.Combine(outputDir, "functions");
            Assert.IsTrue(Directory.Exists(functionsFolder),
                "functions/ subfolder should be created when the annotation enables the Functions level.");

            var files = Directory.GetFiles(functionsFolder, "*.json")
                .Select(Path.GetFileName)
                .OrderBy(n => n)
                .ToArray();
            CollectionAssert.AreEqual(new[]
            {
                "DaxLib.Convert.AcresToSqm.json",
                "SimpleFunction.json"
            }, files);

            var databaseJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "database.json")));
            Assert.IsNull(databaseJson["model"]?["functions"],
                "functions array should be extracted out of database.json.");
        }

        [TestMethod]
        public void LoadFolder_WithPerFileUdfs_RoundTripsThroughAnnotatedOptions()
        {
            var outputDir = TestOutputRoot(nameof(LoadFolder_WithPerFileUdfs_RoundTripsThroughAnnotatedOptions));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var writer = new TabularModelHandler(1702);
            AddFunction(writer.Model, "SimpleFunction", "() => 42");
            AddFunction(writer.Model, "DaxLib.Text.Capitalize", "(s) => UPPER(s)");
            writer.SerializeOptions = SerializeOptions.DefaultFolder;
            writer.Save(outputDir, SaveFormat.TabularEditorFolder, null, useAnnotatedSerializeOptions: true);

            var reader = new TabularModelHandler(Path.Combine(outputDir, "database.json"));
            Assert.AreEqual(2, reader.Model.Functions.Count);

            var byName = reader.Model.Functions.ToDictionary(f => f.Name);
            Assert.AreEqual("() => 42", byName["SimpleFunction"].Expression);
            Assert.AreEqual("(s) => UPPER(s)", byName["DaxLib.Text.Capitalize"].Expression);
        }

        [TestMethod]
        public void SaveWithAnnotatedOptions_PreservesInlineUdfs_WhenAnnotationOmitsFunctionsLevel()
        {
            // AC #7: existing folder-serialized models whose annotation predates this feature must
            // keep UDFs inline in database.json, so they don't churn on save.
            var outputDir = TestOutputRoot(nameof(SaveWithAnnotatedOptions_PreservesInlineUdfs_WhenAnnotationOmitsFunctionsLevel));
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = new TabularModelHandler(1702);
            AddFunction(handler.Model, "Legacy.Fn", "() => 1");

            // Explicit annotation without Functions in Levels — this is what a pre-6545 model has.
            handler.SerializeOptions = FolderOptionsWithoutFunctions();
            handler.Save(outputDir, SaveFormat.TabularEditorFolder, null, useAnnotatedSerializeOptions: true);

            Assert.IsFalse(Directory.Exists(Path.Combine(outputDir, "functions")),
                "No functions/ folder should be created when the annotation opts out.");

            var databaseJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "database.json")));
            var inlineFunctions = databaseJson["model"]?["functions"] as JArray;
            Assert.IsNotNull(inlineFunctions, "functions array should remain inline in database.json.");
            Assert.AreEqual(1, inlineFunctions.Count);
        }
    }
}

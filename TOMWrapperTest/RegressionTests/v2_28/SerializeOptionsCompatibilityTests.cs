using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_28
{
    /// <summary>
    /// Previous versions of Tabular Editor had a bug with an inconsistency in the name of the
    /// calculation items serialization level tag: SerializeOptions.DefaultFolder produced
    /// "Tables/CalculationItems" while the serializer and the Preferences UI use
    /// "Tables/Calculation Items". TE3 normalizes the legacy string when reading the
    /// TabularEditor_SerializeOptions annotation; TE2 must do the same so that both tools
    /// produce the same folder layout from the same model.
    /// </summary>
    [TestClass]
    public class SerializeOptionsCompatibilityTests
    {
        private const string ANN_SERIALIZEOPTIONS = "TabularEditor_SerializeOptions";

        [TestMethod]
        public void LegacyCalculationItemsTagInAnnotationIsNormalizedOnRead()
        {
            var handler = new TabularModelHandler(1500);
            handler.Model.SetAnnotation(ANN_SERIALIZEOPTIONS,
                "{\"IgnoreInferredObjects\":true,\"IgnoreInferredProperties\":true,\"IgnoreTimestamps\":true," +
                "\"SplitMultilineStrings\":true,\"PrefixFilenames\":false,\"LocalTranslations\":false," +
                "\"LocalPerspectives\":false,\"LocalRelationships\":false," +
                "\"Levels\":[\"Data Sources\",\"Perspectives\",\"Relationships\",\"Roles\",\"Tables\",\"Tables/CalculationItems\",\"Translations\"]}");

            var options = handler.SerializeOptions;
            Assert.IsTrue(options.Levels.Contains("Tables/Calculation Items"),
                "The legacy 'Tables/CalculationItems' tag must be normalized to 'Tables/Calculation Items' when read from the annotation.");
            Assert.IsFalse(options.Levels.Contains("Tables/CalculationItems"));
        }

        [TestMethod]
        public void LegacyCalculationItemsTagSplitsCalculationItemsOnAnnotatedSave()
        {
            var outputDir = Path.Combine("Output", "LegacyCalcItemsTag");
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);

            var handler = new TabularModelHandler(1500);
            var calcGroup = handler.Model.AddCalculationGroup("MyCalcGroup");
            calcGroup.AddCalculationItem("Item1", "SELECTEDMEASURE()");
            handler.Model.SetAnnotation(ANN_SERIALIZEOPTIONS,
                "{\"Levels\":[\"Tables\",\"Tables/CalculationItems\"]}");

            handler.Save(outputDir, SaveFormat.TabularEditorFolder, null, useAnnotatedSerializeOptions: true);

            var itemFile = Path.Combine(outputDir, "tables", "MyCalcGroup", "calculationItems", "Item1.json");
            Assert.IsTrue(File.Exists(itemFile),
                "A legacy annotation must produce per-file calculation items, same as TE3 does for the same model.");

            var tableJson = JObject.Parse(File.ReadAllText(Path.Combine(outputDir, "tables", "MyCalcGroup", "MyCalcGroup.json")));
            Assert.IsNull(tableJson["calculationGroup"]?["calculationItems"],
                "calculationItems should be extracted out of the table file.");
        }
    }
}

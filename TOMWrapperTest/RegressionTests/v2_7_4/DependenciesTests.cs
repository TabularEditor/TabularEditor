using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using System.Linq;
using System.IO;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class TranslationsTests
    {
        #region Model.bim metadata
        const string DatabaseJson = @"{
  ""name"": ""SemanticModel"",
  ""compatibilityLevel"": 1200,
  ""model"": {
    ""tables"": [
      {
        ""name"": ""NewTable"",
        ""partitions"": [
          {
            ""name"": ""NewTable"",
            ""mode"": ""import"",
            ""source"": {
              ""type"": ""calculated""
            }
          }
        ],
        ""measures"": [
          {
            ""name"": ""Measure1"",
            ""expression"": ""123"",
            ""annotations"": [
              {
                ""name"": ""TabularEditor_TranslatedNames"",
                ""value"": ""{\""da-DK\"":\""Measure1DK\"",\""en-US\"":\""Measure1US\""}""
              }
            ]
          }
        ]
      }
    ],
    ""annotations"": [
      {
        ""name"": ""TabularEditor_SerializeOptions"",
        ""value"": ""{\""IgnoreInferredObjects\"":true,\""IgnoreInferredProperties\"":true,\""IgnoreTimestamps\"":true,\""SplitMultilineStrings\"":true,\""PrefixFilenames\"":false,\""LocalTranslations\"":true,\""LocalPerspectives\"":false,\""LocalRelationships\"":false,\""Levels\"":[]}""
      },
      {
        ""name"": ""TabularEditor_Cultures"",
        ""value"": ""[\""da-DK\"",\""en-US\""]""
      }
    ]
  }
}";
        #endregion

        /// <summary>
        /// https://github.com/otykier/TabularEditor/issues/205
        /// </summary>
        [TestMethod]
        public void ModelWithTranslationsSerialization()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("NewTable");
            var m1 = t1.AddMeasure("Measure1", "123");
            var culture1 = model.AddTranslation("en-US");
            var culture2 = model.AddTranslation("da-DK");
            m1.TranslatedNames[culture1] = "Measure1US";
            m1.TranslatedNames[culture2] = "Measure1DK";

            Assert.AreEqual("{\"da-DK\":\"Measure1DK\",\"en-US\":\"Measure1US\"}", m1.TranslatedNames.ToJson());

            Directory.CreateDirectory("test_2_7_4_translation_serialization");
            handler.Save("test_2_7_4_translation_serialization", SaveFormat.TabularEditorFolder,
                new TabularEditor.TOMWrapper.Serialization.SerializeOptions()
                {
                    Levels = new System.Collections.Generic.HashSet<string>(),
                    LocalTranslations = true
                });

            var savedMetadata = File.ReadAllText(@"test_2_7_4_translation_serialization\database.json");
            Assert.AreEqual(DatabaseJson, savedMetadata);
        }

        [TestMethod]
        public void LoadPureDictionary()
        {
            Directory.CreateDirectory("test_2_7_4_translation_serialization_pure");
            File.WriteAllText(@"test_2_7_4_translation_serialization_pure\database.json", DatabaseJson);

            var handler = new TabularModelHandler("test_2_7_4_translation_serialization_pure");
            var model = handler.Model;
            var t1 = model.Tables["NewTable"];
            var m1 = t1.Measures["Measure1"];
            Assert.AreEqual(2, model.Cultures.Count);
            Assert.AreEqual("Measure1DK", m1.TranslatedNames["da-DK"]);
            Assert.AreEqual("Measure1US", m1.TranslatedNames["en-US"]);
        }

        [TestMethod]
        public void LoadKVPDictionary()
        {
            Directory.CreateDirectory("test_2_7_4_translation_serialization_pure");
            File.WriteAllText(@"test_2_7_4_translation_serialization_pure\database.json", 
                DatabaseJson.Replace(
                    @"{\""da-DK\"":\""Measure1DK\"",\""en-US\"":\""Measure1US\""}",
                    @"[{\""Key\"":\""da-DK\"",\""Value\"":\""Measure1DK\""},{\""Key\"":\""en-US\"",\""Value\"":\""Measure1US\""}]"));

            var handler = new TabularModelHandler("test_2_7_4_translation_serialization_pure");
            var model = handler.Model;
            var t1 = model.Tables["NewTable"];
            var m1 = t1.Measures["Measure1"];
            Assert.AreEqual(2, model.Cultures.Count);
            Assert.AreEqual("Measure1DK", m1.TranslatedNames["da-DK"]);
            Assert.AreEqual("Measure1US", m1.TranslatedNames["en-US"]);
        }
    }
}

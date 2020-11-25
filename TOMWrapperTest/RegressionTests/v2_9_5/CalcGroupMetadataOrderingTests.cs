using System;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TOMWrapperTest.RegressionTests.v2_9_5
{
    [TestClass]
    public class CalcGroupMetadataOrderingTests
    {
        const string expectedTmsl = @"{
  ""createOrReplace"": {
    ""object"": {
      ""database"": ""db""
    },
    ""database"": {
      ""name"": ""db"",
      ""compatibilityLevel"": 1500,
      ""model"": {
        ""discourageImplicitMeasures"": true,
        ""tables"": [
          {
            ""name"": ""New Calculation Group"",
            ""calculationGroup"": {},
            ""columns"": [
              {
                ""name"": ""zName"",
                ""dataType"": ""string"",
                ""sourceColumn"": ""Name"",
                ""sortByColumn"": ""xOrdinal""
              },
              {
                ""name"": ""xOrdinal"",
                ""dataType"": ""int64"",
                ""isHidden"": true,
                ""sourceColumn"": ""Ordinal""
              },
              {
                ""type"": ""calculated"",
                ""name"": ""bColumn"",
                ""dataType"": ""unknown"",
                ""isDataTypeInferred"": true
              },
              {
                ""type"": ""calculated"",
                ""name"": ""yColumn"",
                ""dataType"": ""unknown"",
                ""isDataTypeInferred"": true
              }
            ],
            ""partitions"": [
              {
                ""name"": ""Partition"",
                ""source"": {
                  ""type"": ""calculationGroup""
                }
              }
            ]
          }
        ],
        ""annotations"": [
          {
            ""name"": ""TabularEditor_SerializeOptions"",
            ""value"": ""{\""IgnoreInferredObjects\"":false,\""IgnoreInferredProperties\"":false,\""IgnoreTimestamps\"":false,\""SplitMultilineStrings\"":false,\""PrefixFilenames\"":false,\""LocalTranslations\"":true,\""LocalPerspectives\"":true,\""LocalRelationships\"":true,\""Levels\"":[\""Data Sources\"",\""Roles\"",\""Tables\"",\""Tables/Calculation Items\"",\""Tables/Columns\"",\""Tables/Hierarchies\"",\""Tables/Measures\"",\""Tables/Partitions\""]}""
          }
        ]
      },
      ""id"": ""db""
    }
  }
}";

        /// <summary>
        /// See issue https://github.com/otykier/TabularEditor/issues/411
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            if (Directory.Exists("TestData\\CalcGroupOrderTest")) Directory.Delete("TestData\\CalcGroupOrderTest", true);
            ZipFile.ExtractToDirectory("TestData\\CalcGroupOrderTest.zip", "TestData\\CalcGroupOrderTest");
            var handler = new TabularModelHandler("TestData\\CalcGroupOrderTest");
            var model = handler.Model;

            var tmsl = TabularDeployer.DeployNewTMSL(handler.Database, "db", DeploymentOptions.Full, false, Microsoft.AnalysisServices.CompatibilityMode.AnalysisServices);

            Assert.AreEqual(expectedTmsl, tmsl);
        }
    }
}

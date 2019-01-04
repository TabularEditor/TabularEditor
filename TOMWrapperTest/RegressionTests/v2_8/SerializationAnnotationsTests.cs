using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_8
{
    [TestClass]
    public class SerializationAnnotationsTests
    {
        #region const
        public const string DatabaseJsonCompare = @"{
  ""name"": ""New Tabular Database"",
  ""compatibilityLevel"": 1400,
  ""model"": {
    ""dataSources"": [
      {
        ""name"": ""New Provider Data Source"",
        ""impersonationMode"": ""impersonateServiceAccount""
      }
    ],
    ""tables"": [
      {
        ""name"": ""Table 1"",
        ""columns"": [
          {
            ""name"": ""Column 1"",
            ""dataType"": ""string"",
            ""annotations"": [
              {
                ""name"": ""TabularEditor_TranslatedNames"",
                ""value"": ""{\""da-DK\"":\""Test 1\""}""
              },
              {
                ""name"": ""TabularEditor_InPerspective"",
                ""value"": ""[\""Perspective 1\""]""
              }
            ]
          },
          {
            ""name"": ""Column 2"",
            ""dataType"": ""string""
          },
          {
            ""name"": ""Column 3"",
            ""dataType"": ""string""
          }
        ],
        ""partitions"": [
          {
            ""name"": ""Table 1"",
            ""source"": {
              ""type"": ""query"",
              ""dataSource"": ""New Provider Data Source""
            }
          }
        ],
        ""annotations"": [
          {
            ""name"": ""TabularEditor_InPerspective"",
            ""value"": ""[\""Perspective 1\""]""
          },
          {
            ""name"": ""TabularEditor_Relationships"",
            ""value"": [
              ""["",
              ""  {"",
              ""    \""name\"": \""4cbb1f88-29fa-4c09-9b0d-d51bd8823af9\"","",
              ""    \""fromTable\"": \""Table 1\"","",
              ""    \""fromColumn\"": \""Column 1\"","",
              ""    \""toTable\"": \""Table 2\"","",
              ""    \""toColumn\"": \""Column 1\"""",
              ""  }"",
              ""]""
            ]
          }
        ]
      },
      {
        ""name"": ""Table 2"",
        ""columns"": [
          {
            ""name"": ""Column 1"",
            ""dataType"": ""string""
          },
          {
            ""name"": ""Column 2"",
            ""dataType"": ""string"",
            ""annotations"": [
              {
                ""name"": ""TabularEditor_InPerspective"",
                ""value"": ""[\""Perspective 2\""]""
              }
            ]
          },
          {
            ""name"": ""Column 3"",
            ""dataType"": ""string"",
            ""annotations"": [
              {
                ""name"": ""TabularEditor_InPerspective"",
                ""value"": ""[\""Perspective 2\""]""
              }
            ]
          }
        ],
        ""partitions"": [
          {
            ""name"": ""Table 2"",
            ""source"": {
              ""type"": ""query"",
              ""dataSource"": ""New Provider Data Source""
            }
          }
        ],
        ""annotations"": [
          {
            ""name"": ""TabularEditor_InPerspective"",
            ""value"": ""[\""Perspective 2\""]""
          },
          {
            ""name"": ""TabularEditor_Relationships"",
            ""value"": ""[]""
          }
        ]
      }
    ],
    ""annotations"": [
      {
        ""name"": ""TabularEditor_SerializeOptions"",
        ""value"": ""{\""IgnoreInferredObjects\"":true,\""IgnoreInferredProperties\"":true,\""IgnoreTimestamps\"":true,\""SplitMultilineStrings\"":true,\""PrefixFilenames\"":false,\""LocalTranslations\"":true,\""LocalPerspectives\"":true,\""LocalRelationships\"":true,\""Levels\"":[]}""
      },
      {
        ""name"": ""TabularEditor_Cultures"",
        ""value"": ""[\""da-DK\"",\""en-US\""]""
      },
      {
        ""name"": ""TabularEditor_Perspectives"",
        ""value"": ""[{\""Name\"":\""Perspective 1\"",\""Description\"":\""\"",\""Annotations\"":{}},{\""Name\"":\""Perspective 2\"",\""Description\"":\""\"",\""Annotations\"":{}}]""
      }
    ]
  }
}";
        #endregion

        [TestMethod]
        public void EnsureAnnotationsAreRemovedTest()
        {
            Directory.CreateDirectory("test_2_8_annotations2");
            File.WriteAllText(@"test_2_8_annotations1\database.json", DatabaseJsonCompare);
            var handler = new TabularModelHandler(@"test_2_8_annotations1\database.json");

            var items = handler.Model.GetChildrenRecursive(true).ToList();
            foreach(var item in items.OfType<IAnnotationObject>())
            {
                if (item is Model)
                {
                    Assert.AreEqual(1, item.GetAnnotationsCount());
                    Assert.IsTrue(item.HasAnnotation("TabularEditor_SerializeOptions"));
                }
                else Assert.AreEqual(0, item.GetAnnotationsCount());
            }

            handler.Save("test_2_8_annotations1", SaveFormat.TabularEditorFolder, null, true);

            var databaseJson = File.ReadAllText(@"test_2_8_annotations1\database.json");
            Assert.AreEqual(DatabaseJsonCompare, databaseJson);
        }

        [TestMethod]
        public void AlteringObjectPerspectiveTest()
        {
            Directory.CreateDirectory("test_2_8_annotations2");
            File.WriteAllText(@"test_2_8_annotations2\database.json", DatabaseJsonCompare);
            var handler = new TabularModelHandler(@"test_2_8_annotations2\database.json");
            var model = handler.Model;
            var t2 = model.Tables["Table 2"];
            var c2 = t2.Columns["Column 2"];
            Assert.IsTrue(c2.InPerspective["Perspective 2"]);

            c2.InPerspective["Perspective 2"] = false;
            Assert.IsFalse(c2.InPerspective["Perspective 2"]);
            handler.Save("test_2_8_annotations2", SaveFormat.TabularEditorFolder, null, true);

            handler = new TabularModelHandler(@"test_2_8_annotations2\database.json");
            model = handler.Model;
            t2 = model.Tables["Table 2"];
            c2 = t2.Columns["Column 2"];
            Assert.IsFalse(c2.InPerspective["Perspective 2"]);

            c2.InPerspective["Perspective 2"] = true;
            Assert.IsTrue(c2.InPerspective["Perspective 2"]);
            handler.Save("test_2_8_annotations2", SaveFormat.TabularEditorFolder, null, true);

            var databaseJson = File.ReadAllText(@"test_2_8_annotations2\database.json");
            Assert.AreEqual(DatabaseJsonCompare, databaseJson);

        }
    }
}

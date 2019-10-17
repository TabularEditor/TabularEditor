using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;

namespace TabularEditor.BestPracticeAnalyzer
{
    [TestClass]
    public class FileTests
    {
        bool backupLocalUsersFile = false;
        bool backupLocalMachineFile = false;
        string LocalUserRules => BestPracticeCollection.LocalUserRulesFile;
        string LocalMachineRules => BestPracticeCollection.LocalMachineRulesFile;

        string ModelFile => @"AdventureWorks.bim";
        string ModelFolder => FileSystemHelper.DirectoryFromPath(
            FileSystemHelper.GetAbsolutePath(Environment.CurrentDirectory, ModelFile)
            );

        #region JSON
        const string RelativePathRulesJson = @"[
  {
    ""ID"": ""RP_RULE1"",
    ""Name"": ""RP_Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE1"",
    ""Name"": ""Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE2"",
    ""Name"": ""Rule2"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  }
]";
        const string AbsolutePathRulesJson = @"[
  {
    ""ID"": ""RULE2"",
    ""Name"": ""Rule2"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE3"",
    ""Name"": ""Rule3"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""AP_RULE1"",
    ""Name"": ""AP_Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE1"",
    ""Name"": ""Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  }
]";

        const string LocalUserRulesJson = @"[
  {
    ""ID"": ""LU_RULE1"",
    ""Name"": ""LU_Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE1"",
    ""Name"": ""Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE2"",
    ""Name"": ""Rule2"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE3"",
    ""Name"": ""Rule3"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE4"",
    ""Name"": ""Rule4"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  }
]";

        const string LocalMachineRulesJson = @"[
  {
    ""ID"": ""LM_RULE1"",
    ""Name"": ""LM_Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE1"",
    ""Name"": ""Rule1"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE2"",
    ""Name"": ""Rule2"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE3"",
    ""Name"": ""Rule3"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE4"",
    ""Name"": ""Rule4"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  },
  {
    ""ID"": ""RULE5"",
    ""Name"": ""Rule5"",
    ""Category"": """",
    ""Description"": """",
    ""Severity"": 1,
    ""Scope"": ""Model"",
    ""Expression"": ""true"",
    ""FixExpression"": null,
    ""CompatibilityLevel"": 1200,
    ""ObjectCount"": 0,
    ""ErrorMessage"": null
  }
]";
        #endregion

        [TestInitialize]
        public void BackupRuleFiles()
        {
            backupLocalUsersFile = File.Exists(LocalUserRules);
            backupLocalMachineFile = File.Exists(LocalMachineRules);

            if (backupLocalUsersFile && !File.Exists(LocalUserRules + ".ut1")) File.Move(LocalUserRules, LocalUserRules + ".ut1");
            if (backupLocalMachineFile && !File.Exists(LocalMachineRules + ".ut1")) File.Move(LocalMachineRules, LocalMachineRules + ".ut1");

            File.WriteAllText(LocalUserRules, LocalUserRulesJson);
            File.WriteAllText(LocalMachineRules, LocalMachineRulesJson);

            var modelJson = File.ReadAllText(ModelFile);
            File.WriteAllText(ModelFile, modelJson.Replace("${DEBUGPATH}", Environment.CurrentDirectory.Replace(@"\", @"\\\\")));

            File.WriteAllText("AbsolutePathRules.json", AbsolutePathRulesJson);
            File.WriteAllText("RelativePathRules.json", RelativePathRulesJson);
        }

        [TestCleanup]
        public void RestoreRuleFiles()
        {
            File.Delete(LocalUserRules);
            File.Delete(LocalMachineRules);

            if (backupLocalUsersFile) File.Move(LocalUserRules + ".ut1", LocalUserRules);
            if (backupLocalMachineFile) File.Move(LocalMachineRules + ".ut1", LocalMachineRules);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var h = new TabularModelHandler(ModelFile);
            var a = new Analyzer();
            a.BasePath = ModelFolder;
            a.Model = h.Model;
            Assert.AreEqual(2, a.ExternalRuleCollections.Count);

            Assert.AreEqual(LocalMachineRules, a.LocalMachineRules.FilePath);
            Assert.AreEqual(6, a.LocalMachineRules.Rules.Count);

            Assert.AreEqual(LocalUserRules, a.LocalUserRules.FilePath);
            Assert.AreEqual(5, a.LocalUserRules.Rules.Count);

            Assert.AreEqual(Environment.CurrentDirectory + @"\AbsolutePathRules.json", a.ExternalRuleCollections[1].FilePath);
            Assert.AreEqual(4, a.ExternalRuleCollections[1].Rules.Count);

            Assert.AreEqual("RelativePathRules.json", a.ExternalRuleCollections[0].FilePath);
            Assert.AreEqual(3, a.ExternalRuleCollections[0].Rules.Count);

            Assert.IsNull(a.ModelRules.FilePath);
            Assert.AreEqual(2, a.ModelRules.Rules.Count);

            Assert.AreEqual(20, a.AllRules.Count());
            Assert.AreEqual(10, a.EffectiveRules.Count());

            Assert.AreSame(a.ModelRules, a.EffectiveCollectionForRule("RULE1"));
            Assert.AreSame(a.ExternalRuleCollections[0], a.EffectiveCollectionForRule("RULE2"));
            Assert.AreSame(a.ExternalRuleCollections[1], a.EffectiveCollectionForRule("RULE3"));
            Assert.AreSame(a.LocalUserRules, a.EffectiveCollectionForRule("RULE4"));
            Assert.AreSame(a.LocalMachineRules, a.EffectiveCollectionForRule("RULE5"));

            Assert.AreSame(a.ModelRules, a.EffectiveCollectionForRule("CM_RULE1"));
            Assert.AreSame(a.ExternalRuleCollections[0], a.EffectiveCollectionForRule("RP_RULE1"));
            Assert.AreSame(a.ExternalRuleCollections[1], a.EffectiveCollectionForRule("AP_RULE1"));
            Assert.AreSame(a.LocalUserRules, a.EffectiveCollectionForRule("LU_RULE1"));
            Assert.AreSame(a.LocalMachineRules, a.EffectiveCollectionForRule("LM_RULE1"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.IO;
using Newtonsoft.Json;
using TabularEditor.TOMWrapper;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace TabularEditor.BestPracticeAnalyzer
{
    public enum RuleScope
    {
        Model = 1,
        Table = 2,
        Column = 3,
        Measure = 4,
        Hierarchy = 5,
        Level = 6,
        Relationship = 7,
        Perspective = 8,
        Culture = 9,
        Partition = 10,
        DataSource = 11,
        DataColumn = 12,
        CalculatedColumn = 13,
        CalculatedTable = 14
    }

    public static class RuleScopeHelper
    {
        public static string GetTypeName(this RuleScope scope)
        {
            var x = scope.ToString().SplitCamelCase();
            if (scope == RuleScope.Hierarchy) x = "Hierarchies";
            else if (scope != RuleScope.Model) x += "s";
            return x;
        }
        public static Type GetScopeType(this RuleScope scope)
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(Model));
            var t = assembly.GetType("TabularEditor.TOMWrapper." + scope.ToString());
            return t;
        }

        public static RuleScope GetScope(string scope)
        {
            if (scope == "Hierarchies") return RuleScope.Hierarchy;
            if (scope == "Model") return RuleScope.Model;
            else return (RuleScope)Enum.Parse(typeof(RuleScope), (scope.EndsWith("s") ? scope.Substring(0, scope.Length - 1) : scope).Replace(" ", ""));
        }
    }



    public class BestPracticeRule
    {
        public string ID { get; set; } = "";
        public string Name { get; set; }
        public string Category { get; set; }

        [JsonIgnore]
        public bool Enabled { get; set; }

        public string Description { get; set; }
        public int Severity { get; set; } = 10;

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleScope Scope { get; set; }
        public string Expression { get; set; }
        public string FixExpression { get; set; }
        public HashSet<int> Compatibility { get; set; }

        public bool IsValid { get; }
    }

    static public class StandardBestPractices
    {
        static public BestPracticeCollection GetStandardBestPractices()
        {
            var bpc = new BestPracticeCollection();

            bpc.Add(new BestPracticeRule
            {
                Name = "Do not summarize key columns",
                ID = "KEYCOLUMNS_SUMMARIZEBY_NONE",
                Scope = RuleScope.Column,
                Description = "Visible numeric columns whose name end with Key or ID should have their 'Summarize By' property set to 'Do Not Summarize'.",
                Severity = 1,
                Expression = "(Name.EndsWith(\"Key\", true, null) or Name.EndsWith(\"ID\", true, null)) and SummarizeBy <> \"None\" and not IsHidden and not Table.IsHidden",
                FixExpression = "SummarizeBy = TOM.AggregateFunction.None",
                Compatibility = new HashSet<int> { 1200, 1400 }
            });

            bpc.Add(new BestPracticeRule
            {
                Name = "Hide foreign key columns",
                ID = "FKCOLUMNS_HIDDEN",
                Scope = RuleScope.Column,
                Description = "Columns used on the Many side of a relationship should be hidden.",
                Severity = 1,
                Expression = "Model.Relationships.Any(FromColumn = outerIt) and not IsHidden and not Table.IsHidden",
                FixExpression = "IsHidden = true",
                Compatibility = new HashSet<int> { 1200, 1400 }
            });

            bpc.Add(new BestPracticeRule {
                Name = "Test rule",
                ID = "TEST_RULE",
                Scope = RuleScope.Measure,
                Expression = "not RegEx.IsMatch(Table.Name,\"KPIs\") and (not Expression.Contains(\"Test\") or (Expression = \"Hej\" and Expression = \"Dav\") or not Name.StartsWith(\"test\",true,null) or Name <> \"Hej\" or Name <> \"1\" or not (Name <> \"2\") or Name <> \"3\") and Description = \"\"",
                Compatibility = new HashSet<int> { 1200, 1400 }
            });

            bpc.Add(new BestPracticeRule {
                Name = "Test rule 2",
                ID = "TEST_RULE_2",
                Scope = RuleScope.Measure,
                Expression = "!string.IsNullOrEmpty(Table.Name)",
                Compatibility = new HashSet<int> { 1200, 1400 }
            });
            return bpc;
        }
    }

    public class BestPracticeCollection: List<BestPracticeRule>
    {
        static public BestPracticeCollection LoadFromJsonFile(string filePath)
        {
            if (!File.Exists(filePath)) return new BestPracticeCollection();
            return LoadFromJson(File.ReadAllText(filePath));
        }

        public void AddFromJsonFile(string filePath)
        {
            var bpc = LoadFromJsonFile(filePath);
            AddRange(
                bpc.Where(r => !this.Any(rule => rule.ID.Equals(r.ID, StringComparison.InvariantCultureIgnoreCase)))
                );
        }

        static public BestPracticeCollection LoadFromJson(string json)
        {
            return JsonConvert.DeserializeObject<BestPracticeCollection>(json);
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        public void SaveToFile(string filePath)
        {
            (new FileInfo(filePath)).Directory.Create();
            File.WriteAllText(filePath, SerializeToJson());
        }

        public BestPracticeRule this[string ruleId]
        {
            get
            {
                return this.FirstOrDefault(r => r.ID.Equals(ruleId, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public bool Contains(string Id)
        {
            return this.Any(r => r.ID.Equals(Id, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

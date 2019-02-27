extern alias json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.IO;
using json.Newtonsoft.Json;
using TabularEditor.TOMWrapper;
using json.Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Threading;

namespace TabularEditor.BestPracticeAnalyzer
{
    [Flags]
    public enum RuleScope
    {
        Model                   = 0x0001,
        Table                   = 0x0002, // Excludes Calculated Tables even though they derive from this type
        Measure                 = 0x0004,
        Hierarchy               = 0x0008,
        Level                   = 0x0010,
        Relationship            = 0x0020,
        Perspective             = 0x0040,
        Culture                 = 0x0080,
        Partition               = 0x0100,
        ProviderDataSource      = 0x0200,
        DataColumn              = 0x0400,
        CalculatedColumn        = 0x0800,
        CalculatedTable         = 0x1000,
        CalculatedTableColumn   = 0x2000,
        KPI                     = 0x4000,
        StructuredDataSource    = 0x8000,
        Variation               = 0x10000,
        NamedExpression         = 0x20000,
        ModelRole               = 0x40000
    }

    public static class RuleScopeHelper
    {
        public static RuleScope Combine(this IEnumerable<RuleScope> scopes)
        {
            if (!scopes.Any()) return (RuleScope)0;
            return scopes.Aggregate((r1, r2) => r1 | r2);
        }

        public static bool IsMultiple(this RuleScope scope)
        {
            return ((scope & (scope - 1)) != 0);
        }

        public static string GetTypeName(this RuleScope scope)
        {
            if (scope.IsMultiple()) throw new InvalidOperationException("The provided RuleScope enum value has more than one flag set.");

            var x = scope.ToString().SplitCamelCase();
            if (scope == RuleScope.Hierarchy) x = "Hierarchies";
            else if (scope != RuleScope.Model) x += "s";
            return x;
        }
        public static Type GetScopeType(this RuleScope scope)
        {
            if (scope.IsMultiple()) throw new InvalidOperationException("The provided RuleScope enum value has more than one flag set.");

            switch(scope)
            {
                case RuleScope.Model: return typeof(Model);
                case RuleScope.Table: return typeof(Table);
                case RuleScope.Measure: return typeof(Measure);
                case RuleScope.Hierarchy: return typeof(Hierarchy);
                case RuleScope.Level: return typeof(Level);
                case RuleScope.Relationship: return typeof(SingleColumnRelationship);
                case RuleScope.Perspective: return typeof(Perspective);
                case RuleScope.Culture: return typeof(Culture);
                case RuleScope.Partition: return typeof(Partition);
                case RuleScope.ProviderDataSource: return typeof(ProviderDataSource);
                case RuleScope.StructuredDataSource: return typeof(StructuredDataSource);
                case RuleScope.DataColumn: return typeof(DataColumn);
                case RuleScope.CalculatedColumn: return typeof(CalculatedColumn);
                case RuleScope.CalculatedTable: return typeof(CalculatedTable);
                case RuleScope.CalculatedTableColumn: return typeof(CalculatedTableColumn);
                case RuleScope.KPI: return typeof(KPI);
                case RuleScope.Variation: return typeof(Variation);
                case RuleScope.NamedExpression: return typeof(NamedExpression);
                case RuleScope.ModelRole: return typeof(ModelRole);
                default:
                    throw new InvalidOperationException("Unknown scope type");
            }
        }

        public static RuleScope GetScope(string scope)
        {
            if (scope == "Hierarchies") return RuleScope.Hierarchy;
            if (scope == "Model") return RuleScope.Model;
            else return (RuleScope)Enum.Parse(typeof(RuleScope), (scope.EndsWith("s") ? scope.Substring(0, scope.Length - 1) : scope).Replace(" ", ""));
        }

        /// <summary>
        /// Enumerates the currently set flags on this Enum as individual Enum values.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<RuleScope> Enumerate(this RuleScope input)
        {
            foreach (RuleScope value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value)) yield return value;
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
        public int Severity { get; set; } = 1;

        [JsonConverter(typeof(RuleScopeConverter))]
        public RuleScope Scope
        {
            get
            {
                return _scope;
            }
            set
            {
                if (_scope != value) _needsRecompile = true;
                _scope = value;
            }
        }
        private RuleScope _scope;

        [JsonIgnore]
        public string ScopeString
        {
            get {
                switch (Scope) {
                    case RuleScope.CalculatedColumn | RuleScope.CalculatedTableColumn | RuleScope.DataColumn:
                        return "Columns";
                    default:
                        return string.Join(",", Scope.Enumerate().Select(s => s.GetTypeName()));
                }
            }
        }
        private string _expression;
        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                if (_expression != value) _needsRecompile = true;
                _expression = value;
            }
        }
        private bool _needsRecompile = true;
        public string FixExpression { get; set; }
        public int CompatibilityLevel { get; set; }

        [JsonIgnore]
        public bool IsValid { get; private set; }

        private List<IQueryable> _queries;
        public List<IQueryable> GetQueries(Model model)
        {
            if(_needsRecompile)
            {
                CompileQueries(model);
            }
            return _queries;
        }

        private IQueryable CompileQuery(IQueryable collection)
        {
            var lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(collection.ElementType, typeof(bool), Expression);
            return collection.Provider.CreateQuery(
                        System.Linq.Expressions.Expression.Call(
                            typeof(Queryable), "Where",
                            new Type[] { collection.ElementType },
                            collection.Expression, System.Linq.Expressions.Expression.Quote(lambda))
                        );
        }

        private bool invalidCompatibilityLevel = false;
        private RuleScope errorScope;

        private void CompileQueries(Model model)
        {
            _queries = new List<IQueryable>();
            ErrorMessage = null;

            if (CompatibilityLevel > model.Database.CompatibilityLevel)
            {
                invalidCompatibilityLevel = true;
                IsValid = false;
                return;
            }

            try
            {
                foreach (var scope in Scope.Enumerate())
                {
                    errorScope = scope;
                    var collection = Analyzer.GetCollection(model, scope);
                    _queries.Add(CompileQuery(collection));
                }
                IsValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
                ErrorMessage = ex.Message;
            }
        }

        internal IEnumerable<AnalyzerResult> Analyze(Model model, CancellationToken ct)
        {
            ObjectCount = 0;
            var queries = GetQueries(model);
            if (!IsValid)
            {
                yield return new AnalyzerResult
                {
                    Rule = this,
                    InvalidCompatibilityLevel = invalidCompatibilityLevel,
                    RuleError = ErrorMessage,
                    RuleErrorScope = errorScope
                };
                yield break;
            }

            foreach (var query in queries)
            {
                if (ct.IsCancellationRequested) yield break;
                var results = query.OfType<ITabularNamedObject>().ToList();
                ObjectCount += results.Count;
                foreach (var obj in results)
                {
                    if (ct.IsCancellationRequested) yield break;
                    yield return new AnalyzerResult { Rule = this, Object = obj };
                }
            }
        }

        private void UpdateEnabled(Model model)
        {
            var ignoreRules = new AnalyzerIgnoreRules(model);
            Enabled = !ignoreRules.RuleIDs.Contains(ID);
        }

        public IEnumerable<AnalyzerResult> Analyze(Model model)
        {
            UpdateEnabled(model);
            if (!Enabled)
            {
                yield return new AnalyzerResult
                {
                    Rule = this,
                    RuleIgnored = true
                };
                yield break;
            }

            ObjectCount = 0;
            var queries = GetQueries(model);
            if (!IsValid) {
                yield return new AnalyzerResult
                {
                    Rule = this,
                    InvalidCompatibilityLevel = invalidCompatibilityLevel,
                    RuleError = ErrorMessage,
                    RuleErrorScope = errorScope
                };
                yield break;
            }

            foreach(var query in queries)
            {
                var results = query.OfType<ITabularNamedObject>().ToList();
                ObjectCount += results.Count;
                foreach(var obj in results) yield return new AnalyzerResult { Rule = this, Object = obj };
            }
        }

        public int ObjectCount { get; private set; }

        public string ErrorMessage { get; private set; }
    }

    static public class StandardBestPractices
    {
        static public BestPracticeCollection GetStandardBestPractices()
        {
            var bpc = new BestPracticeCollection();

            // These should be loaded from GitHub instead
            /*
            bpc.Add(new BestPracticeRule
            {
                Name = "Do not summarize key columns",
                ID = "KEYCOLUMNS_SUMMARIZEBY_NONE",
                Scope = RuleScope.CalculatedColumn | RuleScope.CalculatedTableColumn | RuleScope.DataColumn,
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
                Scope = RuleScope.CalculatedColumn | RuleScope.CalculatedTableColumn | RuleScope.DataColumn,
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
            });*/

            return bpc;
        }
    }

    public class RuleScopeConverter: StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.ValueType == typeof(string) && objectType == typeof(RuleScope))
            {
                var types = ((string)reader.Value).Split(',').ToList();

                // For backwards compatibility with rules created when "Column" existed as a RuleScope:
                if (types.Contains("Column"))
                {
                    types.Remove("Column");
                    types.Add("DataColumn");
                    types.Add("CalculatedColumn");
                    types.Add("CalculatedTableColumn");
                }
                // For backwards compatibility with rules created when "DataSource" existed as a RuleScope:
                if (types.Contains("DataSource"))
                {
                    types.Remove("DataSource");
                    types.Add("ProviderDataSource");
                }

                return types.Select(t => RuleScopeHelper.GetScope(t)).Combine();
            }
            else
                return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            base.WriteJson(writer, value, serializer);
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

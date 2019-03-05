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
using System.Collections;
using TabularEditor.UIServices;
using System.Net;

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
        ModelRole               = 0x40000,
        TablePermission         = 0x80000,
        CalculationGroup        = 0x100000,
        CalculationItem         = 0x200000
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
                case RuleScope.CalculationGroup: return typeof(CalculationGroupTable);
                case RuleScope.CalculationItem: return typeof(CalculationItem);
                case RuleScope.TablePermission: return typeof(TablePermission);
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
        public bool Enabled { get; set; } = true;

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

        public void UpdateEnabled(Model model)
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

        public BestPracticeRule Clone()
        {
            return MemberwiseClone() as BestPracticeRule;
        }
        public void AssignFrom(BestPracticeRule other)
        {
            Category = other.Category;
            CompatibilityLevel = other.CompatibilityLevel;
            Description = other.Description;
            Enabled = other.Enabled;
            ErrorMessage = other.ErrorMessage;
            Expression = other.Expression;
            FixExpression = other.FixExpression;
            ID = other.ID;
            IsValid = other.IsValid;
            Name = other.Name;
            ObjectCount = other.ObjectCount;
            Scope = other.Scope;
            Severity = other.Severity;
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

    public class BestPracticeCollection: IEnumerable<BestPracticeRule>, IRuleDefinition
    {
        internal const string BPAAnnotation = "BestPracticeAnalyzer";
        public string FilePath { get; set; }

        public string Url { get; set; }

        [JsonIgnore]
        public bool Internal { get; set; }

        [JsonIgnore]
        public bool AllowEdit { get; set; } = false;

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public List<BestPracticeRule> Rules { get; private set; } = new List<BestPracticeRule>();

        string IRuleDefinition.Name => Name;

        IEnumerable<BestPracticeRule> IRuleDefinition.Rules => Rules;

        private Model model;

        public bool Save()
        {
            if(model != null)
            {
                if (Rules.Count == 0)
                    model.RemoveAnnotation(BPAAnnotation);
                else
                    model.SetAnnotation(BPAAnnotation, SerializeToJson());
                return true;
            }
            else if(!string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    (new FileInfo(FilePath)).Directory.Create();
                    File.WriteAllText(FilePath, SerializeToJson());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static BestPracticeCollection GetCurrentModelCollection(Model model)
        {
            if (model == null) return null;
            
            var result = new BestPracticeCollection();
            result.model = model;
            var localRulesJson = model.GetAnnotation(BPAAnnotation) ?? model.GetAnnotation("BestPractizeAnalyzer"); // Stupid typo in earlier version
            if (!string.IsNullOrEmpty(localRulesJson)) result.Rules = LoadFromJson(localRulesJson);
            result.AllowEdit = true;
            result.Name = "Rules within the current model";
            result.Internal = true;

            return result;
        }

        public static string GetUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 10000;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static BestPracticeCollection GetCollectionFromUrl(string url)
        {
            var result = new BestPracticeCollection();
            result.AllowEdit = false;
            result.Internal = false;
            result.Url = url;
            result.Name = url;

            try
            {
                result.Rules = LoadFromJson(GetUrl(url));
            }
            catch (Exception ex) { }

            return result;
        }

        public static BestPracticeCollection GetLocalUserCollection()
        {
            var localUserFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";
            var result = GetCollectionFromFile(localUserFileName);
            result.Name = "Rules for the local user";
            result.Internal = true;

            return result;
        }

        public static BestPracticeCollection GetLocalMachineCollection()
        {
            var localMachineFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TabularEditor\BPARules.json";
            var result = GetCollectionFromFile(localMachineFileName);
            result.Name = "Rules on the local machine";
            result.Internal = true;

            return result;
        }

        public static BestPracticeCollection GetCollectionFromFile(string filePath)
        {
            var result = new BestPracticeCollection();
            result.Name = filePath;            
            result.FilePath = filePath;

            var fi = new FileInfo(filePath);
            result.AllowEdit = FileSystemHelper.IsDirectoryWritable(fi.DirectoryName);
            result.AddFromJsonFile(filePath);

            return result;
        }

        public void AddFromJsonFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var rules = LoadFromJson(json);
                Rules.AddRange(
                    rules.Where(r => !Rules.Any(rule => rule.ID.Equals(r.ID, StringComparison.InvariantCultureIgnoreCase)))
                    );
            }
            catch
            {

            }
        }

        static private List<BestPracticeRule> LoadFromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<BestPracticeRule>>(json);
            }
            catch (Exception ex)
            {
                return new List<BestPracticeRule>();
            }
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(Rules, Formatting.Indented);
        }

        public BestPracticeRule this[string ruleId]
        {
            get
            {
                return Rules.FirstOrDefault(r => r.ID.Equals(ruleId, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public void Add(BestPracticeRule rule)
        {
            Rules.Add(rule);
        }

        public bool Contains(string Id)
        {
            return Rules.Any(r => r.ID.Equals(Id, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerator<BestPracticeRule> GetEnumerator()
        {
            return ((IEnumerable<BestPracticeRule>)Rules).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<BestPracticeRule>)Rules).GetEnumerator();
        }
    }
}

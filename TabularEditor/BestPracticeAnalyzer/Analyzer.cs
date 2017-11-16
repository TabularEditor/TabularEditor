extern alias json;

using json.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.BestPracticeAnalyzer
{
    public class AnalyzerIgnoreRules
    {
        public HashSet<string> RuleIDs;
        public AnalyzerIgnoreRules(IAnnotationObject obj)
        {
            RuleIDs = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var json = obj.GetAnnotation("BestPracticeAnalyzer_IgnoreRules") ?? obj.GetAnnotation("BestPractizeAnalyzer_IgnoreRules"); // Stupid typo in earlier version
            if(!string.IsNullOrEmpty(json))
            {
                JsonConvert.PopulateObject(json, this);
            }
        }
        public void Save(IAnnotationObject obj)
        {
            obj.RemoveAnnotation("BestPractizeAnalyzer_IgnoreRules"); // Stupid typo in earlier version
            obj.SetAnnotation("BestPracticeAnalyzer_IgnoreRules", JsonConvert.SerializeObject(this), false);
        }
    }

    public class AnalyzerResult
    {
        public bool RuleHasError { get { return !string.IsNullOrEmpty(RuleError); } }
        public bool InvalidCompatibilityLevel { get; set; }
        public string RuleError { get; set; }
        public RuleScope RuleErrorScope { get; set; }
        public string ObjectName
        {
            get
            {
                if (Object is KPI) return (Object as KPI).Measure.DaxObjectFullName + ".KPI";
                return (Object as IDaxObject)?.DaxObjectFullName ?? Object.Name;
            }
        }
        public string RuleName { get { return Rule.Name; } }
        public ITabularNamedObject Object { get; set; }
        public BestPracticeRule Rule { get; set; }
        public bool CanFix { get { return Rule.FixExpression != null; } }
        public void Fix() {
            throw new NotImplementedException();
        }
        public bool Ignored
        {
            get
            {
                var obj = Object as IAnnotationObject;
                if (obj != null)
                {
                    var air = new AnalyzerIgnoreRules(obj);
                    return air.RuleIDs.Contains(Rule.ID);
                }
                return false;
            }
        }
    }

    public class Analyzer: INotifyCollectionChanged
    {
        private Model _model;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public BestPracticeCollection GlobalRules { get; private set; }
        public BestPracticeCollection LocalRules { get; private set; }
        public IEnumerable<BestPracticeRule> AllRules { get { return GlobalRules.Concat(LocalRules); } }


        public string GetUniqueId(string id)
        {
            int suffix = 1;
            var testId = id;
            while(
                GlobalRules.Contains(testId) || 
                LocalRules.Contains(testId)
            )
            {
                suffix++;
                testId = id + "_" + suffix;
            }
            return testId;
        }

        public void AddRule(BestPracticeRule rule, bool global = false)
        {
            rule.ID = GetUniqueId(rule.ID);
            if (global) GlobalRules.Add(rule);
            else LocalRules.Add(rule);

            DoCollectionChanged(NotifyCollectionChangedAction.Add, rule);
        }

        public Model Model { get
            {
                return _model;
            }
            set
            {
                _model = value;
                if (_model != null)
                {
                    var localRulesJson = _model.GetAnnotation("BestPracticeAnalyzer") ?? _model.GetAnnotation("BestPractizeAnalyzer"); // Stupid typo in earlier version
                    if (!string.IsNullOrEmpty(localRulesJson))
                    {
                        LocalRules = BestPracticeCollection.LoadFromJson(localRulesJson);
                    }
                    else LocalRules = new BestPracticeCollection();

                    var ignoreRules = new AnalyzerIgnoreRules(_model);
                    foreach (var rule in LocalRules)
                    {
                        rule.Enabled = !ignoreRules.RuleIDs.Contains(rule.ID);

                        // If the global rules contain a rule with the same ID as the local rules being loaded, ignore it in the global list:
                        var existingGlobalRule = GlobalRules[rule.ID];
                        if (existingGlobalRule != null) GlobalRules.Remove(existingGlobalRule);
                    }
                    foreach (var rule in GlobalRules) rule.Enabled = !ignoreRules.RuleIDs.Contains(rule.ID);
                }
                else
                {
                    LocalRules = new BestPracticeCollection();
                }
                DoCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        private void DoCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }
        private void DoCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        public void IgnoreRule(BestPracticeRule rule, bool ignore = true, IAnnotationObject obj = null)
        {
            if (obj == null) obj = _model;

            var ignoreRules = new AnalyzerIgnoreRules(obj ?? _model);
            if (ignore)
            {
                if (!ignoreRules.RuleIDs.Contains(rule.ID)) ignoreRules.RuleIDs.Add(rule.ID);
            }
            else
            {
                if (ignoreRules.RuleIDs.Contains(rule.ID)) ignoreRules.RuleIDs.Remove(rule.ID);
            }

            ignoreRules.Save(obj);
        }

        public void SaveLocalRulesToModel()
        {
            if (_model == null) return;
            _model.RemoveAnnotation("BestPractizeAnalyzer"); // Stupid typo in earlier version
            _model.SetAnnotation("BestPracticeAnalyzer", LocalRules.SerializeToJson(), false);
        }

        public Analyzer()
        {
            Model = null;

            GlobalRules = new BestPracticeCollection();

            try
            {
                var p1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";
                var p2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TabularEditor\BPARules.json";
                if (File.Exists(p1)) GlobalRules.AddFromJsonFile(p1);
                if (File.Exists(p2)) GlobalRules.AddFromJsonFile(p2);
            }
            catch { }

            //StandardBestPractices.GetStandardBestPractices().SaveToFile(@"c:\Projects\test.json");
        }

        public IEnumerable<AnalyzerResult> AnalyzeAll()
        {
            return Analyze(AllRules);
        }

        public IEnumerable<AnalyzerResult> Analyze(BestPracticeRule rule)
        {
            if (rule.CompatibilityLevel > Model.Database.CompatibilityLevel) yield return new AnalyzerResult { Rule = rule, InvalidCompatibilityLevel = true };

            // Loop through the types of objects in scope for this rule:
            foreach (var currentScope in rule.Scope.Enumerate())
            {

                // Gets a collection of all objects of this type:
                var collection = GetCollection(currentScope);

                LambdaExpression lambda = null;

                bool isError = false;
                string errMessage = string.Empty;

                // Parse the expression specified on the rule (this can fail if the expression is malformed):
                try
                {
                    lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(collection.ElementType, typeof(bool), rule.Expression);
                }
                catch (Exception ex)
                {
                    // Hack, since compiler does not allow to yield return directly from a catch block:
                    isError = true;
                    errMessage = ex.Message;
                }
                if (isError)
                {
                    yield return new AnalyzerResult { Rule = rule, RuleError = errMessage, RuleErrorScope = currentScope };
                }
                else
                {
                    var result = new List<ITabularNamedObject>();
                    try
                    {
                        result = collection.Provider.CreateQuery(
                            Expression.Call(
                                typeof(Queryable), "Where",
                                new Type[] { collection.ElementType },
                                collection.Expression, Expression.Quote(lambda))).OfType<ITabularNamedObject>().ToList();
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        errMessage = ex.Message;
                    }
                    if (isError)
                    {
                        yield return new AnalyzerResult { Rule = rule, RuleError = errMessage, RuleErrorScope = currentScope };
                    }
                    else
                        foreach (var res in result) yield return new AnalyzerResult { Rule = rule, Object = res };
                }
            }
        }

        public IEnumerable<AnalyzerResult> Analyze(IEnumerable<BestPracticeRule> rules)
        {
            if (Model != null)
            {
                return rules.SelectMany(r => Analyze(r));
            }
            return Enumerable.Empty<AnalyzerResult>();
        }



        private IQueryable GetCollection(RuleScope scope)
        {
            switch (scope)
            {
                case RuleScope.KPI:
                    return Model.AllMeasures.Where(m => m.KPI != null).Select(m => m.KPI).AsQueryable();
                case RuleScope.CalculatedColumn:
                    return Model.AllColumns.OfType<CalculatedColumn>().AsQueryable();
                case RuleScope.CalculatedTable:
                    return Model.Tables.OfType<CalculatedTable>().AsQueryable();
                case RuleScope.CalculatedTableColumn:
                    return Model.Tables.OfType<CalculatedTable>().SelectMany(t => t.Columns).OfType<CalculatedTableColumn>().AsQueryable();
                case RuleScope.Culture:
                    return Model.Cultures.AsQueryable();
                case RuleScope.DataColumn:
                    return Model.AllColumns.OfType<DataColumn>().AsQueryable();
                case RuleScope.ProviderDataSource:
                    return Model.DataSources.OfType<ProviderDataSource>().AsQueryable();
                case RuleScope.StructuredDataSource:
                    return Model.DataSources.OfType<StructuredDataSource>().AsQueryable();
                case RuleScope.Hierarchy:
                    return Model.AllHierarchies.AsQueryable();
                case RuleScope.Level:
                    return Model.AllLevels.AsQueryable();
                case RuleScope.Measure:
                    return Model.AllMeasures.AsQueryable();
                case RuleScope.Model:
                    return Enumerable.Repeat(Model, 1).AsQueryable();
                case RuleScope.Partition:
                    return Model.AllPartitions.AsQueryable();
                case RuleScope.Perspective:
                    return Model.Perspectives.AsQueryable();
                case RuleScope.Relationship:
                    return Model.Relationships.OfType<SingleColumnRelationship>().AsQueryable();
                case RuleScope.Table:
                    return Model.Tables.Where(t => !(t is CalculatedTable)).AsQueryable();
                case RuleScope.ModelRole:
                    return Model.Roles.AsQueryable();
                case RuleScope.NamedExpression:
                    return Model.Expressions.AsQueryable();
                case RuleScope.Variation:
                    return Model.AllColumns.SelectMany(c => c.Variations).AsQueryable();
                default:
                    return Enumerable.Empty<TabularNamedObject>().AsQueryable();
            }

            
        }

    }
}

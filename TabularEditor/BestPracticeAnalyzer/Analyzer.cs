using Newtonsoft.Json;
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
            var json = obj.GetAnnotation("BestPractizeAnalyzer_IgnoreRules");
            if(!string.IsNullOrEmpty(json))
            {
                JsonConvert.PopulateObject(json, this);
            }
        }
        public void Save(IAnnotationObject obj)
        {
            obj.SetAnnotation("BestPractizeAnalyzer_IgnoreRules", JsonConvert.SerializeObject(this), false);
        }
    }

    public class AnalyzerResult
    {
        public bool RuleHasError { get { return !string.IsNullOrEmpty(RuleError); } }
        public string RuleError { get; set; }
        public string ObjectName { get { return (Object as IDaxObject)?.DaxObjectFullName ?? Object.GetLinqPath(); } }
        public string RuleName { get { return Rule.Name; } }
        public TabularNamedObject Object { get; set; }
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
                    var localRulesJson = _model.GetAnnotation("BestPractizeAnalyzer");
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
            _model.SetAnnotation("BestPractizeAnalyzer", LocalRules.SerializeToJson(), false);
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
        public Analyzer(Model model)
        {
            Model = model;
        }

        public IEnumerable<AnalyzerResult> Analyze(BestPracticeRule rule)
        {
            // Gets a collection of all objects in scope for this rule:
            var collection = GetCollection(rule.Scope);

            LambdaExpression lambda;

            // Parse the expression specified on the rule (this can fail if the expression is malformed):
            try
            {
                lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(collection.ElementType, typeof(bool), rule.Expression);
            }
            catch (Exception ex)
            {
                return Enumerable.Repeat(new AnalyzerResult { Rule = rule, RuleError = ex.Message }, 1);
            }

            var result = collection.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { collection.ElementType },
                    collection.Expression, Expression.Quote(lambda))).OfType<TabularNamedObject>();

            return result.Select(obj => new AnalyzerResult { Rule = rule, Object = obj });
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
                case RuleScope.CalculatedColumn:
                    return Model.Tables.SelectMany(t => t.Columns).OfType<CalculatedColumn>().AsQueryable();
                case RuleScope.CalculatedTable:
                    return Model.Tables.OfType<CalculatedTable>().AsQueryable();
                case RuleScope.Column:
                    return Model.Tables.SelectMany(t => t.Columns).AsQueryable();
                case RuleScope.Culture:
                    return Model.Cultures.AsQueryable();
                case RuleScope.DataColumn:
                    return Model.Tables.SelectMany(t => t.Columns).OfType<DataColumn>().AsQueryable();
                case RuleScope.DataSource:
                    return Model.DataSources.AsQueryable();
                case RuleScope.Hierarchy:
                    return Model.Tables.SelectMany(t => t.Hierarchies).AsQueryable();
                case RuleScope.Level:
                    return Model.Tables.SelectMany(t => t.Hierarchies).SelectMany(h => h.Levels).AsQueryable();
                case RuleScope.Measure:
                    return Model.Tables.SelectMany(t => t.Measures).AsQueryable();
                case RuleScope.Model:
                    return Enumerable.Repeat(Model, 1).AsQueryable();
                case RuleScope.Partition:
                    return Model.Tables.SelectMany(t => t.Partitions).AsQueryable();
                case RuleScope.Perspective:
                    return Model.Perspectives.AsQueryable();
                case RuleScope.Relationship:
                    return Model.Relationships.AsQueryable();
                case RuleScope.Table:
                    return Model.Tables.AsQueryable();

                default:
                    return Enumerable.Empty<TabularNamedObject>().AsQueryable();
            }
        }

    }
}

extern alias json;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using json.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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
            var json = obj.GetAnnotation(Analyzer.BPAAnnotationIgnore) ?? obj.GetAnnotation("BestPractizeAnalyzer_IgnoreRules"); // Stupid typo in earlier version
            if(!string.IsNullOrEmpty(json))
            {
                JsonConvert.PopulateObject(json, this);
            }
        }
        public void Save(IAnnotationObject obj)
        {
            obj.RemoveAnnotation("BestPractizeAnalyzer_IgnoreRules"); // Stupid typo in earlier version
            obj.SetAnnotation(Analyzer.BPAAnnotationIgnore, JsonConvert.SerializeObject(this));
            UI.UIController.Current.Handler.UndoManager.FlagChange();
        }
    }

    internal class AnalyzerResultsModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        Dictionary<BestPracticeRule, List<AnalyzerResult>> _results;

        public int RuleCount { get; private set; } = 0;
        public int ObjectCount { get; private set; } = 0;
        public int ObjectCountByRule(BestPracticeRule rule)
        {
            if (_results.TryGetValue(rule, out List<AnalyzerResult> results))
                return results.Count;
            return 0;
        }

        public AnalyzerResultsModel()
        {
            _results = new Dictionary<BestPracticeRule, List<AnalyzerResult>>();
        }

        public void Update(IEnumerable<AnalyzerResult> results)
        {
            var newResults = results.Where(r => !r.InvalidCompatibilityLevel && !r.RuleHasError && !r.Ignored)
                .GroupBy(r => r.Rule, r => r).ToDictionary(r => r.Key, r => r.ToList());

            if(!newResults.SelectMany(r => r.Value).SequenceEqual(_results.SelectMany(r => r.Value)))
            {
                _results = newResults;
                RuleCount = _results.Count;
                ObjectCount = _results.Sum(r => r.Value.Count);
                StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty()) return _results.Keys;
            else
            {
                return _results[treePath.LastNode as BestPracticeRule];
            }
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.IsEmpty()) return false;
            if (treePath.LastNode is BestPracticeRule) return false;
            else return true;
        }
    }

    public class AnalyzerResultTooltip : IToolTipProvider
    {
        public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
        {
            if (node.Tag is AnalyzerResult result)
            {
                return (result.Rule.Name);
            }
            return null;
        }
    }

    public class AnalyzerResult
    {
        public bool RuleHasError { get { return !string.IsNullOrEmpty(RuleError); } }
        public bool InvalidCompatibilityLevel { get; set; }
        public string RuleError { get; set; }
        public RuleScope RuleErrorScope { get; set; }
        public string ObjectType => Object.GetTypeName();
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
        internal const string BPAAnnotation = "BestPracticeAnalyzer";
        internal const string BPAAnnotationIgnore = "BestPracticeAnalyzer_IgnoreRules";

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
                    var localRulesJson = _model.GetAnnotation(BPAAnnotation) ?? _model.GetAnnotation("BestPractizeAnalyzer"); // Stupid typo in earlier version
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
            var previousAnnotation = _model.GetAnnotation(BPAAnnotation);
            var newAnnotation = LocalRules.SerializeToJson();
            _model.SetAnnotation(BPAAnnotation, newAnnotation);
            if (previousAnnotation != newAnnotation) UI.UIController.Current.Handler.UndoManager.FlagChange();
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
            return rule.Analyze(Model);
        }

        public IEnumerable<AnalyzerResult> Analyze(IEnumerable<BestPracticeRule> rules)
        {
            if (Model != null)
            {
                return rules.SelectMany(r => r.Analyze(Model));
            }
            return Enumerable.Empty<AnalyzerResult>();
        }

        internal List<AnalyzerResult> AnalyzeAll(CancellationToken ct)
        {
            var results = new List<AnalyzerResult>();
            if(Model != null)
            {
                foreach(var rule in AllRules)
                {
                    if (ct.IsCancellationRequested) return new List<AnalyzerResult>();
                    results.AddRange(rule.Analyze(Model));
                }
            }
            if (ct.IsCancellationRequested) return new List<AnalyzerResult>();
            return results;
        }

        private IQueryable GetCollection(RuleScope scope)
        {
            return GetCollection(Model, scope);
        }

        static public IQueryable GetCollection(Model model, RuleScope scope)
        {
            switch (scope)
            {
                case RuleScope.KPI:
                    return model.AllMeasures.Where(m => m.KPI != null).Select(m => m.KPI).AsQueryable();
                case RuleScope.CalculatedColumn:
                    return model.AllColumns.OfType<CalculatedColumn>().AsQueryable();
                case RuleScope.CalculatedTable:
                    return model.Tables.OfType<CalculatedTable>().AsQueryable();
                case RuleScope.CalculatedTableColumn:
                    return model.Tables.OfType<CalculatedTable>().SelectMany(t => t.Columns).OfType<CalculatedTableColumn>().AsQueryable();
                case RuleScope.Culture:
                    return model.Cultures.AsQueryable();
                case RuleScope.DataColumn:
                    return model.AllColumns.OfType<DataColumn>().AsQueryable();
                case RuleScope.ProviderDataSource:
                    return model.DataSources.OfType<ProviderDataSource>().AsQueryable();
                case RuleScope.StructuredDataSource:
                    return model.DataSources.OfType<StructuredDataSource>().AsQueryable();
                case RuleScope.Hierarchy:
                    return model.AllHierarchies.AsQueryable();
                case RuleScope.Level:
                    return model.AllLevels.AsQueryable();
                case RuleScope.Measure:
                    return model.AllMeasures.AsQueryable();
                case RuleScope.Model:
                    return Enumerable.Repeat(model, 1).AsQueryable();
                case RuleScope.Partition:
                    return model.AllPartitions.AsQueryable();
                case RuleScope.Perspective:
                    return model.Perspectives.AsQueryable();
                case RuleScope.Relationship:
                    return model.Relationships.OfType<SingleColumnRelationship>().AsQueryable();
                case RuleScope.Table:
                    return model.Tables.Where(t => !(t is CalculatedTable)).AsQueryable();
                case RuleScope.ModelRole:
                    return model.Roles.AsQueryable();
                case RuleScope.NamedExpression:
                    return model.Expressions.AsQueryable();
                case RuleScope.Variation:
                    return model.AllColumns.SelectMany(c => c.Variations).AsQueryable();
                default:
                    return Enumerable.Empty<TabularNamedObject>().AsQueryable();
            }

            
        }

    }
}

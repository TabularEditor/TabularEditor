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
            if (obj != null)
            {
                var json = obj.GetAnnotation(Analyzer.BPAAnnotationIgnore) ?? obj.GetAnnotation("BestPractizeAnalyzer_IgnoreRules"); // Stupid typo in earlier version
                if (!string.IsNullOrEmpty(json))
                {
                    JsonConvert.PopulateObject(json, this);
                }
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
        public event EventHandler<EventArgs> UpdateComplete;

        Dictionary<BestPracticeRule, List<AnalyzerResult>> _results;

        public int RuleCount { get; private set; } = 0;
        public int ObjectCount { get; private set; } = 0;
        public int ObjectCountByRule(BestPracticeRule rule)
        {
            return ResultsByRule(rule).Count(r => !r.Ignored);
        }

        public List<AnalyzerResult> ResultsByRule(BestPracticeRule rule)
        {
            if (_results.TryGetValue(rule, out List<AnalyzerResult> results))
                return results;
            return new List<AnalyzerResult>();
        }

        public AnalyzerResultsModel()
        {
            _results = new Dictionary<BestPracticeRule, List<AnalyzerResult>>();
        }

        public void Update(IEnumerable<AnalyzerResult> results)
        {
            var newResults = results.Where(r => !r.InvalidCompatibilityLevel && !r.RuleHasError)
                .GroupBy(r => r.Rule, r => r).ToDictionary(r => r.Key, r => r.Where(res => !res.RuleIgnored).ToList());

            if(!newResults.SelectMany(r => r.Value).SequenceEqual(_results.SelectMany(r => r.Value)))
            {
                _results = newResults;
                RuleCount = _results.Count;
                ObjectCount = _results.Sum(r => r.Value.Count);
                OnStructureChanged();

                UpdateComplete?.Invoke(this, new EventArgs());
            }
        }

        private void OnStructureChanged()
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
        }

        public void Clear()
        {
            _results.Clear();
            OnStructureChanged();
        }

        private bool _showIgnored = false;
        public bool ShowIgnored
        {
            get { return _showIgnored; }
            set
            {
                if (_showIgnored == value) return;
                _showIgnored = value;
                OnStructureChanged();
            }
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty()) return _results.Keys.Where(r => r.Enabled || ShowIgnored);
            else
            {
                if (ShowIgnored)
                    return _results[treePath.LastNode as BestPracticeRule];
                else
                    return _results[treePath.LastNode as BestPracticeRule].Where(r => !r.Ignored);
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
                if (string.IsNullOrWhiteSpace(result.Rule.Description))
                    return result.Rule.Name;
                else
                    return result.Rule.Description
                        .Replace("%object%", result.ObjectName)
                        .Replace("%objectname%", result.Object.Name)
                        .Replace("%objecttype%", result.Object.GetTypeName());
            }
            return null;
        }
    }

    public class AnalyzerResult
    {
        public bool RuleIgnored { get; set; }
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
        internal const string BPAAnnotationIgnore = "BestPracticeAnalyzer_IgnoreRules";
        internal const string BPAAnnotationExternalRules = "BestPracticeAnalyzer_ExternalRuleFiles";

        private Model _model;

        public string GetUniqueId(string prefix)
        {
            prefix = !string.IsNullOrWhiteSpace(prefix) ? "NEW_RULE" : prefix;
            var result = prefix;
            var suffix = 0;
            while(EffectiveRules.Any(r => r.ID.EqualsI(result)))
            {
                suffix++;
                result = $"{prefix}_{suffix}";
            }
            return result;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public BestPracticeCollection LocalMachineRules { get; private set; }
        public BestPracticeCollection LocalUserRules { get; private set; }
        public BestPracticeCollection ModelRules { get; private set; }
        public List<BestPracticeCollection> ExternalRuleCollections { get; private set; }

        public IEnumerable<BestPracticeRule> EffectiveRules {
            get {
                var rulePrecedence = new Dictionary<string, BestPracticeRule>(StringComparer.InvariantCultureIgnoreCase);
                if (LocalMachineRules != null) foreach (var rule in LocalMachineRules) rulePrecedence[rule.ID] = rule;
                if (LocalUserRules != null) foreach (var rule in LocalUserRules) rulePrecedence[rule.ID] = rule;
                for(int i = ExternalRuleCollections.Count - 1; i >= 0; i--)
                    foreach (var rule in ExternalRuleCollections[i]) rulePrecedence[rule.ID] = rule;
                if (ModelRules != null) foreach (var rule in ModelRules) rulePrecedence[rule.ID] = rule;

                return rulePrecedence.Values;
            }
        }

        public IEnumerable<BestPracticeCollection> Collections
        {
            get
            {
                foreach (var rc in ExternalRuleCollections) yield return rc;
                if (ModelRules != null) yield return ModelRules;
                if (LocalUserRules != null) yield return LocalUserRules;
                if (LocalMachineRules != null) yield return LocalMachineRules;
            }
        }

        public IEnumerable<BestPracticeRule> AllRules
        {
            get
            {
                foreach (var externalRuleCollection in ExternalRuleCollections)
                    foreach (var rule in externalRuleCollection) yield return rule;
                if (LocalMachineRules != null) foreach (var rule in LocalMachineRules) yield return rule;
                if (LocalUserRules != null) foreach (var rule in LocalUserRules) yield return rule;
                if (ModelRules != null) foreach (var rule in ModelRules) yield return rule;
            }
        }

        public BestPracticeCollection EffectiveCollectionForRule(string ruleId)
        {
            foreach (var externalRuleCollection in ExternalRuleCollections)
                if (externalRuleCollection.Any(r => r.ID.EqualsI(ruleId))) return externalRuleCollection;
            if (ModelRules != null && ModelRules.Any(r => r.ID.EqualsI(ruleId))) return ModelRules;
            if (LocalUserRules != null && LocalUserRules.Any(r => r.ID.EqualsI(ruleId))) return LocalUserRules;
            if (LocalMachineRules != null && LocalMachineRules.Any(r => r.ID.EqualsI(ruleId))) return LocalMachineRules;
            return null;
        }

        public void SaveExternalRuleCollections()
        {
            if (_model != null)
            {
                if (ExternalRuleCollections?.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(ExternalRuleCollections.Select(rc => string.IsNullOrEmpty(rc.FilePath) ? rc.Url : rc.FilePath).ToList());
                    _model.SetAnnotation(BPAAnnotationExternalRules, json);
                }
                else
                    _model.RemoveAnnotation(BPAAnnotationExternalRules);
            }
        }

        public void LoadExternalRuleCollections()
        {
            ExternalRuleCollections = new List<BestPracticeCollection>();
            if (_model != null)
            {
                var externalRuleCollectionsJson = _model.GetAnnotation(BPAAnnotationExternalRules);
                if (externalRuleCollectionsJson != null)
                {
                    try
                    {
                        var externalRuleFilePaths = JsonConvert.DeserializeObject<List<string>>(externalRuleCollectionsJson);
                        foreach (var filePath in externalRuleFilePaths)
                        {
                            try
                            {
                                if (filePath.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromUrl(filePath));
                                }
                                else
                                    ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromFile(filePath));
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
        }

        public Model Model { get
            {
                return _model;
            }
            set
            {
                _model = value;
                LoadModelRules();
                LoadExternalRuleCollections();
                UpdateEnabled();
                DoCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public void LoadModelRules()
        {
            ModelRules = BestPracticeCollection.GetCurrentModelCollection(_model);
        }

        public void UpdateEnabled()
        {
            var ignoreRules = new AnalyzerIgnoreRules(Model);
            foreach (var rule in AllRules) rule.Enabled = !ignoreRules.RuleIDs.Contains(rule.ID);
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
            if (obj == null) rule.Enabled = !ignore;
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

        /*
        public void SaveLocalRulesToModel()
        {
            if (_model == null) return;
            _model.RemoveAnnotation("BestPractizeAnalyzer"); // Stupid typo in earlier version
            var previousAnnotation = _model.GetAnnotation(BPAAnnotation);
            var newAnnotation = ModelRules.SerializeToJson();
            _model.SetAnnotation(BPAAnnotation, newAnnotation);
            if (previousAnnotation != newAnnotation) UI.UIController.Current.Handler.UndoManager.FlagChange();
        }*/

        public Analyzer()
        {
            Model = null;

            LoadInternalRules();
        }

        public void LoadInternalRules()
        {
            LocalMachineRules = BestPracticeCollection.GetLocalMachineCollection();
            LocalUserRules = BestPracticeCollection.GetLocalUserCollection();
        }

        public IEnumerable<AnalyzerResult> AnalyzeAll()
        {
            return Analyze(EffectiveRules);
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
                foreach(var rule in EffectiveRules)
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

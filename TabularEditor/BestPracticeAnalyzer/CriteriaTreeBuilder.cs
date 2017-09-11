using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.BestPracticeAnalyzer
{
    /// <summary>
    /// Represents a collection of criterias that are AND'ed or OR'ed together.
    /// </summary>
    public class MultiNode: BaseNode
    {
        public override bool IsBlank
        {
            get
            {
                return Children.All(c => c.IsBlank);
            }
        }

        public enum Operator
        {
            All,
            Any
        }

        public bool Not { get; set; }

        public Operator Mode;

        public override string ToString()
        {
            var sep = Mode == Operator.All ? " and " : " or ";

            return string.Format("{0}({1})", Not ? " not " : "", string.Join(sep, Children.Where(c => !c.IsBlank)));
        }
    }

    public class CriteriaNode: BaseNode
    {
        public CriteriaNode()
        {

        }

        public CriteriaNode(MemberExpression me)
        {
            Op = Operator.IsTrue;
            Expression = me.Expression;

            GetPropertyPath(me).ForEach(ppi => this.PropertyPath.Push(ppi));
        }

        public CriteriaNode(MemberExpression me, ConstantExpression ce, ExpressionType nodeType): this(me)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    Op = Operator.E; break;
                case ExpressionType.LessThan:
                    Op = Operator.LT; break;
                case ExpressionType.LessThanOrEqual:
                    Op = Operator.LToE; break;
                case ExpressionType.GreaterThan:
                    Op = Operator.GT; break;
                case ExpressionType.GreaterThanOrEqual:
                    Op = Operator.GToE; break;
                case ExpressionType.NotEqual:
                    Op = Operator.NE; break;
            }

            Value = ce.Value;
        }

        public class PropertyPathItem
        {
            public MemberInfo Member;
            //public string Expression;
        }

        static public List<PropertyPathItem> GetPropertyPath(MemberExpression me)
        {
            var list = new List<PropertyPathItem>();

            list.Add(new PropertyPathItem { Member = me.Member });

            while (me.Expression is MemberExpression)
            {
                list.Insert(0, new PropertyPathItem { Member = (me.Expression as MemberExpression).Member });
                me = me.Expression as MemberExpression;
            }

            return list;
        }

        public Type LastType
        {
            get
            {
                if (IsBlank) return null;
                var mi = PropertyPath.Peek().Member;
                return (mi as PropertyInfo)?.PropertyType ?? (mi as MethodInfo)?.ReturnType;
            }
        }

        public override bool IsBlank { get { return PropertyPath.Count == 0 || PropertyPath.Peek().Member == null; } }

        public IEnumerable<Operator> ValidOperators {
            get
            {
                if (IsBlank) return Enumerable.Empty<Operator>();

                if (LastType == typeof(string))
                {
                    return Enum.GetValues(typeof(Operator)).Cast<int>().Where(v => v >= 10 && v < 30).Cast<Operator>();
                } else if (LastType.IsEnum)
                {
                    return new[] { Operator.E, Operator.NE };
                } else if (LastType == typeof(bool))
                {
                    return new[] { Operator.IsTrue, Operator.IsFalse };
                } else if (LastType.IsPrimitive)
                {
                    return Enum.GetValues(typeof(Operator)).Cast<int>().Where(v => v >= 10 && v < 20).Cast<Operator>();
                } else
                {
                    return new[] { Operator.IsNull, Operator.IsNotNull };
                }
            }
        }

        public Stack<PropertyPathItem> PropertyPath { get; protected set; } = new Stack<PropertyPathItem>();
        public Type CriteriaType { get { return IsBlank ? null : LastType; } }

        public override string ToString()
        {
            var res = PropertyPathString;
            if (Op == Operator.IsTrue) return res;

            // If the member accessed is a string or an enum, add pings:
            var v = CriteriaType.IsEnum || CriteriaType == typeof(string) ? "\"" + Value + "\"" : Value;

            if (CriteriaType == typeof(string)) res = string.Format(GetStringExpression(Op), res, v);
            else res = string.Format(Op.GetExpression(), res, v);

            return res;
        }

        public bool CaseSensitive { get; set; }

        private string GetStringExpression(Operator op)
        {
            switch (op)
            {
                case Operator.NotStartsWith:
                    return "not {0}.StartsWith({1}," + (!CaseSensitive).ToString() + ",null)";
                case Operator.StartsWith:
                    return "{0}.StartsWith({1}," + (!CaseSensitive).ToString() + ",null)";
                case Operator.NotEndsWith:
                    return "not {0}.EndsWith({1}," + (!CaseSensitive).ToString() + ",null)";
                case Operator.EndsWith:
                    return "{0}.EndsWith({1}," + (!CaseSensitive).ToString() + ",null)";
                case Operator.Contains:
                    return "{0}.IndexOf({1}, StringComparison.InvariantCulture" + (CaseSensitive ? "" : "IgnoreCase") + ") >= 0";
                case Operator.NotContains:
                    return "{0}.IndexOf({1}, StringComparison.InvariantCulture" + (CaseSensitive ? "" : "IgnoreCase") + ") < 0";
                case Operator.NotRegExMatch:
                    return "not Regex.IsMatch({0},{1})";
                case Operator.RegExMatch:
                    return "Regex.IsMatch({0},{1})";
                case Operator.E:
                case Operator.GT:
                case Operator.GToE:
                case Operator.LT:
                case Operator.LToE:
                case Operator.NE:
                    var res = "String.Compare({0},{1}," + (!CaseSensitive).ToString() + ")";
                    if (op == Operator.E) return res += " = 0";
                    if (op == Operator.GT) return res += " > 0";
                    if (op == Operator.GToE) return res += " >= 0";
                    if (op == Operator.LT) return res += " < 0";
                    if (op == Operator.LToE) return res += " <= 0";
                    else return res += " <> 0";
                case Operator.IsBlank:
                    return "String.IsNullOrEmpty({0})";
                case Operator.IsNotBlank:
                    return "not String.IsNullOrEmpty({0})";
            }
            throw new NotSupportedException();
        }

        public string PropertyPathString
        {
            get
            {
                return string.Join(".", PropertyPath.Reverse().Select(p => p.Member?.Name));
            }
        }

        public Operator Op { get; set; }
        public object Value { get; set; }
    }

    public abstract class BaseNode
    {
        public virtual bool IsBlank { get; }

        private List<BaseNode> _children = new List<BaseNode>();
        public List<BaseNode> Children
        {
            get
            {
                if (!(this is MultiNode)) throw new InvalidOperationException("Can only access child nodes of a MultiNode.");
                return _children;
            }
        }

        public void AddNode(BaseNode node, int index = -1)
        {
            if (!(this is MultiNode)) throw new InvalidOperationException("Can only add child nodes to a MultiNode.");

            if (node == null) return;
            if (index == -1) Children.Add(node);
            else Children.Insert(index, node);
            if (node.HasUnparsedChilds) HasUnparsedChilds = true;
            node.Parent = this;
        }
        private bool _hasUnparsedChilds;
        public bool HasUnparsedChilds
        {
            get
            {
                return _hasUnparsedChilds;
            }
            set
            {
                if (Parent != null) Parent.HasUnparsedChilds = true;
                _hasUnparsedChilds = value;
            }
        }

        public BaseNode Parent;
        public Expression Expression { get; set; }

        /// <summary>
        /// Injects a new MultiNode into the tree, in place of the current node, and makes the current node a child of the MultiNode.
        /// </summary>
        /// <returns>The injected MultiNode</returns>
        public MultiNode PromoteToMulti()
        {
            var multi = new MultiNode();
            if(Parent != null)
            {
                var i = Parent.Children.IndexOf(this);
                Parent.Children.Remove(this);
                Parent.AddNode(multi, i);
            }
            multi.AddNode(this);

            return multi;
        }
    }

    public class UnparsedNode: BaseNode
    {
        public UnparsedNode()
        {
            HasUnparsedChilds = true;
        }

        public bool Negate { get; set; }
        public override string ToString()
        {
            return base.ToString() + Expression.ToString();
        }
    }

    public class CriteriaTreeBuilder
    {
        private enum NodeType
        {
            Multi,
            Negatable,
            TrueCriteria,
            LeftCriteria,
            RightCriteria,
            MethodCriteria,
            Unparsed,
            StringCompareCriteria,
            StringContainsCriteria
        }
        static private MultiNode BuildMulti(BinaryExpression be, BaseNode parent, bool negate)
        {
            var result = new MultiNode();
            result.Not = negate;
            result.Mode = be.NodeType == ExpressionType.AndAlso ? MultiNode.Operator.All : MultiNode.Operator.Any;
            if ((parent as MultiNode)?.Mode == result.Mode)
            {
                (parent as MultiNode).AddNode(BuildFromExpression(be.Left, parent));
                (parent as MultiNode).AddNode(BuildFromExpression(be.Right, parent));
                return null;
            }
            else
            {
                result.AddNode(BuildFromExpression(be.Left, result));
                result.AddNode(BuildFromExpression(be.Right, result));
            }

            return result;
        }

        static private BaseNode BuildNegatable(UnaryExpression ue)
        {
            var result = BuildFromExpression(ue.Operand, null, true);
            return result;
        }

        static private CriteriaNode BuildMethodCriteria(MethodCallExpression mce)
        {
            CriteriaNode result;

            // Support for standard string methods (contains, ends with, starts with, etc.):
            if (mce.Method.DeclaringType == typeof(string))
            {
                var res = new CriteriaNode();

                switch (mce.Method.Name)
                {
                    case "StartsWith":
                    case "EndsWith":
                        res.CaseSensitive =
                            !(mce.Arguments.Count == 3 &&
                            mce.Arguments[1] is ConstantExpression &&
                            (bool)(mce.Arguments[1] as ConstantExpression).Value);
                        res.Value = (mce.Arguments[0] as ConstantExpression).Value.ToString();
                        res.Op = mce.Method.Name == "StartsWith" ? Operator.StartsWith : Operator.EndsWith;
                        CriteriaNode.GetPropertyPath(mce.Object as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));
                        break;
                    case "IsNullOrEmpty":
                        res.CaseSensitive = false;
                        res.Op = Operator.IsBlank;
                        CriteriaNode.GetPropertyPath(mce.Arguments[0] as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));
                        break;
                    case "Contains":
                        // The string Contains() method is always case sensitive:
                        res.CaseSensitive = true;
                        res.Value = (mce.Arguments[0] as ConstantExpression).Value.ToString();
                        res.Op = Operator.Contains;
                        CriteriaNode.GetPropertyPath(mce.Object as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));
                        break;
                    case "Compare":
                        res.CaseSensitive = false;
                        if (mce.Arguments.Count == 3) res.CaseSensitive = !(bool)(mce.Arguments[2] as ConstantExpression).Value;
                        res.Value = (mce.Arguments[1] as ConstantExpression).Value.ToString();
                        CriteriaNode.GetPropertyPath(mce.Arguments[0] as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));


                        break;
                    case "IndexOf":
                        // For case insensitive comparisons, the IndexOf method can be used:
                        res.CaseSensitive = true;
                        if (
                            mce.Arguments.Count == 2 &&
                            mce.Arguments[1] is ConstantExpression &&
                            (mce.Arguments[1] as ConstantExpression).Type == typeof(StringComparison))
                        {
                            var ct = (StringComparison)(mce.Arguments[1] as ConstantExpression).Value;
                            switch (ct)
                            {
                                case StringComparison.CurrentCultureIgnoreCase:
                                case StringComparison.InvariantCultureIgnoreCase:
                                case StringComparison.OrdinalIgnoreCase:
                                    res.CaseSensitive = false;
                                    break;
                            }
                        }
                        res.Value = (mce.Arguments[0] as ConstantExpression).Value.ToString();
                        res.Op = Operator.Contains;
                        CriteriaNode.GetPropertyPath(mce.Object as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));
                        break;
                }

                result = res;
            }
            else if (mce.Method.DeclaringType == typeof(System.Text.RegularExpressions.Regex))
            {
                var res = new CriteriaNode();

                if (mce.Method.Name == "IsMatch" && mce.Arguments.Count == 2 && mce.Arguments[1].Type == typeof(string))
                {
                    res.CaseSensitive = true;
                    res.Op = Operator.RegExMatch;
                    res.Value = (mce.Arguments[1] as ConstantExpression).Value.ToString();

                    CriteriaNode.GetPropertyPath(mce.Arguments[0] as MemberExpression).ForEach(ppi => res.PropertyPath.Push(ppi));
                }
                else return null;

                result = res;
            }
            else
                // Other methods not supported:
                return null;

            return result;
        }

        static private NodeType GetConditionType(Expression expr)
        {
            if (expr is UnaryExpression && expr.NodeType == ExpressionType.Not) return NodeType.Negatable;

            if (expr is BinaryExpression &&
                (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)) return NodeType.Multi;

            if (expr is MemberExpression && (expr.Type == typeof(bool))) return NodeType.TrueCriteria;
            if (expr is BinaryExpression)
            {
                var be = expr as BinaryExpression;
                if (be.Right is ConstantExpression && be.Left is MemberExpression) return NodeType.LeftCriteria;
                if (be.Left is ConstantExpression && be.Right is MemberExpression) return NodeType.RightCriteria;
                if (be.Left is MethodCallExpression && be.Right is ConstantExpression)
                {
                    var bm = (be.Left as MethodCallExpression).Method;
                    if (bm.Name == "Compare") return NodeType.StringCompareCriteria;
                    if (bm.Name == "IndexOf") return NodeType.MethodCriteria;
                    if (bm.Name == "Contains") return NodeType.MethodCriteria;
                }
            }
            if (expr is MethodCallExpression && expr.Type == typeof(bool)) return NodeType.MethodCriteria;

            return NodeType.Unparsed;
        }

        static public BaseNode BuildFromExpression(Expression expr, BaseNode parent = null, bool negate = false)
        {
            CriteriaNode result;

            switch(GetConditionType(expr))
            {
                // Structural nodes:
                case NodeType.Multi:
                    return BuildMulti(expr as BinaryExpression, parent, negate);
                case NodeType.Negatable:
                    return BuildNegatable(expr as UnaryExpression);
                default:
                    if (parent != null) parent.HasUnparsedChilds = true;
                    return new UnparsedNode { Expression = expr, Negate = negate };

                // Criteria Nodes (leafs):
                case NodeType.TrueCriteria:
                    result = new CriteriaNode(expr as MemberExpression); break;
                case NodeType.LeftCriteria:
                    var be = expr as BinaryExpression;
                    result = new CriteriaNode(be.Left as MemberExpression, be.Right as ConstantExpression, expr.NodeType); break;
                case NodeType.RightCriteria:
                    var be2 = expr as BinaryExpression;
                    var nt = expr.NodeType;
                    if (nt == ExpressionType.LessThan) nt = ExpressionType.GreaterThan;
                    else if (nt == ExpressionType.LessThanOrEqual) nt = ExpressionType.GreaterThanOrEqual;
                    else if (nt == ExpressionType.GreaterThan) nt = ExpressionType.LessThan;
                    else if (nt == ExpressionType.GreaterThanOrEqual) nt = ExpressionType.LessThanOrEqual;
                    result = new CriteriaNode(be2.Right as MemberExpression, be2.Left as ConstantExpression, nt); break;
                case NodeType.MethodCriteria:
                    result = BuildMethodCriteria(expr as MethodCallExpression); break;

                case NodeType.StringCompareCriteria:
                    var be3 = expr as BinaryExpression;
                    result = BuildMethodCriteria(be3.Left as MethodCallExpression);
                    switch (be3.NodeType)
                    {
                        case ExpressionType.Equal: result.Op = Operator.E; break;
                        case ExpressionType.LessThan: result.Op = Operator.LT; break;
                        case ExpressionType.LessThanOrEqual: result.Op = Operator.LToE; break;
                        case ExpressionType.GreaterThan: result.Op = Operator.GT; break;
                        case ExpressionType.GreaterThanOrEqual: result.Op = Operator.GToE; break;
                        case ExpressionType.NotEqual: result.Op = Operator.NE; break;
                        default: throw new NotSupportedException();
                    }
                    break;
            }

            if (negate) result.Op = result.Op.Negate();
            return result;
        }
    }
}

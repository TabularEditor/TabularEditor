using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.BestPracticeAnalyzer
{
    public enum Operator
    {
        // Boolean operators:
        IsTrue = 0,
        IsFalse = 1,

        // Object operators:
        IsNull = 2,
        IsNotNull = 3,

        // Generic operators:
        E = 10,
        NE = 11,
        LT = 12,
        GToE = 13,
        GT = 14,
        LToE = 15,

        // String operators:
        StartsWith = 20,
        NotStartsWith = 21,
        EndsWith = 22,
        NotEndsWith = 23,
        Contains = 24,
        NotContains = 25,
        RegExMatch = 26,
        NotRegExMatch = 27,
        IsBlank = 28,
        IsNotBlank = 29
    }

    public class OperatorItem
    {
        public Operator Operator { get; private set; }
        public OperatorItem(Operator op)
        {
            Operator = op;
        }
        public override string ToString()
        {
            return Operator.GetName();
        }
        public string Name { get { return ToString(); } }
    }

    public static class OperatorHelper
    {
        public static string[] AsNames(this IEnumerable<Operator> operators)
        {
            return operators.Select(op => op.GetName()).ToArray();
        }

        public static Operator Negate(this Operator op)
        {
            // For operator N, where N is even, the inverse operator is always N + 1.
            // For operator N, where N is odd, the inverse operator is always N - 1.
            return ((int)op % 2 == 0) ? op + 1 : op - 1;
        }

        public static string GetExpression(this Operator op)
        {
            switch (op)
            {
                case Operator.E:            return "{0} = {1}";
                case Operator.GT:           return "{0} > {1}";
                case Operator.GToE:         return "{0} >= {1}";
                case Operator.LT:           return "{0} < {1}";
                case Operator.LToE:         return "{0} <= {1}";
                case Operator.NE:           return "{0} <> {1}";
                case Operator.IsTrue:       return "{0}";
                case Operator.IsFalse:      return "not {0}";
                case Operator.IsNull:       return "{0} = null";
                case Operator.IsNotNull:    return "{0} <> null";
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool CaseSensitivity(this Operator op)
        {
            return (int)op >= 10 && (int)op <= 27;
        }

        public static string GetName(this Operator op)
        {
            switch (op)
            {
                case Operator.E: return "=";
                case Operator.GT: return ">";
                case Operator.GToE: return ">=";
                case Operator.LT: return "<";
                case Operator.LToE: return "<=";
                case Operator.NE: return "<>";
                case Operator.StartsWith: return "starts with";
                case Operator.EndsWith: return "ends with";
                case Operator.Contains: return "contains";
                case Operator.RegExMatch: return "matches";
                case Operator.NotStartsWith: return "doesn't start with";
                case Operator.NotEndsWith: return "doesn't end with";
                case Operator.NotContains: return "doesn't contain";
                case Operator.NotRegExMatch: return "doesn't match";
                case Operator.IsBlank: return "is blank";
                case Operator.IsNotBlank: return "is not blank";
                case Operator.IsTrue: return "is true";
                case Operator.IsFalse: return "is false";
                case Operator.IsNull: return "is null";
                case Operator.IsNotNull: return "is not null";
            }
            return "";
        }

        public static Operator Parse(string op)
        {
            switch (op)
            {
                case "=": return Operator.E;
                case ">": return Operator.GT;
                case ">=": return Operator.GToE;
                case "<": return Operator.LT;
                case "<=": return Operator.LToE;
                case "<>": return Operator.NE;
                case "starts with": return Operator.StartsWith;
                case "ends with": return Operator.EndsWith;
                case "contains": return Operator.Contains;
                case "matches": return Operator.RegExMatch;
                case "doesn't start with": return Operator.NotStartsWith;
                case "doesn't end with": return Operator.NotEndsWith;
                case "doesn't contain": return Operator.NotContains;
                case "doesn't match": return Operator.NotRegExMatch;
                case "is blank": return Operator.IsBlank;
                case "is not blank": return Operator.IsNotBlank;
                case "is true": return Operator.IsTrue;
                case "is false": return Operator.IsFalse;
                case "is null": return Operator.IsNull;
                case "is not null": return Operator.IsNotNull;
            }
            return Operator.IsTrue;
        }

        public static bool IsUnary(this Operator op)
        {
            if ((int)op < 10) return true;

            switch(op)
            {
                case Operator.IsBlank:
                case Operator.IsNotBlank:
                    return true;
                default:
                    return false;
            }
        }
    }
}

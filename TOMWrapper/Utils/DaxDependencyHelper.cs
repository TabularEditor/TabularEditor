using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TextServices;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class DaxDependencyHelper
    {
        /// <summary>
        /// Return a list of tokens representing the DAX expression on the current object.
        /// </summary>
        [IntelliSense("Return a list of tokens representing the DAX expression on the current object.")]
        public static IList<DaxToken> Tokenize(this IDaxDependantObject obj)
        {
            return Tokenize(obj, includeHidden: true);
        }

        /// <summary>
        /// Return a list of tokens representing the DAX expression on the current object.
        /// </summary>
        [IntelliSense("Return a list of tokens representing the DAX expression on the current object.")]
        public static IList<DaxToken> Tokenize(this IDaxDependantObject obj, bool includeHidden)
        {
            return Tokenize(obj, DAXProperty.Expression, includeHidden);
        }

        /// <summary>
        /// Return a list of tokens representing the specified DAX property on the current object.
        /// </summary>
        [IntelliSense("Return a list of tokens representing the specified DAX property on the current object.")]
        public static IList<DaxToken> Tokenize(this IDaxDependantObject obj, DAXProperty property)
        {
            return Tokenize(obj, property, includeHidden: true);
        }

        /// <summary>
        /// Return a list of tokens representing the specified DAX property on the current object.
        /// </summary>
        [IntelliSense("Return a list of tokens representing the specified DAX property on the current object.")]
        public static IList<DaxToken> Tokenize(this IDaxDependantObject obj, DAXProperty property, bool includeHidden)
        {
            return Tokenize(obj.GetDAX(property), includeHidden);
        }

        /// <summary>
        /// Return a list of tokens representing the specified DAX expression
        /// </summary>
        [IntelliSense("Return a list of tokens representing the specified DAX expression.")]
        public static IList<DaxToken> Tokenize(string dax, bool includeHidden)
        {
            var result = new List<DaxToken>();
            if (string.IsNullOrEmpty(dax)) return result;
            var lexer = new DAXLexer(new DAXCharStream(dax, false));
            lexer.RemoveErrorListeners();
            var lexerTokens = lexer.GetAllTokens();
            if (includeHidden)
            {
                for (int i = 0; i < lexerTokens.Count; i++)
                {
                    result.Add(new DaxToken(lexerTokens[i], result, i));
                }
            }
            else
            {
                var i = 0;
                foreach (var token in lexerTokens)
                {
                    if (token.Channel == DAXLexer.COMMENTS_CHANNEL || token.Type == DAXLexer.WHITESPACES) continue;
                    result.Add(new DaxToken(token, result, i++));
                }
            }

            return result;
        }

        public static IEnumerable<DAXProperty> GetDAXProperties(this IDaxDependantObject obj)
        {
            if (obj is Measure || obj is CalculatedColumn || obj is CalculatedTable) yield return DAXProperty.Expression;
            if (TabularModelHandler.Singleton.CompatibilityLevel >= 1400)
            {
                if (obj is Measure)
                {
                    if (TabularModelHandler.Singleton.CompatibilityLevel >= 1601) yield return DAXProperty.FormatStringExpression;
                    yield return DAXProperty.DetailRowsExpression;
                }
                if (obj.ObjectType == ObjectType.Function) yield return DAXProperty.Expression;
                if (obj.ObjectType == ObjectType.Table) yield return DAXProperty.DefaultDetailRowsExpression;
                if (obj.ObjectType == ObjectType.CalculationGroupTable)
                {
                    yield return DAXProperty.DefaultDetailRowsExpression;
                    if(TabularModelHandler.Singleton.CompatibilityLevel >= CalculationGroupTable.DefaultExpressionRequiredCompatibilityLevel)
                    {
                        yield return DAXProperty.NoSelectionExpression;
                        yield return DAXProperty.NoSelectionFormatStringExpression;
                        yield return DAXProperty.MultipleOrEmptySelectionExpression;
                        yield return DAXProperty.MultipleOrEmptySelectionFormatStringExpression;
                    }
                }
            }
            if (obj is KPI)
            {
                yield return DAXProperty.StatusExpression;
                yield return DAXProperty.TargetExpression;
                yield return DAXProperty.TrendExpression;
            }
            if (obj is TablePermission)
            {
                yield return DAXProperty.Expression;
            }
            if (obj.ObjectType == ObjectType.CalculationItem)
            {
                yield return DAXProperty.Expression;
                yield return DAXProperty.FormatStringExpression;
            }
            if (obj is Partition p && p.ShowDataCoverageDefinition())
            {
                yield return DAXProperty.DataCoverageExpression;
            }
        }

        public static DAXProperty GetDefaultDAXProperty(this IDaxDependantObject obj)
        {
            if (obj is CalculatedTable) return DAXProperty.Expression;
            if (obj is Table) return DAXProperty.DefaultDetailRowsExpression;
            if (obj is KPI) return DAXProperty.StatusExpression;
            if (obj is TablePermission) return DAXProperty.Expression;
            else return DAXProperty.Expression;
        }

        public static string GetDAXProperty(this IDaxDependantObject obj, DAXProperty property)
        {
            if (obj is TablePermission tp && property == DAXProperty.Expression)
            {
                return Properties.FILTEREXPRESSION;
            }
            if (obj is KPI)
            {
                if (property == DAXProperty.StatusExpression) return Properties.STATUSEXPRESSION;
                if (property == DAXProperty.TargetExpression) return Properties.TARGETEXPRESSION;
                if (property == DAXProperty.TrendExpression) return Properties.TRENDEXPRESSION;
            }

            if (obj is IExpressionObject && property == DAXProperty.Expression)
            {
                return Properties.EXPRESSION;
            }

            if (obj is CalculationItem ci)
            {
                if (property == DAXProperty.Expression) return Properties.EXPRESSION;
                if (property == DAXProperty.FormatStringExpression) return Properties.FORMATSTRINGEXPRESSION;
            }

            if (TabularModelHandler.Singleton.CompatibilityLevel >= 1400)
            {
                if (obj is Measure)
                {
                    if (property == DAXProperty.DetailRowsExpression) return Properties.DETAILROWSEXPRESSION;
                    if (property == DAXProperty.FormatStringExpression && TabularModelHandler.Singleton.CompatibilityLevel >= 1601) return Properties.FORMATSTRINGEXPRESSION;
                }
                if (obj is Table && property == DAXProperty.DefaultDetailRowsExpression)
                {
                    return Properties.DEFAULTDETAILROWSEXPRESSION;
                }
                if (obj is CalculationGroupTable cgt)
                {
                    if (property == DAXProperty.NoSelectionExpression) return nameof(cgt.NoSelectionExpression);
                    if (property == DAXProperty.NoSelectionFormatStringExpression) return nameof(cgt.NoSelectionFormatStringExpression);
                    if (property == DAXProperty.MultipleOrEmptySelectionExpression) return nameof(cgt.MultipleOrEmptySelectionExpression);
                    if (property == DAXProperty.MultipleOrEmptySelectionFormatStringExpression) return nameof(cgt.MultipleOrEmptySelectionFormatStringExpression);
                }
            }

            if (obj is Partition p)
            {
                return nameof(p.DataCoverageDefinitionExpression);
            }

            throw new ArgumentException(string.Format(Messages.InvalidExpressionProperty, obj.GetTypeName(), property), "property");
        }

        public static string GetDAX(this IDaxDependantObject obj, DAXProperty property)
        {
            if (obj is TablePermission tp && property == DAXProperty.Expression)
            {
                return tp.FilterExpression;
            }
            if(obj is KPI)
            {
                if (property == DAXProperty.StatusExpression) return (obj as KPI).StatusExpression;
                if (property == DAXProperty.TargetExpression) return (obj as KPI).TargetExpression;
                if (property == DAXProperty.TrendExpression) return (obj as KPI).TrendExpression;
            }

            if(obj is IExpressionObject && property == DAXProperty.Expression)
            {
                return (obj as IExpressionObject).Expression;
            }

            if(obj is CalculationItem ci)
            {
                if (property == DAXProperty.Expression) return ci.Expression;
                if (property == DAXProperty.FormatStringExpression) return ci.FormatStringExpression;
            }

            if (TabularModelHandler.Singleton.CompatibilityLevel >= 1400)
            {
                if (obj is Measure m)
                {
                    if (property == DAXProperty.DetailRowsExpression)
                        return m.DetailRowsExpression;
                    if (property == DAXProperty.FormatStringExpression && TabularModelHandler.Singleton.CompatibilityLevel >= 1601)
                        return m.FormatStringExpression;
                }
                if (obj is Table && property == DAXProperty.DefaultDetailRowsExpression)
                {
                    return (obj as Table).DefaultDetailRowsExpression;
                }
                if (obj is CalculationGroupTable cgt)
                {
                    if (property == DAXProperty.NoSelectionExpression) return cgt.NoSelectionExpression;
                    if (property == DAXProperty.NoSelectionFormatStringExpression) return cgt.NoSelectionFormatStringExpression;
                    if (property == DAXProperty.MultipleOrEmptySelectionExpression) return cgt.MultipleOrEmptySelectionExpression;
                    if (property == DAXProperty.MultipleOrEmptySelectionFormatStringExpression) return cgt.MultipleOrEmptySelectionFormatStringExpression;
                }
            }

            if (obj is Partition p)
            {
                return p.DataCoverageDefinitionExpression;
            }

            throw new ArgumentException(string.Format(Messages.InvalidExpressionProperty, obj.GetTypeName(), property), "property");
        }

        public static void SetDAX(this IDaxDependantObject obj, DAXProperty property, string expression)
        {
            if (obj is TablePermission tp && property == DAXProperty.Expression)
            {
                tp.FilterExpression = expression;
                return;
            }
            if (obj is KPI)
            {
                if (property == DAXProperty.StatusExpression) { (obj as KPI).StatusExpression = expression; return; }
                if (property == DAXProperty.TargetExpression) { (obj as KPI).TargetExpression = expression; return; }
                if (property == DAXProperty.TrendExpression) { (obj as KPI).TrendExpression = expression; return; }
            }

            if (obj is IExpressionObject && property == DAXProperty.Expression)
            {
                (obj as IExpressionObject).Expression = expression;
                return;
            }

            if (obj is Measure m)
            {
                if (property == DAXProperty.DetailRowsExpression)
                {
                    m.DetailRowsExpression = expression;
                    return;
                }
                if (property == DAXProperty.FormatStringExpression && TabularModelHandler.Singleton.CompatibilityLevel >= 1601)
                {
                    m.FormatStringExpression = expression;
                    return;
                }
            }
            if (obj is Table && property == DAXProperty.DefaultDetailRowsExpression)
            {
                (obj as Table).DefaultDetailRowsExpression = expression;
                return;
            }

            if (obj is CalculationItem ci)
            {
                if (property == DAXProperty.Expression) { ci.Expression = expression; return; }
                if (property == DAXProperty.FormatStringExpression) { ci.FormatStringExpression = expression; return; }
            }
            if (obj is CalculationGroupTable cgt)
            {
                if (property == DAXProperty.NoSelectionExpression) { cgt.NoSelectionExpression = expression; return; }
                if (property == DAXProperty.NoSelectionFormatStringExpression) { cgt.NoSelectionFormatStringExpression = expression; return; }
                if (property == DAXProperty.MultipleOrEmptySelectionExpression) { cgt.MultipleOrEmptySelectionExpression = expression; return; }
                if (property == DAXProperty.MultipleOrEmptySelectionFormatStringExpression) { cgt.MultipleOrEmptySelectionFormatStringExpression = expression; return; }
            }

            if (obj is Partition p)
            {
                p.DataCoverageDefinitionExpression = expression;
                return;
            }

            throw new ArgumentException(string.Format(Messages.InvalidExpressionProperty, obj.GetTypeName(), property), "property");
        }
    }
}

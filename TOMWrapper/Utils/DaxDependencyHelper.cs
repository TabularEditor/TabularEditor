using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class DaxDependencyHelper
    {
        public static IEnumerable<DAXProperty> GetDAXProperties(this IDaxDependantObject obj)
        {
            if (obj is Measure || obj is CalculatedColumn || obj is CalculatedTable) yield return DAXProperty.Expression;
            if (TabularModelHandler.Singleton.CompatibilityLevel >= 1400)
            {
                if (obj is Measure) yield return DAXProperty.DetailRowsExpression;
                if (obj.ObjectType == ObjectType.Table) yield return DAXProperty.DefaultDetailRowsExpression;
            }
            if (TabularModelHandler.Singleton.CompatibilityLevel >= 1470)
            {
                if (obj is Measure) yield return DAXProperty.FormatStringExpression;
            }
            if (obj is KPI)
            {
                yield return DAXProperty.StatusExpression;
                yield return DAXProperty.TargetExpression;
                yield return DAXProperty.TrendExpression;
            }
            if (obj is RLSFilterExpression)
            {
                yield return DAXProperty.Expression;
            }
            if (obj.ObjectType == ObjectType.CalculationItem)
            {
                yield return DAXProperty.Expression;
                yield return DAXProperty.FormatStringExpression;
            }
        }

        public static DAXProperty GetDefaultDAXProperty(this IDaxDependantObject obj)
        {
            if (obj is CalculatedTable) return DAXProperty.Expression;
            if (obj is Table) return DAXProperty.DefaultDetailRowsExpression;
            if (obj is KPI) return DAXProperty.StatusExpression;
            if (obj is RLSFilterExpression) return DAXProperty.Expression;
            else return DAXProperty.Expression;
        }

        public static string GetDAX(this IDaxDependantObject obj, DAXProperty property)
        {
            if (obj is RLSFilterExpression && property == DAXProperty.Expression)
            {
                var rls = (obj as RLSFilterExpression);
                return rls.Role.RowLevelSecurity[rls.Table];
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
                if (obj is Measure && property == DAXProperty.DetailRowsExpression)
                {
                    return (obj as Measure).DetailRowsExpression;
                }
                if (obj is Table && property == DAXProperty.DefaultDetailRowsExpression)
                {
                    return (obj as Table).DefaultDetailRowsExpression;
                }
            }

            if (obj is Measure m && property == DAXProperty.FormatStringExpression)
                return m.FormatStringExpression;

            throw new ArgumentException(string.Format(Messages.InvalidExpressionProperty, obj.GetTypeName(), property), "property");
        }

        public static void SetDAX(this IDaxDependantObject obj, DAXProperty property, string expression)
        {
            if (obj is RLSFilterExpression && property == DAXProperty.Expression)
            {
                var rls = (obj as RLSFilterExpression);
                rls.Role.RowLevelSecurity[rls.Table] = expression;
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

            if (obj is Measure && property == DAXProperty.DetailRowsExpression)
            {
                (obj as Measure).DetailRowsExpression = expression;
                return;
            }
            if (obj is Table && property == DAXProperty.DefaultDetailRowsExpression)
            {
                (obj as Table).DefaultDetailRowsExpression = expression;
                return;
            }

            if (obj is Measure m && property == DAXProperty.FormatStringExpression)
            {
                m.FormatStringExpression = expression;
                return;
            }
            if (obj is CalculationItem ci)
            {
                if (property == DAXProperty.Expression) { ci.Expression = expression; return; }
                if (property == DAXProperty.FormatStringExpression) { ci.FormatStringExpression = expression; return; }
            }


            throw new ArgumentException(string.Format(Messages.InvalidExpressionProperty, obj.GetTypeName(), property), "property");
        }
    }
}

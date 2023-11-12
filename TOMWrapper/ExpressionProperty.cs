using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    public enum ExpressionProperty
    {
        // 0 - 99 are DAX expressions:
        Expression = 0,
        DetailRowsExpression = 1,
        TargetExpression = 2,
        StatusExpression = 3,
        TrendExpression = 4,
        FormatStringExpression = 5,

        // 100 is an SQL expression:
        SqlExpression = 100,

        // 101 and above are M expressions:
        MExpression = 101,
        MSourceExpression = 102,
        MPollingExpression = 103
    }
    public enum ExpressionPropertyType
    {
        Dax,
        M,
        Sql
    }
}

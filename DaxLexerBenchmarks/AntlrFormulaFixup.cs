using System.Linq;
using Antlr4.Runtime;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using DAXLexer = DaxLexerOracle.DAXLexer;
using DAXCharStream = DaxLexerOracle.DAXCharStream;

namespace DaxLexerBenchmarks
{
    /// <summary>
    /// FormulaFixup as it was with the ANTLR lexer, to compare the layer above the lexer on the same model.
    /// </summary>
    internal static class AntlrFormulaFixup
    {
        private static Model Model { get { return TabularModelHandler.Singleton.Model; } }

        public static DependsOnList GetDependencies(IDaxDependantObject expressionObj, string dax, DAXProperty prop)
        {
            var dependsOn = new DependsOnList(null);
            ParseExpression(dax.Replace("\r", ""), expressionObj, prop, dependsOn);
            return dependsOn;
        }

        private static void ParseExpression(string dax, IDaxDependantObject expressionObj, DAXProperty prop, DependsOnList dependsOn = null)
        {
            var lexer = new DAXLexer(new DAXCharStream(dax, false));
            lexer.RemoveErrorListeners();
            var tokens = GetDefaultTokens(lexer).ToList();

            IToken lastTableRef = null;
            int startTableIndex = 0;

            for (var i = 0; i < tokens.Count; i++)
            {
                var tok = tokens[i];

                var isTableOrVariableRef = tok.Type == DAXLexer.TABLE
                    || tok.Type == DAXLexer.TABLE_OR_VARIABLE
                    || (tok.Channel == DAXLexer.KEYWORD_CHANNEL && i < tokens.Count - 1 && tokens[i + 1].Type != DAXLexer.OPEN_PARENS);

                if (isTableOrVariableRef)
                {
                    if (i < tokens.Count - 1 && tokens[i + 1].Type == DAXLexer.OPEN_PARENS)
                    {
                        // function call
                        if (Model.Functions.FindByName(tok.Text) is Function f)
                        {
                            // user-defined function
                            if (dependsOn != null) dependsOn.Add(f, DAXProperty.Expression, tok.StartIndex, tok.StopIndex, false);
                            else expressionObj.AddDep(f, DAXProperty.Expression, tok.StartIndex, tok.StopIndex, false);
                        }
                    }
                    else
                    {
                        if (i < tokens.Count - 1 && tokens[i + 1].Type == DAXLexer.COLUMN_OR_MEASURE)
                        {
                            // Keep the token reference, as the next token should be column (fully qualified).
                            lastTableRef = tok;
                            startTableIndex = tok.StartIndex;
                        }
                        else
                        {
                            // Table referenced directly, don't save the reference for the next token.
                            lastTableRef = null;
                        }

                        var tableOrCalendar = Model.FindTableOrCalendar(tok.Text);
                        if (tableOrCalendar is Table table)
                        {
                            if (dependsOn != null)
                                dependsOn.Add(table, prop, tok.StartIndex, tok.StopIndex, true);
                            else
                                expressionObj.AddDep(table, prop, tok.StartIndex, tok.StopIndex, true);
                        }
                        else if (tableOrCalendar is Calendar calendar)
                        {
                            if (dependsOn != null)
                                dependsOn.Add(calendar, prop, tok.StartIndex, tok.StopIndex, true);
                            else
                                expressionObj.AddDep(calendar, prop, tok.StartIndex, tok.StopIndex, true);
                        }
                        else
                        {
                            // Invalid reference (no table with that name) or possibly a variable or function ref
                        }
                    }
                }
                else if (tok.Type == DAXLexer.COLUMN_OR_MEASURE)
                {
                    // Referencing a table just before the object reference
                    if (lastTableRef != null)
                    {
                        var tableName = lastTableRef.Text;
                        lastTableRef = null;
                        if (!Model.Tables.Contains(tableName)) return; // Invalid reference (no table with that name)

                        var table = Model.Tables[tableName];
                        // Referencing a column on a specific table
                        if (table.Columns.Contains(tok.Text))
                        {
                            if (dependsOn != null) dependsOn.Add(table.Columns[tok.Text], prop, startTableIndex, tok.StopIndex, true);
                            else expressionObj.AddDep(table.Columns[tok.Text], prop, startTableIndex, tok.StopIndex, true);
                        }
                        // Referencing a measure on a specific table
                        else if (table.Measures.Contains(tok.Text))
                        {
                            if (dependsOn != null) dependsOn.Add(table.Measures[tok.Text], prop, startTableIndex, tok.StopIndex, true);
                            else expressionObj.AddDep(table.Measures[tok.Text], prop, startTableIndex, tok.StopIndex, true);
                        }
                    }
                    // No table reference before the object reference
                    else
                    {
                        var table = (expressionObj as ITabularTableObject)?.Table ?? (expressionObj as TablePermission)?.Table;
                        // Referencing a column without specifying a table (assume column in same table):
                        if (table != null && table.Columns.Contains(tok.Text))
                        {
                            if (dependsOn != null) dependsOn.Add(table.Columns[tok.Text], prop, tok.StartIndex, tok.StopIndex, false);
                            else expressionObj.AddDep(table.Columns[tok.Text], prop, tok.StartIndex, tok.StopIndex, false);
                        }
                        // Referencing a measure or column without specifying a table
                        else
                        {
                            Measure m = null;
                            if (table != null && table.Measures.Contains(tok.Text)) m = table.Measures[tok.Text];
                            else
                                m = Model.Tables.FirstOrDefault(t => t.Measures.Contains(tok.Text))?.Measures[tok.Text];

                            if (m != null)
                            {
                                if (dependsOn != null) dependsOn.Add(m, prop, tok.StartIndex, tok.StopIndex, false);
                                else expressionObj.AddDep(m, prop, tok.StartIndex, tok.StopIndex, false);
                            }
                        }
                    }
                }
                else
                {
                    lastTableRef = null;
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<IToken> GetDefaultTokens(DAXLexer lexer)
        {
            var token = lexer.NextToken();
            while (token.Type != -1)
            {
                if (token.Channel == DAXLexer.DefaultTokenChannel || token.Channel == DAXLexer.KEYWORD_CHANNEL) yield return token;
                token = lexer.NextToken();
            }
        }

        public static void BuildDependencyTree(IDaxDependantObject expressionObj)
        {
            FormulaFixup.ClearDependsOn(expressionObj);

            foreach (var prop in expressionObj.GetDAXProperties())
            {
                var dax = expressionObj.GetDAX(prop) ?? "";
                ParseExpression(dax.Replace("\r", ""), expressionObj, prop);
            }
        }
    }
}

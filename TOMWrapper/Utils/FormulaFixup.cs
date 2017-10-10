using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TextServices;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class FormulaFixup
    {
        /// <summary>
        /// Changes all references to object "obj", to reflect "newName"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newName"></param>
        public static void DoFixup(IDaxObject obj)
        {
            //foreach (var d in obj.Model.Tables.OfType<IExpressionObject>().Concat(obj.Model.Tables.SelectMany(t => t.GetChildren().OfType<IExpressionObject>())))
            foreach (var d in obj.Dependants.ToList())
            {
                List<Dependency> depList;
                if (d.Dependencies.TryGetValue(obj, out depList))
                {
                    var pos = 0;
                    var sb = new StringBuilder();
                    foreach (var dep in depList)
                    {
                        sb.Append(d.Expression.Substring(pos, dep.from - pos));
                        sb.Append(dep.fullyQualified ? obj.DaxObjectFullName : obj.DaxObjectName);
                        pos = dep.to + 1;
                    }
                    sb.Append(d.Expression.Substring(pos));
                    d.Expression = sb.ToString();
                }
            }
        }

        private static UndoManager UndoManager { get { return TabularModelHandler.Singleton.UndoManager; } }
        private static Model Model { get { return TabularModelHandler.Singleton.Model; } }

        public static void BuildDependencyTree(IDAXExpressionObject expressionObj)
        {
            foreach (var d in expressionObj.Dependencies.Keys) d.Dependants.Remove(expressionObj);
            expressionObj.Dependencies.Clear();

            var tokens = new DAXLexer(new AntlrInputStream(expressionObj.Expression ?? "")).GetAllTokens();

            IToken lastTableRef = null;

            for (var i = 0; i < tokens.Count; i++)
            {
                // TODO: This parsing could be used to check for invalid object references, for example to use in syntax highlighting or validation of expressions

                var tok = tokens[i];
                switch (tok.Type)
                {
                    case DAXLexer.TABLE:
                    case DAXLexer.TABLE_OR_VARIABLE:
                        if (lastTableRef != null)
                        {
                            if (Model.Tables.Contains(lastTableRef.Text.NoQ(true))) expressionObj.AddDep(Model.Tables[lastTableRef.Text.NoQ(true)], lastTableRef.StartIndex, lastTableRef.StopIndex, true);
                        }
                        lastTableRef = tok;
                        break;
                    case DAXLexer.COLUMN_OR_MEASURE:
                        var tableName = lastTableRef?.Text.NoQ(true);

                        // Referencing a table just before the object reference
                        if (tableName != null && Model.Tables.Contains(tableName))
                        {
                            var table = Model.Tables[tableName];
                            // Referencing a column on a specific table
                            if (table.Columns.Contains(tok.Text.NoQ()))
                                expressionObj.AddDep(table.Columns[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                            // Referencing a measure on a specific table
                            else if (table.Measures.Contains(tok.Text.NoQ()))
                                expressionObj.AddDep(table.Measures[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                        }
                        // No table reference before the object reference
                        else
                        {
                            var table = (expressionObj as ITabularTableObject)?.Table;
                            // Referencing a column without specifying a table (assume column in same table):
                            if (table != null && table.Columns.Contains(tok.Text.NoQ()))
                            {
                                expressionObj.AddDep(table.Columns[tok.Text.NoQ()], tok.StartIndex, tok.StopIndex, false);
                            }
                            // Referencing a measure or column without specifying a table
                            else
                            {
                                Measure m = null;
                                if (table != null && table.Measures.Contains(tok.Text.NoQ())) m = table.Measures[tok.Text.NoQ()];
                                else
                                    m = Model.Tables.FirstOrDefault(t => t.Measures.Contains(tok.Text.NoQ()))?.Measures[tok.Text.NoQ()];

                                if (m != null)
                                    expressionObj.AddDep(m, tok.StartIndex, tok.StopIndex, false);
                            }
                        }
                        lastTableRef = null;
                        break;
                    default:
                        if (lastTableRef != null)
                        {
                            if (Model.Tables.Contains(lastTableRef.Text.NoQ(true))) expressionObj.AddDep(Model.Tables[lastTableRef.Text.NoQ(true)], lastTableRef.StartIndex, lastTableRef.StopIndex, true);
                            lastTableRef = null;
                        }
                        break;
                }
                if (lastTableRef != null)
                {
                    if (Model.Tables.Contains(lastTableRef.Text.NoQ(true))) expressionObj.AddDep(Model.Tables[lastTableRef.Text.NoQ(true)], lastTableRef.StartIndex, lastTableRef.StopIndex, true);
                    //lastTableRef = null;
                }
            }
        }

        public static void BuildDependencyTree()
        {
            if (UndoManager.UndoInProgress)
            {
                DelayBuildDependencyTree = true;
                UndoManager.RebuildDependencyTree = true;
            }
            if (DelayBuildDependencyTree) return;
            var sw = new Stopwatch();
            sw.Start();

            foreach (var eo in Model.Tables.SelectMany(t => t.GetChildren()).Concat(Model.Tables).OfType<IDAXExpressionObject>())
            {
                BuildDependencyTree(eo);
            }

            sw.Stop();

            Console.WriteLine("Dependency tree built in {0} ms", sw.ElapsedMilliseconds);
        }

        public static bool DelayBuildDependencyTree { get; set; } = false;
    }
}

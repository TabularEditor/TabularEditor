using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace DaxLexerBenchmarks
{
    /// <summary>
    /// A real model loaded through the TOM wrapper, extended with generated measures of the size of an enterprise model
    /// (the AdventureWorks test model only has short expressions), and its DAX expressions.
    /// </summary>
    internal static class DaxModel
    {
        private const int GeneratedMeasures = 2000;

        private static TabularModelHandler _handler;

        public static List<IDaxDependantObject> Objects { get; private set; }

        public static List<(IDaxDependantObject Object, DAXProperty Property, string Dax)> Expressions { get; private set; }

        public static void Load(bool generateMeasures)
        {
            if (_handler != null) return;

            _handler = new TabularModelHandler(Path.Combine(AppContext.BaseDirectory, "AdventureWorks.bim"));
            var model = _handler.Model;

            if (generateMeasures)
            {
                var table = model.Tables.OrderByDescending(t => t.Columns.Count).First();
                var columns = table.Columns.Select(c => c.Name).ToArray();
                var other = model.Tables.First(t => t != table);
                model.Handler.BeginUpdate("measures");
                for (var i = 0; i < GeneratedMeasures; i++)
                {
                    table.AddMeasure("Generated " + i, DaxSamples.Measure(i, table.Name, columns[i % columns.Length], other.Name, other.Columns[0].Name));
                }
                model.Handler.EndUpdate();
            }

            // The objects of FormulaFixup.BuildDependencyTree()
            Objects = model.Tables.SelectMany(t => t.GetChildren()).Concat(model.Tables).OfType<IDaxDependantObject>()
                .Concat(model.CalculationGroups.SelectMany(cg => cg.CalculationItems))
                .Concat(model.Roles.SelectMany(r => r.TablePermissions))
                .ToList();

            Expressions = new List<(IDaxDependantObject, DAXProperty, string)>();
            foreach (var obj in Objects)
            {
                foreach (var property in obj.GetDAXProperties())
                {
                    var dax = obj.GetDAX(property);
                    if (!string.IsNullOrEmpty(dax)) Expressions.Add((obj, property, dax));
                }
            }
        }
    }

    internal static class DaxSamples
    {
        // What measures tend to look like: time intelligence, variables, filters, switches, comments
        public static string Measure(int i, string table, string column, string otherTable, string otherColumn)
        {
            var t = "'" + table + "'";
            var o = "'" + otherTable + "'";
            switch (i % 5)
            {
                case 0:
                    return $"CALCULATE ( SUM ( {t}[{column}] ), {o}[{otherColumn}] = \"Value {i}\" )";
                case 1:
                    return $"VAR Current = SUM ( {t}[{column}] )\nVAR Previous =\n    CALCULATE (\n        SUM ( {t}[{column}] ),\n        DATEADD ( {o}[{otherColumn}], -1, YEAR )\n    )\nRETURN\n    IF ( Previous <> 0, DIVIDE ( Current - Previous, Previous ), BLANK () )";
                case 2:
                    return $"// Share of the total for measure {i}\nDIVIDE (\n    [Generated {i - 1}],\n    CALCULATE ( [Generated {i - 1}], ALLSELECTED ( {t} ) ),\n    0\n)";
                case 3:
                    return $"SUMX (\n    FILTER (\n        ADDCOLUMNS (\n            VALUES ( {o}[{otherColumn}] ),\n            \"@Amount\", CALCULATE ( SUM ( {t}[{column}] ) )\n        ),\n        [@Amount] > {i}.5\n    ),\n    [@Amount] * 1.21\n)";
                default:
                    return $"/* Banded result\n   for the report */\nSWITCH (\n    TRUE (),\n    [Generated {i - 1}] < 100, \"Low\",\n    [Generated {i - 1}] < 1000, \"Medium\",\n    [Generated {i - 1}] < 10000, \"High\",\n    FORMAT ( [Generated {i - 1}], \"#,##0.00\" ) & \" \" & SELECTEDVALUE ( {o}[{otherColumn}], \"n/a\" )\n)";
            }
        }

        public static readonly string Small = Measure(1, "Internet Sales", "Sales Amount", "Date", "Date");

        public static readonly string Keystroke = Join(1900);

        public static readonly string Large = Join(50000);

        private static string Join(int length)
        {
            var result = new System.Text.StringBuilder();
            for (var i = 0; result.Length + 400 < length; i++)
            {
                result.Append(Measure(i, "Internet Sales", "Sales Amount", "Date", "Date")).Append('\n');
            }
            return result.ToString();
        }
    }
}

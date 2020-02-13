using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;
using TabularEditor.UI;
using TabularEditor.UI.Actions;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.Scripting
{
    public static class ScriptHelper
    {
        [ScriptMethod]
        public static void SchemaCheck(Partition partition)
        {
            var changes = TableMetadata.GetChanges(partition);
            ReportSchemaCheckChanges(changes, partition.Model);
        }

        [ScriptMethod]
        public static void SchemaCheck(Table table)
        {
            var changes = TableMetadata.GetChanges(table.Partitions[0]);
            ReportSchemaCheckChanges(changes, table.Model);
        }

        [ScriptMethod]
        public static void SchemaCheck(ProviderDataSource source)
        {
            var changes = TableMetadata.GetChanges(source);
            ReportSchemaCheckChanges(changes, source.Model);
        }

        [ScriptMethod]
        public static void SchemaCheck(Model model)
        {
            var changes = new List<MetadataChange>();
            foreach (var source in model.DataSources.OfType<ProviderDataSource>())
            {
                changes.AddRange(TableMetadata.GetChanges(source));
            }
            ReportSchemaCheckChanges(changes, model);
        }

        private static void ReportSchemaCheckChangesToNUnit(List<MetadataChange> changes, Model model, TestRun nUnit)
        {
            Program.testRun.StartSuite("Schema Checks");
            foreach (var table in model.Tables)
            {
                var changesForTable = changes.Where(c => c.ModelTable == table).ToList();

                if (changesForTable.Any(c => c.ChangeType == MetadataChangeType.SourceQueryError))
                {
                    var change = changesForTable.First(c => c.ChangeType == MetadataChangeType.SourceQueryError);
                    Program.testRun.Fail("Schema Checks", $"Table '{table.Name}' source query is valid", change.ToString(), change.SourceQuery);
                    Program.testRun.Inconclude("Schema Checks", $"Table '{table.Name}' imports all columns");
                    Program.testRun.Inconclude("Schema Checks", $"Table '{table.Name}' does not import nonexisting columns");
                    Program.testRun.Inconclude("Schema Checks", $"Table '{table.Name}' maps data types correctly");
                    continue;
                }

                Program.testRun.Pass("Schema Checks", $"Table '{table.Name}' source query is valid");

                var notImported = changesForTable.Where(c => c.ChangeType == MetadataChangeType.SourceColumnAdded).ToList();
                if (notImported.Count > 0)
                {
                    Program.testRun.Fail("Schema Checks", $"Table '{table.Name}' imports all columns", 
                        "Table does not import all columns", 
                        "Columns not imported:\r\n  " + string.Join("\r\n  ", notImported.Select(c => $"[{c.SourceColumn}]").ToArray()));
                }
                else
                    Program.testRun.Pass("Schema Checks", $"Table '{table.Name}' imports all columns");

                var notFound = changesForTable.Where(c => c.ChangeType == MetadataChangeType.SourceColumnNotFound).ToList();
                if (notFound.Count > 0)
                {
                    Program.testRun.Fail("Schema Checks", $"Table '{table.Name}' does not import nonexisting columns",
                        "Table imports columns that do not exist in source query",
                        "Columns without corresponding source column:\r\n  " + string.Join("\r\n  ", notFound.Select(c => $"[{c.ModelColumn.Name}] (source column: {c.SourceColumn})").ToArray()));
                }
                else
                    Program.testRun.Pass("Schema Checks", $"Table '{table.Name}' does not import nonexisting columns");

                var dtChange = changesForTable.Where(c => c.ChangeType == MetadataChangeType.DataTypeChange).ToList();
                if (dtChange.Count > 0)
                {
                    Program.testRun.Fail("Schema Checks", $"Table '{table.Name}' maps data types correctly",
                        "One or more imported columns do not have a matching data type in the source",
                        "Columns with non-matching data type:\r\n  " + string.Join("\r\n  ", dtChange.Select(c => $"[{c.ModelColumn.Name}] {c.ModelColumn.DataType} (source column: {c.SourceColumn} {c.SourceProviderType})").ToArray()));
                }
                else
                    Program.testRun.Pass("Schema Checks", $"Table '{table.Name}' maps data types correctly");
            }
        }

        private static void ReportSchemaCheckChanges(List<MetadataChange> changes, Model model)
        {
            if (Program.CommandLineMode)
            {
                if (Program.testRun != null) ReportSchemaCheckChangesToNUnit(changes, model, Program.testRun);

                foreach (var change in changes)
                {
                    var msg = change.ToString();
                    if (change.ChangeType == MetadataChangeType.SourceColumnNotFound || change.ChangeType == MetadataChangeType.SourceQueryError) Error(msg,-1,true);
                    else Warning(msg,-1,true);
                }
            }
            else
            {
                if (changes.Count == 0)
                {
                    MessageBox.Show("No changes detected.", "Refresh Table Metadata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    SchemaDiffDialog.Show(changes);
            }
        }


        [ScriptMethod]
        public static string ConvertDax(string dax, bool useSemicolons = true)
        {
            return useSemicolons ? ExpressionParser.CommasToSemicolons(dax) : ExpressionParser.SemicolonsToCommas(dax);
        }

        [ScriptMethod]
        public static string FormatDax(string dax, bool shortFormat = false)
        {
            var textToFormat = "x :=" + dax;
            try
            {
                var result = TabularEditor.Dax.DaxFormatterProxy.FormatDax(textToFormat, false, shortFormat).FormattedDax;
                if (string.IsNullOrWhiteSpace(result))
                {
                    return dax;
                }
                return result.Substring(6).Trim();
            }
            catch
            {
                return dax;
            }
        }

        [ScriptMethod]
        public static string ReadFile(string filePath)
        {
            using (var fileStream = new System.IO.FileStream(filePath,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.ReadWrite))
            using (var textReader = new System.IO.StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }
        }

        [ScriptMethod]
        public static void SaveFile(string filePath, string content)
        {
            System.IO.File.WriteAllText(filePath, content);
        }

        [ScriptMethod]
        public static void Output(this object value, int lineNumber = -1)
        {
            if (Program.CommandLineMode)
            {
                Info(string.Format(lineNumber != -1 ? "Script output line #{0}: {1}" : "Script output: {1}", lineNumber, value));
                return;
            }

            if (ScriptOutputForm.DontShow) return;

            var caption = string.Format("Script output{0}", lineNumber > 0 ? " at line " + lineNumber : "");

            var isHourglass = Hourglass.Enabled;
            if (isHourglass) Hourglass.Enabled = false;
            ScriptOutputForm.ShowObject(value, caption);
            if (isHourglass) Hourglass.Enabled = true;
        }

        [ScriptMethod]
        public static void Info(string message, int lineNumber = -1)
        {
            var header = string.Format("Script info{0}", lineNumber >= 0 ? " (line " + lineNumber + ")": "", message);

            if (Program.CommandLineMode) Console.WriteLine(message);
            else MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        [ScriptMethod]
        public static void Warning(string message, int lineNumber = -1, bool suppressHeader = false)
        {
            var header = string.Format("Script warning{0}", lineNumber >= 0 ? " (line " + lineNumber + ")" : "", message);

            if (Program.CommandLineMode) Program.Warning((suppressHeader ? "" : (header + ": ")) + message);
            else MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        [ScriptMethod]
        public static void Error(string message, int lineNumber = -1, bool suppressHeader = false)
        {
            var header = string.Format("Script error{0}", lineNumber >= 0 ? " (line " + lineNumber + ")" : "", message);

            if (Program.CommandLineMode) Program.Error((suppressHeader ? "" : ( header + ": ")) + message);
            else MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void OutputErrors(IEnumerable<TabularNamedObject> items)
        {
            if (ScriptOutputForm.DontShow) return;

            var caption = string.Format("Objects with errors ({0})", items.Count());
            ScriptOutputForm.ShowObject(items, caption, true);
        }

        [ScriptMethod, IntelliSense("Invoke the custom action with the given name.")]
        public static void CustomAction(string actionName)
        {
            var act = UI.UIController.Current.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteInScript(null);
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }

        [ScriptMethod, IntelliSense("Invoke the custom action on the given set of objects with the given name.")]
        public static void CustomAction(this IEnumerable<ITabularNamedObject> selection, string actionName)
        {
            var act = UI.UIController.Current.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteWithSelection(null, selection);
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }

        [ScriptMethod, IntelliSense("Invoke the custom action on the given object with the given name.")]
        public static void CustomAction(this ITabularNamedObject selection, string actionName)
        {
            var act = UI.UIController.Current.Actions.OfType<CustomAction>().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteWithSelection(null, Enumerable.Repeat(selection, 1));
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.Dax;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI;
using TabularEditor.UI.Actions;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.Scripting
{
    public static class ScriptHelper
    {
        [ScriptMethod,IntelliSense("Displays a dialog that allows the user to select a column from the specified collection")]
        public static Column SelectColumn(this IEnumerable<Column> columns, Column preselect = null, string label = "Select column:")
        {
            return SelectObject(columns, preselect, label);
        }

        [ScriptMethod]
        public static Column SelectColumn(this Table table, Column preselect = null, string label = "Select column:")
        {
            return SelectColumn(table.Columns, preselect, label);
        }
        [ScriptMethod]
        public static Table SelectTable(this IEnumerable<Table> tables, Table preselect = null, string label = "Select table:")
        {
            return SelectObject(tables, preselect, label);
        }
        [ScriptMethod]
        public static Table SelectTable(Table preselect = null, string label = "Select table:")
        {
            return SelectObject(TabularModelHandler.Singleton.Model.Tables, preselect, label);
        }
        [ScriptMethod]
        public static Table SelectTable(this Model model, Table preselect = null, string label = "Select table:")
        {
            return SelectObject(model.Tables, preselect, label);
        }
        [ScriptMethod]
        public static Measure SelectMeasure(this IEnumerable<Measure> measures, Measure preselect = null, string label = "Select measure:")
        {
            return SelectObject(measures, preselect, label);
        }

        [ScriptMethod]
        public static Measure SelectMeasure(this Table table, Measure preselect = null, string label = "Select measure:")
        {
            return SelectMeasure(table.Measures, preselect, label);
        }
        [ScriptMethod]
        public static Measure SelectMeasure(Measure preselect = null, string label = "Select measure:")
        {
            return SelectMeasure(TabularModelHandler.Singleton.Model.AllMeasures, preselect, label);
        }
        [ScriptMethod]
        public static Measure SelectMeasure(this Model model, Measure preselect = null, string label = "Select measure:")
        {
            return SelectMeasure(model.AllMeasures, preselect, label);
        }

        [ScriptMethod]
        public static T SelectObject<T>(this IEnumerable<T> columns, T preselect = null, string label = "Select object:") where T : TabularNamedObject
        {
            var isHourglass = Hourglass.Enabled;
            if (isHourglass) Hourglass.Enabled = false;
            var result = ObjectSelectDialog<T>.SelectObject(columns, preselect, label);
            if (isHourglass) Hourglass.Enabled = true;
            return result;
        }

        [ScriptMethod]
        public static object EvaluateDax(string dax)
        {
            return TabularModelHandler.Singleton.Model.Database.EvaluateDax(dax);
        }

        [ScriptMethod]
        public static DataSet ExecuteDax(string dax)
        {
            return TabularModelHandler.Singleton.Model.Database.ExecuteDax(dax);
        }

        [ScriptMethod]
        public static IDataReader ExecuteReader(string dax)
        {
            return TabularModelHandler.Singleton.Model.Database.ExecuteReader(dax);
        }

        [ScriptMethod]
        public static void ExecuteCommand(string tmsl)
        {
            TabularModelHandler.Singleton.Model.Database.ExecuteCommand(tmsl);
        }

        [ScriptMethod]
        public static void SchemaCheck(Partition partition)
        {
            var changes = TableMetadata.GetChanges(partition);
            ReportSchemaCheckChanges(changes, partition.Model);
        }

        [ScriptMethod]
        public static void SchemaCheck(Table table)
        {
            var changes = TableMetadata.GetChanges(table);
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

        internal static IDaxFormatterProxy DaxFormatter { get; set; } = DaxFormatterProxy.Instance;

        [ScriptMethod]
        public static string ConvertDax(string dax, bool useSemicolons = true)
        {
            return useSemicolons ? ExpressionParser.CommasToSemicolons(dax) : ExpressionParser.SemicolonsToCommas(dax);
        }

        [ScriptMethod][Obsolete]
        public static string FormatDax(string dax, bool shortFormat = false, bool skipSpaceAfterFunctionName = false)
        {
            daxFormatterDirectCalls++;
            if(daxFormatterDirectCalls > 3 && !daxFormatterWarningShown)
            {
                if(Program.CommandLineMode)
                    Warning("This script is making multiple calls to the \"FormatDax\" method, which has been deprecated! Calls will be throttled to not overload the DaxFormatter.com service. To avoid throttling, please change your script to use the FormatDax extension method going forward. For more information, see: https://docs.tabulareditor.com/FormatDax.html");
                else
                {
                    var mr = MessageBox.Show("This script is making multiple calls to the \"FormatDax\" method, which has been deprecated! Calls will be throttled to not overload the DaxFormatter.com service. To avoid throttling, please change your script to use the FormatDax extension method going forward.\n\nFor more information, see:\n\nhttps://docs.tabulareditor.com/FormatDax\n\nContinue script execution?", "FormatDax method deprecated!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (mr == DialogResult.No) throw new ScriptCancelledException();
                }
                daxFormatterWarningShown = true;
            }
            // To avoid overloading the DaxFormatter service, let's add a 5 second delay if less than 5 seconds have passed since the last call:
            if(daxFormatterWarningShown && (DateTime.Now - daxFormatterLastCall).TotalSeconds < 5.0)
            {
                Thread.Sleep(5000);
            }
            daxFormatterLastCall = DateTime.Now;

            var textToFormat = "x :=" + dax;
            try
            {
                var result = DaxFormatter.FormatDax(textToFormat, false, shortFormat, skipSpaceAfterFunctionName).FormattedDax;
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

        public static void BeforeScriptExecution()
        {
            daxFormatterWarningShown = false;
            daxFormatterDirectCalls = 0;
            objectsFlaggedForFormatting.Clear();
        }
        public static void AfterScriptExecution()
        {
            if(objectsFlaggedForFormatting.Count > 0)
            {
                CallDaxFormatter();
            }
            objectsFlaggedForFormatting.Clear();
        }
        private static bool daxFormatterWarningShown = false;
        private static int daxFormatterDirectCalls = 0;
        private static DateTime daxFormatterLastCall = DateTime.MinValue;
        private static HashSet<IDaxDependantObject> objectsFlaggedForFormatting = new HashSet<IDaxDependantObject>();

        [ScriptMethod]
        public static void FormatDax(this IDaxDependantObject obj)
        {
            objectsFlaggedForFormatting.Add(obj);
        }

        [ScriptMethod]
        public static void FormatDax(this IEnumerable<IDaxDependantObject> objects, bool shortFormat = false, bool? skipSpaceAfterFunctionName = null)
        {
            // Call DAX Formatter with an array of all expressions to be formatted:
            var objectRefs = new List<Tuple<IDaxDependantObject, DAXProperty>>();
            var expressions = new List<string>();
            foreach (var obj in objects)
            {
                if (obj.IsRemoved) continue;

                foreach (var prop in obj.GetDAXProperties())
                {
                    var dax = obj.GetDAX(prop);
                    if (!string.IsNullOrWhiteSpace(dax))
                    {
                        objectRefs.Add(new Tuple<IDaxDependantObject, DAXProperty>(obj, prop));
                        expressions.Add("x :=" + dax);
                    }
                }
            }
            var formatResult = DaxFormatter.FormatDaxMulti(expressions, false, shortFormat, skipSpaceAfterFunctionName ?? Preferences.Current.DaxFormatterSkipSpaceAfterFunctionName);

            // Assign the formatted dax back to the objects:
            if (formatResult.Count == expressions.Count)
            {
                for (int i = 0; i < formatResult.Count; i++)
                {
                    var result = formatResult[i];
                    if (result.errors.Count > 0) continue;

                    var obj = objectRefs[i].Item1;
                    var daxProperty = objectRefs[i].Item2;
                    var formattedDax = result.FormattedDax.Substring(6).Trim();

                    obj.SetDAX(daxProperty, formattedDax);
                }
            }
        }

        [ScriptMethod]
        public static void CallDaxFormatter(bool shortFormat = false, bool? skipSpaceAfterFunctionName = null)
        {
            FormatDax(objectsFlaggedForFormatting, shortFormat, skipSpaceAfterFunctionName);

            objectsFlaggedForFormatting.Clear();
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

            if (Program.CommandLineMode) Program.CommandLine.Warning((suppressHeader ? "" : (header + ": ")) + message);
            else MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        [ScriptMethod]
        public static void Error(string message, int lineNumber = -1, bool suppressHeader = false)
        {
            var header = string.Format("Script error{0}", lineNumber >= 0 ? " (line " + lineNumber + ")" : "", message);

            if (Program.CommandLineMode) Program.CommandLine.Error((suppressHeader ? "" : ( header + ": ")) + message);
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
            var act = GetCustomActions().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteInScript(null);
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }

        [ScriptMethod, IntelliSense("Invoke the custom action on the given set of objects with the given name.")]
        public static void CustomAction(this IEnumerable<ITabularNamedObject> selection, string actionName)
        {
            var act = GetCustomActions().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteWithSelection(null, selection);
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }

        [ScriptMethod, IntelliSense("Invoke the custom action on the given object with the given name.")]
        public static void CustomAction(this ITabularNamedObject selection, string actionName)
        {
            var act = GetCustomActions().FirstOrDefault(a => a.BaseName == actionName);
            if (act != null)
            {
                act.ExecuteWithSelection(null, Enumerable.Repeat(selection, 1));
            }
            else throw new InvalidOperationException(string.Format("There is no Custom Action with the name '{0}'.", actionName));
        }

        private static List<IBaseAction> commandLineCustomActions;

        private static IEnumerable<CustomAction> GetCustomActions()
        {
            if (UI.UIController.Current != null)
                return UI.UIController.Current.Actions.OfType<CustomAction>();

            if (commandLineCustomActions == null)
            {
                commandLineCustomActions = new List<IBaseAction>();
                ScriptEngine.AddCustomActions(commandLineCustomActions);
            }
            return commandLineCustomActions.OfType<CustomAction>();
        }
    }
}

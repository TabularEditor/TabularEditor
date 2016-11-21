using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using TabularEditor.TOMWrapper;
using TabularEditor.UI;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;
using TabularEditor.UI.Actions;

namespace TabularEditor
{
    public static class ScriptEngine
    {
        static readonly string WrapperDllPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TabularEditor\TOMWrapper.dll";
        public static readonly string CustomActionsJsonPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\CustomActions.json";

        public static Action<Model, UITreeSelection> ScriptAction(string script, out CompilerResults compilerResults)
        {
            var code = string.Format(@"namespace TabularEditor.Scripting
{{
    public static class ScriptHost
    {{
        public static void Execute(TabularEditor.TOMWrapper.Model Model, TabularEditor.UI.UITreeSelection Selected)
        {{ 
{0}
        }}
    }}
}}", script);

            compilerResults = Compile(code);

            if (compilerResults.Errors.Count > 0) return null;

            var compiledAssembly = compilerResults.CompiledAssembly;
            var type = compiledAssembly.GetType("TabularEditor.Scripting.ScriptHost");
            var method = type.GetMethod("Execute");
            return (Action<Model, UITreeSelection>)Delegate.CreateDelegate(typeof(Action<Model, UITreeSelection>), method);
        }

        private static string DelegateCode(string delegateCode, bool isString)
        {
            if (string.IsNullOrEmpty(delegateCode)) return "true";

            if (delegateCode.Contains("return"))
            {
                return "{" + delegateCode + "}";
            }
            else if (isString)
                return '"' + delegateCode.Replace("\"", "\\\"") + '"';
            else
                return delegateCode;
        }
        private static string GetCustomActionsCode(CustomActionsJson actions)
        {
            var t1 = new string(' ', 4);
            var t2 = new string(' ', 8);
            var t3 = new string(' ', 12);
            var t4 = new string(' ', 16);
            var sb = new StringBuilder();
            sb.AppendLine(@"namespace TabularEditor.Scripting
{
    public static class CustomActions 
    {
        public static void CreateCustomActions(TabularEditor.UI.Actions.ModelActionManager am)
        {"
);
            var i = 0;
            foreach (var act in actions.Actions)
            {
                sb.Append(t3 + "am.Add(CustomAction");
                sb.Append(i);
                sb.AppendLine("());");
                i++;
            }
            CustomActionCount = i;
            i = 0;
            sb.AppendLine(t2 + "}"); // End Method
            foreach (var act in actions.Actions)
            {
                sb.Append(t2 + "private static TabularEditor.UI.Actions.CustomAction CustomAction");
                sb.Append(i);
                sb.AppendLine("() {");
                sb.AppendLine(t3 + "return new TabularEditor.UI.Actions.CustomAction(");

                // EnabledDelegate:
                sb.Append(t4 + "(Selected, Model) => ");
                sb.Append(DelegateCode(act.Enabled, false));
                sb.AppendLine(",");

                // ExecuteDelegate:
                sb.AppendLine(t4 + "(Selected, Model) => {");
                sb.AppendLine(act.Execute);
                sb.AppendLine(t4 + "},");

                // Name:
                sb.Append(t4 + "@\"" + act.Name.Replace("\"", "\"\"") + "\")");

                if (!string.IsNullOrEmpty(act.Tooltip))
                {
                    sb.Append("{ ToolTip = \"");
                    sb.Append(act.Tooltip);
                    sb.Append("\"}");
                }

                sb.AppendLine(";");

                sb.AppendLine(t2 + "}");
                i++;
            }
            sb.AppendLine(t1 + "}"); // End Class
            sb.AppendLine("}"); // End Namespace

            //            Add(new Action(calcContext, (s, m) => s.Table.AddMeasure(displayFolder: s.DisplayFolder).Edit(), (s, m) => @"Create New\Measure"));
            
            return sb.ToString();
        }

        public static void CompileCustomActions(CustomActionsJson actions)
        {
            var sw = new Stopwatch();
            sw.Start();

            if (actions == null || actions.Actions.Length == 0) return;

            var code = GetCustomActionsCode(actions);
            var result = Compile(code);

            if(result.Errors.Count > 0)
            {
                Debug.WriteLine("Could not compile Custom Actions.");
                foreach(CompilerError err in result.Errors)
                {
                    Debug.WriteLine("Line {0}, col {1}: {2}", err.Line, err.Column, err.ErrorText);
                }
                CustomActionError = true;
            } else
            {
                var assembly = result.CompiledAssembly;
                var type = assembly.GetType("TabularEditor.Scripting.CustomActions");
                var method = type.GetMethod("CreateCustomActions");
                AddCustomActions = (Action<ModelActionManager>)Delegate.CreateDelegate(typeof(Action<ModelActionManager>), method);
            }

            sw.Stop();
            CustomActionCompiletime = sw.ElapsedMilliseconds;
        }

        public static long CustomActionCompiletime { get; private set; } = -1;
        public static int CustomActionCount { get; private set; } = 0;
        public static bool CustomActionError { get; private set; } = false;

        public static Action<ModelActionManager> AddCustomActions { get; private set; } = null;
        

        private static CompilerResults Compile(string code)
        {
            // Allowed namespaces:
            var includeUsings = new HashSet<string>(new[] { "System", "System.Linq", "TabularEditor.TOMWrapper", "TabularEditor.UI" });

            CompilerResults result;

            string source = string.Format("{0}\n{1}", GetUsing(includeUsings), code);

            using (var compiler = new CSharpCodeProvider()) {
                // Allowed assemblies:
                var tom = Assembly.GetAssembly(typeof(Microsoft.AnalysisServices.Tabular.Database)).Location;
                var includeAssemblies = new HashSet<string>(new[] { "system.dll", "system.core.dll", Assembly.GetExecutingAssembly().Location, WrapperDllPath });
                var cp = new CompilerParameters(includeAssemblies.ToArray()) { GenerateInMemory = true, IncludeDebugInformation = true };

                result = compiler.CompileAssemblyFromSource(cp, source);
            }


            return result;
        }

        private static string GetUsing(HashSet<string> usingStatements)
        {
            StringBuilder result = new StringBuilder();
            foreach (string usingStatement in usingStatements)
            {
                result.AppendLine(string.Format("using {0};", usingStatement));
            }
            return result.ToString();
        }

        /// <summary>
        /// This method ensures that the TOMWrapper.dll file exists (needed for Advanced scripting).
        /// Furthermore, if a CustomActions.json file is provided, it is compiled into memory and
        /// loaded to the action manager.
        /// </summary>
        public static void InitScriptEngine()
        {
            try
            {

                if (!File.Exists(WrapperDllPath))
                {
                    (new FileInfo(WrapperDllPath)).Directory.Create();
                    OutputWrapperDll();
                }
                else
                {
                    // Check if WrapperDll is of same version as the TabularEditor.exe. If not, output a new one:
                    var wrapperVersion = FileVersionInfo.GetVersionInfo(WrapperDllPath);
                    var currentVersion = Assembly.GetAssembly(typeof(TabularModelHandler)).GetName().Version;
                    if (wrapperVersion.FileVersion != currentVersion.ToString()) OutputWrapperDll();
                }

                if (File.Exists(CustomActionsJsonPath))
                {
                    CompileCustomActions(CustomActionsJson.LoadFromJson(CustomActionsJsonPath));
                } else
                {
                    ScriptEngineStatus = "File not found: " + CustomActionsJsonPath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ScriptEngineStatus = "Error: " + ex.Message;
            }
        }

        public static string ScriptEngineStatus { get; private set; }

        private static void OutputWrapperDll()
        {
            // Export the TOMWrapper library to a .DLL for use with the custom script execution:
            MemoryStream memory = new MemoryStream();
            DeflateStream stream = new DeflateStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("costura.tomwrapper.dll.zip"), CompressionMode.Decompress);
            if (stream != null)
            {
                using (stream)
                {
                    stream.CopyTo(memory);
                    byte[] data = memory.ToArray();
                    File.WriteAllBytes(WrapperDllPath, data);
                }
            }
        }
    }
}

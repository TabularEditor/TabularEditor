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
using TabularEditor.Scripting;
using TabularEditor.TextServices;

namespace TabularEditor
{
    public class AssemblyNamespace: IEquatable<AssemblyNamespace>
    {
        public Assembly Assembly;
        public string Namespace;
        public override int GetHashCode()
        {
            return Namespace.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}.dll) {1}", Assembly.GetName().Name, Namespace);
        }

        public bool Equals(AssemblyNamespace other)
        {
            return Namespace == other.Namespace;
        }
    }

    public static class ScriptEngine
    {
        static readonly string WrapperDllPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\TOMWrapper14.dll";
        public static readonly string CustomActionsJsonPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\CustomActions.json";

        private static string AddOutputLineNumbers(string script)
        {
            var parser = new TextServices.ScriptParser();
            parser.Lex(script);
            var tokens = parser.GetTokens();

            // Replace method calls: Output() --> Output(<linenumber>)
            var methodCalls = tokens.FindMethodCall("Output", 0); // Find all parameter-less calls to "Output"
            var sb = new StringBuilder();
            var pos = 0;
            foreach (var call in methodCalls)
            {
                sb.Append(script.Substring(pos, call.StopIndex + 1 - pos));
                sb.Append("(" + (call.Line) + ")");
                pos = tokens[call.TokenIndex + 2].StartIndex + 1;
            }
            sb.Append(script.Substring(pos));
            return sb.ToString();
        }

        private static string ReplaceGlobalMethodCalls(string script)
        {
            var parser = new TextServices.ScriptParser();
            parser.Lex(script);
            var tokens = parser.GetTokens();

            // Replace method calls: Output() --> Output(<linenumber>)
            var methodCalls = tokens.FindMethodCall();
            var sb = new StringBuilder();
            var pos = 0;
            foreach (var call in methodCalls.Where(t => ScriptMethods.ContainsKey(t.Text)))
            {
                if(call.TokenIndex == 0 || tokens[call.TokenIndex - 1].Type != CSharpLexer.DOT)
                {
                    var method = ScriptMethods[call.Text];

                    // Global method called directly:
                    sb.Append(script.Substring(pos, call.StartIndex - pos));
                    sb.Append(method.DeclaringType.FullName);
                    sb.Append(".");
                    pos = call.StartIndex;
                }
            }
            sb.Append(script.Substring(pos));
            return sb.ToString();
        }

        public static Action<Model, UITreeSelection> CompileScript(string script, out CompilerResults compilerResults)
        {
            script = AddOutputLineNumbers(script);
            script = ReplaceGlobalMethodCalls(script);

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
            for (var ix = 0; ix < actions.Actions.Length; ix++)
            {
                sb.Append(t3 + "am.Add(CustomAction");
                sb.Append(ix);
                sb.AppendLine("());");
            }
            CustomActionCount = actions.Actions.Length;
            var i = 0;
            sb.AppendLine(t2 + "}"); // End Method
            foreach (var act in actions.Actions)
            {
                sb.Append(t2 + "private static TabularEditor.UI.Actions.CustomAction CustomAction");
                sb.Append(i);
                sb.AppendLine("() {");
                sb.AppendLine(t3 + "var act = new TabularEditor.UI.Actions.CustomAction(");

                // EnabledDelegate:
                sb.Append(t4 + "(Selected, Model) => ");
                sb.Append(DelegateCode(act.Enabled, false));
                sb.AppendLine(",");

                // ExecuteDelegate:
                sb.AppendLine(t4 + "(Selected, Model) => {");
                sb.AppendLine(act.Execute);
                sb.AppendLine(t4 + "},");

                // Name:
                sb.AppendLine(t4 + "@\"" + act.Name.Replace("\"", "\"\"") + "\");");

                if (!string.IsNullOrEmpty(act.Tooltip))
                {
                    sb.Append(t3 + "act.ToolTip = \"");
                    sb.Append(act.Tooltip);
                    sb.AppendLine("\";");
                }

                sb.Append(t3 + "act.ValidContexts = (Context)");
                sb.Append((int)act.ValidContexts);
                sb.AppendLine(";");

                sb.AppendLine();
                sb.AppendLine("return act;");

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

            CustomActionError = false;

            if(result.Errors.Count > 0)
            {
                Console.WriteLine("Could not compile Custom Actions.");
                foreach(CompilerError err in result.Errors)
                {
                    Console.WriteLine("Line {0}, col {1}: {2}", err.Line, err.Column, err.ErrorText);
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
            var includeUsings = new HashSet<string>(new[] {
                "System",
                "System.Linq",
                "TabularEditor.TOMWrapper",
                "TabularEditor.TOMWrapper.Utils",
                "TabularEditor.UI",
                "TOM = Microsoft.AnalysisServices.Tabular"
            });
            foreach (var pluginNs in TabularEditor.UIServices.Preferences.Current.Scripting_UsingNamespaces)
                if(_pluginNamespaces.Any(an => an.Namespace == pluginNs)) includeUsings.Add(pluginNs);

            CompilerResults result;

            string source = string.Format("{0}\n{1}", GetUsing(includeUsings), code);

            using (var compiler = new CSharpCodeProvider()) {
                // Allowed assemblies:
                var tom = Assembly.GetAssembly(typeof(Microsoft.AnalysisServices.Tabular.Database)).Location;
                var includeAssemblies = new HashSet<string>(new[] { "system.dll", "system.windows.forms.dll", "system.core.dll", Assembly.GetExecutingAssembly().Location, WrapperDllPath, tom });
                foreach(var asm in Plugins) if(!includeAssemblies.Contains(asm.Location)) includeAssemblies.Add(asm.Location);
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

        static IList<Assembly> Plugins = new List<Assembly>();
        static IList<AssemblyNamespace> _pluginNamespaces = new List<AssemblyNamespace>();
        static public IList<AssemblyNamespace> PluginNamespaces { get { return _pluginNamespaces; } }

        private static void InitPlugins(IList<Assembly> plugins)
        {
            Plugins = plugins;
            _pluginNamespaces = plugins.SelectMany(p => p.GetExportedTypes()).Select(t => new AssemblyNamespace { Assembly = t.Assembly, Namespace = t.Namespace }).Distinct().ToList();
            try
            {

                if (!File.Exists(WrapperDllPath))
                {
                    (new FileInfo(WrapperDllPath)).Directory.Create();
                    OutputWrapperDll();
                }
                else
                {
                    // Check if WrapperDll is of same version as the TabularEditor.exe and same Compatibility Level. If not, output a new one:
                    var wrapperVersion = FileVersionInfo.GetVersionInfo(WrapperDllPath);
                    var currentVersion = Assembly.GetAssembly(typeof(TabularModelHandler)).GetName().Version;
                    if (wrapperVersion.FileVersion != currentVersion.ToString()) OutputWrapperDll();
                }

                if (File.Exists(CustomActionsJsonPath))
                {
                    Console.WriteLine("Loading custom actions from: " + CustomActionsJsonPath);
                    var actions = CustomActionsJson.LoadFromJson(CustomActionsJsonPath);
                    actions.SaveToJson(@"C:\TE\testactions.json");
                    CompileCustomActions(actions);
                }
                else
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

        public static Dictionary<string, MethodInfo> ScriptMethods;

        private static void FindScriptMethods(IEnumerable<Assembly> assemblies)
        {
            ScriptMethods = new Dictionary<string, MethodInfo>();

            foreach(var asm in assemblies)
            {
                asm.GetTypes()
                    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    .Where(m => m.GetCustomAttributes(typeof(ScriptMethodAttribute), false).Length > 0)
                    .ToList().ForEach(mi => ScriptMethods.Add(mi.Name, mi));
            }
        }

        /// <summary>
        /// This method ensures that the TOMWrapper.dll file exists (needed for Advanced scripting).
        /// Furthermore, if a CustomActions.json file is provided, it is compiled into memory and
        /// loaded to the action manager.
        /// </summary>
        public static void InitScriptEngine(IList<Assembly> plugins)
        {
            InitPlugins(plugins);
            FindScriptMethods(plugins.Concat(Enumerable.Repeat(typeof(ScriptHelper).Assembly, 1)));
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

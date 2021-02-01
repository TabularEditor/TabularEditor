extern alias json;

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
using json::Newtonsoft.Json;
using System.Runtime.InteropServices;

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
        static readonly string NewtonsoftJsonDllPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\newtonsoft.json.dll";
        static readonly string TomDllPath = Assembly.GetAssembly(typeof(Microsoft.AnalysisServices.Tabular.Database)).Location;
        public static readonly string CustomActionsJsonPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\CustomActions.json";
        public static readonly string CustomActionsErrorLogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\CustomActionsError.log";

        internal static string AddOutputLineNumbers(string script)
        {
            var parser = new TextServices.ScriptParser();
            parser.Lex(script);
            var tokens = parser.GetTokens();

            // Replace method calls: Output() --> Output(<linenumber>)
            var methodCalls = tokens.FindMethodCall("Output", 0); // Find all parameter-less calls to "Output"
            methodCalls.AddRange(tokens.FindMethodCall("Output", 1)); // Find all single-parameter calls to "Output";
            methodCalls.AddRange(tokens.FindMethodCall("Info", 1)); // Find all single-parameter calls to "Info";
            methodCalls.AddRange(tokens.FindMethodCall("Warning", 1)); // Find all single-parameter calls to "Warning";
            methodCalls.AddRange(tokens.FindMethodCall("Error", 1)); // Find all single-parameter calls to "Error";

            // Build map of Custom Actions:
            var lines = script.Split('\n').ToList();
            var customActionsMap = new List<int>();
            for(int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(CUSTOMACTIONS_CODEINDICATOR)) customActionsMap.Add(i + 1);
            }

            var sb = new StringBuilder();
            var pos = 0;
            foreach (var call in methodCalls.OrderBy(n => n.StartToken.StartIndex))
            {
                int actionOffset = customActionsMap.LastOrDefault(c => c < call.StopToken.Line);
                sb.Append(script.Substring(pos, call.StopToken.StartIndex - pos));
                if (call.ParamCount > 0) sb.Append(",");
                sb.Append(call.StartToken.Line - actionOffset);

                pos = call.StopToken.StartIndex;
            }
            sb.Append(script.Substring(pos));
            return sb.ToString();
        }

        internal static string ParseAssemblyRefsAndUsings(string script, out List<string> assemblyRefs, out List<string> usings)
        {
            bool assemblyRefAllowed = true;
            bool inBlockComment = false;
            assemblyRefs = new List<string>();
            usings = new List<string>();
            var lines = script.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for(int i = 0; i < lines.Length; i++)
            {
                var trimmedLine = lines[i].Trim();
                if(inBlockComment && trimmedLine.Contains("*/"))
                {
                    inBlockComment = false;
                }
                else if (assemblyRefAllowed && trimmedLine.StartsWith("#r \"") && trimmedLine.EndsWith("\"") && trimmedLine.Length > 5)
                {
                    assemblyRefs.Add(trimmedLine.Substring(4, trimmedLine.Length - 5));
                    lines[i] = "";
                }
                else if (trimmedLine.StartsWith("using ") && !trimmedLine.Contains("(") && trimmedLine.EndsWith(";") && trimmedLine.Length > 8)
                {
                    assemblyRefAllowed = false;
                    usings.Add(trimmedLine);
                    lines[i] = "";
                }
                else if (trimmedLine.StartsWith("//")) { }
                else if (trimmedLine.Contains("/*"))
                {
                    inBlockComment = true; // If the block comment appears within a string, this will not work... but I'm too lazy to properly parse the code 
                }
                else if (trimmedLine == "")
                {
                }
                else
                {
                    break;
                }
            }
            return string.Join(Environment.NewLine, lines);
        }
        
        internal static string ReplaceGlobalMethodCalls(string script)
        {
            var parser = new TextServices.ScriptParser();
            parser.Lex(script);
            var tokens = parser.GetTokens();

            // Replace method calls: Output() --> Output(<linenumber>)
            var methodCalls = tokens.FindMethodCall();
            var sb = new StringBuilder();
            var pos = 0;
            foreach (var call in methodCalls.Where(t => ScriptMethods.ContainsKey(t.StartToken.Text)))
            {
                if(call.StartToken.TokenIndex == 0 || tokens[call.StartToken.TokenIndex - 1].Type != CSharpLexer.DOT)
                {
                    var method = ScriptMethods[call.StartToken.Text];

                    // Global method called directly:
                    sb.Append(script.Substring(pos, call.StartToken.StartIndex - pos));
                    sb.Append(method.DeclaringType.FullName);
                    sb.Append(".");

                    pos = call.StartToken.StartIndex;
                }
            }
            sb.Append(script.Substring(pos));
            return sb.ToString();
        }

        public static Action<Model, UITreeSelection> CompileScript(string script, out CompilerResults compilerResults)
        {
            script = ParseAssemblyRefsAndUsings(script, out List<string> assemblyRefs, out List<string> usings);
            script = AddOutputLineNumbers(script);
            script = ReplaceGlobalMethodCalls(script);


            var code = string.Format(@"{0}
namespace TabularEditor.Scripting
{{
    public static class ScriptHost
    {{
        public static void Execute(TabularEditor.TOMWrapper.Model Model, TabularEditor.UI.UITreeSelection Selected)
        {{ 
#line 1
{1}
#line default
        }}
    }}
}}", string.Join(Environment.NewLine, usings), script);

            compilerResults = Compile(code, assemblyRefs);

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

        private const string CUSTOMACTIONS_CODEINDICATOR = "/* <#CUSTOMACTION#> */";

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
        public static void __CreateCustomActions(IList<TabularEditor.UI.Actions.IBaseAction> __am)
        {"
);
            for (var ix = 0; ix < actions.Actions.Length; ix++)
            {
                sb.Append(t3 + "__am.Add(__CustomAction");
                sb.Append(ix);
                sb.AppendLine("());");
            }
            CustomActionCount = actions.Actions.Length;
            var i = 0;
            sb.AppendLine(t2 + "}"); // End Method
            foreach (var act in actions.Actions)
            {
                sb.Append(t2 + "private static TabularEditor.UI.Actions.CustomAction __CustomAction");
                sb.Append(i);
                sb.AppendLine("() {");
                sb.AppendLine(t3 + "var __act = new TabularEditor.UI.Actions.CustomAction(");

                // EnabledDelegate:
                sb.Append(t4 + "(Selected, Model) => ");
                sb.Append(DelegateCode(act.Enabled, false));
                sb.AppendLine(",");

                // ExecuteDelegate:
                sb.AppendLine(t4 + "(Selected, Model) => {");
                sb.AppendLine(CUSTOMACTIONS_CODEINDICATOR);
                sb.AppendLine(act.CleansedCode);
                sb.AppendLine(t4 + "},");

                // Name:
                sb.AppendLine(t4 + "@\"" + act.Name.Replace("\"", "\"\"") + "\");");

                if (!string.IsNullOrEmpty(act.Tooltip))
                {
                    sb.Append(t3 + "__act.ToolTip = @\"");
                    sb.Append(act.Tooltip.Replace("\"", "\"\""));
                    sb.AppendLine("\";");
                }

                sb.Append(t3 + "__act.ValidContexts = (Context)");
                sb.Append((int)act.ValidContexts);
                sb.AppendLine(";");

                sb.AppendLine();
                sb.AppendLine("return __act;");

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
            if (actions == null || actions.Actions.Length == 0) return;

            var sw = new Stopwatch();
            sw.Start();

            var assemblyRefs = new List<string>();
            var usings = new List<string>();
            foreach(var act in actions.Actions)
            {
                act.CleansedCode = ParseAssemblyRefsAndUsings(act.Execute, out List<string> actionAssemblyRefs, out List<string> actionUsings);
                assemblyRefs.AddRange(actionAssemblyRefs);
                usings.AddRange(actionUsings);
            }

            var code = GetCustomActionsCode(actions);
            code = AddOutputLineNumbers(code);
            code = ReplaceGlobalMethodCalls(code);

            code = string.Format("{0}\n{1}", string.Join(Environment.NewLine, usings), code);

            var result = Compile(code, assemblyRefs, errorCallback);

            CustomActionError = result.Errors.Count > 0;

            if (!CustomActionError)
            {
                var assembly = result.CompiledAssembly;
                var type = assembly.GetType("TabularEditor.Scripting.CustomActions");
                var method = type.GetMethod("__CreateCustomActions");
                AddCustomActions = (Action<IList<IBaseAction>>)Delegate.CreateDelegate(typeof(Action<IList<IBaseAction>>), method);
            }

            sw.Stop();
            CustomActionCompiletime = sw.ElapsedMilliseconds;

            void errorCallback(CompilerErrorCollection errors, string source)
            {
                Console.WriteLine("Could not compile Custom Actions.");

                var messages = errors.Cast<CompilerError>()
                    .Select(e => $"({ e.Line + errors.Count },{ e.Column }) { (e.IsWarning ? "warning" : "error") } {e.ErrorNumber}: {e.ErrorText}").ToList();

                messages.ForEach(Console.WriteLine);
                try
                {
                    messages.Add(source);
                    File.WriteAllLines(CustomActionsErrorLogPath, messages);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to file [{ CustomActionsErrorLogPath }] { ex }");
                }
            }
        }

        public static long CustomActionCompiletime { get; private set; } = -1;
        public static int CustomActionCount { get; private set; } = 0;
        public static bool CustomActionError { get; private set; } = false;

        public static Action<IList<IBaseAction>> AddCustomActions { get; private set; } = null;

        private static readonly string[] defaultUsings = new string[]
        {
            "System",
            "System.Linq",
            "System.Collections.Generic",
            "Newtonsoft.Json",
            "TabularEditor.TOMWrapper",
            "TabularEditor.TOMWrapper.Utils",
            "TabularEditor.UI",
            "TOM = Microsoft.AnalysisServices.Tabular"
        };

        private static readonly string[] defaultAssemblies = new string[]
        {
            "system.data.dll",
            "system.dll",
            "system.windows.forms.dll",
            "system.core.dll",
            "microsoft.csharp.dll"
        };


        private static CompilerResults Compile(string code, List<string> customAssemblies = null, Action<CompilerErrorCollection, string> errorCallback = null)
        {
            CompilerResults result;

            // Apply default usings:
            string source = string.Format("{0}\n{1}", GetUsing(defaultUsings), code);

            // Default assemblies:
            var includeAssemblies = new HashSet<string>(defaultAssemblies, StringComparer.OrdinalIgnoreCase);
            includeAssemblies.Add(Assembly.GetExecutingAssembly().Location);       // TabularEditor.exe
            includeAssemblies.Add(WrapperDllPath);                                 // TOMWrapper.dll
            includeAssemblies.Add(TomDllPath);                                            // Microsoft.AnalysisServices.Tabular.dll
            includeAssemblies.Add(NewtonsoftJsonDllPath);                          // Newtonsoft.Json.dll
            includeAssemblies.Add("System.Xml.dll");                          // Newtonsoft.Json.dll

            // Custom assemblies:
            if (customAssemblies != null)
            {
                foreach (var asm in customAssemblies)
                {
                    if (string.IsNullOrEmpty(asm)) continue;

                    // First, probe the Tabular Editor installation folder for the assembly:
                    var probe = AppDomain.CurrentDomain.BaseDirectory + asm;
                    if (File.Exists(probe))
                    {
                        includeAssemblies.Add(probe);
                        continue;
                    }
                    
                    // Next, probe GAC:
                    var asmPath = GetAssemblyPath(asm);
                    if (!string.IsNullOrEmpty(asmPath))
                    {
                        includeAssemblies.Add(asmPath);
                        continue;
                    }

                    // Lastly, include ref as is - compiler will throw an error if not resolved:
                    includeAssemblies.Add(asm);
                }
            }

            using (var provider = GetProviderWithPreferences())
            {
                var compilerParams = GetCompilerParametersWithPreferences(includeAssemblies);
                result = provider.CompileAssemblyFromSource(compilerParams, source);
            }

            if (result.Errors.Count > 0)
            {
                errorCallback?.Invoke(result.Errors, source);
            }

            return result;
        }

        private static CodeDomProvider GetProviderWithPreferences()
        {
            var providerOptions = new Dictionary<string, string>();
            if(Directory.Exists(UIServices.Preferences.Current.ScriptCompilerDirectoryPath))
            {
                providerOptions["CompilerDirectoryPath"] = UIServices.Preferences.Current.ScriptCompilerDirectoryPath;
            }
            return new CSharpCodeProvider(providerOptions);
        }
        private static CompilerParameters GetCompilerParametersWithPreferences(IEnumerable<string> includeAssemblies)
        {
            var compilerParameters = new CompilerParameters(includeAssemblies.ToArray())
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true
            };
            if(!string.IsNullOrEmpty(UIServices.Preferences.Current.ScriptCompilerOptions))
            {
                compilerParameters.CompilerOptions = UIServices.Preferences.Current.ScriptCompilerOptions;
            }
            return compilerParameters;
        }

        // See: https://stackoverflow.com/questions/6121276/is-it-possible-to-load-an-assembly-from-the-gac-without-the-fullname
        public static string GetAssemblyPath(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            string finalName = name;
            AssemblyInfo aInfo = new AssemblyInfo();
            aInfo.cchBuf = 1024; // should be fine...
            aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);

            IAssemblyCache ac;
            int hr = CreateAssemblyCache(out ac, 0);
            if (hr >= 0)
            {
                hr = ac.QueryAssemblyInfo(0, finalName, ref aInfo);
                if (hr < 0)
                    return null;
            }

            return aInfo.currentAssemblyPath;
        }


        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        private interface IAssemblyCache
        {
            void Reserved0();

            [PreserveSig]
            int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AssemblyInfo
        {
            public int cbAssemblyInfo;
            public int assemblyFlags;
            public long assemblySizeInKB;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string currentAssemblyPath;
            public int cchBuf; // size of path buf.
        }

        [DllImport("fusion.dll")]
        private static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

        private static string GetUsing(IEnumerable<string> usingStatements)
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
                    if (wrapperVersion.FileVersion != currentVersion.ToString())
                    {
                        OutputWrapperDll();
                    }
                }

                if (!File.Exists(NewtonsoftJsonDllPath))
                {
                    (new FileInfo(NewtonsoftJsonDllPath)).Directory.Create();
                    OutputNewtonsoftJsonDll();
                }
                else
                {
                    // Check if Newtonsoft.Json.dll is of same version as that used within TabularEditor.exe. If not, output a new one:
                    var wrapperVersion = FileVersionInfo.GetVersionInfo(NewtonsoftJsonDllPath);
                    var currentVersion = Assembly.GetAssembly(typeof(JsonConvert)).GetName().Version;
                    if (wrapperVersion.FileVersion != currentVersion.ToString())
                    {
                        OutputNewtonsoftJsonDll();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ScriptEngineStatus = "Error: " + ex.Message;
            }
        }

        private static void InitCustomActions()
        {
            try
            {
                if (File.Exists(CustomActionsJsonPath))
                {
                    Console.WriteLine("Loading custom actions from: " + CustomActionsJsonPath);
                    var actions = CustomActionsJson.LoadFromJson(CustomActionsJsonPath);
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

        public static Dictionary<string, MethodInfo> ScriptMethods = new Dictionary<string, MethodInfo>();

        private static void FindScriptMethods(IEnumerable<Assembly> assemblies)
        {
            foreach(var asm in assemblies)
            {
                asm.GetTypes()
                    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    .Where(m => m.GetCustomAttributes(typeof(ScriptMethodAttribute), false).Length > 0)
                    .ToList().ForEach(mi => { if (!ScriptMethods.ContainsKey(mi.Name)) ScriptMethods.Add(mi.Name, mi); });
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
            InitCustomActions();
        }

        public static string ScriptEngineStatus { get; private set; }

        private static void OutputWrapperDll()
        {
            // Export the TOMWrapper library to a .DLL for use with the custom script execution:
            MemoryStream memory = new MemoryStream();
            var currentAssembly = Assembly.GetAssembly(typeof(TabularEditor.Program));
            DeflateStream stream = new DeflateStream(currentAssembly.GetManifestResourceStream("costura.tomwrapper.dll.compressed"), CompressionMode.Decompress);
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

        private static void OutputNewtonsoftJsonDll()
        {
            // Export the TOMWrapper library to a .DLL for use with the custom script execution:
            MemoryStream memory = new MemoryStream();
            var currentAssembly = Assembly.GetAssembly(typeof(TabularEditor.Program));
            DeflateStream stream = new DeflateStream(currentAssembly.GetManifestResourceStream("costura.newtonsoft.json.dll.compressed"), CompressionMode.Decompress);
            if (stream != null)
            {
                using (stream)
                {
                    stream.CopyTo(memory);
                    byte[] data = memory.ToArray();
                    File.WriteAllBytes(NewtonsoftJsonDllPath, data);
                }
            }
        }
    }

    public class ScriptCancelledException: Exception
    {
    }
}

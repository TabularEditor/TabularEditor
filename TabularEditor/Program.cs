using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using TabularEditor.TOMWrapper;
using BPA = TabularEditor.BestPracticeAnalyzer;
using Microsoft.WindowsAPICodePack.Dialogs;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.Scripting;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Runtime.InteropServices;

namespace TabularEditor
{
    static class Program
    {
        public static bool CommandLineMode { get; private set; } = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                cw.WriteLine("");
                cw.WriteLine(Application.ProductName + " " + Application.ProductVersion);
                cw.WriteLine("--------------------------------");
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var plugins = LoadPlugins();
            SetupLibraries(plugins);

            CommandLineMode = true;
            if (args.Length > 1 && HandleCommandLine(args))
            {
                if (enableVSTS)
                {
                    cw.WriteLine("##vso[task.complete result={0};]Done.", errorCount > 0 ? "Failed" : ((warningCount > 0) ? "SucceededWithIssues" : "Succeeded"));
                }
                Environment.Exit(errorCount > 0 ? 1 : 0);
                return;
            }
            CommandLineMode = false;

            var mf = new FormMain();
            mf.ShowDialog();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            if (
                asmName.Name == "Microsoft.AnalysisServices.Core" ||
                asmName.Name == "Microsoft.AnalysisServices.Tabular" ||
                asmName.Name == "Microsoft.AnalysisServices.Tabular.Json"
                )
            {
                var td = new TaskDialog();
                td.Text = @"This version of Tabular Editor requires the SQL AS AMO library version 15.0.0 (or newer).

Microsoft.AnalysisServices.Core.dll
Microsoft.AnalysisServices.Tabular.dll
Microsoft.AnalysisServices.Tabular.Json.dll

The AMO library may be downloaded from <A HREF=""https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-data-providers"">here</A>.";
                td.Caption = "Missing DLL dependencies";

                td.Icon = TaskDialogStandardIcon.Error;
                td.HyperlinksEnabled = true;
                td.HyperlinkClick += Td_HyperlinkClick;
                td.Show();
                Environment.Exit(1);
            }

            return null;
        }

        private static void Td_HyperlinkClick(object sender, TaskDialogHyperlinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        public static readonly GUIConsoleWriter cw = new GUIConsoleWriter();

        /// <summary>
        /// Make sure that the TOMWrapper.dll is available in the current user's temp folder.
        /// Also, compiles current user's CustomActions.xml and loads them into the editor.
        /// </summary>
        static void SetupLibraries(IList<Assembly> plugins)
        {
            ScriptEngine.InitScriptEngine(plugins);
        }

        /// <summary>
        /// Scans executable directory for .dll's to load
        /// </summary>
        static IList<Assembly> LoadPlugins()
        {
            List<Assembly> pluginAssemblies = new List<Assembly>();

            foreach (var dll in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                try
                {
                    var pluginAssembly = Assembly.LoadFile(dll);
                    if (pluginAssembly != null && !pluginAssembly.FullName.StartsWith("TOMWrapper"))
                    {
                        var pluginType = pluginAssembly.GetTypes().Where(t => typeof(ITabularEditorPlugin).IsAssignableFrom(t)).FirstOrDefault();
                        if (pluginType != null)
                        {
                            var plugin = Activator.CreateInstance(pluginType) as ITabularEditorPlugin;
                            if (plugin != null)
                            {
                                Plugins.Add(plugin);
                                pluginAssemblies.Add(pluginAssembly);
                                Console.WriteLine("Succesfully loaded plugin " + pluginType.Name + " from assembly " + Path.GetFileName(dll));
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            return pluginAssemblies;
        }

        public static List<ITabularEditorPlugin> Plugins = new List<ITabularEditorPlugin>();

        static bool enableVSTS;
        static int errorCount = 0;
        static int warningCount = 0;

        public static void Error(string errorMessage, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine("##vso[task.logissue type=error;]" + errorMessage, args);
            }
            else
                cw.WriteLine(errorMessage, args);

            errorCount++;
        }
        public static void Warning(string errorMessage, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine("##vso[task.logissue type=warning;]" + errorMessage, args);
            }
            else
                cw.WriteLine(errorMessage, args);

            warningCount++;
        }

        static void ErrorX(string errorMessage, string sourcePath, int line, int column, string code, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine(string.Format("##vso[task.logissue type=error;sourcepath={0};linenumber={1};columnnumber={2};code={3}]{4}", sourcePath, line, column, code, errorMessage), args);
            }
            else
                cw.WriteLine(string.Format("Error {0} on line {1}, col {2}: {3}", code, line, column, errorMessage));

            errorCount++;
        }

        static bool HandleCommandLine(string[] args)
        {
            var upperArgList = args.Select(arg => arg.ToUpper()).ToList();
            var argList = args.Select(arg => arg).ToList();
            if (upperArgList.Contains("-?") || upperArgList.Contains("/?") || upperArgList.Contains("-H") || upperArgList.Contains("/H") || upperArgList.Contains("HELP"))
            {
                OutputUsage();
                return true;
            }

            enableVSTS = upperArgList.IndexOf("-VSTS") > -1 || upperArgList.IndexOf("-V") > -1;
            var warnOnUnprocessed = upperArgList.IndexOf("-WARN") > -1 || upperArgList.IndexOf("-W") > -1;
            var errorOnDaxErr = upperArgList.IndexOf("-ERR") > -1 || upperArgList.IndexOf("-E") > -1;

            TabularModelHandler h;
            if (args.Length == 2 || args[2].StartsWith("-"))
            {
                // File argument provided (either alone or with switches), i.e.:
                //      TabularEditor.exe myfile.bim
                //      TabularEditor.exe myfile.bim -...

                if (!File.Exists(args[1]) && !File.Exists(args[1] + "\\database.json"))
                {
                    Error("File not found: {0}", args[1]);
                    return true;
                }
                else
                {
                    // If nothing else was specified on the command-line, open the UI:
                    if (args.Length == 2) return false;
                }

                try
                {
                    h = new TOMWrapper.TabularModelHandler(args[1]);
                }
                catch (Exception e)
                {
                    Error("Error loading file: " + e.Message);
                    return true;
                }

            }
            else if (args.Length == 3 || args[3].StartsWith("-"))
            {
                // Server + Database argument provided (either alone or with switches), i.e.:
                //      TabularEditor.exe localhost AdventureWorks
                //      TabularEditor.exe localhost AdventureWorks -...
                // If nothing else was specified on the command-line, open the UI:
                if (args.Length == 3) return false;

                try
                {
                    h = new TOMWrapper.TabularModelHandler(args[1], args[2]);
                }
                catch (Exception e)
                {
                    Error("Error loading model: " + e.Message);
                    return true;
                }
            }
            else
            {
                // Otherwise, it's nonsensical
                return false;
            }

            string script = null;
            string scriptFile = null;

            var doScript = upperArgList.IndexOf("-SCRIPT");
            if (doScript == -1) doScript = upperArgList.IndexOf("-S");
            if (doScript > -1)
            {
                if (upperArgList.Count <= doScript)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                scriptFile = argList[doScript + 1];
                if (!File.Exists(scriptFile))
                {
                    Error("Specified script file not found.\n");
                    return true;
                }

                script = File.ReadAllText(scriptFile);
            }

            var doCheckDs = upperArgList.IndexOf("-SCHEMACHECK");
            if (doCheckDs == -1) doCheckDs = upperArgList.IndexOf("-SC");

            string saveToFolderOutputPath = null;
            string saveToFolderReplaceId = null;

            var doSaveToFolder = upperArgList.IndexOf("-FOLDER");
            if (doSaveToFolder == -1) doSaveToFolder = upperArgList.IndexOf("-F");
            if (doSaveToFolder > -1)
            {
                if (upperArgList.Count <= doSaveToFolder)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                saveToFolderOutputPath = argList[doSaveToFolder + 1];
                if (doSaveToFolder + 2 < argList.Count && !argList[doSaveToFolder + 2].StartsWith("-")) saveToFolderReplaceId = argList[doSaveToFolder + 2];
                var directoryName = new FileInfo(saveToFolderOutputPath).Directory.FullName;
                Directory.CreateDirectory(saveToFolderOutputPath);
            }

            string buildOutputPath = null;
            string buildReplaceId = null;

            var doSave = upperArgList.IndexOf("-BUILD");
            if (doSave == -1) doSave = upperArgList.IndexOf("-B");
            if (doSave == -1) doSave = upperArgList.IndexOf("-BIM");
            if (doSave > -1)
            {
                if (upperArgList.Count <= doSave)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                buildOutputPath = argList[doSave + 1];
                if (doSave + 2 < argList.Count && !argList[doSave + 2].StartsWith("-")) buildReplaceId = argList[doSave + 2];
                var directoryName = new FileInfo(buildOutputPath).Directory.FullName;
                Directory.CreateDirectory(directoryName);
            }

            if (doSaveToFolder > -1 && doSave > -1)
            {
                Error("-FOLDER and -BUILD arguments are mutually exclusive.\n");
                OutputUsage();
                return true;
            }

            // Load model:
            cw.WriteLine("Loading model...");

            if (!string.IsNullOrEmpty(script))
            {
                cw.WriteLine("Executing script...");

                System.CodeDom.Compiler.CompilerResults result;
                Scripting.ScriptOutputForm.Reset(false);
                var dyn = ScriptEngine.CompileScript(script, out result);
                if (result.Errors.Count > 0)
                {
                    Error("Script compilation errors:");
                    foreach (System.CodeDom.Compiler.CompilerError err in result.Errors)
                    {
                        ErrorX(err.ErrorText, scriptFile, err.Line, err.Column, err.ErrorNumber);
                    }
                    return true;
                }
                try
                {
                    dyn.Invoke(h.Model, null);
                }
                catch (Exception ex)
                {
                    Error("Script execution error: " + ex.Message);
                    return true;
                }
            }

            if (doCheckDs > -1)
            {
                cw.WriteLine("Checking source schema...");
                ScriptHelper.SchemaCheck(h.Model);
            }

            if (!string.IsNullOrEmpty(buildOutputPath))
            {
                cw.WriteLine("Building Model.bim file...");
                if (buildReplaceId != null) { h.Database.Name = buildReplaceId; h.Database.ID = buildReplaceId; }
                h.Save(buildOutputPath, SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
            }
            else if (!string.IsNullOrEmpty(saveToFolderOutputPath))
            {
                cw.WriteLine("Saving Model.bim file to Folder Output Path ...");
                if (buildReplaceId != null) { h.Database.Name = buildReplaceId; h.Database.ID = buildReplaceId; }

                //Note the last parameter, we use whatever SerializeOptions are already in the file
                h.Save(saveToFolderOutputPath, SaveFormat.TabularEditorFolder, null, true);
            }

            var replaceMap = new Dictionary<string, string>();

            var analyze = upperArgList.IndexOf("-ANALYZE");
            if (analyze == -1) analyze = upperArgList.IndexOf("-A");
            if (analyze > -1)
            {
                var rulefile = analyze + 1 < argList.Count ? argList[analyze + 1] : "";
                if (rulefile.StartsWith("-") || string.IsNullOrEmpty(rulefile)) rulefile = null;

                var analyzer = new BPA.Analyzer() { Model = h.Model };

                BPA.BestPracticeCollection suppliedRules = null;
                if (!string.IsNullOrEmpty(rulefile))
                {
                    if (!File.Exists(rulefile))
                    {
                        Error("Rulefile not found: {0}", rulefile);
                        return true;
                    }
                    try
                    {
                        suppliedRules = BPA.BestPracticeCollection.GetCollectionFromFile(rulefile);
                    }
                    catch
                    {
                        Error("Invalid rulefile: {0}", rulefile);
                        return true;
                    }
                }

                cw.WriteLine("Running Best Practice Analyzer...");
                cw.WriteLine("=================================");
                IEnumerable<BPA.AnalyzerResult> bpaResults;
                if (suppliedRules == null) bpaResults = analyzer.AnalyzeAll();
                else bpaResults = analyzer.Analyze(suppliedRules.Concat(analyzer.ModelRules));

                bool none = true;
                foreach (var res in bpaResults.Where(r => !r.Ignored))
                {
                    if (res.InvalidCompatibilityLevel)
                    {
                        cw.WriteLine("Skipping rule '{0}' as it does not apply to Compatibility Level {1}.", res.RuleName, h.CompatibilityLevel);
                    }
                    else
                    if (res.RuleHasError)
                    {
                        none = false;
                        Warning("Error on rule '{0}': {1}", res.RuleName, res.RuleError);
                    }
                    else
                    {
                        none = false;
                        if (res.Object != null)
                        {
                            var text = string.Format("{0} {1} violates rule \"{2}\"",
                                res.Object.GetTypeName(),
                                (res.Object as IDaxObject)?.DaxObjectFullName ?? res.ObjectName,
                                res.RuleName
                                );
                            if (res.Rule.Severity <= 1) cw.WriteLine(text);
                            else if (res.Rule.Severity == 2) Warning(text);
                            else if (res.Rule.Severity >= 3) Error(text);
                        }
                    }

                }
                if (none) cw.WriteLine("No objects in violation of Best Practices.");
                cw.WriteLine("=================================");
            }

            var deploy = upperArgList.IndexOf("-DEPLOY");
            if (deploy == -1) deploy = upperArgList.IndexOf("-D");
            if (deploy > -1)
            {
                var serverName = argList.Skip(deploy + 1).FirstOrDefault(); if (serverName != null && serverName.StartsWith("-")) serverName = null;
                var databaseID = argList.Skip(deploy + 2).FirstOrDefault(); if (databaseID != null && databaseID.StartsWith("-")) databaseID = null;

                var conn = upperArgList.IndexOf("-CONNECTIONS");
                if (conn == -1) conn = upperArgList.IndexOf("-C");
                if (conn > -1)
                {
                    var replaces = argList.Skip(conn + 1).TakeWhile(s => s[0] != '-').ToList();

                    if (replaces.Count > 0 && replaces.Count % 2 == 0)
                    {
                        // Placeholder replacing:
                        for (var index = 0; index < replaces.Count; index = index + 2)
                        {
                            replaceMap.Add(replaces[index], replaces[index + 1]);
                        }
                    }
                }

                string userName = null;
                string password = null;
                var options = DeploymentOptions.StructureOnly;

                var switches = args.Skip(deploy + 1).Where(arg => arg.StartsWith("-")).Select(arg => arg.ToUpper()).ToList();

                if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(databaseID))
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                if (switches.Contains("-L") || switches.Contains("-LOGIN"))
                {
                    var switchPos = upperArgList.IndexOf("-LOGIN"); if (switchPos == -1) switchPos = upperArgList.IndexOf("-L");
                    userName = argList.Skip(switchPos + 1).FirstOrDefault(); if (userName != null && userName.StartsWith("-")) userName = null;
                    password = argList.Skip(switchPos + 2).FirstOrDefault(); if (password != null && password.StartsWith("-")) password = null;
                    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    {
                        Error("Missing username or password.\n");
                        OutputUsage();
                        return true;
                    }
                    switches.Remove("-L"); switches.Remove("-LOGIN");
                }
                if (switches.Contains("-O") || switches.Contains("-OVERWRITE"))
                {
                    options.DeployMode = DeploymentMode.CreateOrAlter;
                    switches.Remove("-O"); switches.Remove("-OVERWRITE");
                }
                else
                {
                    options.DeployMode = DeploymentMode.CreateDatabase;
                }
                if (switches.Contains("-P") || switches.Contains("-PARTITIONS"))
                {
                    options.DeployPartitions = true;
                    switches.Remove("-P"); switches.Remove("-PARTITIONS");
                }
                if (switches.Contains("-C") || switches.Contains("-CONNECTIONS"))
                {
                    options.DeployConnections = true;
                    switches.Remove("-C"); switches.Remove("-CONNECTIONS");
                }
                if (switches.Contains("-R") || switches.Contains("-ROLES"))
                {
                    options.DeployRoles = true;
                    switches.Remove("-R"); switches.Remove("-ROLES");

                    if (switches.Contains("-M") || switches.Contains("-MEMBERS"))
                    {
                        options.DeployRoleMembers = true;
                        switches.Remove("-M"); switches.Remove("-MEMBERS");
                    }
                }
                var xmla_scripting_only = switches.Contains("-X") || switches.Contains("-XMLA");
                string xmla_script_file = null;
                if (xmla_scripting_only)
                {
                    var switchPos = upperArgList.IndexOf("-XMLA"); if (switchPos == -1) switchPos = upperArgList.IndexOf("-X");
                    xmla_script_file = argList.Skip(switchPos + 1).FirstOrDefault(); if (String.IsNullOrWhiteSpace(xmla_script_file) && xmla_script_file.StartsWith("-")) xmla_script_file = null;
                    if (string.IsNullOrEmpty(xmla_script_file))
                    {
                        Error("Missing xmla_script_file.\n");
                        OutputUsage();
                        return true;
                    }
                    switches.Remove("-X");
                    switches.Remove("-XMLA");

                }
                /*if(switches.Count > 0)
                {
                    Error("Unknown switch {0}\n", switches[0]);
                    OutputUsage();
                    return true;
                }*/

                try
                {
                    if (replaceMap.Count > 0)
                    {
                        cw.WriteLine("Switching connection string placeholders...");
                        foreach (var map in replaceMap) h.Model.DataSources.SetPlaceholder(map.Key, map.Value);
                    }

                    var cs = string.IsNullOrEmpty(userName) ? TabularConnection.GetConnectionString(serverName) :
                        TabularConnection.GetConnectionString(serverName, userName, password);
                    if (xmla_scripting_only)
                    {
                        cw.WriteLine("Generating XMLA/TMSL script...");
                        var s = new TOM.Server();
                        s.Connect(cs);
                        var xmla = TabularDeployer.GetTMSL(h.Database, s, databaseID, options);
                        using (var sw = new StreamWriter(xmla_script_file))
                        {
                            sw.Write(xmla);
                        }
                        cw.WriteLine("XMLA/TMSL script is generated.");
                    }
                    else
                    {
                        cw.WriteLine("Deploying...");
                        var deploymentResult = TabularDeployer.Deploy(h, cs, databaseID, options);
                        cw.WriteLine("Deployment succeeded.");
                        foreach (var err in deploymentResult.Issues) if (errorOnDaxErr) Error(err); else Warning(err);
                        foreach (var err in deploymentResult.Warnings) Warning(err);
                        foreach (var err in deploymentResult.Unprocessed)
                            if (warnOnUnprocessed) Warning(err); else cw.WriteLine(err);
                    }
                }
                catch (Exception ex)
                {
                    Error($"{(xmla_scripting_only ? "Script generation" : "Deployment")} failed! {ex.Message}");
                }
                return true;
            }

            return true;
        }

        static void OutputUsage()
        {
            cw.WriteLine(@"Usage:

TABULAREDITOR ( file | server database ) [-S script] [-SC] [(-B | -F) output [id]] [-A [rulefile]] [-V]
    [-D server database [-L user pass] [-O [-C [plch1 value1 [plch2 value2 [...]]]] [-P]] [-R [-M]] [-W]] -X xmla_script_file

file                Full path of the Model.bim file or database.json model folder to load.
server              Server\instance name or connection string from which to load the model
database            Database ID of the model to load
-S / -SCRIPT        Execute the specified script on the model after loading.
  script              Full path of a file containing a C# script to execute.
-SC / -SCHEMACHECK  Attempts to connect to all Provider Data Sources in order to detect
                      table schema changes. Outputs...
                        ...warnings for mismatched data types and unmapped source columns
                        ...errors for unmapped model columns.
-B / -BUILD         Saves the model (after optional script execution) as a Model.bim file.
  output              Full path of the Model.bim file to save to.
-F / -FOLDER         Saves the model (after optional script execution) as a series of JSON objects in a folder.
  output              Full path of the folder to save to.
  id                  Optional id/name to assign to the Database object when saving.
-V / -VSTS          Output Visual Studio Team Services logging commands.
-A / -ANALYZE       Runs Best Practice Analyzer and outputs the result to the console.
  rulefile            Optional path of file containing BPA rules to be analyzed. If not
                      specified, model is analyzed against global rules on the machine.
-D / -DEPLOY        Command-line deployment
  server              Name of server to deploy to or connection string to Analysis Services.
  database            ID of the database to deploy (create/overwrite).
  -L / -LOGIN         Disables integrated security when connecting to the server. Specify:
    user                Username (must be a user with admin rights on the server)
    pass                Password
  -O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
    -C / -CONNECTIONS   Deploy (overwrite) existing data sources in the model. After the -C
                        switch, you can (optionally) specify any number of placeholder-value
                        pairs. Doing so, will replace any occurrence of the specified
                        placeholders (plch1, plch2, ...) in the connection strings of every
                        data source in the model, with the specified values (value1, value2, ...).
    -P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
  -R / -ROLES         Deploy roles.
    -M / -MEMBERS       Deploy role members.
  -W / -WARN          Outputs information about unprocessed objects as warnings.
  -X / -XMLA          No deployment will be performed. Generate XMLA/TMSL script for later deployment instead. 
    xmla_script_file    File name of the new XMLA/TMSL script output");
        }
    }
}

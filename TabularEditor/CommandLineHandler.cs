using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TabularEditor.TOMWrapper;
using BPA = TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.Scripting;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.UIServices;
using System.Text.RegularExpressions;

namespace TabularEditor
{
    internal class CommandLineHandler
    {
        public bool EnableVSTS { get; private set; }
        public int ErrorCount { get; private set; } = 0;
        public int WarningCount { get; private set; } = 0;
        public bool LaunchUi { get; private set; } = false;
        internal bool CommandLineMode { get; private set; } = false;

        internal void Error(string errorMessage, params object[] args)
        {
            if (EnableVSTS)
            {
                if (args.Length == 0)
                    Console.WriteLine("##vso[task.logissue type=error;]" + errorMessage);
                else
                    Console.WriteLine("##vso[task.logissue type=error;]" + errorMessage, args);
            }
            else
                if (args.Length == 0)
                Console.WriteLine(errorMessage);
            else
                Console.WriteLine(errorMessage, args);

            ErrorCount++;
        }
        internal void Warning(string errorMessage, params object[] args)
        {
            if (EnableVSTS)
            {
                if (args.Length == 0)
                    Console.WriteLine("##vso[task.logissue type=warning;]" + errorMessage);
                else
                    Console.WriteLine("##vso[task.logissue type=warning;]" + errorMessage, args);
            }
            else
                if (args.Length == 0)
                Console.WriteLine(errorMessage);
            else
                Console.WriteLine(errorMessage, args);

            WarningCount++;
        }

        void ErrorX(string errorMessage, string sourcePath, int line, int column, string code, params object[] args)
        {
            if (EnableVSTS)
            {
                Console.WriteLine(string.Format("##vso[task.logissue type=error;sourcepath={0};linenumber={1};columnnumber={2};code={3}]{4}", sourcePath, line, column, code, errorMessage), args);
            }
            else
                Console.WriteLine(string.Format("Error {0} on line {1}, col {2}: {3}", code, line, column, errorMessage));

            ErrorCount++;
        }

        List<string> Scripts = new List<string>();
        List<string> ScriptFiles = new List<string>();
        TabularModelHandler Handler;

        internal void HandleCommandLine(string[] args)
        {
            CommandLineMode = true;
            try
            {
                InternalHandleCommandLine(args);
            }
            catch(CommandLineException)
            {

            }
            CommandLineMode = false;
        }

        List<string> upperArgList;
        List<string> argList;
        bool warnOnUnprocessed;
        bool errorOnDaxErr;
        Dictionary<string, string> replaceMap = new Dictionary<string, string>();


        private void InternalHandleCommandLine(string[] args)
        {
            upperArgList = args.Select(arg => arg.ToUpper()).ToList();
            argList = args.ToList();
            if (upperArgList.Contains("-?") || upperArgList.Contains("/?") || upperArgList.Contains("-H") || upperArgList.Contains("/H") || upperArgList.Contains("HELP"))
            {
                OutputUsage();
                return;
            }

            EnableVSTS = upperArgList.IndexOf("-VSTS") > -1 || upperArgList.IndexOf("-V") > -1;
            warnOnUnprocessed = upperArgList.IndexOf("-WARN") > -1 || upperArgList.IndexOf("-W") > -1;
            errorOnDaxErr = upperArgList.IndexOf("-ERR") > -1 || upperArgList.IndexOf("-E") > -1;

            LoadModel();

            var doTestRun = upperArgList.IndexOf("-T");
            string testRunFile = null;
            if (doTestRun == -1) doTestRun = upperArgList.IndexOf("-TRX");
            if (doTestRun > -1)
            {
                if (upperArgList.Count <= doTestRun || upperArgList[doTestRun + 1].StartsWith("-"))
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return;
                }
                Program.testRun = new TestRun(Handler.Database?.Name ?? Handler.Source);
                testRunFile = argList[doTestRun + 1];
            }

            var doScript = upperArgList.IndexOf("-SCRIPT");
            if (doScript == -1) doScript = upperArgList.IndexOf("-S");
            if (doScript > -1)
            {
                if (upperArgList.Count <= doScript)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return;
                }
                ScriptFiles = argList.Skip(doScript + 1).TakeWhile(s => !s.StartsWith("-")).ToList();
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
                    return;
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
                    return;
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
                return;
            }

            if (doScript > -1) ExecuteScripts();

            if (doCheckDs > -1)
            {
                Console.WriteLine("Checking source schema...");
                ScriptHelper.SchemaCheck(Handler.Model);
            }

            if (!string.IsNullOrEmpty(buildOutputPath))
            {
                Console.WriteLine("Building Model.bim file...");
                if (buildReplaceId != null) { Handler.Database.Name = buildReplaceId; Handler.Database.ID = buildReplaceId; }
                Handler.Save(buildOutputPath, SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
            }
            else if (!string.IsNullOrEmpty(saveToFolderOutputPath))
            {
                Console.WriteLine("Saving Model.bim file to Folder Output Path ...");
                if (buildReplaceId != null) { Handler.Database.Name = buildReplaceId; Handler.Database.ID = buildReplaceId; }

                //Note the last parameter, we use whatever SerializeOptions are already in the file
                Handler.Save(saveToFolderOutputPath, SaveFormat.TabularEditorFolder, null, true);
            }

            var doAnalyze = upperArgList.IndexOf("-ANALYZE");
            if (doAnalyze == -1) doAnalyze = upperArgList.IndexOf("-A");
            if (doAnalyze > -1)
            {
                var rulefile = doAnalyze + 1 < argList.Count ? argList[doAnalyze + 1] : "";
                if (rulefile.StartsWith("-") || string.IsNullOrEmpty(rulefile)) rulefile = null;

                AnalyzeBestPracticeRules(rulefile);
            }

            var doDeploy = upperArgList.IndexOf("-DEPLOY");
            if (doDeploy == -1) doDeploy = upperArgList.IndexOf("-D");
            if (doDeploy > -1)
            {
                var serverName = argList.Skip(doDeploy + 1).FirstOrDefault(); if (serverName == null || serverName.StartsWith("-")) serverName = null;
                var databaseID = argList.Skip(doDeploy + 2).FirstOrDefault(); if (databaseID != null && databaseID.StartsWith("-")) databaseID = null;

                Deploy(serverName, databaseID, doDeploy);
            }
            if (Program.testRun != null)
            {
                Program.testRun.SerializeAsVSTest(testRunFile);
                Console.WriteLine("VSTest XML file saved: " + testRunFile);
            }
        }

        void LoadModel()
        {
            if (argList.Count == 2 || argList[2].StartsWith("-"))
            {
                // File argument provided (either alone or with switches), i.e.:
                //      TabularEditor.exe myfile.bim
                //      TabularEditor.exe myfile.bim -...

                if (!File.Exists(argList[1]) && !File.Exists(argList[1] + "\\database.json"))
                {
                    Error("File not found: {0}", argList[1]);
                    throw new CommandLineException();
                }
                else
                {
                    // If nothing else was specified on the command-line, open the UI:
                    if (argList.Count == 2)
                    {
                        LaunchUi = true;
                        throw new CommandLineException();
                    }
                }

                try
                {
                    // Load model:
                    Console.WriteLine("Loading model...");
                    Handler = new TOMWrapper.TabularModelHandler(argList[1]);
                }
                catch (Exception e)
                {
                    Error("Error loading file: " + e.Message);
                    throw new CommandLineException();
                }

            }
            else if (argList.Count == 3 || argList[3].StartsWith("-"))
            {
                // Server + Database argument provided (either alone or with switches), i.e.:
                //      TabularEditor.exe localhost AdventureWorks
                //      TabularEditor.exe localhost AdventureWorks -...
                // If nothing else was specified on the command-line, open the UI:
                if (argList.Count == 3)
                {
                    LaunchUi = true;
                    throw new CommandLineException();
                }

                try
                {
                    // Load model:
                    Console.WriteLine("Loading model...");
                    Handler = new TOMWrapper.TabularModelHandler(argList[1], argList[2]);
                }
                catch (Exception e)
                {
                    Error("Error loading model: " + e.Message);
                    throw new CommandLineException();
                }
            }
            else
            {
                // Otherwise, it's nonsensical
                LaunchUi = true;
                throw new CommandLineException();
            }
        }

        void OutputUsage()
        {
            Console.WriteLine(@"Usage:

TABULAREDITOR ( file | server database ) [-S script1 [script2] [...]]
    [-SC] [-A [rules]] [(-B | -F) output [id]] [-V] [-T resultsfile]
    [-D [server database [-L user pass] [-O [-C [plch1 value1 [plch2 value2 [...]]]] [-P] [-R [-M]]]
        [-X xmla_script]] [-W] [-E]]

file                Full path of the Model.bim file or database.json model folder to load.
server              Server\instance name or connection string from which to load the model
database            Database ID of the model to load
-S / -SCRIPT        Execute the specified script on the model after loading.
  scriptN             Full path of one or more files containing a C# script to execute or an inline
                      script.
-SC / -SCHEMACHECK  Attempts to connect to all Provider Data Sources in order to detect table schema
                    changes. Outputs...
                      ...warnings for mismatched data types and unmapped source columns
                      ...errors for unmapped model columns.
-A / -ANALYZE       Runs Best Practice Analyzer and outputs the result to the console.
  rules               Optional path of file or URL of additional BPA rules to be analyzed. If
                      specified, model is not analyzed against local user/local machine rules,
                      but rules defined within the model are still applied.
-B / -BIM / -BUILD  Saves the model (after optional script execution) as a Model.bim file.
  output              Full path of the Model.bim file to save to.
  id                  Optional id/name to assign to the Database object when saving.
-F / -FOLDER        Saves the model (after optional script execution) as a Folder structure.
  output              Full path of the folder to save to. Folder is created if it does not exist.
  id                  Optional id/name to assign to the Database object when saving.
-V / -VSTS          Output Visual Studio Team Services logging commands.
-T / -TRX         Produces a VSTEST (trx) file with details on the execution.
  resultsfile       File name of the VSTEST XML file.
-D / -DEPLOY        Command-line deployment
                      If no additional parameters are specified, this switch will save model metadata
                      back to the source (file or database).
  server              Name of server to deploy to or connection string to Analysis Services.
  database            ID of the database to deploy (create/overwrite).
  -L / -LOGIN         Disables integrated security when connecting to the server. Specify:
    user                Username (must be a user with admin rights on the server)
    pass                Password
  -O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
    -C / -CONNECTIONS   Deploy (overwrite) existing data sources in the model. After the -C switch, you
                        can (optionally) specify any number of placeholder-value pairs. Doing so, will
                        replace any occurrence of the specified placeholders (plch1, plch2, ...) in the
                        connection strings of every data source in the model, with the specified values
                        (value1, value2, ...).
    -P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
    -R / -ROLES         Deploy roles.
      -M / -MEMBERS       Deploy role members.
  -X / -XMLA        No deployment. Generate XMLA/TMSL script for later deployment instead.
    xmla_script       File name of the new XMLA/TMSL script output.
  -W / -WARN        Outputs information about unprocessed objects as warnings.
  -E / -ERR         Returns a non-zero exit code if Analysis Services returns any error messages after
                      the metadata was deployed / updated.");
        }

        void Deploy(string serverName, string databaseID, int doDeploy)
        {
            // Perform direct save:
            if (serverName == null)
            {
                var nextSwitch = upperArgList.Skip(doDeploy + 1).FirstOrDefault();
                var deploySwitches = new[] { "-L", "-LOGIN", "-O", "-OVERWRITE", "-C", "-CONNECTIONS", "-P", "-PARTITIONS", "-R", "-ROLES", "-M", "-MEMBERS", "-X", "-XMLA" };
                if (deploySwitches.Contains(nextSwitch))
                {
                    Error("Invalid argument syntax.");
                    OutputUsage();
                    throw new CommandLineException();
                }

                Console.WriteLine("Saving model metadata back to source...");
                if (Handler.SourceType == ModelSourceType.Database)
                {
                    try
                    {
                        Handler.SaveDB();
                        Console.WriteLine("Model metadata saved.");

                        var deploymentResult = Handler.GetLastDeploymentResults();
                        foreach (var err in deploymentResult.Issues) if (errorOnDaxErr) Error(err); else Warning(err);
                        foreach (var err in deploymentResult.Warnings) Warning(err);
                        foreach (var err in deploymentResult.Unprocessed) if (warnOnUnprocessed) Warning(err); else Console.WriteLine(err);
                    }
                    catch (Exception ex)
                    {
                        Error("Save failed: " + ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        Handler.Save(Handler.Source, Handler.SourceType == ModelSourceType.Folder ? SaveFormat.TabularEditorFolder : Handler.SourceType == ModelSourceType.Pbit ? SaveFormat.PowerBiTemplate : SaveFormat.ModelSchemaOnly, Handler.SerializeOptions, true);
                        Console.WriteLine("Model metadata saved.");
                    }
                    catch (Exception ex)
                    {
                        Error("Save failed: " + ex.Message);
                    }
                }
                throw new CommandLineException();
            }

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

            var switches = argList.Skip(doDeploy + 1).Where(arg => arg.StartsWith("-")).Select(arg => arg.ToUpper()).ToList();

            if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(databaseID))
            {
                Error("Invalid argument syntax.\n");
                OutputUsage();
                throw new CommandLineException();
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
                    throw new CommandLineException();

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
                xmla_script_file = argList.Skip(switchPos + 1).FirstOrDefault(); if (String.IsNullOrWhiteSpace(xmla_script_file) || xmla_script_file.StartsWith("-")) xmla_script_file = null;
                if (string.IsNullOrEmpty(xmla_script_file))
                {
                    Error("Missing xmla_script_file.\n");
                    OutputUsage();
                    throw new CommandLineException();
                }
                switches.Remove("-X");
                switches.Remove("-XMLA");
            }

            try
            {
                if (replaceMap.Count > 0)
                {
                    Console.WriteLine("Switching connection string placeholders...");
                    foreach (var map in replaceMap) Handler.Model.DataSources.SetPlaceholder(map.Key, map.Value);
                }

                var cs = string.IsNullOrEmpty(userName) ? TabularConnection.GetConnectionString(serverName) :
                    TabularConnection.GetConnectionString(serverName, userName, password);
                if (xmla_scripting_only)
                {
                    Console.WriteLine("Generating XMLA/TMSL script...");
                    var s = new TOM.Server();
                    s.Connect(cs);
                    var xmla = TabularDeployer.GetTMSL(Handler.Database, s, databaseID, options);
                    using (var sw = new StreamWriter(xmla_script_file))
                    {
                        sw.Write(xmla);
                    }
                    Console.WriteLine("XMLA/TMSL script generated.");
                }
                else
                {
                    Console.WriteLine("Deploying...");
                    Handler.Model.UpdateDeploymentMetadata(DeploymentModeMetadata.CLI);
                    var deploymentResult = TabularDeployer.Deploy(Handler, cs, databaseID, options);
                    Console.WriteLine("Deployment succeeded.");
                    foreach (var err in deploymentResult.Issues) if (errorOnDaxErr) Error(err); else Warning(err);
                    foreach (var err in deploymentResult.Warnings) Warning(err);
                    foreach (var err in deploymentResult.Unprocessed)
                        if (warnOnUnprocessed) Warning(err); else Console.WriteLine(err);
                }
            }
            catch (Exception ex)
            {
                Error($"{(xmla_scripting_only ? "Script generation" : "Deployment")} failed! {ex.Message}");
            }
        }

        void AnalyzeBestPracticeRules(string rulefile)
        {
            Console.WriteLine("Running Best Practice Analyzer...");
            Console.WriteLine("=================================");

            var analyzer = new BPA.Analyzer();
            analyzer.SetModel(Handler.Model, Handler.SourceType == ModelSourceType.Database ? null : FileSystemHelper.DirectoryFromPath(Handler.Source));

            BPA.BestPracticeCollection suppliedRules = null;
            if (!string.IsNullOrEmpty(rulefile))
            {
                if (File.Exists(rulefile))
                {
                    try
                    {
                        suppliedRules = BPA.BestPracticeCollection.GetCollectionFromFile(Environment.CurrentDirectory, rulefile);
                    }
                    catch
                    {
                        Error("Invalid rulefile: {0}", rulefile);
                        throw new CommandLineException();
                    }
                }
                else
                {
                    suppliedRules = BPA.BestPracticeCollection.GetCollectionFromUrl(rulefile);
                    if (suppliedRules.Count == 0)
                    {
                        Error("No rules defined in specified URL: {0}", rulefile);
                        throw new CommandLineException();
                    }
                }

            }

            IEnumerable<BPA.AnalyzerResult> bpaResults;
            if (suppliedRules == null) bpaResults = analyzer.AnalyzeAll();
            else
            {
                var effectiveRules = analyzer.GetEffectiveRules(false, false, true, true, suppliedRules);
                bpaResults = analyzer.Analyze(effectiveRules);
            }

            bool none = true;
            foreach (var res in bpaResults.Where(r => !r.Ignored))
            {
                if (res.InvalidCompatibilityLevel)
                {
                    Console.WriteLine("Skipping rule '{0}' as it does not apply to Compatibility Level {1}.", res.RuleName, Handler.CompatibilityLevel);
                }
                else
                if (res.RuleHasError)
                {
                    none = false;
                    Error("Error on rule '{0}': {1}", res.RuleName, res.RuleError);
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
                        if (res.Rule.Severity <= 1) Console.WriteLine(text);
                        else if (res.Rule.Severity == 2) Warning(text);
                        else if (res.Rule.Severity >= 3) Error(text);
                    }
                }

            }
            if (none) Console.WriteLine("No objects in violation of Best Practices.");
            Console.WriteLine("=================================");
        }

        void ExecuteScripts()
        {
            if (ScriptFiles.Count == 0)
            {
                Error("No scripts / script files provided");
                throw new CommandLineException();
            }

            foreach (var s in ScriptFiles)
            {
                if (File.Exists(s))
                {
                    Scripts.Add(File.ReadAllText(s));
                    Console.WriteLine("Loaded script: " + s);
                }
                else if (s.IndexOfAny(new[] { ';', ',', '"' }) != -1)
                {
                    Scripts.Add(s);
                }
                else
                {
                    Error("Script file not found: " + s);
                    throw new CommandLineException();
                }
            }

            for (int i = 0; i < Scripts.Count; i++)
            {
                var script = Scripts[i];
                Console.WriteLine("Executing script {0}...", i);

                System.CodeDom.Compiler.CompilerResults result;
                Scripting.ScriptOutputForm.Reset(false);
                var dyn = ScriptEngine.CompileScript(script, out result);
                //nUnit.StartSuite("Script Compilation");
                if (result.Errors.Count > 0)
                {
                    Error("Script compilation errors:");
                    var errIndex = 0;
                    foreach (System.CodeDom.Compiler.CompilerError err in result.Errors)
                    {
                        errIndex++;
                        ErrorX(err.ErrorText, ScriptFiles[i], err.Line, err.Column, err.ErrorNumber);
                        //nUnit.Failure("Script Compilation", $"Compilation Error #{errIndex}", err.ErrorText, $"{scriptFile} line {err.Line}, column {err.Column}");
                    }
                    throw new CommandLineException();
                }
                try
                {
                    Handler.BeginUpdate("script");
                    dyn.Invoke(Handler.Model, null);
                    Handler.EndUpdateAll();
                }
                catch (Exception ex)
                {
                    Error("Script execution error: " + ex.Message);
                    throw new CommandLineException();
                }
                finally
                {
                    Handler.Model.Database.CloseReader();
                }
            }
        }

        class CommandLineException : Exception
        {

        }
    }

}

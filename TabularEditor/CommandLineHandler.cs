using Microsoft.AnalysisServices;
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
using System.Threading;

namespace TabularEditor
{
    internal interface ICommandLineHandler
    {
        void Error(string errorMessage, params object[] args);
        void Warning(string errorMessage, params object[] args);
        bool CommandLineMode { get; }
        void HandleCommandLine(string[] args);
        LoggingMode LoggingMode { get; }
        int ErrorCount { get; }
        int WarningCount { get; }
        bool LaunchUi { get; }
    }

    enum LoggingMode
    {
        None,
        Vsts,
        GitHub
    }

    internal class CommandLineHandler: ICommandLineHandler
    {
        public LoggingMode LoggingMode { get; private set; }
        public int ErrorCount { get; private set; } = 0;
        public int WarningCount { get; private set; } = 0;
        public bool LaunchUi { get; private set; } = false;
        public bool CommandLineMode { get; private set; } = false;

        private void Log(string logMessage, params object[] args)
        {
            if (args.Length == 0) Console.WriteLine(logMessage);
            else Console.WriteLine(logMessage, args);
        }

        public void Error(string errorMessage, params object[] args)
        {
            if (LoggingMode == LoggingMode.Vsts) Log("##vso[task.logissue type=error;]" + errorMessage, args);
            else if (LoggingMode == LoggingMode.GitHub) Log("::error:: " + errorMessage, args);
            else Log(errorMessage, args);

            ErrorCount++;
        }
        public void Warning(string warningMessage, params object[] args)
        {
            if (LoggingMode == LoggingMode.Vsts) Log("##vso[task.logissue type=warning;]" + warningMessage, args);
            else if (LoggingMode == LoggingMode.GitHub) Log("::warning:: " + warningMessage, args);
            else Log(warningMessage, args);

            WarningCount++;
        }

        private void ErrorX(string errorMessage, string sourcePath, int line, int column, string code, params object[] args)
        {
            if (LoggingMode == LoggingMode.Vsts)
                Console.WriteLine(string.Format("##vso[task.logissue type=error;sourcepath={0};linenumber={1};columnnumber={2};code={3}]{4}", sourcePath, line, column, code, errorMessage), args);
            else if (LoggingMode == LoggingMode.GitHub)
                Console.WriteLine(string.Format("::error file={0},line={1},col={2}:: {3}", sourcePath, line, column, errorMessage), args);
            else
                Console.WriteLine(string.Format("Error {0} on line {1}, col {2}: {3}", code, line, column, errorMessage));

            ErrorCount++;
        }

        List<string> Scripts = new List<string>();
        List<string> ScriptFiles = new List<string>();
        TabularModelHandler Handler;

        public void HandleCommandLine(string[] args)
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

            var vstsLogging = upperArgList.IndexOf("-VSTS") > -1 || upperArgList.IndexOf("-V") > -1;
            var githubLogging = upperArgList.IndexOf("-GITHUB") > -1 || upperArgList.IndexOf("-G") > -1;
            if(vstsLogging && githubLogging)
            {
                Error("Invalid argument syntax (choose either -V/-VSTS or -G/-GITHUB, not both)");
                OutputUsage();
                return;
            }
            LoggingMode = vstsLogging ? LoggingMode.Vsts : githubLogging ? LoggingMode.GitHub : LoggingMode.None;

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

            var doDeploy = upperArgList.IndexOf("-DEPLOY");
            if (doDeploy == -1) doDeploy = upperArgList.IndexOf("-D");

            var doScript = upperArgList.IndexOf("-SCRIPT");
            if (doScript == -1) doScript = upperArgList.IndexOf("-S");
            if (doScript > doDeploy && doDeploy > -1) doScript = -1; // -S was used as a deployment option - not a script, so ignore it here
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
            string saveToTmdlOutputPath = null;
            string saveToTmdlReplaceId = null;

            var doSaveToFolder = upperArgList.IndexOf("-FOLDER");
            if (doSaveToFolder == -1) doSaveToFolder = upperArgList.IndexOf("-F");
            if (doSaveToFolder > doDeploy && doDeploy > -1) doSaveToFolder = -1; // -F was used as a deployment option - not a save option, so ignore it here
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

            var doSaveToTmdl = upperArgList.IndexOf("-TMDL");
            if (doSaveToTmdl > -1)
            {
                if (upperArgList.Count <= doSaveToTmdl)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return;
                }
                saveToTmdlOutputPath = argList[doSaveToTmdl + 1];
                if (doSaveToTmdl + 2 < argList.Count && !argList[doSaveToTmdl + 2].StartsWith("-")) saveToTmdlReplaceId = argList[doSaveToTmdl + 2];
                var directoryName = new FileInfo(saveToTmdlOutputPath).Directory.FullName;
                Directory.CreateDirectory(saveToTmdlOutputPath);
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

            if (doSaveToTmdl > -1 && doSave > -1)
            {
                Error("-TMDL and -BUILD arguments are mutually exclusive.\n");
                OutputUsage();
                return;
            }

            if (doSaveToTmdl > -1 && doSaveToFolder > -1)
            {
                Error("-TMDL and -FOLDER arguments are mutually exclusive.\n");
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
                Handler.Save(buildOutputPath, SaveFormat.ModelSchemaOnly, null, true);
            }
            else if (!string.IsNullOrEmpty(saveToFolderOutputPath))
            {
                Console.WriteLine("Saving Model.bim file to Folder Output Path ...");
                if (saveToFolderReplaceId != null) { Handler.Database.Name = saveToFolderReplaceId; Handler.Database.ID = saveToFolderReplaceId; }

                //Note the last parameter, we use whatever SerializeOptions are already in the file
                Handler.Save(saveToFolderOutputPath, SaveFormat.TabularEditorFolder, null, true);
            }
            else if (!string.IsNullOrEmpty(saveToTmdlOutputPath))
            {
                Console.WriteLine("Saving Model.bim file to TMDL ...");
                if (saveToTmdlReplaceId != null) { Handler.Database.Name = saveToFolderReplaceId; Handler.Database.ID = saveToFolderReplaceId; }

                //Note the last parameter, we use whatever SerializeOptions are already in the file
                Handler.Save(saveToTmdlOutputPath, SaveFormat.TMDL, null, true);
            }

            var doAnalyze = upperArgList.IndexOf("-ANALYZE");
            if (doAnalyze == -1) doAnalyze = upperArgList.IndexOf("-A");
            if (doAnalyze > -1)
            {
                var rulefile = doAnalyze + 1 < argList.Count ? argList[doAnalyze + 1] : "";
                if (rulefile.StartsWith("-") || string.IsNullOrEmpty(rulefile)) rulefile = null;

                AnalyzeBestPracticeRules(rulefile, true);
            }

            var doAnalyzeX = upperArgList.IndexOf("-ANALYZEX");
            if (doAnalyzeX == -1) doAnalyzeX = upperArgList.IndexOf("-AX");
            if (doAnalyzeX > -1)
            {
                var rulefile = doAnalyzeX + 1 < argList.Count ? argList[doAnalyzeX + 1] : "";
                if (rulefile.StartsWith("-") || string.IsNullOrEmpty(rulefile)) rulefile = null;

                AnalyzeBestPracticeRules(rulefile, false);
            }

            doDeploy = upperArgList.IndexOf("-DEPLOY");
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

        void LoadModelFromFile(string file)
        {
            // File argument provided (either alone or with switches), i.e.:
            //      TabularEditor.exe myfile.bim
            //      TabularEditor.exe myfile.bim -...

            if (!File.Exists(argList[1]) 
                && !File.Exists(Path.Combine(argList[1], "database.json")) 
                && !File.Exists(Path.Combine(argList[1], "model.tmd"))
                && !File.Exists(Path.Combine(argList[1], "model.tmdl")))
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
                var settings = new TabularModelHandlerSettings { AutoFixup = true, ChangeDetectionLocalServers = false, PBIFeaturesOnly = false };
                Handler = new TOMWrapper.TabularModelHandler(argList[1], settings);
            }
            catch (Exception e)
            {
                Error("Error loading file: " + e.Message);
                throw new CommandLineException();
            }
        }

        void LoadModelFromServer(string server, string database)
        {
            // Server + Database argument provided (either alone or with switches), i.e.:
            //      TabularEditor.exe localhost AdventureWorks
            //      TabularEditor.exe localhost AdventureWorks -...
            // If nothing else was specified on the command-line, open the UI:
            if (argList.Count == 3 && !argList[2].StartsWith("-"))
            {
                LaunchUi = true;
                throw new CommandLineException();
            }

            try
            {
                // Load model:
                var settings = new TabularModelHandlerSettings { AutoFixup = true, ChangeDetectionLocalServers = false, PBIFeaturesOnly = false };
                if (IsLocalSwitch(server))
                {
                    var localInstances = PowerBIHelper.Instances;
                    if (localInstances.Count == 1 && string.IsNullOrEmpty(database))
                    {
                        server = $"localhost:{localInstances[0].Port}";
                        database = "";
                    }
                    else if (localInstances.Count == 0)
                    {
                        Error("No local instances of Power BI Desktop detected.");
                        throw new CommandLineException();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(database))
                        {
                            Error("Multiple instances of Power BI Desktop detected. Please specify instance name. Available instances:");
                            foreach (var instance in localInstances) Console.WriteLine($"    localhost:{instance.Port}.{instance.Name}");
                            throw new CommandLineException();
                        }
                        var targetInstance = localInstances.FirstOrDefault(i => i.Name.EqualsI(database));
                        if (targetInstance == null)
                        {
                            Error($"No instance of Power BI Desktop with name {database} detected. Available instances:");
                            foreach (var instance in localInstances) Console.WriteLine($"    localhost:{instance.Port}.{instance.Name}");
                            throw new CommandLineException();
                        }
                        server = $"localhost:{targetInstance.Port}";
                        
                        database = "";
                    }
                }
                Console.WriteLine($"Loading model...");
                Handler = new TabularModelHandler(server, database, settings);
            }
            catch (Exception e)
            {
                if (!(e is CommandLineException))
                    Error("Error loading model: " + e.Message);
                throw new CommandLineException();
            }
        }

        private static bool IsLocalSwitch(string arg)
        {
            return arg.EqualsI("-L") || arg.EqualsI("-LOCAL");
        }
        private static bool IsLocalSwitch(IList<string> argList, int argIndex)
        {   
            return argList.Count > argIndex && IsLocalSwitch(argList[argIndex]);
        }
        private static bool IsSwitch(IList<string> argList, int argIndex)
        {
            return argList.Count > argIndex && argList[argIndex].StartsWith("-");
        }

        public enum ModelSource
        {
            None,
            File,
            ServerAndDatabase,
            SingleLocalInstance,
            NamedLocalInstance
        }

        static public ModelSource GetModelSourceFromCLI(IList<string> args, out bool additionalArgs)
        {
            additionalArgs = false;
            if (args.Count == 2)
            {
                if (IsLocalSwitch(args, 1)) return ModelSource.SingleLocalInstance;
                else return ModelSource.File;
            }
            else if (args.Count > 2)
            {
                additionalArgs = IsSwitch(args, 2) || IsSwitch(args, 3);
                if (IsLocalSwitch(args, 1))
                {
                    return IsSwitch(args, 2) ? ModelSource.SingleLocalInstance : ModelSource.NamedLocalInstance;
                }
                else if (IsSwitch(args, 1))
                {
                    return ModelSource.None;
                }
                else
                {
                    return IsSwitch(args, 2) ? ModelSource.File : ModelSource.ServerAndDatabase;
                }
            }
            else
                return ModelSource.None;
        }

        void LoadModel()
        {
            var modelSource = GetModelSourceFromCLI(argList, out bool additionalArgs);
            if(modelSource == ModelSource.None || !additionalArgs)
            {
                LaunchUi = true;
                throw new CommandLineException();
            }
            else if(modelSource == ModelSource.File)
            {
                LoadModelFromFile(argList[1]);
            }
            else
            {
                LoadModelFromServer(argList[1], IsSwitch(argList, 2) ? "" : argList[2]);
            }
        }

        static void OutputUsage()
        {
            Console.WriteLine(@"Usage:

TABULAREDITOR ( file | server database | -L [name] ) [-S script1 [script2] [...]]
    [-SC] [-A [rules] | -AX rules] [(-B | -F | -TMDL) output [id]] [-V | -G] [-T resultsfile]
    [-D [server database [-L user pass] [-F | -O [-C [plch1 value1 [plch2 value2 [...]]]]
        [-P [-Y]] [-S] [-R [-M]]]
        [-X xmla_script]] [-W] [-E]]

file                Full path of the Model.bim file or database.json model folder to load.
server              Server\instance name or connection string from which to load the model
database            Database ID of the model to load. If blank ("") picks the first available
                      database on the server.
-L / -LOCAL         Connects to a Power BI Desktop (local) instance of Analysis Services. If no
                      name is specified, this assumes that exactly 1 instance is running. Otherwise,
                      name should match the name of the .pbix file loaded in Power BI Desktop.
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
-AX / -ANALYZEX     Same as -A / -ANALYZE but excludes rules specified in the model annotations.
-B / -BIM / -BUILD  Saves the model (after optional script execution) as a Model.bim file.
  output              Full path of the Model.bim file to save to.
  id                  Optional id/name to assign to the Database object when saving.
-F / -FOLDER        Saves the model (after optional script execution) as a Folder structure.
  output              Full path of the folder to save to. Folder is created if it does not exist.
  id                  Optional id/name to assign to the Database object when saving.
-TMDL               Saves the model (after optional script execution) as a TMDL folder structure.
  output              Full path of the TMDL folder to save to. Folder is created if it does not exist.
  id                  Optional id/name to assign to the Database object when saving.
-V / -VSTS          Output Visual Studio Team Services logging commands.
-G / -GITHUB        Output GitHub Actions workflow commands.
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
  -F / -FULL          Deploy the full model metadata, allowing overwrite of an existing database.
  -O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
    -C / -CONNECTIONS   Deploy (overwrite) existing data sources in the model. After the -C switch, you
                        can (optionally) specify any number of placeholder-value pairs. Doing so, will
                        replace any occurrence of the specified placeholders (plch1, plch2, ...) in the
                        connection strings of every data source in the model, with the specified values
                        (value1, value2, ...).
    -P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
      -Y / -SKIPPOLICY    Do not overwrite partitions that have Incremental Refresh Policies defined.
    -S / -SHARED        Deploy (overwrite) shared expressions.
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
                var deploySwitches = new[] { "-L", "-LOGIN", "-O", "-OVERWRITE", "-F", "-FULL", "-C", "-CONNECTIONS", "-P", "-PARTITIONS", "-Y", "-SKIPPOLICY", "-S", "-SHARED", "-R", "-ROLES", "-M", "-MEMBERS", "-X", "-XMLA" };
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
                        var saveFormat = Handler.SourceType == ModelSourceType.Folder ? SaveFormat.TabularEditorFolder 
                            : Handler.SourceType == ModelSourceType.Pbit ? SaveFormat.PowerBiTemplate
                            : Handler.SourceType == ModelSourceType.TMDL ? SaveFormat.TMDL
                            : SaveFormat.ModelSchemaOnly;

                        Handler.Save(Handler.Source, saveFormat, Handler.SerializeOptions, true);
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

            var full = false;
            if (switches.Contains("-F") || switches.Contains("-FULL"))
            {
                full = true;
                options.DeployMode = DeploymentMode.CreateOrAlter;
                options.DeploySharedExpressions = true;
                options.DeployConnections = true;
                options.DeployPartitions = true;
                options.DeployRoles = true;
                options.DeployRoleMembers = true;
                options.SkipRefreshPolicyPartitions = false;
                switches.Remove("-F"); switches.Remove("-F");
            }
            if (switches.Contains("-O") || switches.Contains("-OVERWRITE"))
            {
                if (full)
                {
                    Error("-FULL and -OVERWRITE are mutually exclusive.\n");
                    OutputUsage();
                    throw new CommandLineException();
                }

                options.DeployMode = DeploymentMode.CreateOrAlter;
                switches.Remove("-O"); switches.Remove("-OVERWRITE");
            }
            else if(!full)
            {
                options.DeployMode = DeploymentMode.CreateDatabase;
            }
            if (switches.Contains("-P") || switches.Contains("-PARTITIONS"))
            {
                options.DeployPartitions = true;
                switches.Remove("-P"); switches.Remove("-PARTITIONS");

                if (switches.Contains("-Y") || switches.Contains("-SKIPPOLICY"))
                {
                    options.SkipRefreshPolicyPartitions = true;
                    switches.Remove("-Y"); switches.Remove("-SKIPPOLICY");
                }
            }
            if (switches.Contains("-C") || switches.Contains("-CONNECTIONS"))
            {
                options.DeployConnections = true;
                switches.Remove("-C"); switches.Remove("-CONNECTIONS");
            }

            if (switches.Contains("-S") || switches.Contains("-SHARED"))
            {
                options.DeploySharedExpressions = true;
                switches.Remove("-S"); switches.Remove("-SHARED");
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

                var cs = string.IsNullOrEmpty(userName) ? TabularConnection.GetConnectionString(serverName, Program.ApplicationName) :
                    TabularConnection.GetConnectionString(serverName, userName, password, Program.ApplicationName, ProtocolFormat.Default, InteractiveLogin.Default, IdentityMode.Default);
                if (xmla_scripting_only)
                {
                    Console.WriteLine("Generating XMLA/TMSL script...");
                    var s = new TOM.Server();
                    s.Connect(cs);
                    var xmla = Handler.TabularDeployer.GetTMSL(Handler.Database, s, databaseID, options);
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
                    var deploymentResult = Handler.TabularDeployer.Deploy(Handler.Database, cs, databaseID, options, CancellationToken.None);
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

        void AnalyzeBestPracticeRules(string rulefile, bool includeModelRules)
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
            if (suppliedRules == null)
            {
                // When no rule file is provided, we should also include local user/locale machine rules:
                var effectiveRules = analyzer.GetEffectiveRules(true, true, includeModelRules, includeModelRules);
                bpaResults = analyzer.Analyze(effectiveRules);
            }
            else
            {
                // When a rule file is provided, we ignore local user/locale machine rules:
                var effectiveRules = analyzer.GetEffectiveRules(false, false, includeModelRules, includeModelRules, suppliedRules);
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
            if (Policies.Instance.DisableCSharpScripts)
            {
                Error("Script execution disabled through policy.");
                throw new CommandLineException();
            }

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
    }

    class CommandLineException : Exception
    {

    }

}

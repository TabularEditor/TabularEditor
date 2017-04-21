using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TabularEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupLibraries();

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && HandleCommandLine(args))
            {
                if(enableVSTS)
                {
                    cw.WriteLine("##vso[task.complete result={0};]Done.", errorCount > 0 ? "Failed" :( issueCount > 0 ? "SucceededWithIssues" : "Succeeded" ));
                }
                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        static GUIConsoleWriter cw = new GUIConsoleWriter();

        /// <summary>
        /// Make sure that the TOMWrapper.dll is available in the current user's temp folder.
        /// Also, compiles current user's CustomActions.xml and loads them into the editor.
        /// </summary>
        static void SetupLibraries()
        {
            ScriptEngine.InitScriptEngine();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static bool enableVSTS;
        static int errorCount = 0;
        static int issueCount = 0;
        static void Error(string errorMessage, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine("##vso[task.logissue type=error;]" + errorMessage, args);
            }
            else
                cw.WriteLine(errorMessage, args);

            errorCount++;
        }
        static void Issue(string errorMessage, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine("##vso[task.logissue type=error;]" + errorMessage, args);
            }
            else
                cw.WriteLine(errorMessage, args);

            issueCount++;
        }
        static void Warning(string errorMessage, params object[] args)
        {
            if (enableVSTS)
            {
                cw.WriteLine("##vso[task.logissue type=warning;]" + errorMessage, args);
            }
            else
                cw.WriteLine(errorMessage, args);

            issueCount++;
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
            cw.WriteLine("");
            cw.WriteLine(Application.ProductName + " " + Application.ProductVersion);
            cw.WriteLine("--------------------------------");


            var upperArgList = args.Select(arg => arg.ToUpper()).ToList();
            var argList = args.Select(arg => arg).ToList();
            if (upperArgList.Contains("-?") || upperArgList.Contains("/?") || upperArgList.Contains("-H") || upperArgList.Contains("/H") || upperArgList.Contains("HELP"))
            {
                OutputUsage();
                return true;
            }

            enableVSTS = upperArgList.IndexOf("-VSTS") > -1 || upperArgList.IndexOf("-V") > -1;

            if (!File.Exists(args[1]) && !File.Exists(args[1] + "\\database.json"))
            {
                Error("File not found: {0}", args[1]);
                return true;
            } else
            {
                // If the file specified as 1st argument exists, and nothing else was specified on the command-line, open the UI:
                if (args.Length == 2) return false;
            }

            var fileName = args[1];

            string script = null;
            string scriptFile = null;

            var doScript = upperArgList.IndexOf("-SCRIPT");
            if (doScript == -1) doScript = upperArgList.IndexOf("-S");
            if (doScript > -1)
            {
                if(upperArgList.Count <= doScript)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                scriptFile = argList[doScript + 1];
                if(!File.Exists(scriptFile))
                {
                    Error("Specified script file not found.\n");
                    return true;
                }

                script = File.ReadAllText(scriptFile);
            }

            string savePath = null;

            var doSave = upperArgList.IndexOf("-BUILD");
            if (doSave == -1) doSave = upperArgList.IndexOf("-B");
            if (doSave > -1)
            {
                if(upperArgList.Count <= doSave)
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                savePath = argList[doSave + 1];
                var directoryName = new FileInfo(savePath).Directory.FullName;
                Directory.CreateDirectory(directoryName);
            }

            // Load model:
            cw.WriteLine("Loading model...");

            var h = new TOMWrapper.TabularModelHandler(fileName);
            h.Tree = new TOMWrapper.NullTree(h.Model);

            if (!string.IsNullOrEmpty(script))
            {
                cw.WriteLine("Executing script...");

                System.CodeDom.Compiler.CompilerResults result;
                var dyn = ScriptEngine.ScriptAction(script, out result);
                if (result.Errors.Count > 0)
                {
                    cw.WriteLine("Script compilation errors:");
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

            if(!string.IsNullOrEmpty(savePath))
            {
                cw.WriteLine("Saving file...");
                h.SaveFile(savePath);
            }

            var deploy = upperArgList.IndexOf("-DEPLOY");
            if (deploy == -1) deploy = upperArgList.IndexOf("-D");
            if(deploy > -1)
            {
                var serverName = argList.Skip(deploy + 1).FirstOrDefault(); if (serverName != null && serverName.StartsWith("-")) serverName = null;
                var databaseID = argList.Skip(deploy + 2).FirstOrDefault(); if (databaseID != null && databaseID.StartsWith("-")) databaseID = null;
                string userName = null;
                string password = null;
                var options = TOMWrapper.DeploymentOptions.StructureOnly;

                var switches = args.Skip(deploy + 1).Where(arg => arg.StartsWith("-")).Select(arg => arg.ToUpper()).ToList();

                if(string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(databaseID))
                {
                    Error("Invalid argument syntax.\n");
                    OutputUsage();
                    return true;
                }
                if(switches.Contains("-L") || switches.Contains("-LOGIN"))
                {
                    var switchPos = upperArgList.IndexOf("-LOGIN"); if (switchPos == -1) switchPos = upperArgList.IndexOf("-L");
                    userName = argList.Skip(switchPos + 1).FirstOrDefault(); if (userName != null && userName.StartsWith("-")) userName = null;
                    password = argList.Skip(switchPos + 2).FirstOrDefault(); if (password != null && password.StartsWith("-")) password = null;
                    if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    {
                        Error("Missing username or password.\n");
                        OutputUsage();
                        return true;
                    }
                    switches.Remove("-L"); switches.Remove("-LOGIN");
                }
                if (switches.Contains("-O") || switches.Contains("-OVERWRITE"))
                {
                    options.DeployMode = TOMWrapper.DeploymentMode.CreateOrAlter;
                    switches.Remove("-O"); switches.Remove("-OVERWRITE");
                } else
                {
                    options.DeployMode = TOMWrapper.DeploymentMode.CreateDatabase;
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
                /*if(switches.Count > 0)
                {
                    Error("Unknown switch {0}\n", switches[0]);
                    OutputUsage();
                    return true;
                }*/

                try
                {
                    cw.WriteLine("Deploying...");
                    var cs = string.IsNullOrEmpty(userName) ? TOMWrapper.TabularConnection.GetConnectionString(serverName) :
                        TOMWrapper.TabularConnection.GetConnectionString(serverName, userName, password);
                    var deploymentResult = TOMWrapper.TabularDeployer.Deploy(h.Database, cs, databaseID, options);
                    cw.WriteLine("Deployment succeeded.");
                    foreach (var err in deploymentResult.Issues) Issue(err);
                    foreach (var err in deploymentResult.Warnings) Warning(err);
                }
                catch (Exception ex)
                {
                    Error("Deployment failed! " + ex.Message);
                }
                return true;
            }

            return true;
        }

        static void OutputUsage()
        {
            cw.WriteLine(@"Usage:

TABULAREDITOR file [-S script] [-B output] [-D server database [-L username password] [-O [-C] [-P]] [-R [-M]]] [-V]

file                Full path of the Model.bim file or database.json model folder to load.
-S / -SCRIPT        Execute the specified script on the model after loading.
  script              Full path of a file containing a C# script to execute.
-B / -BUILD         Saves the model (after optional script execution) as a Model.bim file.
  output              Full path of the Model.bim file to save to.
-D / -DEPLOY        Command-line deployment
  server              Name of server to deploy to.
  database            ID of the database to deploy (create/overwrite).
-L / -LOGIN         Disables integrated security when connecting to the server. Specify:
  username            Username (must be a user with admin rights on the server)
  password            Password
-O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
-C / -CONNECTIONS   Deploy (overwrite) existing connections in the model.
-P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
-R / -ROLES         Deploy roles.
-M / -MEMBERS       Deploy role members.
-V / -VSTS          Output Visual Studio Team Services logging commands.");

        }
    }
}

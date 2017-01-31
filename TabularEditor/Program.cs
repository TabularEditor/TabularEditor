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
            if (args.Length > 1 && !File.Exists(args[1]) && !File.Exists(args[1] + "\\database.json"))
            {
                OutputUsage();
                Application.Exit();
                return;
            }
            if (args.Length > 2 && HandleCommandLine(args))
            {
                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

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

        public static void OutputUsage()
        {
            var cw = new GUIConsoleWriter();
            cw.WriteLine("");
            cw.WriteLine(Application.ProductName + " " + Application.ProductVersion);
            cw.WriteLine("--------------------------------");
            OutputUsage(cw);
        }

        static bool HandleCommandLine(string[] args)
        {
            var cw = new GUIConsoleWriter();
            cw.WriteLine("");
            cw.WriteLine(Application.ProductName + " " + Application.ProductVersion);
            cw.WriteLine("--------------------------------");

            if (!File.Exists(args[1]) && !File.Exists(args[1] + "\\database.json"))
            {
                cw.WriteLine("File not found: {0}", args[1]);
                return true;
            }
            var fileName = args[1];

            var upperArgList = args.Select(arg => arg.ToUpper()).ToList();
            var argList = args.Select(arg => arg).ToList();
            
            var deploy = upperArgList.IndexOf("-DEPLOY");
            if (deploy == -1) deploy = upperArgList.IndexOf("-D");
            if(deploy >= 0)
            {
                var serverName = argList.Skip(deploy + 1).FirstOrDefault(); if (serverName != null && serverName.StartsWith("-")) serverName = null;
                var databaseID = argList.Skip(deploy + 2).FirstOrDefault(); if (databaseID != null && databaseID.StartsWith("-")) databaseID = null;
                string userName = null;
                string password = null;
                var options = TOMWrapper.DeploymentOptions.StructureOnly;

                var switches = args.Skip(deploy + 1).Where(arg => arg.StartsWith("-")).Select(arg => arg.ToUpper()).ToList();

                if(string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(databaseID))
                {
                    cw.WriteLine("Invalid syntax.\n");
                    OutputUsage(cw);
                    return true;
                }
                if(switches.Contains("-L") || switches.Contains("-LOGIN"))
                {
                    var switchPos = upperArgList.IndexOf("-LOGIN"); if (switchPos == -1) switchPos = upperArgList.IndexOf("-L");
                    userName = argList.Skip(switchPos + 1).FirstOrDefault(); if (userName != null && userName.StartsWith("-")) userName = null;
                    password = argList.Skip(switchPos + 2).FirstOrDefault(); if (password != null && password.StartsWith("-")) password = null;
                    if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    {
                        cw.WriteLine("Missing username or password.\n");
                        OutputUsage(cw);
                        return true;
                    }
                    switches.Remove("-L"); switches.Remove("-LOGIN");
                }
                if (switches.Contains("-O") || switches.Contains("-OVERWRITE"))
                {
                    options.DeployMode = TOMWrapper.DeploymentMode.CreateOrAlter;
                    switches.Remove("-O"); switches.Remove("-OVERWRITE");
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
                if(switches.Count > 0)
                {
                    cw.WriteLine("Unknown switch {0}\n", switches[0]);
                    OutputUsage(cw);
                    return true;
                }

                try
                {
                    cw.WriteLine("Loading model...");
                    
                    var h = new TOMWrapper.TabularModelHandler(fileName);
                    cw.WriteLine("Deploying...");
                    var cs = string.IsNullOrEmpty(userName) ? TOMWrapper.TabularConnection.GetConnectionString(serverName) :
                        TOMWrapper.TabularConnection.GetConnectionString(serverName, userName, password);
                    TOMWrapper.TabularDeployer.Deploy(h.Database, cs, databaseID, options);
                    cw.WriteLine("Deployment succeeded.");
                }
                catch (Exception ex)
                {
                    cw.WriteLine("Deployment failed! Error:");
                    cw.WriteLine(ex.Message);
                }
                return true;
            }

            return false;
        }

        static void OutputUsage(GUIConsoleWriter cw)
        {
            cw.WriteLine(@"Usage:

TABULAREDITOR file [-DEPLOY server database [-L username password] [-O [-C] [-P]] [-R [-M]]]

file                Full path of the Model.bim file or database.json model folder to load.
-D / -DEPLOY        Command-line deployment
  server            Name of server to deploy to.
  database          ID of the database to deploy (create/overwrite).
-L / -LOGIN         Specifies the use of username/password to connect to the server.
-O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
-C / -CONNECTIONS   Deploy (overwrite) existing connections in the model.
-P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
-R / -ROLES         Deploy roles.
-M / -MEMBERS       Deploy role members.");
        }
    }
}

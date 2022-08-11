using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using TabularEditor.TOMWrapper;
using Microsoft.WindowsAPICodePack.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor
{
    internal static class Program
    {
        public static readonly string ApplicationName = "Tabular Editor " + typeof(Program).Assembly.GetName().Version + " " + Guid.NewGuid().ToString();

        public static bool CommandLineMode => CommandLine.CommandLineMode;
        public static ICommandLineHandler CommandLine { get; internal set; } = new CommandLineHandler();

        public static bool RunWithArgs(params string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("");
                Console.WriteLine($"{ Application.ProductName } { Application.ProductVersion } (build { UpdateService.CurrentBuild })");
                Console.WriteLine("--------------------------------");
            }

            var plugins = LoadPlugins();
            SetupLibraries(plugins);
            
            if (args.Length > 1)
            {
                CommandLine.HandleCommandLine(args);
                Environment.ExitCode = CommandLine.ErrorCount > 0 ? 1 : 0;

                if (CommandLine.LoggingMode == LoggingMode.Vsts)
                {
                    Console.WriteLine("##vso[task.complete result={0};]Done.", CommandLine.ErrorCount > 0 ? "Failed" : ((CommandLine.WarningCount > 0) ? "SucceededWithIssues" : "Succeeded"));
                }
                if(!CommandLine.LaunchUi) return false;
            }
            
            return true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConsoleHandler.RedirectToParent();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            System.Net.WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var args = Environment.GetCommandLineArgs();
            if (RunWithArgs(args))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            TabularModelHandler.Cleanup();
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

        /// <summary>
        /// Make sure that the TOMWrapper.dll is available in the current user's temp folder.
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
                                Console.WriteLine("Successfully loaded plugin " + pluginType.Name + " from assembly " + Path.GetFileName(dll));
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
        public static TestRun testRun;
    }
}

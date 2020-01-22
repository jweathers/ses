#pragma warning disable CA1303 //literal string passing
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Configuration;
namespace SES.Store.MSSQL.DBUp
{
    class Program
    {
        private static ProgramOptions options;       
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true)
                           .AddCommandLine(args, ProgramOptions.SwitchMappings)
                           .AddUserSecrets<Program>();
            var config = builder.Build();
            
            options = config.CreateProgramOptions();

            Execute(options);
        }
        static string rootNamespace = typeof(Program).Namespace;
        static string MakeDbScriptResourceName(string name) => $"{rootNamespace}.db.{name}";
        static void Execute(ProgramOptions options)
        {

            var upgrader = DeployChanges
                                    .To.SqlDatabase(options.ConnectionString)
                                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ns => ns.StartsWith(MakeDbScriptResourceName("pre"),StringComparison.InvariantCultureIgnoreCase), new SqlScriptOptions() { RunGroupOrder = 0, ScriptType = DbUp.Support.ScriptType.RunAlways })
                                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ns => ns.StartsWith(MakeDbScriptResourceName("deploy"),StringComparison.InvariantCultureIgnoreCase), new SqlScriptOptions() { RunGroupOrder = 10, ScriptType = DbUp.Support.ScriptType.RunOnce })
                                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ns => ns.StartsWith(MakeDbScriptResourceName("required"),StringComparison.InvariantCultureIgnoreCase), new SqlScriptOptions() { RunGroupOrder = 40, ScriptType = DbUp.Support.ScriptType.RunOnce })
                                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), ns => ns.StartsWith(MakeDbScriptResourceName("post"),StringComparison.InvariantCultureIgnoreCase), new SqlScriptOptions() { RunGroupOrder = 90, ScriptType = DbUp.Support.ScriptType.RunAlways })
                                    .LogToConsole()
                                    .WithTransaction();
            if(options.Verbose)
            {
                upgrader.LogScriptOutput();
            }
            var upgradeEngine = upgrader.Build();//.PerformUpgrade();
            if(options.OutputInsteadOfExecute)
            {
                try
                {
                    upgradeEngine.ExportScriptsToExecute(options);
                }
                catch(Exception e)
                {
                    ExitWithError(e);
                }
            }
            else
            {
                var upgradeResult = upgradeEngine.PerformUpgrade();
                if(!upgradeResult.Successful)
                {
                    ExitWithError(upgradeResult.Error);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            Environment.Exit(0);
        } 
        static void ExitWithError(Exception e)       
        {
                ExitWithError(e.ToString());
        }
        static void ExitWithError(string error)       
        {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"<DBUp - Error> - {error}");
                Console.ResetColor();
                Environment.Exit(-1);
        }        
    }
}

#pragma warning restore CA1303 //literal string passing
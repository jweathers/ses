#pragma warning disable CA1303 //literal string passing
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SES.Store.MSSQL.DBUp
{
    internal class ProgramOptions
    {
        public string ConnectionString { get; set; }
        public bool Verbose{get;set;}
        public string Output{get;set;}
        public bool OutputInsteadOfExecute=>!string.IsNullOrWhiteSpace(Output);
        internal string FullOutputPath{get;set;}
        internal void Validate(){
            
        }
        internal const string ConnectionStringKey="ConnectionStrings:SES.Store";
        internal const string DbUpConfigKeyRoot = "SES.Store.MSSQL.DBUp:";
        internal const string VerbosityKey=DbUpConfigKeyRoot + "Verbose";
        internal const string OutputKey=DbUpConfigKeyRoot + "Output";
        internal const string ConnectionStringCommandLineSwitch = "--connectionstring";
        internal const string VerbosityCommandLineSwitch="--verbose";
        internal const string OutputCommandLineSwitch="--output";
        internal static readonly Dictionary<string,string> SwitchMappings=new Dictionary<string, string>
        {
            {ConnectionStringCommandLineSwitch, ConnectionStringKey },
            {VerbosityCommandLineSwitch, VerbosityKey },
            {OutputCommandLineSwitch,OutputKey}
        }; 
    }

    internal static class ConfigurationProgramOptionsFactory
    {
        public static ProgramOptions CreateProgramOptions(this IConfigurationRoot config)
        {
            var options = new ProgramOptions();
            options.ConnectionString = config.GetValue<string>(ProgramOptions.ConnectionStringKey);
            options.Output=config.GetValue<string>(ProgramOptions.OutputKey);
            options.Verbose=config.GetValue<bool>(ProgramOptions.VerbosityKey);
            if(string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new Exception($"The connection string must be set in {ProgramOptions.ConnectionStringKey} in app settings, via the command line switch {ProgramOptions.ConnectionStringCommandLineSwitch} <connection_string_here>, or setting user-secrets for {ProgramOptions.ConnectionStringKey}. Settings get evaluated in the order 1) appsettings.json 2) command line args 3) user-secrets such that user-secrets overrides command line args which overrides appsettings.json");
            }
            if(!string.IsNullOrWhiteSpace(options.Output))
            {
                var fullpath = System.IO.Path.GetFullPath(options.Output);
                if(!string.IsNullOrWhiteSpace(Path.GetFileName(fullpath)) && (Path.GetFileName(fullpath)!=Path.GetFileNameWithoutExtension(fullpath)))
                {
                    throw new Exception($"The path provided for output [{options.Output}] is a file and not a directory. A directory is required.");
                }
                options.FullOutputPath=fullpath;
            }
            return options;
        }
    }
}



#pragma warning restore CA1303 //literal string passing
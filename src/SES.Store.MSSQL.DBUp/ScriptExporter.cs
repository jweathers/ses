using System;
using System.Collections.Generic;
using System.IO;
using DbUp.Engine;
using Microsoft.Extensions.Configuration;

namespace SES.Store.MSSQL.DBUp
{
    internal static class ScriptExporter
    {
        public static void ExportScriptsToExecute(this UpgradeEngine dbUpEngine, ProgramOptions options)
        {
            if (dbUpEngine is null)
            {
                throw new ArgumentNullException(nameof(dbUpEngine));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            var scriptsToExecute = dbUpEngine.GetScriptsToExecute();
            foreach(var script in scriptsToExecute)
            {
                var pathOut = Path.Combine(options.FullOutputPath,script.SqlScriptOptions.ScriptType.ToString());

                if(!Directory.Exists(pathOut))
                {
                    Directory.CreateDirectory(pathOut);
                }

                using(var f = File.Open(Path.Combine(pathOut,$"{script.SqlScriptOptions.RunGroupOrder}-{script.Name}"),FileMode.Create))
                using(var sw = new StreamWriter(f))
                {
                    sw.Write(script.Contents);
                    sw.Flush();
                    sw.Close();
                }
            }
        }        
    }
}
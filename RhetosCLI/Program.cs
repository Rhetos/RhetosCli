using NLog;
using RhetosCLI.Attributes;
using RhetosCLI.Commands;
using RhetosCLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RhetosCLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Logging.Logger = LogManager.GetCurrentClassLogger();
                Logging.LogInfo("Rhetos CLI version {0}", Assembly.GetExecutingAssembly().GetName().Version);
                CreateCacheFolder();
                //TODO implement help and command discovery
                //var allCommands = LoadAllCommands();
                var command = ParseCommandLine(args);

                if (string.IsNullOrEmpty(command.Command))
                {
                    //ShowHelp
                    //Generate help from all commands
                    Console.WriteLine ("Showing Help");
                }
                else
                {
                    //Executecommand
                    var cmd = new Rhetos();
                    MiscHelpers.SetParams(cmd, command);
                    cmd.Execute(command);
                }
            }
            catch (Exception ex)
            {
                Logging.LogFatal(ex, "" );
            }
            finally
            {
                MiscHelpers.WriteLine("Press key to exit   ");
                Console.ReadKey();
            }
        }

        private static void CreateCacheFolder()
        {
            var cacheDir = MiscHelpers.GetCachePath();
            Logging.LogInfo("Checking for cache dir at {0}", cacheDir);
            Directory.CreateDirectory(cacheDir);
        }

        private static CliCommand ParseCommandLine(string[] args)
        {
            var cmdParams = new CliCommand();

            if (args.Length>0)
            {
                cmdParams.Command = args[0];
                foreach (var arg in args.Skip(1))
                {
                    var value = arg.Split('=');
                    cmdParams.Parameters.Add(value[0].ToUpper(), value[1]);
                }
            }
            return cmdParams;
        }

        private static List<Type> LoadAllCommands()
        {
            return  Assembly.GetExecutingAssembly().GetExportedTypes().Where(t=>t.GetCustomAttribute<CliCommandAttribute>() != null).ToList();
        }
    }
}
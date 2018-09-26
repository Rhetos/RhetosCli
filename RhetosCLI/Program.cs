using NLog;
using RhetosCLI.Helpers;
using System;
using System.IO;
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
                var commands = MiscHelpers.LoadAllCommands();
                var command = MiscHelpers.ParseCommandLine(args);

                if (string.Equals(command.Module, "help", StringComparison.OrdinalIgnoreCase))
                {
                    Logging.LogInfo("Usage RhetosCli [Module] [Command] [Parameters]");
                    Logging.LogInfo("For help use RhetosCli help or RhetosCli [module] help");
                }
                else
                {
                    if (MiscHelpers.ResolveCommand(commands, command))
                    {
                        var cmd = Activator.CreateInstance(command.Type);
                        MiscHelpers.SetParams(cmd, command);
                        var methodInfo = cmd.GetType().GetMethod(command.Method);
                        methodInfo.Invoke(cmd, null);
                    }
                    else
                    {
                        Logging.LogFatal("Invalid command specified. Aborting.");
                        Logging.LogInfo("Usage RhetosCli [Module] [Command] [Parameters]");
                        Logging.LogInfo("For help use RhetosCli help or RhetosCli [module] help");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogFatal(ex, "");
            }
            finally
            {
                MiscHelpers.WriteLine("Press key to exit   ");
                if (MiscHelpers.IsInteractive())
                {
                    Console.ReadKey();
                }
            }
        }

        private static void CreateCacheFolder()
        {
            var cacheDir = MiscHelpers.GetCachePath();
            Logging.LogTrace("Checking for cache dir at {0}", cacheDir);
            Directory.CreateDirectory(cacheDir);
        }
    }
}
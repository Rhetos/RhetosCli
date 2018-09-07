using RhetosCLI.Attributes;
using RhetosCLI.Helpers;
using System;
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
                //TODO implement help and command discovery
                MiscHelpers.WriteLine("Rhetos CLI version {0}", Assembly.GetExecutingAssembly().GetName().Version);
                CreateCacheFolder();
                LoadAllCommands();
                var cmdParams = ParseCommandLine(args);

                //CreateApp.Create("v2.7.0", "vlado_Test", "Vlado_test", @"<Your username>", "<Your password>", false);
            }
            catch (Exception ex)
            {
                MiscHelpers.WriteLine("ERROR: {0}", ConsoleColor.Red, ex.ToString());
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
            MiscHelpers.WriteLine("Checking for cache dir at {0}", cacheDir);
            Directory.CreateDirectory(cacheDir);
        }

        private static CliCommandParams ParseCommandLine(string[] args)
        {
            var cmdParams = new CliCommandParams
            {
                // first param is command  and value for it is empty
                Command = args[0]
            };
            foreach (var arg in args.ToList().Skip(1))
            {
                var value = arg.Split('=');
                cmdParams.Parameters.Add(value[0], value[1]);
            }
            return cmdParams;
        }

        private static void LoadAllCommands()
        {

            var types = Assembly.GetExecutingAssembly().GetExportedTypes();

            foreach (var type in types)
            {
                var isCommand = type.GetMethods().Where(m => m.GetCustomAttribute<CliCommandAttribute>() != null);
            }
         
        }
    }
}
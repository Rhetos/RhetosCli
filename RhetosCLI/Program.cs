using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RhetosCLI.Commands;
using System.Configuration;

namespace RhetosCLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                //TODO implement help and command discovery
                Helpers.WriteLine("Rhetos CLI version {0}", Assembly.GetExecutingAssembly().GetName().Version);
                CreateCacheFolder();
                CreateApp.Create("v2.7.0", "vlado_Test", "Vlado_test", @"<Your username>", "<Your password>", false);
            }
            catch (Exception ex)
            {
                Helpers.WriteLine("ERROR: {0}", ConsoleColor.Red, ex.ToString());
            }
            finally
            {
                Helpers.WriteLine("Press key to exit   ");
                Console.ReadKey();
            }
        }

        private static void CreateCacheFolder()
        {
            var cacheDir = Helpers.GetCachePath();
            Helpers.WriteLine("Checking for cache dir at {0}", cacheDir);
            Directory.CreateDirectory(cacheDir);
        }
    }
}
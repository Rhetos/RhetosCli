using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace RhetosCLI.Helpers
{
    public static class MiscHelpers
    {
        public static string DownloadRelease(string url)
        {
            //Download relase save to disk and return filename
            var client = new DownloadClient(url);
            var file = client.DownloadFile();
            return file;
        }

        public static void WriteLine(string message, ConsoleColor color, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(message, args);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteLine(string message, params object[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine(message, args);
            }
        }

        public static void WriteLine(string message)
        {
            if (Environment.UserInteractive && Debugger.IsAttached)
            {
                Console.WriteLine(message);
            }
        }

        public static string GetCachePath()
        {
            return Path.Combine(Path.GetTempPath(), "RhetosCLI\\Cache");
        }

        public static void UnzipFile(string source, string destination)
        {
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            ZipFile.ExtractToDirectory(source, destination);
        }

        public static void SetParams(object target,CliCommand command)
        {
            var props = target.GetType().GetProperties();
            foreach (var prop in props)
            {
                var propName= prop.Name.ToUpper();
                var propInfo = target.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var paramValue = command.Parameters.ContainsKey(propName) ? command.Parameters[propName] : null;
                propInfo.SetValue(target,Convert.ChangeType(paramValue, propInfo.PropertyType),  null);
            }
        }

        public static bool StartExternalExe(string target)
        {
            Logging.LogWarn("Staring external exe {0}", target);
            Logging.LogWarn("--------External exe output START-----");
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = target;
            pProcess.StartInfo.Arguments = "/NoPause";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.Start();
            pProcess.WaitForExit();
            string output = pProcess.StandardOutput.ReadToEnd();
            if (!String.IsNullOrEmpty(output))
            {
                Logging.LogInfo(output);
            }
            string err = pProcess.StandardError.ReadToEnd();
            if (!String.IsNullOrEmpty(err))
            {
                Logging.LogError(err);
            }
            Logging.LogWarn("--------External exe output END-----");
            Logging.LogWarn("External exe {0} finished", target);
            return pProcess.ExitCode == 0;
        }
    }
}
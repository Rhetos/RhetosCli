using RhetosCLI.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            if (IsInteractive())
            {
                Console.WriteLine(message, args);
            }
        }

        public static void WriteLine(string message)
        {
            if (IsInteractive())
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

        public static void SetParams(object target, CliCommand command)
        {
            var props = target.GetType().GetProperties();
            foreach (var prop in props)
            {
                var propName = prop.Name.ToLower();
                var propInfo = target.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var paramValue = command.Parameters.ContainsKey(propName) ? command.Parameters[propName] : GetDefault(propInfo.PropertyType);
                propInfo.SetValue(target, Convert.ChangeType(paramValue, propInfo.PropertyType), null);
            }
        }

        public static bool ResolveCommand(List<CliCommand> commands, CliCommand command)
        {
            var retValue = true;

            if (!commands.Any(c => c.Module == command.Module))
            {
                Logging.LogFatal("Module {0} doesn't exist", command.Module);
                retValue = false;
            }
            else
            {
                if (!commands.Any(c => c.Module == command.Module && c.Command == command.Command))
                {
                    Logging.LogFatal("Module {0} has no command {1}", command.Module, command.Command);
                    retValue = false;
                }
            }
            if (retValue)
            {
                var resolvedCommand = commands.Single(c => c.Module == command.Module && c.Command == command.Command);
                command.Type = resolvedCommand.Type;
                command.Method = resolvedCommand.Method;
            }
            return retValue;
        }

        public static List<CliCommand> LoadAllCommands()
        {
            Logging.LogTrace("Loading commands");
            var types = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => t.GetCustomAttribute<ClicommandModuleAttribute>() != null).ToList();
            var cliCommands = new List<CliCommand>();
            foreach (var type in types)
            {
                var methods = type.GetMethods().Where(m => m.GetCustomAttribute<CliCommandAttribute>() != null).ToList();
                foreach (var method in methods)
                {
                    var cmd = new CliCommand
                    {
                        Type = type,
                        Module = type.GetCustomAttribute<ClicommandModuleAttribute>().ModuleName.ToLower(),
                        Command = method.GetCustomAttribute<CliCommandAttribute>().CommandName.ToLower(),
                        Method = method.Name
                    };
                    cliCommands.Add(cmd);
                }
            }
            Logging.LogTrace("{0} commands loaded in {1} module(s)", cliCommands.Count, cliCommands.Select(c => c.Module).Distinct().Count());
            return cliCommands;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static bool IsInteractive()
        {
            return Environment.UserInteractive;
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

        public static CliCommand ParseCommandLine(string[] args)
        {
            var cmdParams = new CliCommand();

            if (args.Length > 0)
            {
                cmdParams.Module = args[0].ToLower();
                cmdParams.Command = args[1].ToLower();
                foreach (var arg in args.Skip(2))
                {
                    var value = arg.Split('=');
                    cmdParams.Parameters.Add(value[0].ToLower().Trim(), value[1].Trim());
                }
            }
            return cmdParams;
        }
    }
}
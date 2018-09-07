using System;
using System.IO;
using System.IO.Compression;

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
            Console.WriteLine(message, args);
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
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
    }
}
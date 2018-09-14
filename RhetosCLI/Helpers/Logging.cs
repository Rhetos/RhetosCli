using System;
using NLog;

namespace RhetosCLI.Helpers
{
    public static class Logging
    {

        public static ILogger Logger { get; set; }

        public static void LogFatal(Exception ex, string message)
        {
            Logger.Log(LogLevel.Fatal, ex, message);
        }

        public static void LogError(string message, params object[] args)
        {
            Logger.Log(LogLevel.Error, string.Format(message, args));
        }

        public static void LogError(Exception ex,string message, params object[] args)
        {
            Logger.Log(LogLevel.Error, ex, string.Format(message, args));
        }

        public static void LogWarn(string message, params object[] args)
        {
            Logger.Log(LogLevel.Warn, string.Format(message, args));
        }
        public static void LogInfo(string message, params object[] args)
        {
            Logger.Log(LogLevel.Info, string.Format(message, args));
        }
        public static void LogDebug(Exception ex, string message)
        {
            Logger.Log(LogLevel.Debug, message);
        }
        public static void LogTrace(Exception ex, string message)
        {
            Logger.Log(LogLevel.Trace, message);
        }
    }
    
}

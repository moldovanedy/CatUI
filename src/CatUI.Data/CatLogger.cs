using System;
using System.Diagnostics;

namespace CatUI.Data
{
    /// <summary>
    /// A basic logger that writes messages to <see cref="Debug"/> when the DEBUG constant is set
    /// and to <see cref="Trace"/> when the TRACE constant is set and DEBUG is not. You can control more logger settings
    /// inside <see cref="CatApplication"/>.
    /// </summary>
    public static class CatLogger
    {
        public static void Log(object message, LogLevel logLevel = LogLevel.Info, bool showTimestamp = true)
        {
#if DEBUG
            if (CatApplication.Instance.DebugLogLevel == LogLevel.None ||
                logLevel < CatApplication.Instance.DebugLogLevel)
            {
                return;
            }

            Debug.WriteLine(FormatMessage(message, logLevel, showTimestamp));
#elif TRACE
            if (CatApplication.Instance.ReleaseLogLevel == LogLevel.None || 
                logLevel < CatApplication.Instance.ReleaseLogLevel)
            {
                return;
            }
            
            Trace.WriteLine(FormatMessage(message, logLevel, showTimestamp));
#endif
        }

        public static void LogDebug(object message, bool showTimestamp = true)
        {
            Log(message, LogLevel.Debug, showTimestamp);
        }

        public static void LogInfo(object message, bool showTimestamp = true)
        {
            Log(message, LogLevel.Info, showTimestamp);
        }

        public static void LogWarning(object message, bool showTimestamp = true)
        {
            Log(message, LogLevel.Warning, showTimestamp);
        }

        public static void LogError(object message, bool showTimestamp = true)
        {
            Log(message, LogLevel.Error, showTimestamp);
        }

        private static string FormatMessage(object message, LogLevel logLevel, bool showTimestamp = true)
        {
            //this is done simply for performance reasons (i.e. not calling ToUpper every time)
            string logLevelString;
            switch (logLevel)
            {
                case LogLevel.Verbose:
                    logLevelString = "VERBOSE";
                    break;
                case LogLevel.Debug:
                    logLevelString = "DEBUG";
                    break;
                case LogLevel.Info:
                    logLevelString = "INFO";
                    break;
                case LogLevel.Warning:
                    logLevelString = "WARNING";
                    break;
                case LogLevel.Error:
                    logLevelString = "ERROR";
                    break;
                case LogLevel.Critical:
                    logLevelString = "CRITICAL";
                    break;
                case LogLevel.None:
                default:
                    logLevelString = logLevel.ToString().ToUpperInvariant();
                    break;
            }

            return showTimestamp
                ? $"[{logLevelString} ({DateTime.Now:HH:mm:ss.fff})]: {message}"
                : $"[{logLevelString}]: {message}";
        }

        /// <summary>
        /// The type of log message specific to the application. Logs are written by internal CatUI components.
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// No logging, only used when configuring the logging level.
            /// </summary>
            None = 0,

            /// <summary>
            /// Contains lots of details that are generally only used for debugging;
            /// very rarely enabled in a production app.
            /// </summary>
            Verbose = 1,

            /// <summary>
            /// Used when debugging or for internal events.
            /// </summary>
            Debug = 2,

            /// <summary>
            /// Generally used for suggestions when debugging. Generally disabled in production.
            /// </summary>
            Info = 3,

            /// <summary>
            /// Something is not configured properly. Generally enabled in production.
            /// </summary>
            Warning = 4,

            /// <summary>
            /// Something went wrong and these messages might help you to diagnose the problem.
            /// It is strongly encouraged to enable this level (or a lower one) in production.
            /// </summary>
            Error = 5,

            /// <summary>
            /// Something happened that makes the application unusable and cannot continue normal operation,
            /// generally something wrong on the client machine. Use this to present an error message, gather data and
            /// quit the application. This is very rarely (if at all) triggered.
            /// It is strongly encouraged to enable this level (or a lower one) in production.
            /// </summary>
            Critical = 6
        }
    }
}

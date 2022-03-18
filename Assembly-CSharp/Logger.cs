using System;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable file UnusedMember.Global

namespace Modding
{
    /// <summary>
    ///     Shared logger for mods to use.
    /// </summary>
    [PublicAPI]
    // This is threadsafe, but it's blocking.  Hopefully mods don't try to log so much that it becomes an issue.  If it does we'll have to look at a better system.
    public static class Logger
    {
        private static readonly object Locker = new();
        private static readonly StreamWriter Writer;

        private static LogLevel _logLevel;

        internal static readonly SimpleLogger APILogger = new("API");

        /// <summary>
        ///     Logger Constructor.  Initializes file to write to.
        /// </summary>
        static Logger()
        {
            Debug.Log("Creating Mod Logger");
            _logLevel = LogLevel.Debug;

            string oldLogDir = Path.Combine(Application.persistentDataPath, "Old ModLogs");
            if (!Directory.Exists(oldLogDir))
            {
                Directory.CreateDirectory(oldLogDir);
            }

            string currLogName = Path.Combine(Application.persistentDataPath, "ModLog.txt");
            if (File.Exists(currLogName))
            {
                string oldLogName = "ModLog " + File.GetCreationTimeUtc(currLogName)
                                        .ToString("MM dd yyyy (HH mm ss)", CultureInfo.InvariantCulture) + ".txt";
                File.Move(currLogName, Path.Combine(oldLogDir, oldLogName));
            }

            var fileStream = new FileStream(currLogName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            Writer = new StreamWriter(fileStream, Encoding.UTF8) {AutoFlush = true};

            File.SetCreationTimeUtc(currLogName, DateTime.UtcNow);
        }

        internal static void ClearOldModlogs()
        {
            string oldLogDir = Path.Combine(Application.persistentDataPath, "Old ModLogs");
            APILogger.Log($"Deleting modlogs older than {ModHooks.GlobalSettings.ModlogMaxAge} days ago");
            foreach (string fileName in Directory.GetFiles(oldLogDir))
            {
                if (File.GetCreationTimeUtc(fileName) < DateTime.UtcNow.AddDays(-ModHooks.GlobalSettings.ModlogMaxAge))
                {
                    File.Delete(fileName);
                }
            }
        }

        internal static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        /// <summary>
        ///     Checks to ensure that the logger level is currently high enough for this message, if it is, write it.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Level of Log</param>
        public static void Log(string message, LogLevel level)
        {
            if (_logLevel > level) 
                return;
            
            string levelText = $"[{level.ToString().ToUpper()}]:";
            
            WriteToFile(levelText + message.Replace("\n", "\n" + levelText) + Environment.NewLine, level);
        }

        /// <summary>
        ///     Checks to ensure that the logger level is currently high enough for this message, if it is, write it.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Level of Log</param>
        public static void Log(object message, LogLevel level)
        {
            Log(message.ToString(), level);
        }


        /// <summary>
        ///     Finest/Lowest level of logging.  Usually reserved for developmental testing.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogFine(string message)
        {
            Log(message, LogLevel.Fine);
        }

        /// <summary>
        ///     Finest/Lowest level of logging.  Usually reserved for developmental testing.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogFine(object message)
        {
            Log(message.ToString(), LogLevel.Fine);
        }

        /// <summary>
        ///     Log at the debug level.  Usually reserved for diagnostics.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogDebug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        /// <summary>
        ///     Log at the debug level.  Usually reserved for diagnostics.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogDebug(object message)
        {
            Log(message, LogLevel.Debug);
        }

        /// <summary>
        ///     Log at the info level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        ///     Log at the info level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(object message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        ///     Log at the warning level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogWarn(string message)
        {
            Log(message, LogLevel.Warn);
        }

        /// <summary>
        ///     Log at the warning level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogWarn(object message)
        {
            Log(message, LogLevel.Warn);
        }

        /// <summary>
        ///     Log at the error level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        /// <summary>
        ///     Log at the error level.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogError(object message)
        {
            Log(message, LogLevel.Error);
        }

        /// <summary>
        ///     Locks file to write, writes to file, releases lock.
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="level">Level of Log</param>
        private static void WriteToFile(string text, LogLevel level)
        {
            lock (Locker)
            {
                if (ModHooks.IsInitialized)
                {
                    ModHooks.LogConsole(text, level);
                }

                Writer.Write(text);
            }
        }
    }
}

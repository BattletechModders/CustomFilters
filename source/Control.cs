using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BattleTech;
using HBS.Logging;
using HBS.Util;


namespace CustomFilters
{


    public static class Control
    {
        public static CustomFiltersSettings Settings = new CustomFiltersSettings();

        private static ILog Logger;
        private static FileLogAppender logAppender;


        public static void Init(string directory, string settingsJSON)
        {
            Logger = HBS.Logging.Logger.GetLogger("CustomFilters", LogLevel.Debug);

            try
            {
                try
                {
                    Settings = new CustomFiltersSettings();
                    JSONSerializationUtility.FromJSON(Settings, settingsJSON);
                    HBS.Logging.Logger.SetLoggerLevel(Logger.Name, Settings.LogLevel);
                }
                catch (Exception)
                {
                    Settings = new CustomFiltersSettings();
                }

                Settings.Complete();
                SetupLogging(directory);

                var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Logger.Log("Loaded CustomFilters v0.1 for bt 1.6.2");

                CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());

                Logger.LogDebug("done");
                Logger.LogDebug(JSONSerializationUtility.ToJSON(Settings));
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        #region LOGGING
        [Conditional("CCDEBUG")]
        public static void LogDebug(string message)
        {
            Logger.LogDebug(message);
        }
        [Conditional("CCDEBUG")]
        public static void LogDebug(string message, Exception e)
        {
            Logger.LogDebug(message, e);
        }

        public static void LogError(string message)
        {
            Logger.LogError(message);
        }
        public static void LogError(string message, Exception e)
        {
            Logger.LogError(message, e);
        }
        public static void LogError(Exception e)
        {
            Logger.LogError(e);
        }

        public static void Log(string message)
        {
            Logger.Log(message);
        }



        internal static void SetupLogging(string Directory)
        {
            var logFilePath = Path.Combine(Directory, "log.txt");

            try
            {
                ShutdownLogging();
                AddLogFileForLogger(logFilePath);
            }
            catch (Exception e)
            {
                Logger.Log("CustomSalvage: can't create log file", e);
            }
        }

        internal static void ShutdownLogging()
        {
            if (logAppender == null)
            {
                return;
            }

            try
            {
                HBS.Logging.Logger.ClearAppender("CustomFilters");
                logAppender.Flush();
                logAppender.Close();
            }
            catch
            {
            }

            logAppender = null;
        }

        private static void AddLogFileForLogger(string logFilePath)
        {
            try
            {
                logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);
                HBS.Logging.Logger.AddAppender("CustomFilters", logAppender);

            }
            catch (Exception e)
            {
                Logger.Log("CustomFilters: can't create log file", e);
            }
        }

        #endregion

    }
}

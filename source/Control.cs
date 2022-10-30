using Harmony;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using BattletechPerformanceFix.MechLabFix;
using HBS.Logging;
using HBS.Util;

namespace CustomFilters;

internal static class Control
{
    public static CustomFiltersSettings Settings;
    private static ILog Logger;

    public static void Init(string directory, string settingsJSON)
    {
        try
        {
            Settings = new CustomFiltersSettings();
            JSONSerializationUtility.FromJSON(Settings, settingsJSON);
        }
        catch (Exception)
        {
            Settings = new CustomFiltersSettings();
        }

        Logger = HBS.Logging.Logger.GetLogger("CustomFilters", Settings.LogLevel);

        try
        {
            var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());

            MechLabFixPublic.FilterFunc = list =>
            {
                LogDebug("FilterUsingHBSCode");
                return list.Where(i => UIHandler.ApplyFilter(i.componentDef)).ToList();
            };

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
    #endregion
}
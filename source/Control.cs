using Harmony;
using System;
using System.Reflection;
using System.Linq;
using BattletechPerformanceFix.MechLabFix;
using CustomFilters.MechLabInventory;
using HBS.Util;

namespace CustomFilters;

internal static class Control
{
    public static CustomFiltersSettings Settings;

    public static void Init(string directory, string settingsJson)
    {
        try
        {
            Settings = new();
            JSONSerializationUtility.FromJSON(Settings, settingsJson);
        }
        catch (Exception)
        {
            Settings = new();
        }

        Logging.Init(Settings.LogLevel);
        Logging.LogDebug("Starting CustomerFilters");

        try
        {
            var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());

            MechLabFixPublic.FilterFunc = list =>
            {
                Logging.LogDebug("FilterUsingHBSCode");
                return list.Where(i => UIHandler.ApplyFilter(i.componentDef)).ToList();
            };

            Logging.LogDebug("done");
            if (Settings.DumpSettings)
            {
                Logging.LogDebug(JSONSerializationUtility.ToJSON(Settings));
            }
        }
        catch (Exception e)
        {
            Logging.LogError(e);
        }
    }
}
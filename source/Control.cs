using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CustomFilters.ModCompatibility;
using CustomFilters.TabConfig;
using HBS.Util;
using Newtonsoft.Json;

namespace CustomFilters;

internal static class Control
{
    public static Settings Settings;
    public static TabInfo[] Tabs;

    public static void Init(string directory, string settingsJson)
    {
        Exception settingsEx = null;
        try
        {
            Settings = new();
            JSONSerializationUtility.FromJSON(Settings, settingsJson);
        }
        catch (Exception e)
        {
            Settings = new();
            settingsEx = e;
        }

        Logging.Init(Settings.LogLevel);
        Logging.Debug?.Log("Starting");
        if (settingsEx != null)
        {
            Logging.Error?.Log("Could not read settings", settingsEx);
        }

        try
        {
            var tabsConfigPath = Path.Combine(directory, Settings.TabsConfigFile);
            var tabsConfigJsonString = File.ReadAllText(tabsConfigPath);
            Tabs = JsonConvert.DeserializeObject<TabInfo[]>(tabsConfigJsonString);
        }
        catch (Exception e)
        {
            Logging.Error?.Log($"Could not read tabs config from {Settings.TabsConfigFile}", e);
            throw;
        }

        try
        {
            var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            throw;
        }

        Logging.Info?.Log("Loaded");
    }

    public static void FinishedLoading(List<string> loadOrder)
    {
        try
        {
            if (loadOrder.Contains("CustomComponents"))
            {
                CustomComponentsModCompatibility.Setup();
            }

            if (loadOrder.Contains("BattletechPerformanceFix"))
            {
                BattleTechPerformanceFixModCompatibility.Setup();
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            throw;
        }
    }
}
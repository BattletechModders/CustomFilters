using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomFilters.MechBaySorting;
using CustomFilters.MechLabFiltering.TabConfig;
using CustomFilters.ModCompatibility;
using HBS.Util;
using Newtonsoft.Json;

namespace CustomFilters;

internal static class Control
{
    public static Settings Settings = null!;
    public static TabInfo[] Tabs = null!;

    public static void Init(string directory, string settingsJson)
    {
        Exception? settingsEx = null;
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

        Logging.Setup(Settings.TraceEnabled);
        Logging.Debug?.Log("Starting");
        if (settingsEx != null)
        {
            Logging.Error?.Log("Could not read settings", settingsEx);
        }

        try
        {
            {
                var tabsConfigPath = Path.Combine(directory, Settings.TabsConfigFile);
                Logging.Info?.Log($"Reading tabs configuration from {Settings.TabsConfigFile}");
                var tabsConfigJsonString = File.ReadAllText(tabsConfigPath);
                Tabs = JsonConvert.DeserializeObject<TabInfo[]>(tabsConfigJsonString);

                if (Tabs?.FirstOrDefault()?.Buttons?.FirstOrDefault() == null)
                {
                    throw new NullReferenceException("no tabs or no buttons in first tab");
                }
            }

            MechBayDynamicSorting.SetSortOrder(Settings.MechBayDefaultSortOrder);

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
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            throw;
        }
    }
}
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomFilters.MechBaySorting;
using CustomFilters.MechLabFiltering.TabConfig;
using CustomFilters.ModCompatibility;
using CustomFilters.Settings;
using Newtonsoft.Json;

namespace CustomFilters;

internal static class Control
{
    public static MainSettings MainSettings = new();
    public static TabInfo[] Tabs = null!;

    public static void Init(string directory)
    {
        try
        {
            MainSettings = LoadSettings<MainSettings>(directory, "Settings.json");

            Logging.Setup(MainSettings.Logging);
            Logging.Debug?.Log("Starting");

            { // tabs
                Tabs = LoadSettings<TabInfo[]>(directory, MainSettings.MechLab.TabsConfigFile);
                if (Tabs?.FirstOrDefault()?.Buttons?.FirstOrDefault() == null)
                {
                    throw new NullReferenceException("no tabs or no buttons in first tab");
                }
            }

            MechBayDynamicSorting.SetSortOrder(MainSettings.MechBay.DefaultSortOrder);

            var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logging.Info?.Log("Loaded");
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            throw;
        }
    }

    private static T LoadSettings<T>(string directory, string filename)
    {
        var configPath = Path.Combine(directory, filename);
        Logging.Info?.Log($"Reading {Path.GetFileName(configPath)}");
        var jsonString = File.ReadAllText(configPath);
        return JsonConvert.DeserializeObject<T>(jsonString);
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
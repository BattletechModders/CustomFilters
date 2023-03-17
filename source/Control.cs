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
using Newtonsoft.Json.Converters;

namespace CustomFilters;

internal static class Control
{
    public static MainSettings MainSettings = new();

    public static void Init(string directory)
    {
        try
        {
            MainSettings = LoadSettings(Path.Combine(directory, "Settings.json"), MainSettings, true);

            Log.Main.Debug?.Log("Starting");

            { // tabs
                MainSettings.MechLab.Tabs
                    = LoadSettings<TabInfo[]>(Path.Combine(directory, MainSettings.MechLab.TabsConfigFile));

                if (MainSettings.MechLab.Tabs.FirstOrDefault()?.Buttons.FirstOrDefault() == null)
                {
                    throw new NullReferenceException("no tabs, or no buttons in first tab");
                }

                for (var tabIndex = 0; tabIndex < MainSettings.MechLab.Tabs.Length; tabIndex++)
                {
                    var tabInfo = MainSettings.MechLab.Tabs[tabIndex];
                    tabInfo.Index = tabIndex;
                    for (var buttonIndex = 0; buttonIndex < tabInfo.Buttons.Length; buttonIndex++)
                    {
                        var buttonInfo = tabInfo.Buttons[buttonIndex];
                        buttonInfo.Index = buttonIndex;
                    }
                }
            }

            MechBayDynamicSorting.SetSortOrder(MainSettings.MechBay.DefaultSortOrder);

            var harmony = HarmonyInstance.Create("io.github.denadan.CustomFilters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Main.Info?.Log("initialized");
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
            throw;
        }
    }

    private static T LoadSettings<T>(string configPath, T? defaultSettings = null, bool saveLast = false) where T: class
    {
        if (defaultSettings != null)
        {
            SaveSettings(defaultSettings, configPath, ".help.json", true);
        }

        T settings;
        // current
        {
            Log.Main.Info?.Log($"Reading {Path.GetFileName(configPath)}");
            var jsonString = File.ReadAllText(configPath);
            settings = JsonConvert.DeserializeObject<T>(jsonString);
        }

        if (saveLast)
        {
            SaveSettings(settings, configPath, ".last.json", false);
        }

        return settings;
    }

    private static void SaveSettings<T>(T settings, string originalPath, string newFileSuffix, bool saveDescriptions = true)
    {
        var newFilename = Path.GetFileNameWithoutExtension(originalPath) + newFileSuffix;
        var newPath = Path.Combine(Path.GetDirectoryName(originalPath)!, newFilename);

        using var sw = new StreamWriter(newPath);
        using var jw = new JsonTextWriter(sw);
        jw.Formatting = Formatting.Indented;
        jw.IndentChar = ' ';
        jw.Indentation = 4;
        var serializer = new JsonSerializer();
        serializer.Converters.Add(new StringEnumConverter());
        serializer.Serialize(jw, settings);
    }

    public static void FinishedLoading(List<string> loadOrder)
    {
        try
        {
            // avoids eager loading references that can't be resolved
            if (loadOrder.Contains("CustomComponents"))
            {
                CustomComponentsModCompatibility.Setup();
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
            throw;
        }
    }
}
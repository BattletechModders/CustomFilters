#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(SkirmishMechBayPanel), nameof(SkirmishMechBayPanel.RequestResources))]
public static class SkirmishMechBayPanel_RequestResources_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static bool Prefix(SkirmishMechBayPanel __instance)
    {
        try
        {
            TagManagerFeature.Shared.RequestResources(__instance);
            return false;
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
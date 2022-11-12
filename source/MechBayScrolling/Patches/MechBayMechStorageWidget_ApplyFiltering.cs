#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ApplyFiltering))]
public static class MechBayMechStorageWidget_ApplyFiltering
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Logging.Trace?.Log("MechBayMechStorageWidget.ApplyFiltering");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(true);
                return false;
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ApplyFiltering))]
public static class MechBayMechStorageWidget_ApplyFiltering
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_ApplyFiltering));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(true);
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
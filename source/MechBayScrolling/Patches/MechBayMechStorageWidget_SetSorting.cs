#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetSorting))]
public static class MechBayMechStorageWidget_SetSorting
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_SetSorting));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(false);
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
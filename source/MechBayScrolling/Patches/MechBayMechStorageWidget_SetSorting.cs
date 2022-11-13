#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetSorting))]
public static class MechBayMechStorageWidget_SetSorting
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Logging.Trace?.Log(nameof(MechBayMechStorageWidget_SetSorting));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(false);
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
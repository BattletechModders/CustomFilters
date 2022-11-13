#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ClearInventory))]
public static class MechBayMechStorageWidget_ClearInventory
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Logging.Trace?.Log(nameof(MechBayMechStorageWidget_ClearInventory));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.ClearInventory();
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
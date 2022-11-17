#nullable disable
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<ChassisDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_Chassis
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, List<ChassisDef> chassisDefs, bool resetFilters)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_InitInventory_Chassis));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.InitInventory(chassisDefs, resetFilters);
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
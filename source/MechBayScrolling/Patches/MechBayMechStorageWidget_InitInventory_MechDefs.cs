#nullable disable
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<MechDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_MechDefs
{

    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, List<MechDef> mechDefs, bool resetFilters)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_InitInventory_MechDefs));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.InitInventory(mechDefs, resetFilters);
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
#nullable disable
// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<ChassisDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_Chassis
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, List<ChassisDef> chassisDefs, bool resetFilters)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_InitInventory_Chassis));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.InitInventory(chassisDefs, resetFilters);
            __runOriginal = false;
        }
    }
}
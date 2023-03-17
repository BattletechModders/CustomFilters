#nullable disable
// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<MechDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_MechDefs
{

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, List<MechDef> mechDefs, bool resetFilters)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_InitInventory_MechDefs));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.InitInventory(mechDefs, resetFilters);
            __runOriginal = false;
        }
    }
}
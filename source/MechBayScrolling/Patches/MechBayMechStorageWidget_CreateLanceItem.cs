#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.CreateLanceItem))]
public static class MechBayMechStorageWidget_CreateLanceItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, MechDef def, ref LanceLoadoutMechItem __result)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_CreateLanceItem));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.CreateLanceItem(def);
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
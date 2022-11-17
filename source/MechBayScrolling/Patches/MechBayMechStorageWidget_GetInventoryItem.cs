#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetInventoryItem))]
public static class MechBayMechStorageWidget_GetInventoryItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, string id, ref IMechLabDraggableItem __result)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_GetInventoryItem));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.GetInventoryItem(id);
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
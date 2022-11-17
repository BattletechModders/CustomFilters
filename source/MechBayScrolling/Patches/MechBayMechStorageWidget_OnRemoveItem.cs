#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnRemoveItem))]
public static class MechBayMechStorageWidget_OnRemoveItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item, ref bool __result)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnRemoveItem));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.OnRemoveItem(item);
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
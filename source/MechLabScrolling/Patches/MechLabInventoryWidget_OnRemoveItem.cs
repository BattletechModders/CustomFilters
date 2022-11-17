#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnRemoveItem))]
internal static class MechLabInventoryWidget_OnRemoveItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance, IMechLabDraggableItem item)
    {
        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnRemoveItem));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.OnRemoveItem(item);
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
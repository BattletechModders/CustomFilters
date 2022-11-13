#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnAddItem))]
internal static class MechLabInventoryWidget_OnAddItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance, IMechLabDraggableItem item)
    {
        Logging.Trace?.Log(nameof(MechLabInventoryWidget_OnAddItem));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.OnAddItem(item);
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
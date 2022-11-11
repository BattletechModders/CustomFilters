#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.MechCanEquipItem))]
internal static class MechLabPanel_MechCanEquipItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, InventoryItemElement_NotListView item)
    {
        Logging.Trace?.Log("[LimitItems] MechCanEquipItem_Pre");
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                // TODO figure this one out
                return mechLabFixState.MechCanEquipItem(item);
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
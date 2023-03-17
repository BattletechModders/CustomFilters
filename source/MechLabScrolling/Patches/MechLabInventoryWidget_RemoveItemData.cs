#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.RemoveItemData))]
internal static class MechLabInventoryWidget_RemoveItemData
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance, ListElementController_BASE_NotListView itemData)
    {
        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_RemoveItemData));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.RemoveItemData(itemData);
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
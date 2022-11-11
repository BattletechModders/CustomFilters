#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

// Fix some annoying seemingly vanilla log spam
[HarmonyPatch(typeof(InventoryItemElement_NotListView), nameof(InventoryItemElement_NotListView.OnDestroy))]
internal static class InventoryItemElement_NotListView_OnDestroy
{
    [HarmonyBefore(Mods.BattleTechPerformanceFix)]
    [HarmonyPrefix]
    public static bool Prefix(InventoryItemElement_NotListView __instance)
    {
        Logging.Trace?.Log("[LimitItems] OnDestroy");
        if (__instance.iconMech != null) __instance.iconMech.sprite = null;
        return false;
    }
}
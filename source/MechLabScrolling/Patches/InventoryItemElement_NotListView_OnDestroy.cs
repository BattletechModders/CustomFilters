#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

// Fix some annoying seemingly vanilla log spam
[HarmonyPatch(typeof(InventoryItemElement_NotListView), nameof(InventoryItemElement_NotListView.OnDestroy))]
internal static class InventoryItemElement_NotListView_OnDestroy
{
    [HarmonyBefore(Mods.BattleTechPerformanceFix)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, InventoryItemElement_NotListView __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(InventoryItemElement_NotListView_OnDestroy));
        if (__instance.iconMech != null) __instance.iconMech.sprite = null;
        __runOriginal = false;
    }
}
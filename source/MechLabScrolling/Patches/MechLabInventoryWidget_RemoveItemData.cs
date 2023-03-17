#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.RemoveItemData))]
internal static class MechLabInventoryWidget_RemoveItemData
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance, ListElementController_BASE_NotListView itemData)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_RemoveItemData));

        if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
        {
            mechLabFixState.RemoveItemData(itemData);
            __runOriginal = false;
        }
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnAddItem))]
internal static class MechLabInventoryWidget_OnAddItem
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance, IMechLabDraggableItem item)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnAddItem));

        if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
        {
            mechLabFixState.OnAddItem(item);
            __runOriginal = false;
        }
    }
}
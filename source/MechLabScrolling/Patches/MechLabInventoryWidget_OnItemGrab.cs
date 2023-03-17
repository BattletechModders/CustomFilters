#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnItemGrab))]
internal static class MechLabInventoryWidget_OnItemGrab
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance, ref IMechLabDraggableItem item)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnItemGrab));

        if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
        {
            mechLabFixState.OnItemGrab(ref item);
        }
    }
}
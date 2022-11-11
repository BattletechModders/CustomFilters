#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ResetFilters))]
internal static class MechLabInventoryWidget_ResetFilters
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        if (UIHandlerTracker.GetInstance(__instance, out var handler))
        {
            handler.ResetFilters();
            return false;
        }
        return true;
    }
}
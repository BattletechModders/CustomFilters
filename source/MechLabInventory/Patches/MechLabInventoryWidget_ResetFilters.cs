using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ResetFilters))]
internal static class MechLabInventoryWidget_ResetFilters
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        return __instance != UIHandler.Widget;
    }
}
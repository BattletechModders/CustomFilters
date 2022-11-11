#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnFilterButtonClicked))]
internal static class MechLabInventoryWidget_OnFilterButtonClicked
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        return !UIHandlerTracker.GetInstance(__instance, out _);
    }
}
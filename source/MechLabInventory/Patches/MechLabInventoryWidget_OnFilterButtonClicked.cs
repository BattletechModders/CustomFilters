#nullable disable
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnFilterButtonClicked))]
internal static class MechLabInventoryWidget_OnFilterButtonClicked
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        return __instance != UIHandler.Widget;
    }
}
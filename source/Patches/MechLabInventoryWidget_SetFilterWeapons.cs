using BattleTech.UI;
using Harmony;

namespace CustomFilters.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.SetFiltersWeapons))]
internal static class MechLabInventoryWidget_SetFilterWeapons
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        return __instance != UIHandler.widget;
    }
}
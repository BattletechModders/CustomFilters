using BattleTech.UI;
using Harmony;

namespace CustomFilters.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnFilterButtonClicked))]
internal static class MechLabInventoryWidget_OnFilterButtonClicked
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        return __instance != UIHandler.widget;
    }
}
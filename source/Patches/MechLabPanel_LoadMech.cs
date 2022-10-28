using BattleTech.UI;
using Harmony;

namespace CustomFilters.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
internal static class MechLabPanel_LoadMech
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        UIHandler.CallFilter();
    }
}
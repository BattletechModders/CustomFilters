#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

// fix for unused shop clones
[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabPanel_InitWidgets));

        MechLabFixStateTracker.SetInstance(__instance);
    }
}
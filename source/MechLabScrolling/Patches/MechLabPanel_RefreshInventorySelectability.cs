#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.RefreshInventorySelectability))]
internal static class MechLabPanel_RefreshInventorySelectability
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabPanel_RefreshInventorySelectability));

        if (MechLabFixStateTracker.GetInstance(__instance, out var state))
        {
            state.RefreshInventorySelectability();
            __runOriginal = false;
        }
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyAfter(Mods.CustomComponents)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ApplySorting))]
internal static class MechLabInventoryWidget_ApplySorting
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_ApplySorting));

        if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
        {
            mechLabFixState.ApplySorting();
            __runOriginal = false;
        }
    }
}
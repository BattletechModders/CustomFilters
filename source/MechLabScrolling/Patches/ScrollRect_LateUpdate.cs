#nullable disable
// ReSharper disable InconsistentNaming
using UnityEngine.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyPatch(typeof(ScrollRect), nameof(ScrollRect.LateUpdate))]
internal static class ScrollRect_LateUpdate
{
    [HarmonyBefore(Mods.BattleTechPerformanceFix)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ScrollRect __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
        {
            mechLabFixState.LateUpdate();
        }
    }
}
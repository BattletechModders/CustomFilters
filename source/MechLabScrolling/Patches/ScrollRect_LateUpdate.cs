#nullable disable
// ReSharper disable InconsistentNaming
using System;
using Harmony;
using UnityEngine.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyPatch(typeof(ScrollRect), nameof(ScrollRect.LateUpdate))]
internal static class ScrollRect_LateUpdate
{
    [HarmonyBefore(Mods.BattleTechPerformanceFix)]
    [HarmonyPrefix]
    public static void Prefix(ScrollRect __instance)
    {
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.LateUpdate();
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}
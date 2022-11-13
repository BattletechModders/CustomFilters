#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ApplySorting))]
internal static class MechLabInventoryWidget_ApplySorting
{
    [HarmonyPriority(Priority.First + 1)]
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        Logging.Trace?.Log(nameof(MechLabInventoryWidget_ApplySorting));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.ApplySorting();
                return false;
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
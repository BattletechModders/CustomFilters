#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

// fix for unused shop clones
[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    [HarmonyPrefix]
    public static void Prefix(MechLabPanel __instance)
    {
        Logging.Trace?.Log("[LimitItems] InitWidgets");
        try
        {
            MechLabFixStateTracker.SetInstance(__instance);
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}
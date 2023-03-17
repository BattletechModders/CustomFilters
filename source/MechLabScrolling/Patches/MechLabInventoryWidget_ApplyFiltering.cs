#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ApplyFiltering))]
internal static class MechLabInventoryWidget_ApplyFiltering
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance, bool refreshPositioning)
    {
        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_ApplyFiltering));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.ApplyFiltering(refreshPositioning);
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
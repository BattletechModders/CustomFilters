#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.PopulateInventory))]
internal static class MechLabPanel_PopulateInventory
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance)
    {
        Log.Main.Trace?.Log(nameof(MechLabPanel_PopulateInventory));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.PopulateInventory();
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
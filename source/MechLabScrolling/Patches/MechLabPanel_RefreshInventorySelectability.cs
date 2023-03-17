#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.RefreshInventorySelectability))]
internal static class MechLabPanel_RefreshInventorySelectability
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance)
    {
        Log.Main.Trace?.Log(nameof(MechLabPanel_RefreshInventorySelectability));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var state))
            {
                state.RefreshInventorySelectability();
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
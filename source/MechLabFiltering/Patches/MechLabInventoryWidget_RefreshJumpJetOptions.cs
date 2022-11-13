#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.RefreshJumpJetOptions))]
internal static class MechLabInventoryWidget_RefreshJumpJetOptions
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance, float tonnage)
    {
        Logging.Trace?.Log(nameof(MechLabInventoryWidget_RefreshJumpJetOptions));
        try
        {
            if (UIHandlerTracker.GetInstance(__instance, out var handler))
            {
                handler.RefreshJumpJetOptions(tonnage);
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
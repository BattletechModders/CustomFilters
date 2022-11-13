#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ResetFilters))]
internal static class MechLabInventoryWidget_ResetFilters
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        Logging.Trace?.Log(nameof(MechLabInventoryWidget_ResetFilters));
        try
        {
            if (UIHandlerTracker.GetInstance(__instance, out var handler))
            {
                handler.ResetFilters();
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
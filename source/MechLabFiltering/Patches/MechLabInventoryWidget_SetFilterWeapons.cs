#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.SetFiltersWeapons))]
internal static class MechLabInventoryWidget_SetFilterWeapons
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        Logging.Trace?.Log(nameof(MechLabInventoryWidget_SetFilterWeapons));
        try
        {
            return !UIHandlerTracker.GetInstance(__instance, out _);
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
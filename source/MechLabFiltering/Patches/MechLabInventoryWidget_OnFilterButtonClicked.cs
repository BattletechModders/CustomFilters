#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnFilterButtonClicked))]
internal static class MechLabInventoryWidget_OnFilterButtonClicked
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabInventoryWidget __instance)
    {
        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnFilterButtonClicked));
        try
        {
            return !UIHandlerTracker.GetInstance(__instance, out _);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
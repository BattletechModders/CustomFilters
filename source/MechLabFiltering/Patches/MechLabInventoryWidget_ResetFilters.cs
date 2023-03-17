#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.ResetFilters))]
internal static class MechLabInventoryWidget_ResetFilters
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_ResetFilters));
        try
        {
            if (UIHandlerTracker.GetInstance(__instance, out var handler))
            {
                handler.ResetFilters();
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
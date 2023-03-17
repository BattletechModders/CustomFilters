#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.SetFiltersWeapons))]
internal static class MechLabInventoryWidget_SetFilterWeapons
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_SetFilterWeapons));
        try
        {
            __runOriginal = !UIHandlerTracker.GetInstance(__instance, out _);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnFilterButtonClicked))]
internal static class MechLabInventoryWidget_OnFilterButtonClicked
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnFilterButtonClicked));
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
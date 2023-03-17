#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.RefreshJumpJetOptions))]
internal static class MechLabInventoryWidget_RefreshJumpJetOptions
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabInventoryWidget __instance, float tonnage)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_RefreshJumpJetOptions));
        try
        {
            if (UIHandlerTracker.GetInstance(__instance, out var handler))
            {
                handler.RefreshJumpJetOptions(tonnage);
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
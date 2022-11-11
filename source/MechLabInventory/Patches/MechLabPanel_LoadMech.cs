#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
internal static class MechLabPanel_LoadMech
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            if (UIHandlerTracker.GetInstance(__instance.inventoryWidget, out var handler))
            {
                handler.ApplyFiltering();
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}
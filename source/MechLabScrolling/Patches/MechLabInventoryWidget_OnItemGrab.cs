#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabScrolling.Patches;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabInventoryWidget), nameof(MechLabInventoryWidget.OnItemGrab))]
internal static class MechLabInventoryWidget_OnItemGrab
{
    [HarmonyPrefix]
    public static void Prefix(MechLabInventoryWidget __instance, ref IMechLabDraggableItem item)
    {
        Log.Main.Trace?.Log(nameof(MechLabInventoryWidget_OnItemGrab));
        try
        {
            if (MechLabFixStateTracker.GetInstance(__instance, out var mechLabFixState))
            {
                mechLabFixState.OnItemGrab(ref item);
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
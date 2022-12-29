#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnItemGrab))]
public static class MechBayMechStorageWidget_OnItemGrab
{
    [HarmonyPrefix]
    public static void Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnItemGrab));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.OnItemGrab(ref item);
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnButtonClicked))]
public static class MechBayMechStorageWidget_OnButtonClicked
{
    [HarmonyPrefix]
    public static void Prefix(MechBayMechStorageWidget __instance, ref IMechLabDraggableItem item)
    {
        Logging.Trace?.Log("MechBayMechStorageWidget.OnButtonClicked");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.OnButtonClicked(ref item);
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}
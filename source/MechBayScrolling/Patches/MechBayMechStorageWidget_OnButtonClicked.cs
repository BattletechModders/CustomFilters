#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnButtonClicked))]
public static class MechBayMechStorageWidget_OnButtonClicked
{
    [HarmonyPrefix]
    public static void Prefix(MechBayMechStorageWidget __instance, ref IMechLabDraggableItem item)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnButtonClicked));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.OnButtonClicked(ref item);
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnAddItem))]
public static class MechBayMechStorageWidget_OnAddItem
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item, bool validate, ref bool __result)
    {
        Logging.Trace?.Log("MechBayMechStorageWidget.OnAddItem");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.OnAddItem(item, validate);
                return false;
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
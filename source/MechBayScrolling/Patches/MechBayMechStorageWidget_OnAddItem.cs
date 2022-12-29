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
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnAddItem));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.OnAddItem(item, validate);
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
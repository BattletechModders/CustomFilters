#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetMechDefByGUID))]
public static class MechBayMechStorageWidget_GetMechDefByGUID
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, string GUID, ref IMechLabDraggableItem __result)
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_GetMechDefByGUID));
        try
        {
            if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.GetMechDefByGUID(GUID);
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
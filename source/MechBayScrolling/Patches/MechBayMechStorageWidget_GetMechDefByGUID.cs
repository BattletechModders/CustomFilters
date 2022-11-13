#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetMechDefByGUID))]
public static class MechBayMechStorageWidget_GetMechDefByGUID
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, string GUID, ref IMechLabDraggableItem __result)
    {
        Logging.Trace?.Log(nameof(MechBayMechStorageWidget_GetMechDefByGUID));
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.GetMechDefByGUID(GUID);
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
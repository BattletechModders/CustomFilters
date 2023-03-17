#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetMechDefByGUID))]
public static class MechBayMechStorageWidget_GetMechDefByGUID
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, string GUID, ref IMechLabDraggableItem __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_GetMechDefByGUID));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            __result = customWidget.GetMechDefByGUID(GUID);
            __runOriginal = false;
        }
    }
}
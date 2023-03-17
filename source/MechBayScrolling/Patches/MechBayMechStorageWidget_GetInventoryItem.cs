#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetInventoryItem))]
public static class MechBayMechStorageWidget_GetInventoryItem
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, string id, ref IMechLabDraggableItem __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_GetInventoryItem));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            __result = customWidget.GetInventoryItem(id);
            __runOriginal = false;
        }
    }
}
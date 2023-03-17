#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnRemoveItem))]
public static class MechBayMechStorageWidget_OnRemoveItem
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, IMechLabDraggableItem item, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnRemoveItem));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            __result = customWidget.OnRemoveItem(item);
            __runOriginal = false;
        }
    }
}
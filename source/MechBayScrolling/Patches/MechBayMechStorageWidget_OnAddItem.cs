#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnAddItem))]
public static class MechBayMechStorageWidget_OnAddItem
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, IMechLabDraggableItem item, bool validate, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnAddItem));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            __result = customWidget.OnAddItem(item, validate);
            __runOriginal = false;
        }
    }
}
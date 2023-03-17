#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnItemGrab))]
public static class MechBayMechStorageWidget_OnItemGrab
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, IMechLabDraggableItem item)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnItemGrab));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.OnItemGrab(ref item);
        }
    }
}
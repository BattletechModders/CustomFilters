#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnButtonClicked))]
public static class MechBayMechStorageWidget_OnButtonClicked
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, ref IMechLabDraggableItem item)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_OnButtonClicked));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.OnButtonClicked(ref item);
        }
    }
}
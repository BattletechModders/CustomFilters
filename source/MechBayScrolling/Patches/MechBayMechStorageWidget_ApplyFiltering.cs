#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ApplyFiltering))]
public static class MechBayMechStorageWidget_ApplyFiltering
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_ApplyFiltering));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.FilterAndSort(true);
            __runOriginal = false;
        }
    }
}
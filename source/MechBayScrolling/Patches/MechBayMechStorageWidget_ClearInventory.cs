#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ClearInventory))]
public static class MechBayMechStorageWidget_ClearInventory
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_ClearInventory));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            customWidget.ClearInventory();
            __runOriginal = false;
        }
    }
}
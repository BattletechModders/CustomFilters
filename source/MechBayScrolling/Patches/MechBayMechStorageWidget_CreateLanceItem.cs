#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.CreateLanceItem))]
public static class MechBayMechStorageWidget_CreateLanceItem
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechBayMechStorageWidget __instance, MechDef def, ref LanceLoadoutMechItem __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_CreateLanceItem));

        if (CustomStorageWidgetTracker.TryGet(__instance, out var customWidget))
        {
            __result = customWidget.CreateLanceItem(def);
            __runOriginal = false;
        }
    }
}
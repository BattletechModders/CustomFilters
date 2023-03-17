#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.LanceIsValidForSkirmish))]
public static class MechValidationRules_LanceIsValidForSkirmish_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, LanceDef def, bool requireFullLance, bool includeCustomLances, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = def != null && TagManagerFeature.Shared.LanceIsValidForSkirmish(def, requireFullLance, includeCustomLances);
        __runOriginal = false;
    }
}
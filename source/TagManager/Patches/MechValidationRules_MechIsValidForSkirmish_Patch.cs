#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.MechIsValidForSkirmish))]
public static class MechValidationRules_MechIsValidForSkirmish_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef def, bool includeCustomMechs, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = def != null && TagManagerFeature.Shared.MechIsValidForSkirmish(def, includeCustomMechs);
        __runOriginal = false;
    }
}
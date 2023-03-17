#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.PilotIsValidForSkirmish))]
public static class MechValidationRules_PilotIsValidForSkirmish_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, PilotDef def, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = def != null && TagManagerFeature.Shared.PilotIsValidForSkirmish(def);
        __runOriginal = false;
    }
}
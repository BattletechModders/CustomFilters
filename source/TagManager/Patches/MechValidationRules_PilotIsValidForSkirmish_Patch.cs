#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech;
using Harmony;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.PilotIsValidForSkirmish))]
public static class MechValidationRules_PilotIsValidForSkirmish_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    public static bool Prefix(PilotDef def, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.PilotIsValidForSkirmish(def);
            return false;
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
        return true;
    }
}
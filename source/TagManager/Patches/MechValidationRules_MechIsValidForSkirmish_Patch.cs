#nullable disable
// ReSharper disable InconsistentNaming
using System;
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
    public static bool Prefix(MechDef def, bool includeCustomMechs, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.MechIsValidForSkirmish(def, includeCustomMechs);
            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
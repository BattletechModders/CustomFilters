#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech;
using Harmony;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.LanceIsValidForSkirmish))]
public static class MechValidationRules_LanceIsValidForSkirmish_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    public static bool Prefix(LanceDef def, bool requireFullLance, bool includeCustomLances, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.LanceIsValidForSkirmish(def, requireFullLance, includeCustomLances);
            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
        return true;
    }
}
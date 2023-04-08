#nullable disable
// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using BattleTech.Save;
using Localize;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch]
public static class SkirmishUnitsAndLances_ValidateSerializedMechs_Patch
{
    [HarmonyPrepare]
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(SkirmishUnitsAndLances), nameof(SkirmishUnitsAndLances.GetValidatedMechs))]
    [HarmonyPatch(typeof(SkirmishUnitsAndLances), nameof(SkirmishUnitsAndLances.ValidateArchivedMechs))]
    public static IEnumerable<CodeInstruction> TranspilerValidateSerializedMechs(IEnumerable<CodeInstruction> instructions)
    {
        return instructions
            .MethodReplacer(
                AccessTools.Method(typeof(MechValidationRules), nameof(ValidateSerializedMechs)),
                AccessTools.Method(typeof(SkirmishUnitsAndLances_ValidateSerializedMechs_Patch), nameof(ValidateSerializedMechs))
            );
    }

    public static bool ValidateSerializedMechs(MechDef mechDef, DataManager dataManager, out Text errorString)
    {
        if (mechDef?.Chassis == null || dataManager == null)
        {
            errorString = null;
            return false;
        }

        if (!dataManager.ChassisDefs.Exists(mechDef.ChassisID))
        {
            errorString = new($"{mechDef.Name} : Has an Invalid Chassis");
            return false;
        }

        errorString = null;
        return true;
    }
}
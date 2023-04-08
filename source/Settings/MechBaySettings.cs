using BattleTech;
using CustomFilters.MechBaySorting;
using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MechBaySettings
{
    [JsonProperty]
    internal readonly string DefaultSortOrderDescription =
        $"A list of sort order terms used for default sorting in the MechBay." +
        $" Available terms: {string.Join(", ", MechBayDynamicSorting.Terms)}.";
    [JsonProperty(Required = Required.DisallowNull)]
    internal string[] DefaultSortOrder =
    {
        "!ChassisTonnage",
        "ChassisName",
        "MechName"
    };

    [JsonProperty]
    internal readonly string MechUnitValidationEnabledDescription =
        "Show validation errors when scrolling the mech list in the MechBay.";
    [JsonProperty]
    internal bool MechUnitValidationEnabled = true;

    [JsonProperty]
    internal readonly string MechUnitValidationLevelDescription =
        "Validation level to run through with MechValidationRules.ValidateMechDef." +
        " If set to `null` will run validation through MechValidationRules' GetMechFieldableWarnings and ValidateMechCanBeFielded.";
    [JsonProperty]
    internal MechValidationLevel? MechUnitValidationLevel = MechValidationLevel.Full;

    [JsonProperty]
    internal readonly string MechUnitValidationWarningsDescription =
        "Which validations to show as warnings, all others will be shown as error." +
        " Default set of validation types taken from MechValidationRules.GetMechFieldableWarnings(...).";
    [JsonProperty]
    internal MechValidationType[] MechUnitValidationWarnings = {
        MechValidationType.Underweight,
        MechValidationType.StructureDamaged,
        MechValidationType.AmmoMissing,
        MechValidationType.AmmoUnneeded
    };
}
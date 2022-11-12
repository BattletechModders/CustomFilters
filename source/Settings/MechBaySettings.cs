using CustomFilters.MechBaySorting;
using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MechBaySettings
{
    [JsonProperty]
    internal readonly string DefaultSortOrderDescription =
        $"A list of sort order terms used for default sorting in the MechBay." +
        $" Available terms: {string.Join(", ", MechBayDynamicSorting.Terms)}.";
    [JsonProperty]
    internal string[] DefaultSortOrder =
    {
        "!ChassisTonnage",
        "ChassisName",
        "MechName"
    };
}
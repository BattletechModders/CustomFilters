using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MechBaySettings
{
    [JsonProperty]
    internal string[] DefaultSortOrder =
    {
        "!ChassisTonnage",
        "ChassisName",
        "MechName"
    };
}
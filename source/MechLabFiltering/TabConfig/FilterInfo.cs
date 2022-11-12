using BattleTech;
using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

internal class FilterInfo
{
    [JsonProperty]
    internal ComponentType[]? ComponentTypes = null;

    [JsonProperty]
    internal string[]? WeaponCategories = null;

    [JsonProperty]
    internal string[]? AmmoCategories = null;

    [JsonProperty]
    internal string[]? UILookAndColorIcons = null;

    [JsonProperty]
    internal string[]? Categories = null;

    [JsonProperty]
    internal string[]? NotCategories = null;
}
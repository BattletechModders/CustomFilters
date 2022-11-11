using BattleTech;
using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

public class FilterInfo
{
    [JsonProperty]
    public ComponentType[]? ComponentTypes;
    [JsonProperty]
    public string[]? WeaponCategories;
    [JsonProperty]
    public string[]? AmmoCategories;
    [JsonProperty]
    public string[]? UILookAndColorIcons;
    [JsonProperty]
    public string[]? Categories;
    [JsonProperty]
    public string[]? NotCategories;
}
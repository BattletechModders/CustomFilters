using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

public class TabInfo
{
    [JsonProperty]
    public string? Caption;
    [JsonProperty]
    public FilterInfo? Filter;
    [JsonProperty]
    public ButtonInfo[]? Buttons;
}
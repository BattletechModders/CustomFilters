using Newtonsoft.Json;

namespace CustomFilters.TabConfig;

public class ButtonInfo
{
    [JsonProperty]
    public string? Text;
    [JsonProperty]
    public string? Tag;
    [JsonProperty]
    public string? Icon;
    [JsonProperty]
    public string? Tooltip;
    [JsonProperty]
    public bool Debug;
    [JsonProperty]
    public FilterInfo? Filter;
}
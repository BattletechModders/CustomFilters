using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

internal class ButtonInfo
{
    [JsonIgnore]
    internal int Index = -1;

    [JsonProperty]
    internal string? Text = null;

    [JsonProperty]
    internal string? Tag = null;

    [JsonProperty]
    internal string? Icon = null;

    [JsonProperty]
    internal string? Tooltip = null;

    [JsonProperty]
    internal bool Debug = false;

    [JsonProperty]
    internal FilterInfo? Filter = null;

    public override string ToString()
    {
        if (Text != null)
        {
            return Text + "[" + Index + "]";
        }
        return Index.ToString();
    }
}
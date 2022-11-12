using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

internal class TabInfo
{
    [JsonIgnore]
    internal int Index = -1;

    [JsonProperty]
    internal string? Caption = null;

    [JsonProperty]
    internal FilterInfo? Filter = null;

    [JsonProperty(Required = Required.Always)]
    internal ButtonInfo[] Buttons = null!;

    public override string ToString()
    {
        if (Caption != null)
        {
            return Caption + "[" + Index + "]";
        }
        return Index.ToString();
    }
}
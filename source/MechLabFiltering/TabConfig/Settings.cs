using Newtonsoft.Json;

namespace CustomFilters.MechLabFiltering.TabConfig;

public class Settings
{
    [JsonProperty]
    public bool TraceEnabled = true;
    [JsonProperty]
    public bool ShowDebugButtons;
    [JsonProperty]
    public string TabsConfigFile = "MechEngineerTabs.json";
}

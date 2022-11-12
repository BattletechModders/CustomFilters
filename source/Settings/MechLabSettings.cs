using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MechLabSettings
{
    [JsonProperty]
    internal string TabsConfigFile = "VanillaTabs.json";
    [JsonProperty]
    internal bool ShowDebugButtons = false;
}
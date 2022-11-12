using CustomFilters.MechLabFiltering.TabConfig;
using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MechLabSettings
{
    [JsonProperty]
    internal readonly string TabsConfigFileDescription =
        "Points to a file that contains a predefined set of inventory tabs to show in MechLab." +
        " See *Tab.json files for examples.";
    [JsonProperty]
    internal string TabsConfigFile = "VanillaTabs.json";

    // this is loaded by Control
    [JsonIgnore]
    internal TabInfo[] Tabs = null!;

    [JsonProperty]
    internal bool ShowDebugButtons = false;

    [JsonProperty]
    internal const string ShopPoolingFixEnabledDescription =
        "Fixes a vanilla memory leak where the shop in the MechLab does not get pooled.";
    [JsonProperty]
    public bool ShopPoolingFixEnabled = true;
}
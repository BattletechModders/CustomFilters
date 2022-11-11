using HBS.Logging;
using Newtonsoft.Json;

namespace CustomFilters.TabConfig;

public class Settings
{
    [JsonProperty]
    public LogLevel LogLevel = LogLevel.Debug;
    [JsonProperty]
    public bool ShowDebugButtons;
    [JsonProperty]
    public string TabsConfigFile = "MechEngineerTabs.json";
}

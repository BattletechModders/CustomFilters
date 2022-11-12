using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MainSettings
{
    [JsonProperty]
    internal LogSettings Logging = new();
    [JsonProperty]
    internal MechBaySettings MechBay = new();
    [JsonProperty]
    internal MechLabSettings MechLab = new();
}
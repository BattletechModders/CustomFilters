using CustomFilters.TagManager;
using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class MainSettings
{
    [JsonProperty(Required = Required.DisallowNull)]
    internal MechBaySettings MechBay = new();

    [JsonProperty(Required = Required.DisallowNull)]
    internal MechLabSettings MechLab = new();

    [JsonProperty(Required = Required.DisallowNull)]
    internal TagManagerSettings TagManager = new();
}
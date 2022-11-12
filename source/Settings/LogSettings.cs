using HBS.Logging;
using Newtonsoft.Json;

namespace CustomFilters.Settings;

internal class LogSettings
{
    [JsonProperty]
    internal LogLevel DefaultLogLevel = LogLevel.Log;
    [JsonProperty]
    internal bool TraceEnabled = false;
}

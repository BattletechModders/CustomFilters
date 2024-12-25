using HBS.Logging;
using ModTek.Public;

namespace CustomFilters;

internal static class Log
{
    private const string Name = nameof(CustomFilters);
    internal static readonly NullableLogger Main = NullableLogger.GetLogger(Name, LogLevel.Debug);
}
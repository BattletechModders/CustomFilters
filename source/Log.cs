#nullable enable
using HBS.Logging;
using NullableLogging;

namespace CustomFilters;

internal static class Log
{
    private const string Name = nameof(CustomFilters);
    internal static readonly NullableLogger Main = NullableLogger.GetLogger(Name, LogLevel.Debug);
}
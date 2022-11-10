using System;
using System.Diagnostics;
using HBS.Logging;

namespace CustomFilters;

internal static class Logging
{
    // ReSharper disable once InconsistentNaming
    private static readonly ILog _logger = Logger.GetLogger("CustomFilters");
    public static void Init(LogLevel logLevel)
    {
        Logger.SetLoggerLevel(_logger.Name, logLevel);
    }

    [Conditional("CCDEBUG")]
    public static void LogDebug(string message)
    {
        _logger.LogDebug(message);
    }

    public static void LogError(string message)
    {
        _logger.LogError(message);
    }

    public static void LogError(string message, Exception e)
    {
        _logger.LogError(message, e);
    }

    public static void LogError(Exception e)
    {
        _logger.LogError(e);
    }

    public static void Log(string message)
    {
        _logger.Log(message);
    }
}
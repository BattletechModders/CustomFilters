using System;
using System.Diagnostics;
using HBS.Logging;

namespace CustomFilters;

// see Better(Level)Logger from ME for concept
internal static class Logging
{
    // ReSharper disable once InconsistentNaming
    private static ILog _logger = null!;
    public static void Init(LogLevel logLevel)
    {
        _logger = Logger.GetLogger("CustomFilters", logLevel);

        if (logLevel <= LogLevel.Error)
        {
            Error = new(LogLevel.Error);
        }
        if (logLevel <= LogLevel.Warning)
        {
            Warn = new(LogLevel.Warning);
        }
        if (logLevel <= LogLevel.Log)
        {
            Info = new(LogLevel.Log);
        }
        if (logLevel <= LogLevel.Debug)
        {
            Debug = new(LogLevel.Debug);
        }
    }

    internal static LevelLogger? Error { get; private set; }
    internal static LevelLogger? Warn { get; private set; }
    internal static LevelLogger? Info { get; private set; }
    internal static LevelLogger? Debug { get; private set; }

    internal class LevelLogger
    {
        private readonly LogLevel _level;

        internal LevelLogger(LogLevel level)
        {
            _level = level;
        }

        internal void Log(object message)
        {
            _logger.LogAtLevel(_level, message);
        }

        internal void Log(object message, Exception e)
        {
            _logger.LogAtLevel(_level, message, e);
        }

        internal void Log(Exception e)
        {
            _logger.LogAtLevel(_level, null, e);
        }
    }
}
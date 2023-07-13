using NLog;
using SoundParadise.Api.Interfaces;
using ILogger = NLog.ILogger;
using LogLevel = NLog.LogLevel;

namespace SoundParadise.Api.Services;

/// <summary>
///     Logging service.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LoggingService<T> : ILoggingService<T>
{
    private readonly ILogger _logger;

    /// <summary>
    ///     Constructor for LoggingService
    /// </summary>
    public LoggingService()
    {
        _logger = LogManager.GetLogger(typeof(T).FullName);
    }

    /// <summary>
    ///     Log error.
    /// </summary>
    /// <param name="logLevel">Error level.</param>
    /// <param name="message">Message about error.</param>
    public void Log(LogLevel logLevel, string message)
    {
        _logger.Log(logLevel, message);
    }

    /// <summary>
    ///     Log error.
    /// </summary>
    /// <param name="message">Message error.</param>
    public void LogError(string message)
    {
        _logger.Log(LogLevel.Error, message);
    }

    /// <summary>
    ///     Log exception
    /// </summary>
    /// <param name="exception">Exception.</param>
    /// <param name="message">Message error.</param>
    public void LogException(Exception exception, string message)
    {
        _logger.Log(LogLevel.Error, exception, message);
    }
}
using LogLevel = NLog.LogLevel;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     Logging service implements that interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILoggingService<T>
{
    void Log(LogLevel logLevel, string message);
    void LogError(string message);

    void LogException(Exception exception, string message);
}
using System.Text.Json;

namespace SoundParadise.Api.Extensions;

/// <summary>
///     Session extensions.
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    ///     Set session data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session">Session.</param>
    /// <param name="key">Session key.</param>
    /// <param name="value">Session value.</param>
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    ///     Get session data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="session">Session.</param>
    /// <param name="key">Session key.</param>
    /// <returns></returns>
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
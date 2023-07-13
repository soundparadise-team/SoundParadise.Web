namespace SoundParadise.Api.Options;

/// <summary>
///     Token options.
/// </summary>
public class TokenOptions
{
    /// <summary>
    ///     Secret key.
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    ///     Validate.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new ArgumentNullException(nameof(SecretKey),
                "The secret key cannot be null or empty. Please provide a valid secret in the TokenOptions.");
    }
}
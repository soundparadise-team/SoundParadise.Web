namespace SoundParadise.Api.Options;

/// <summary>
///     Encryption options.
/// </summary>
public class EncryptionOptions
{
    /// <summary>
    ///     Key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     Encryption vector.
    /// </summary>
    public string IV { get; set; }

    /// <summary>
    ///     Validate.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new ArgumentNullException(nameof(Key),
                "The key cannot be null or empty. Please provide a valid key in the EncryptionOptions.");

        if (string.IsNullOrWhiteSpace(IV))
            throw new ArgumentNullException(nameof(IV),
                "The IV cannot be null or empty. Please provide a valid IV in the EncryptionOptions.");
    }
}
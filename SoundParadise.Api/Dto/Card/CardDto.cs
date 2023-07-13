namespace SoundParadise.Web.Dto.Card;

/// <summary>
///     Card Dto.
/// </summary>
public class CardDto
{
    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Card number.
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    ///     Expiration date.
    /// </summary>
    public string ExpirationDate { get; set; } = string.Empty;

    /// <summary>
    ///     CVV code.
    /// </summary>
    public string CVV { get; set; } = string.Empty;
}
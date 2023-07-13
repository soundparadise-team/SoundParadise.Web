namespace SoundParadise.Api.Dto.Product;

/// <summary>
///     Suggestion product dto.
/// </summary>
public class SuggestionProductDto
{
    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Slug.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
}
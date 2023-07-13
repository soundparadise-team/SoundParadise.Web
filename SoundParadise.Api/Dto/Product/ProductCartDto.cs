namespace SoundParadise.Api.Dto.Product;

/// <summary>
///     ProductCrat dto.
/// </summary>
public class ProductCartDto
{
    private string _imagePath = string.Empty;

    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Nice.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Image path.
    /// </summary>
    public string ImagePath
    {
        get => _imagePath.ToLower();
        set => _imagePath = value;
    }
}
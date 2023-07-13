namespace SoundParadise.Api.Dto.Product;

public class ProductDto
{
    private string _imagePath = string.Empty;

    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Slug.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    ///     Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Price.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    ///     Description
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    ///     Image path.
    /// </summary>
    public string ImagePath
    {
        get => _imagePath.ToLower();
        set => _imagePath = value;
    }

    /// <summary>
    ///     Rating.
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    ///     Comments count.
    /// </summary>
    public int? CommentsCount { get; set; }

    /// <summary>
    ///     Quantity.
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    ///     Is new status.
    /// </summary>
    public bool? IsNew { get; set; }

    /// <summary>
    ///     In stock status.
    /// </summary>
    public bool? InStock { get; set; }

    /// <summary>
    ///     In cart status.
    /// </summary>
    public bool? InCart { get; set; }

    /// <summary>
    ///     In wishlist status.
    /// </summary>
    public bool? InWishlist { get; set; }
}
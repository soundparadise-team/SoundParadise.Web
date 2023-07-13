using SoundParadise.Api.Dto.Product;

namespace SoundParadise.Api.Dto.Requests;

/// <summary>
///     Search product dto result.
/// </summary>
public class SearchProductDtoResult
{
    /// <summary>
    ///     List of products.
    /// </summary>
    public List<ProductDto>? Products { get; set; }

    /// <summary>
    ///     Total count.
    /// </summary>
    public int? TotalCount { get; set; }
}
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Dto.Requests;

/// <summary>
///     Search product result.
/// </summary>
public class SearchProductResult
{
    /// <summary>
    ///     List of products.
    /// </summary>
    public List<ProductModel>? Products { get; set; }

    /// <summary>
    ///     Total count.
    /// </summary>
    public int? TotalCount { get; set; }
}
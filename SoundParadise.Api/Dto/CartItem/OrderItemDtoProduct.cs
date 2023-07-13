using SoundParadise.Api.Dto.Product;

namespace SoundParadise.Api.Dto.CartItem;

/// <summary>
///     OrderItem dto product.
/// </summary>
public class OrderItemDtoProduct : CartItemDto
{
    /// <summary>
    ///     Product.
    /// </summary>
    public ProductDto Product { get; set; }

    /// <summary>
    ///     Seller name.
    /// </summary>
    public string SellerName { get; set; }
}
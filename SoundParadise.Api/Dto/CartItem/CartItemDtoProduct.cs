using SoundParadise.Api.Dto.Product;

namespace SoundParadise.Api.Dto.CartItem;

/// <summary>
///     CartItem dto product.
/// </summary>
public class CartItemDtoProduct : CartItemDto
{
    /// <summary>
    ///     Product.
    /// </summary>
    public ProductCartDto Product { get; set; } = new();
}
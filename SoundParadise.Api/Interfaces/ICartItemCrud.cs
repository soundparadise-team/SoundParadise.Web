using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Models.CartItem;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     CartItemCrud implements that interface.
/// </summary>
public interface ICartItemCrud
{
    bool CreateCartItem(CartItemModel cart);
    bool UpdateCartItem(CartItemModel cart);
    bool DeleteCartItem(Guid cartId);
    decimal GetTotalPriceOfOrder(IEnumerable<CartItemDto> cartItems);
    decimal GetTotalPriceOfOrderAuth(IEnumerable<CartItemModel> cartItems);
    List<CartItemModel> GetAllCartItems();
    CartItemModel? GetCartItemById(Guid id);
    CartItemModel GetCartItemByCartId(Guid cartId);
    List<CartItemModel> GetCartItemsByCartId(Guid? cartId);
}
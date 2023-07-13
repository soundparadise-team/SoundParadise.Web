using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.CartItem;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     CartCrud implements that interface.
/// </summary>
public interface ICartCrud
{
    bool CreateCart(CartModel cart);
    bool CartItemExists(Guid cartItemId, out Guid cartId);
    List<CartModel> GetAllCarts();
    List<CartItemDtoProduct> GetCartByUserId(Guid userId);
    CartModel GetCartById(Guid Id);
    Guid GetCartIdByUserId(Guid userId);
    List<CartItemModel> GetCartItems(Guid cartId);
    RequestResult AddCartItem(Guid cartId, Guid productId);
    bool UpdateCart(CartModel cart);
    RequestResult UpdateCartItemQuantity(Guid cartId, Guid cartItemId, short quantity);
    bool DeleteCart(Guid cartId);
    RequestResult RemoveCartItem(Guid cartId, Guid productId);
}
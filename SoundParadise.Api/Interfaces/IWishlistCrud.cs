using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Models.Wishlist;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     WishlistCrud implements that interface.
/// </summary>
public interface IWishlistCrud
{
    RequestResult CreateWishlist(WishlistModel wishlist);
    WishlistModel GetWishlistById(Guid id);
    List<ProductDto> GetWishlistByUserId(Guid userId);
    RequestResult AddToWishlist(Guid userId, Guid productId);
    RequestResult RemoveFromWishlist(Guid userId, Guid productId);
}
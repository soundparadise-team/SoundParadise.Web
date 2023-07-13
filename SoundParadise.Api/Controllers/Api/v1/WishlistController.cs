using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Wishlist;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Wishlist API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Wishlist.Base)]
public class WishlistController : BaseApiController
{
    private readonly IProductCrud _productCrud;
    private readonly IWishlistCrud _wishlistCrud;

    /// <summary>
    ///     Constructor for WishlistController v1
    /// </summary>
    /// <param name="wishlistCrud"></param>
    /// <param name="productCrud"></param>
    public WishlistController(IWishlistCrud wishlistCrud, IProductCrud productCrud)
    {
        _wishlistCrud = wishlistCrud;
        _productCrud = productCrud;
    }

    #region GET

    /// <summary>
    ///     Gets the wishlist of the authenticated user.
    /// </summary>
    /// <returns>Returns list of ProductDtos.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.Wishlist.GetWishlist)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The wishlist is successfully retrieved.", typeof(WishlistModel))]
    public IActionResult GetWishlist()
    {
        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);

        var wishlist = _wishlistCrud.GetWishlistByUserId(userId);

        return wishlist == null!
            ? NotFound(new { error = "The wishlist does not exist" })
            : Ok(wishlist);
    }

    #endregion

    #region PUT

    /// <summary>
    ///     Adds the product to the wishlist.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Product added to the wishlist.</returns>
    [Authorize]
    [HttpPut(ApiRoutes.Wishlist.AddToWishlist)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The product is successfully added to the wishlist.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "The product is already in the wishlist.", typeof(string))]
    public IActionResult AddToWishlist([FromQuery] Guid productId)
    {
        var product = _productCrud.GetProductById(productId);

        if (product == null!) return NotFound(new { error = "The product does not exist" });

        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null) return BadRequest(new { error = "Invalid user ID" });

        var userId = Guid.Parse(id);

        var result = _wishlistCrud.AddToWishlist(userId, productId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    /// <summary>
    ///     Removes the product from the wishlist.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Product removed from wishlist.</returns>
    [Authorize]
    [HttpPut(ApiRoutes.Wishlist.RemoveFromWishlist)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The product is successfully removed from the wishlist.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "The product is not in the wishlist.", typeof(string))]
    public IActionResult RemoveFromWishlist([FromQuery] Guid productId)
    {
        var product = _productCrud.GetProductById(productId);

        if (product == null!) return NotFound(new { error = "The product does not exist" });

        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null) return BadRequest(new { error = "Invalid user ID" });

        var userId = Guid.Parse(id);

        var result = _wishlistCrud.RemoveFromWishlist(userId, productId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion
}
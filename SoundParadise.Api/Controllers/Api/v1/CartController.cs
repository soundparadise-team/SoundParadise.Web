using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Extensions;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Validators.CartItem;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Cart API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Cart.Base)]
public class CartController : BaseApiController
{
    private readonly ICartCrud _cartCrud;
    private readonly CartItemDtoValidator _cartItemDtoValidator;
    private readonly IProductCrud _productCrud;
    private readonly IUserCrud _userCrud;

    /// <summary>
    ///     Constructor for CartController v1.
    /// </summary>
    /// <param name="cartCrud"></param>
    /// <param name="productCrud"></param>
    /// <param name="cartItemDtoValidator"></param>
    /// <param name="userCrud"></param>
    public CartController(ICartCrud cartCrud, IProductCrud productCrud, CartItemDtoValidator cartItemDtoValidator,
        IUserCrud userCrud)
    {
        _cartCrud = cartCrud;
        _productCrud = productCrud;
        _cartItemDtoValidator = cartItemDtoValidator;
        _userCrud = userCrud;
    }

    #region GET

    /// <summary>
    ///     Gets all carts.
    /// </summary>
    /// <returns>Returns all carts.</returns>
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    [HttpGet(ApiRoutes.GetAll)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The carts are successfully retrieved.", typeof(CartModel))]
    public IActionResult GetAllCarts()
    {
        var carts = _cartCrud.GetAllCarts();
        return StatusCode((int)HttpStatusCode.OK, carts);
    }

    /// <summary>
    ///     Gets the cart by id.
    /// </summary>
    /// <returns>Return CartItemDtoProduct.</returns>
    [HttpGet(ApiRoutes.Cart.GetCart)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The cart is successfully retrieved for user.",
        typeof(CartItemDtoProduct))]
    public IActionResult GetCart()
    {
        var cart = HttpContext.Session.Get<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var cartProducts = cart.Select(cartItem => new CartItemDtoProduct
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            Product = _productCrud.GetProductCartDtoById(cartItem.ProductId)
        }).ToList();

        return !cartProducts.Any()
            ? NotFound(new { error = "The cart is empty" })
            : Ok(cartProducts);
    }

    /// <summary>
    ///     Gets the cart by user id.
    /// </summary>
    /// <returns>Returns CartModel.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.Cart.GetCartAuth)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The cart is successfully retrieved for authenticated user.",
        typeof(CartModel))]
    public IActionResult GetCartAuth()
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return NotFound(new { error = "The user does not exist" });
        var userId = Guid.Parse(value);
        var cart = _cartCrud.GetCartByUserId(userId);
        return cart == null!
            ? NotFound(new { error = "The cart is empty" })
            : Ok(cart);
    }

    #endregion

    #region POST

    /// <summary>
    ///     Adds the product to the cart.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Product added to cart successfully.</returns>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Cart.AddToCart)]
    [SwaggerResponse((int)HttpStatusCode.Created, "The product is successfully added to cart.", typeof(CartItemDto))]
    public IActionResult AddToCart(Guid productId)
    {
        if (!_productCrud.ProductExists(productId))
            return StatusCode((int)HttpStatusCode.NotFound, "The product does not exist");

        var cartItems = HttpContext.Session.Get<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var existingCartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (existingCartItem != null)
        {
            var newQuantity = (short)(existingCartItem.Quantity + 1);
            if (!_productCrud.IsQuantityValid(productId, newQuantity))
                return BadRequest(new { error = "The quantity is not valid" });

            existingCartItem.Quantity = newQuantity;
        }
        else
        {
            var newCartItem = new CartItemDto
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = 1
            };

            cartItems.Add(newCartItem);
        }

        HttpContext.Session.Set("Cart", cartItems);

        return Created("Product added to cart successfully", cartItems);
    }

    /// <summary>
    ///     Adds a product to the cart by authenticated user.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Product added successfully.</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Cart.AddToCartAuth)]
    [SwaggerResponse((int)HttpStatusCode.Created, "The product is successfully added to cart of authenticated user.",
        typeof(CartItemDto))]
    public IActionResult AddToCartAuth(Guid productId)
    {
        if (!_productCrud.ProductExists(productId))
            return NotFound(new { error = "The product does not exist" });

        var userClaims = HttpContext.User;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartId = _cartCrud.GetCartIdByUserId(Guid.Parse(userId));
        var result = _cartCrud.AddCartItem(cartId, productId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { message = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region PUT

    /// <summary>
    ///     Updates the quantity of a cart item.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <returns>Cart item quantity updated successfully.</returns>
    [HttpPut(ApiRoutes.Cart.UpdateCartItemQuantity)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Item quantity updated successfully.")]
    public IActionResult UpdateCartItemQuantity(Guid productId, short quantity = 1)
    {
        var cart = HttpContext.Session.Get<List<CartItemDto>>("Cart");
        if (cart == null! || cart.Count == 0)
            return NotFound(new { error = "The cart does not exist or is empty" });

        var itemToUpdate = cart.FirstOrDefault(c => c.ProductId == productId);
        if (itemToUpdate == null)
            return NotFound(new { error = "The cart item does not exist" });

        if (quantity <= 0)
        {
            cart.Remove(itemToUpdate);
            HttpContext.Session.Set("Cart", cart);
            return Ok(new { success = "Cart item removed successfully" });
        }

        if (!_productCrud.IsQuantityValid(productId, quantity))
            return BadRequest(new { error = "The quantity is not valid" });

        itemToUpdate.Quantity = quantity;
        HttpContext.Session.Set("Cart", cart);
        return Ok(new { success = "Cart quantity updated successfully" });
    }


    /// <summary>
    ///     Updates the quantity of a cart item by authenticated user.
    /// </summary>
    /// <param name="cartItemId"></param>
    /// <param name="quantity"></param>
    /// <returns>Cart item quantity updated successfully.</returns>
    [Authorize]
    [HttpPut(ApiRoutes.Cart.UpdateCartItemQuantityAuth)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Item quantity updated successfully by authenticated user.")]
    public IActionResult UpdateCartItemQuantityAuth(Guid cartItemId, short quantity = 1)
    {
        if (!_cartCrud.CartItemExists(cartItemId, out var cartId))
            return NotFound(new { error = "Cart item does not exist" });

        var result = _cartCrud.UpdateCartItemQuantity(cartId, cartItemId, quantity);
        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Removes a cart item from the cart.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Item removed from cart successfully.</returns>
    [HttpDelete(ApiRoutes.Cart.RemoveFromCart)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Item removed from cart successfully by user.")]
    public IActionResult RemoveFromCart(Guid productId)
    {
        var cart = HttpContext.Session.Get<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);

        if (itemToRemove == null) return BadRequest(new { error = "Item not found in cart" });
        cart.Remove(itemToRemove);
        HttpContext.Session.Set("Cart", cart);
        return Ok(new { success = "Item removed from cart successfully" });
    }

    /// <summary>
    ///     Removes a cart item from the cart.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Item removed from cart successfully.</returns>
    [Authorize]
    [HttpDelete(ApiRoutes.Cart.RemoveFromCartAuth)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Item removed from cart successfully by authenticated user.")]
    public IActionResult RemoveFromCartAuth(Guid productId)
    {
        if (!_productCrud.ProductExists(productId))
            return BadRequest(new { message = "Product does not exist" });

        var userClaims = HttpContext.User;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cartId = _cartCrud.GetCartIdByUserId(Guid.Parse(userId));
        var result = _cartCrud.RemoveCartItem(cartId, productId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { message = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion
}
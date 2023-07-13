using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Models.Cart;

public class CartCrud : ICartCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILoggingService<CartCrud> _loggingService;
    private readonly IProductCrud _productCrud;

    /// <summary>
    ///     CartCrud constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    /// <param name="imageService">Service for image.</param>
    /// <param name="productCrud">Product Crud.</param>
    public CartCrud(SoundParadiseDbContext context, ILoggingService<CartCrud> loggingService, IProductCrud productCrud,
        ImageService imageService)
    {
        _context = context;
        _loggingService = loggingService;
        _productCrud = productCrud;
        _imageService = imageService;
    }

    #region CREATE

    /// <summary>
    ///     Add cart to data base.
    /// </summary>
    /// <param name="cart">CartModel object.</param>
    /// <returns>True if add, false if not</returns>
    public bool CreateCart(CartModel cart)
    {
        try
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
            return !_context.Carts.Any(u => u.Id == cart.Id);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while creating cart in {nameof(CartCrud)}.{nameof(CreateCart)}");
            return false;
        }
    }

    #endregion

    #region HELPERS

    /// <summary>
    ///     Check CartItem exists.
    /// </summary>
    /// <param name="cartItemId">CartItem Id.</param>
    /// <returns>True if exsists, false if not</returns>
    public bool CartItemExists(Guid cartItemId, out Guid cartId)
    {
        var cartItem = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        cartId = cartItem?.CartId ?? Guid.Empty;
        return cartItem != null;
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all carts.
    /// </summary>
    /// <returns>List of CartModel objects</returns>
    public List<CartModel> GetAllCarts()
    {
        try
        {
            var carts = _context.Carts.ToList();
            return carts;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting all carts in {nameof(CartCrud)}.{nameof(GetAllCarts)}");
            return Enumerable.Empty<CartModel>().ToList();
        }
    }

    /// <summary>
    ///     Get cart by user Id.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <returns>List of CartItemDtoProduct objects</returns>
    public List<CartItemDtoProduct> GetCartByUserId(Guid userId)
    {
        try
        {
            var cartItems = _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(cartItem => cartItem.Cart!.UserId == userId)
                .Select(cartItem => new CartItemDtoProduct
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Product = new ProductCartDto
                    {
                        Id = cartItem.ProductId,
                        Name = cartItem.Product!.Name,
                        Price = cartItem.Product.Price,
                        ImagePath = _imageService.GetImageUrl(cartItem.Product.Images.FirstOrDefault().Path) ??
                                    string.Empty
                    }
                })
                .ToList();
            return !cartItems.Any()
                ? Enumerable.Empty<CartItemDtoProduct>().ToList()
                : cartItems;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting cart by user ID in {nameof(CartCrud)}.{nameof(GetCartByUserId)}");
            return Enumerable.Empty<CartItemDtoProduct>().ToList();
        }
    }

    /// <summary>
    ///     Get cart by Id.
    /// </summary>
    /// <param name="Id">Cart Id.</param>
    /// <returns>List of CartModel objects</returns>
    public CartModel GetCartById(Guid Id)
    {
        try
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.Id == Id);
            return (cart ?? null)!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting cart by ID in {nameof(CartCrud)}.{nameof(GetCartById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get cart Id by user Id.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <returns>Cart Id</returns>
    public Guid GetCartIdByUserId(Guid userId)
    {
        try
        {
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            return cart?.Id ?? Guid.Empty;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting cart ID in {nameof(CartCrud)}.{nameof(GetCartIdByUserId)}");
            return Guid.Empty;
        }
    }

    /// <summary>
    ///     Get CartItems by cart Id.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>List of CartItemModel objects</returns>
    public List<CartItemModel> GetCartItems(Guid cartId)
    {
        try
        {
            var cart = _context.Carts.Find(cartId);
            return cart?.CartItems ?? Enumerable.Empty<CartItemModel>().ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting cart items in {nameof(CartCrud)}.{nameof(GetCartItems)}");
            return Enumerable.Empty<CartItemModel>().ToList();
        }
    }

    /// <summary>
    ///     Get cart Id by user Id.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>List of CartItemModel objects</returns>
    public RequestResult AddCartItem(Guid cartId, Guid productId)
    {
        try
        {
            var cart = GetCartById(cartId);
            if (cart == null!)
                return RequestResult.Error($"Cart not found with ID {cartId}", HttpStatusCode.NotFound);

            if (cart.CartItems != null)
            {
                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (existingCartItem != null)
                {
                    var newQuantity = (short)(existingCartItem.Quantity + 1);
                    if (!_productCrud.IsQuantityValid(productId, newQuantity))
                        return RequestResult.Error("Not enough stock available for this product",
                            HttpStatusCode.NotFound);
                    existingCartItem.Quantity = newQuantity;
                }
                else
                {
                    var cartItem = new CartItemModel
                    {
                        ProductId = productId,
                        Quantity = 1
                    };

                    cart.CartItems.Add(cartItem);
                }
            }

            _context.SaveChanges();

            return RequestResult.Success("Item added to cart successfully");
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while adding item to cart {nameof(CartCrud)}.{nameof(AddCartItem)}");
            return RequestResult.Error("An error occurred while adding item to cart",
                HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region UPDATE

    /// <summary>
    ///     Update Cart in data base.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>List of CartItemModel objects</returns>
    public bool UpdateCart(CartModel cart)
    {
        try
        {
            _context.Carts.Update(cart);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while updating cart in {nameof(CartCrud)}.{nameof(UpdateCart)}");
            return false;
        }
    }

    /// <summary>
    ///     Update CartItem quantity in data base.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <param name="cartItemId">CartItem Id.</param>
    /// <param name="quantity">Quantity</param>
    /// <returns>List of CartItemModel objects</returns>
    public RequestResult UpdateCartItemQuantity(Guid cartId, Guid cartItemId, short quantity)
    {
        try
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .SingleOrDefault(c => c.Id == cartId);

            if (cart == null)
                return RequestResult.Error($"Cart not found with ID: {cartId}.", HttpStatusCode.NotFound);

            if (cart.CartItems == null)
                return RequestResult.Error($"Cart items not found with ID: {cartId}.", HttpStatusCode.NotFound);

            var existingCartItem = cart.CartItems
                .SingleOrDefault(ci => ci.Id == cartItemId && ci.CartId == cartId);

            if (existingCartItem == null)
                return RequestResult.Error($"Cart not found with ID: {cartId}", HttpStatusCode.NotFound);

            if (quantity <= 0)
            {
                cart.CartItems.Remove(existingCartItem);
                _context.Remove(existingCartItem);
            }
            else
            {
                if (!_productCrud.IsQuantityValid(existingCartItem.ProductId, quantity))
                    return RequestResult.Error("Not enough products in stock for this quantity.",
                        HttpStatusCode.NotFound);

                existingCartItem.Quantity = quantity;
                _context.Update(existingCartItem);
            }

            _context.SaveChanges();
            return RequestResult.Success("Cart item quantity updated successfully.");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while updating cart item quantity.";
            _loggingService.LogException(ex, $"{error} in {nameof(CartCrud)}.{nameof(UpdateCartItemQuantity)}");
            return RequestResult.Error("An error occurred while updating cart item quantity",
                HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete Cart from data base.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>True if delete, false if error.</returns>
    public bool DeleteCart(Guid cartId)
    {
        try
        {
            var cart = _context.Carts.Find(cartId);
            if (cart == null)
                return false;
            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return !_context.Carts.Any(u => u.Id == cartId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while deleting a Cart in {nameof(CartCrud)}.{nameof(DeleteCart)})");
            return false;
        }
    }

    /// <summary>
    ///     Delete Cart from data base.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <param name="productId">Product Id.</param>
    /// <returns>RequestResult object.</returns>
    public RequestResult RemoveCartItem(Guid cartId, Guid productId)
    {
        try
        {
            var cart = GetCartById(cartId);
            if (cart == null)
                return RequestResult.Error($"Cart not found with ID {cartId}.", HttpStatusCode.NotFound);

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingCartItem == null)
                return RequestResult.Error("Item not found in cart.", HttpStatusCode.NotFound);
            var newQuantity = (short)(existingCartItem.Quantity - 1);
            if (!_productCrud.IsQuantityValid(productId, newQuantity))
                return RequestResult.Error("Not enough stock available for this product.", HttpStatusCode.NotFound);

            if (newQuantity == 0)
                cart.CartItems.Remove(existingCartItem);
            else
                existingCartItem.Quantity = newQuantity;

            _context.SaveChanges();

            return RequestResult.Success("Item removed from cart successfully.");
        }
        catch (Exception ex)
        {
            var error = "An error occurred while removing item from cart";
            _loggingService.LogException
                (ex, $"{error} in {nameof(CartCrud)}.{nameof(RemoveCartItem)}");
            return RequestResult.Error(error,
                HttpStatusCode.InternalServerError);
        }
    }

    #endregion
}
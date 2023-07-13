using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Interfaces;

namespace SoundParadise.Api.Models.CartItem;

/// <summary>
///     CartItemModel CRUD.
/// </summary>
public class CartItemCrud : ICartItemCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<CartItemCrud> _loggingService;
    private readonly IProductCrud _productCrud;

    /// <summary>
    ///     CartItemCrud constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    public CartItemCrud(SoundParadiseDbContext context, ILoggingService<CartItemCrud> loggingService,
        IProductCrud productCrud)
    {
        _context = context;
        _loggingService = loggingService;
        _productCrud = productCrud;
    }

    #region CREATE

    /// <summary>
    ///     Add new CartItem to data base.
    /// </summary>
    /// <param name="cart">Db context.</param>
    /// <returns>True if add, false if error</returns>
    public bool CreateCartItem(CartItemModel cart)
    {
        try
        {
            _context.CartItems.Add(cart);
            _context.SaveChanges();
            return !_context.CartItems.Any(u => u.Id == cart.Id);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while creating cart in {nameof(CartItemCrud)}.{nameof(CreateCartItem)}");
            return false;
        }
    }

    #endregion

    #region UPDATE

    /// <summary>
    ///     Update cartitem in data base.
    /// </summary>
    /// <param name="cart">CartModel object.</param>
    /// <returns>True if update, false if not</returns>
    public bool UpdateCartItem(CartItemModel cart)
    {
        try
        {
            _context.CartItems.Update(cart);
            _context.SaveChanges();
            return !_context.Carts.Any(u => u.Id == cart.Id);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while updating cart in {nameof(CartItemCrud)}.{nameof(UpdateCartItem)}");
            return false;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete cartitem from data base.
    /// </summary>
    /// <param name="cartId">CartItem Id.</param>
    /// <returns>True if delete, false if not</returns>
    public bool DeleteCartItem(Guid cartId)
    {
        try
        {
            var cart = _context.CartItems.Find(cartId);
            if (cart == null)
                return false;

            _context.CartItems.Remove(cart);

            _context.SaveChanges();
            return !_context.CartItems.Any(u => u.Id == cartId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while deleting cart in {nameof(CartItemCrud)}.{nameof(DeleteCartItem)}");
            return false;
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get total price of order.
    /// </summary>
    /// <param name="cartItems">CartItemDto object.</param>
    /// <returns>Total price</returns>
    public decimal GetTotalPriceOfOrder(IEnumerable<CartItemDto> cartItems)
    {
        return (from cartItem in cartItems
            let product = _productCrud.GetProductById(cartItem.ProductId)
            where product != null
            select product.Price * cartItem.Quantity).Sum();
    }

    /// <summary>
    ///     Get total price of order auth.
    /// </summary>
    /// <param name="cartItems">CartItemDto object.</param>
    /// <returns>Total price</returns>
    public decimal GetTotalPriceOfOrderAuth(IEnumerable<CartItemModel> cartItems)
    {
        return (from cartItem in cartItems
            let product = _productCrud.GetProductById(cartItem.ProductId)
            where product != null
            select product.Price * cartItem.Quantity).Sum();
    }

    /// <summary>
    ///     Get all cartitems.
    /// </summary>
    /// <returns>List of CartItem models.</returns>
    public List<CartItemModel> GetAllCartItems()
    {
        try
        {
            var carts = _context.CartItems.ToList();
            return carts;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting all carts in {nameof(CartItemCrud)}.{nameof(GetAllCartItems)}");
            return Enumerable.Empty<CartItemModel>().ToList();
        }
    }

    /// <summary>
    ///     Get CartItem by Id.
    /// </summary>
    /// <param name="id">CartItem Id.</param>
    /// <returns>CartItem model.</returns>
    public CartItemModel? GetCartItemById(Guid id)
    {
        try
        {
            var cart = _context.CartItems.Find(id);
            if (cart == null)
                _loggingService.LogError($"CartItem not found with ID: {id} in CartItemCrud.GetCartItemById");
            return cart;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting cart in {nameof(CartItemCrud)}.{nameof(GetCartItemById)}");
            return null;
        }
    }

    /// <summary>
    ///     Get CartItem by Cart Id.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>CartItem model.</returns>
    public CartItemModel GetCartItemByCartId(Guid cartId)
    {
        try
        {
            var cart = _context.CartItems.Include(p => p.Cart).FirstOrDefault(j => j.CartId == cartId);
            return cart ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting cart in {nameof(CartItemCrud)}.{nameof(GetCartItemByCartId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get CartItems by Cart Id.
    /// </summary>
    /// <param name="cartId">Cart Id.</param>
    /// <returns>List of CartItem model.</returns>
    public List<CartItemModel> GetCartItemsByCartId(Guid? cartId)
    {
        try
        {
            if (cartId == null)
                return Enumerable.Empty<CartItemModel>().ToList();

            var cartItems = _context.CartItems.Where(j => j.CartId == cartId).ToList();
            return cartItems;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting cart in {nameof(CartItemCrud)}.{nameof(GetCartItemsByCartId)}");
            return Enumerable.Empty<CartItemModel>().ToList();
        }
    }

    #endregion
}
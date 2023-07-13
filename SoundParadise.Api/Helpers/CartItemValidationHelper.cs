using SoundParadise.Api.Data;

namespace SoundParadise.Api.Helpers;

/// <summary>
///     CartItem validator service helper.
/// </summary>
public static class CartItemValidationHelper
{
    /// <summary>
    ///     Is Product exists.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <param name="context">Db context.</param>
    /// <returns>True if exists, false if not.</returns>
    public static bool IsProductExists(Guid productId, SoundParadiseDbContext context)
    {
        return context.Products.Any(p => p.Id == productId);
    }

    /// <summary>
    ///     Is can be added to cart.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <param name="quantity">Quantity.</param>
    /// <param name="context"></param>
    /// <returns>True if can be added, false if not.</returns>
    public static bool IsCanBeAddedToCart(Guid productId, short quantity, SoundParadiseDbContext context)
    {
        var product = context.Products.Find(productId);
        if (product == null)
            return false;

        return product.Quantity >= quantity;
    }
}
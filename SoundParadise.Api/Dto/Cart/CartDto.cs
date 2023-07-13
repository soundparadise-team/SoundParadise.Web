using SoundParadise.Api.Dto.CartItem;

namespace SoundParadise.Web.Dto.Cart;

/// <summary>
///     Cart dto.
/// </summary>
public class CartDto
{
    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     List of cartitemobjects.
    /// </summary>
    public List<CartItemDto> CartItems { get; set; } = new();
}
using Newtonsoft.Json;

namespace SoundParadise.Api.Dto.CartItem;

/// <summary>
///     CartItem dto.
/// </summary>
public class CartItemDto
{
    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     Product Id.
    /// </summary>
    [JsonIgnore]
    public Guid ProductId { get; set; }

    /// <summary>
    ///     Quantity.
    /// </summary>
    public short Quantity { get; set; }
}
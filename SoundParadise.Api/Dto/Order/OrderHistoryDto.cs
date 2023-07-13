using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Models.Enums;

namespace SoundParadise.Api.Dto.Order;

/// <summary>
///     Order history dto.
/// </summary>
public class OrderHistoryDto
{
    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Order date.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    ///     Total price.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    ///     Order status enum.
    /// </summary>
    public OrderStatusEnum OrderStatus { get; set; }

    public string? CheckoutUrl { get; set; }

    /// <summary>
    ///     List of order items.
    /// </summary>
    public List<OrderItemDtoProduct>? OrderItems { get; set; }
}
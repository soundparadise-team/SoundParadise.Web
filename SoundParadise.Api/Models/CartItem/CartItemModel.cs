using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Models.CartItem;

/// <summary>
///     CartItem data model.
/// </summary>
public class CartItemModel
{
    /// <summary>
    ///     CartItem Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("cart_item_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Product Id.
    /// </summary>
    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    ///     Product navigation property.
    /// </summary>
    public ProductModel? Product { get; set; }

    /// <summary>
    ///     Quantity.
    /// </summary>
    [Column("quantity")]
    public short Quantity { get; set; }

    /// <summary>
    ///     Cart Id.
    /// </summary>
    [Column("cart_id")]
    public Guid? CartId { get; set; }

    /// <summary>
    ///     Cart navigation property.
    /// </summary>
    [JsonIgnore]
    public CartModel? Cart { get; set; }

    /// <summary>
    ///     Order Id.
    /// </summary>
    [Column("order_id")]
    public Guid? OrderId { get; set; }

    /// <summary>
    ///     Order navigation property.
    /// </summary>
    [JsonIgnore]
    public OrderModel? Order { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Cart;

/// <summary>
///     Cart data model.
/// </summary>
public class CartModel
{
    /// <summary>
    ///     Cart Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("cart_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     User Id.
    /// </summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    ///     User navigation property.
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }

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

    /// <summary>
    ///     CartItems navigation property.
    /// </summary>
    public List<CartItemModel>? CartItems { get; set; } = new();
}
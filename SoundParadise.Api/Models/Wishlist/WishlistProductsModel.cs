using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Models.Wishlist;

/// <summary>
///     WhislistProuct data model
/// </summary>
public class WishlistProductsModel
{
    /// <summary>
    ///     WishlistProducts Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("wishlist_products_id")]

    public Guid Id { get; set; }

    /// <summary>
    ///     Wishlist's Id
    /// </summary>
    [Column("wishlists_id")]
    public Guid WishlistId { get; set; }

    /// <summary>
    ///     Wishlist's navigation property
    /// </summary>
    [JsonIgnore]
    public WishlistModel? Wishlist { get; set; }

    /// <summary>
    ///     Product's Id
    /// </summary>
    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    ///     Product's navigation property
    /// </summary>
    public ProductModel? Product { get; set; }
}
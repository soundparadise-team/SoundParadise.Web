using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Wishlist;

/// <summary>
///     Whislist data model
/// </summary>
public class WishlistModel
{
    /// <summary>
    ///     Whislist's Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("wishlist_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Navigation property for whislist products
    /// </summary>
    public List<WishlistProductsModel>? WishlistProducts { get; set; } = new();

    /// <summary>
    ///     User Id
    /// </summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    ///     User navigation property
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }
}
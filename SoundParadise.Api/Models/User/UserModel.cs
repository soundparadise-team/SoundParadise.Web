using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.Card;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Models.TokenJournal;
using SoundParadise.Api.Models.Wishlist;

namespace SoundParadise.Api.Models.User;

/// <summary>
///     User data model
/// </summary>
public class UserModel
{
    /// <summary>
    ///     User ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("user_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     User name
    /// </summary>
    [Column("username")]
    public string Username { get; set; }

    /// <summary>
    ///     Real name of user
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Real surname of user
    /// </summary>
    [Column("surname")]
    public string Surname { get; set; }

    /// <summary>
    ///     User email, need to confirm account
    /// </summary>
    [Column("email")]
    public string Email { get; set; }

    /// <summary>
    ///     User role, can be admin, seller, default user
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [Column("role")]
    public UserRoleEnum Role { get; set; }

    /// <summary>
    ///     Phone number, need to contact with user
    /// </summary>
    [Column("phone_number")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numeric characters.")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///     Password after hashing
    /// </summary>
    [Column("password_hash")]
    public byte[] PasswordHash { get; set; }

    /// <summary>
    ///     Salt for password hashing
    /// </summary>
    [Column("password_salt")]
    public byte[] PasswordSalt { get; set; }

    /// <summary>
    ///     Navigation property for products the user owns
    /// </summary>
    [JsonIgnore]
    public List<ProductModel>? Products { get; set; }

    /// <summary>
    ///     User's cart id
    /// </summary>
    [Column("cart_id")]
    public Guid CartId { get; set; }

    /// <summary>
    ///     User's cart navigation property
    /// </summary>
    [JsonIgnore]
    public CartModel? Cart { get; set; }

    /// <summary>
    ///     User's wishlist id
    /// </summary>
    [Column("wishlist_id")]
    public Guid WishlistId { get; set; }

    /// <summary>
    ///     User's wishlist navigation property
    /// </summary>
    [JsonIgnore]
    public WishlistModel? Wishlist { get; set; }

    [JsonIgnore] public ImageModel? Image { get; set; }

    public Guid? ImageId { get; set; }

    /// <summary>
    ///     Token that is issued after confirmation of mail
    /// </summary>
    [Column("confirmation_token")]
    public string? ConfirmationToken { get; set; }

    /// <summary>
    ///     �onfirmation status
    /// </summary>
    [Column("is_confirmed")]
    public bool IsConfirmed { get; set; }

    /// <summary>
    ///     Navigation property for user's comments
    /// </summary>
    [JsonIgnore]
    public List<CommentModel>? Comments { get; set; }

    /// <summary>
    ///     Tokens that were issued and are issued to the user
    /// </summary>
    [JsonIgnore]
    public List<TokenJournalModel>? TokenJournals { get; set; } = new();

    /// <summary>
    ///     Orders that belong to the user
    /// </summary>
    [JsonIgnore]
    public List<OrderModel>? Orders { get; set; }

    /// <summary>
    ///     Address where user orders are delivered
    /// </summary>
    [JsonIgnore]
    public List<AddressModel>? DeliveryAddresses { get; set; }

    /// <summary>
    ///     �ards for online payment
    /// </summary>
    [JsonIgnore]
    public List<CardModel>? Cards { get; set; }

    public void Confirm()
    {
        ConfirmationToken = null;
        IsConfirmed = true;
    }
}
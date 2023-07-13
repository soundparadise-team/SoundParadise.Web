using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Card;

/// <summary>
///     Card data model.
/// </summary>
public class CardModel
{
    /// <summary>
    ///     Card Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("card_id")]
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
    public UserModel User { get; set; }

    /// <summary>
    ///     Encrypted card number.
    /// </summary>
    [Column("encrypted_card_number")]
    public byte[] EncryptedCardNumber { get; set; }

    /// <summary>
    ///     Encrypted expiry date.
    /// </summary>
    [Column("encrypted_expiry_date")]
    public byte[] EncryptedExpiryDate { get; set; }

    /// <summary>
    ///     Encrypted cvv.
    /// </summary>
    [Column("encrypted_cvv")]
    public byte[] EncryptedCVV { get; set; }
}
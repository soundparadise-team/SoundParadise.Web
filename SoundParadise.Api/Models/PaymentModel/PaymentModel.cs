using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Order;

namespace SoundParadise.Api.Models.PaymentDetails;

/// <summary>
///     Payment data model.
/// </summary>
public class PaymentModel
{
    /// <summary>
    ///     Payment Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("payment_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Transaction Id.
    /// </summary>
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    /// <summary>
    ///     Order Id.
    /// </summary>
    [Column("order_id")]
    public Guid OrderId { get; set; }

    /// <summary>
    ///     Order navigation property.
    /// </summary>
    [JsonIgnore]
    public OrderModel Order { get; set; }

    /// <summary>
    ///     Masked card string.
    /// </summary>
    [Column("masked_card_number")]
    public string MaskedCard { get; set; }

    /// <summary>
    ///     Card Type.
    /// </summary>
    [Column("card_type")]
    public string CardType { get; set; }

    /// <summary>
    ///     Amount.
    /// </summary>
    [Column("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Currency.
    /// </summary>
    [Column("currency")]
    public string Currency { get; set; }
}
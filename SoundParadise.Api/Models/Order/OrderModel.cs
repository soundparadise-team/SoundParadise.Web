using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.PaymentDetails;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Order;

/// <summary>
///     Order data model.
/// </summary>
public class OrderModel
{
    private string _customerName;

    private string _customerSurname;

    /// <summary>
    ///     Order Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("order_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Customer name.
    /// </summary>
    [Column("customer_name")]
    public string CustomerName
    {
        get => _customerName.Transform(To.LowerCase, To.TitleCase);
        set => _customerName = value;
    }

    /// <summary>
    ///     Customer Surname.
    /// </summary>
    [Column("customer_surname")]
    public string CustomerSurname
    {
        get => _customerSurname.Transform(To.LowerCase, To.TitleCase);
        set => _customerSurname = value;
    }

    /// <summary>
    ///     Customer phone number.
    /// </summary>
    [Column("customer_phone_number")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numeric characters.")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///     User Id.
    /// </summary>
    [Column("user_id")]
    public Guid? UserId { get; set; }

    /// <summary>
    ///     User navigation property.
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }

    /// <summary>
    ///     Navigation property for List CartItemModel.
    /// </summary>
    [JsonIgnore]
    public List<CartItemModel>? CartItems { get; set; }

    /// <summary>
    ///     Delivery Address Id.
    /// </summary>
    [Column("delivery_address_id")]
    public Guid DeliveryAddressId { get; set; }

    /// <summary>
    ///     Delivery Address navigation property.
    /// </summary>
    [JsonIgnore]
    public AddressModel Address { get; set; }

    /// <summary>
    ///     Delivery Enum type.
    /// </summary>
    [Column("delivery_type")]
    public DeliveryTypeEnum DeliveryType { get; set; }

    /// <summary>
    ///     Delivery Enum option.
    /// </summary>
    [Column("delivery_option")]
    public DeliveryOptionEnum DeliveryOption { get; set; }

    /// <summary>
    ///     Payment Enum type.
    /// </summary>
    [Column("payment_type")]
    public PaymentTypeEnum PaymentType { get; set; }

    /// <summary>
    ///     Payment Id.
    /// </summary>
    [Column("payment_id")]
    public Guid? PaymentId { get; set; }

    /// <summary>
    ///     Payment navigation property.
    /// </summary>
    [JsonIgnore]
    public PaymentModel? Payment { get; set; }

    /// <summary>
    ///     Is paid status.
    /// </summary>
    [Column("is_paid")]
    public bool IsPaid { get; set; }

    /// <summary>
    ///     Comment.
    /// </summary>
    [Column("comment")]
    public string? Comment { get; set; }

    [Column("checkout_url")] public string? CheckoutUrl { get; set; }

    /// <summary>
    ///     Order status enum.
    /// </summary>
    [Column("order_status")]
    public OrderStatusEnum OrderStatus { get; set; }

    /// <summary>
    ///     Total price.
    /// </summary>
    [Column("total_price")]
    public decimal TotalPrice { get; set; }

    /// <summary>
    ///     Order date.
    /// </summary>
    [Column("order_date")]
    public DateTime OrderDate { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Web.Dto.Address;

namespace SoundParadise.Api.Dto.Order;

/// <summary>
///     Order dto.
/// </summary>
public class OrderDto
{
    private string _customerName = string.Empty;
    private string _customerSurname = string.Empty;

    /// <summary>
    ///     Customer name.
    /// </summary>
    public string CustomerName
    {
        get => _customerName.Transform(To.LowerCase, To.TitleCase);
        set => _customerName = value;
    }

    /// <summary>
    ///     Customer surname.
    /// </summary>
    public string CustomerSurname
    {
        get => _customerSurname.Transform(To.LowerCase, To.TitleCase);
        set => _customerSurname = value;
    }

    /// <summary>
    ///     Phone number.
    /// </summary>
    [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numeric characters.")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///     User Id.
    /// </summary>
    [JsonIgnore]
    public Guid? UserId { get; set; }

    /// <summary>
    ///     Addres Id.
    /// </summary>
    public Guid? AddressId { get; set; }

    /// <summary>
    ///     Address navigation property.
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    ///     Post office address.
    /// </summary>
    public string? PostOfficeAddress { get; set; }

    /// <summary>
    ///     City.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    ///     Delivery option enum.
    /// </summary>

    [JsonConverter(typeof(StringEnumConverter))]
    public DeliveryOptionEnum DeliveryOption { get; set; }

    /// <summary>
    ///     Delivery type enum.
    /// </summary>

    [JsonConverter(typeof(StringEnumConverter))]

    public DeliveryTypeEnum DeliveryType { get; set; }

    /// <summary>
    ///     Payment type enum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]

    public PaymentTypeEnum PaymentType { get; set; }

    /// <summary>
    ///     Comment
    /// </summary>
    public string? Comment { get; set; }

    public string? CheckoutUrl { get; set; }

    public OrderStatusEnum? OrderStatus { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal? TotalPrice { get; set; }

    public List<OrderItemDtoProduct>? OrderItems { get; set; }
}
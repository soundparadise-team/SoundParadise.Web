using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Order;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Address;

/// <summary>
///     Address data model.
/// </summary>
public class AddressModel
{
    /// <summary>
    ///     Address Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("address_id")]
    public Guid Id { get; set; }

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

    // [Column("street")] public string? Street { get; set; }
    /// <summary>
    ///     City.
    /// </summary>
    [Column("city")]
    public string? City { get; set; }

    // [Column("country")] public string? Country { get; set; }

    // [Column("house")] public string? House { get; set; }

    // [Column("apartment")] public string? Apartment { get; set; }

    // [Column("floor")] public string? Floor { get; set; }

    // [Column("postal_code")] public string? PostalCode { get; set; }
    // [Column("street")] public string? Street { get; set; }
    /// <summary>
    ///     PostOfficeAddress.
    /// </summary>
    [Column("post_office_address")]
    public string? PostOfficeAddress { get; set; }

    /// <summary>
    ///     DeliveryOption enum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [Column("delivery_option")]
    public DeliveryOptionEnum DeliveryOption { get; set; }

    /// <summary>
    ///     Order navigation property.
    /// </summary>
    [JsonIgnore]
    public List<OrderModel>? Orders { get; set; }
}
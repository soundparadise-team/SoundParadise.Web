using Humanizer;
using SoundParadise.Api.Models.Enums;

namespace SoundParadise.Web.Dto.Address;

/// <summary>
///     Address Dto.
/// </summary>
public class AddressDto
{
    private string _city = string.Empty;

    private string _postOfficeAddress = string.Empty;

    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Post office address.
    /// </summary>
    public string PostOfficeAddress
    {
        get => _postOfficeAddress.Transform(To.LowerCase, To.TitleCase);
        set => _postOfficeAddress = value;
    }

    /// <summary>
    ///     City.
    /// </summary>
    public string City
    {
        get => _city.Transform(To.LowerCase, To.TitleCase);
        set => _city = value;
    }

    /// <summary>
    ///     Delivery type enum.
    /// </summary>
    public DeliveryTypeEnum DeliveryType { get; set; }

    /// <summary>
    ///     Delivery option enum.
    /// </summary>
    public DeliveryOptionEnum DeliveryOption { get; set; }
}
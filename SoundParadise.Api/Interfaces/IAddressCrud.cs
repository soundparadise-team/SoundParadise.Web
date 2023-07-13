using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Web.Dto.Address;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     AddressCrud implements that interface.
/// </summary>
public interface IAddressCrud
{
    RequestResult AddAddress(AddressDto addressDto, Guid userId);
    RequestResult CreateAddress(AddressModel address);
    AddressModel GetAddressById(Guid? addressId, Guid? userId);
    AddressDto GetAddressDtoById(Guid? addressId, Guid? userId);

    List<AddressDto> GetAddressesByUserId(Guid userId);
    List<AddressDto> GetAddressesByDeliveryOption(DeliveryOptionEnum deliveryOption, Guid userId);

    RequestResult DeleteAddress(Guid addressId, Guid userId);
}
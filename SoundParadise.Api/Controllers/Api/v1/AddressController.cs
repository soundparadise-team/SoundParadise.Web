using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Web.Dto.Address;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Address API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Address.Base)]
public class AddressController : BaseApiController
{
    private readonly IAddressCrud _addressCrud;

    /// <summary>
    ///     Constructor for AddressController v1.
    /// </summary>
    /// <param name="addressCrud"></param>
    public AddressController(IAddressCrud addressCrud)
    {
        _addressCrud = addressCrud;
    }

    #region POST

    /// <summary>
    ///     This method is used to create an order for an unauthenticated user.
    /// </summary>
    /// <param name="address"></param>
    /// <returns>Successfully added address</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Address.AddAddress)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the message that the address was successfully added",
        typeof(string))]
    public IActionResult AddAddress(AddressDto address)
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });

        var userId = Guid.Parse(value);
        var result = _addressCrud.AddAddress(address, userId);

        return result.IsSuccess
            ? Ok(new { message = result.Message })
            : BadRequest(new { error = result.Message });
    }

    #endregion

    #region DELETE

    [Authorize]
    [HttpDelete(ApiRoutes.Address.DeleteAddress)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the message that the address was successfully deleted",
        typeof(string))]
    public IActionResult DeleteAddress(Guid address)
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });

        var userId = Guid.Parse(value);
        var result = _addressCrud.DeleteAddress(address, userId);

        return result.IsSuccess
            ? Ok(new { message = result.Message })
            : BadRequest(new { error = result.Message });
    }

    #endregion

    #region GET

    /// <summary>
    ///     This method is used to get addresses of the authenticated user.
    /// </summary>
    /// <returns>List of all Addresses of User</returns>
    [Authorize]
    [HttpGet(ApiRoutes.GetAll)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of addresses of the authenticated user",
        typeof(List<AddressDto>))]
    public IActionResult GetAddresses()
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });

        var userId = Guid.Parse(value);
        var addresses = _addressCrud.GetAddressesByUserId(userId);

        return !addresses.Any()
            ? NotFound(new { error = "The user does not have any addresses." })
            : Ok(addresses);
    }

    /// <summary>
    ///     This method is used to get addresses of the authenticated user by delivery option.
    /// </summary>
    /// <param name="option"></param>
    /// <returns>List of AddressDtos</returns>
    [Authorize]
    [HttpGet(ApiRoutes.Address.GetAddressByOption)]
    [SwaggerResponse((int)HttpStatusCode.OK,
        "Returns the list of addresses of the authenticated user by delivery option",
        typeof(List<AddressDto>))]
    public IActionResult GetAddressByOption([FromQuery] DeliveryOptionEnum option)
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });

        var userId = Guid.Parse(value);
        var address = _addressCrud.GetAddressesByDeliveryOption(option, userId);

        return address == null!
            ? NotFound(new { error = "The user does not have any addresses." })
            : Ok(address);
    }

    #endregion
}
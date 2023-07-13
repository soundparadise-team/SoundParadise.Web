using System.Net;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Web.Dto.Address;

namespace SoundParadise.Web.Models.Address;

/// <summary>
///     AddressModel CRUD .
/// </summary>
public class AddressCrud : IAddressCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<AddressCrud> _loggingService;

    /// <summary>
    ///     AddressCrud.
    /// </summary>
    /// <param name="context">Db context</param>
    /// <param name="loggingService">Service for logging errors.</param>
    public AddressCrud(SoundParadiseDbContext context, ILoggingService<AddressCrud> loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region DELETE

    public RequestResult DeleteAddress(Guid addressId, Guid userId)
    {
        try
        {
            var address = _context.DeliveryAddresses.FirstOrDefault(address =>
                address.Id == addressId && address.UserId == userId);
            if (address == null)
                return RequestResult.Error("Address not found", HttpStatusCode.NotFound);

            _context.DeliveryAddresses.Remove(address);
            _context.SaveChanges();
            return RequestResult.Success("Address was successfully deleted");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while deleting address";
            _loggingService.LogException(ex,
                $"{error} in {nameof(AddressCrud)}.{nameof(DeleteAddress)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region

    public RequestResult UpdateAddress(AddressModel address)
    {
        try
        {
            _context.DeliveryAddresses.Update(address);
            _context.SaveChanges();
            return RequestResult.Success("Address was successfully updated");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while updating address";
            _loggingService.LogException(ex,
                $"{error} in {nameof(AddressCrud)}.{nameof(UpdateAddress)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region CREATE

    /// <summary>
    ///     Add new address to data base.
    /// </summary>
    /// <param name="addressDto">AddressDto object.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>RequestResult object</returns>
    public RequestResult AddAddress(AddressDto addressDto, Guid userId)
    {
        try
        {
            var address = new AddressModel
            {
                City = addressDto.City,
                PostOfficeAddress = addressDto.PostOfficeAddress,
                DeliveryOption = addressDto.DeliveryOption,
                UserId = userId
            };

            return CreateAddress(address).IsSuccess
                ? RequestResult.Success("Address was successfully added", HttpStatusCode.Created)
                : RequestResult.Error("An error occurred while adding address", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while adding address";
            _loggingService.LogException(ex, $"{error} in {nameof(AddressCrud)}.{nameof(AddAddress)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Create new address to data base.
    /// </summary>
    /// <param name="address">AddressModel object.</param>
    /// <returns>RequestResult object</returns>
    public RequestResult CreateAddress(AddressModel address)
    {
        try
        {
            _context.DeliveryAddresses.Add(address);
            _context.SaveChanges();
            return RequestResult.Success("Address was successfully created", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while creating address";
            _loggingService.LogException(ex,
                $"{error} in {nameof(AddressCrud)}.{nameof(CreateAddress)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region READ

    public AddressDto GetAddressDtoById(Guid? addressId, Guid? userId)
    {
        try
        {
            if (addressId == null || userId == null)
                return null!;

            var address =
                _context.DeliveryAddresses.FirstOrDefault(
                    address => address.Id == addressId && address.UserId == userId);
            return address != null
                ? new AddressDto
                {
                    Id = address.Id,
                    City = address.City,
                    PostOfficeAddress = address.PostOfficeAddress,
                    DeliveryOption = address.DeliveryOption
                }
                : null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting delivery address in {nameof(AddressCrud)}.{nameof(GetAddressById)}");
            return null!;
        }
    }

    public List<AddressDto> GetAddressesByUserId(Guid userId)
    {
        try
        {
            return _context.DeliveryAddresses
                .Where(address => address.UserId == userId)
                .Select(address => new AddressDto
                {
                    Id = address.Id,
                    City = address.City,
                    PostOfficeAddress = address.PostOfficeAddress,
                    DeliveryOption = address.DeliveryOption
                }).ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting delivery addresses in {nameof(AddressCrud)}.{nameof(GetAddressesByUserId)}");
            return Enumerable.Empty<AddressDto>().ToList();
        }
    }

    /// <summary>
    ///     Get address from data base.
    /// </summary>
    /// <param name="addressId">Address Id.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>AddressModel object</returns>
    public AddressModel GetAddressById(Guid? addressId, Guid? userId)
    {
        try
        {
            if (addressId == null || userId == null)
                return null!;

            var address =
                _context.DeliveryAddresses.FirstOrDefault(
                    address => address.Id == addressId && address.UserId == userId);
            return address ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting delivery address in {nameof(AddressCrud)}.{nameof(GetAddressById)}");
            return null!;
        }
    }

    // public List<AddressDto>

    /// <summary>
    ///     Create new address to data base.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <param name="deliveryOption">Delivery option enum.</param>
    /// <returns>List of AddressDto object</returns>
    public List<AddressDto> GetAddressesByDeliveryOption(DeliveryOptionEnum deliveryOption, Guid userId)
    {
        try
        {
            return _context.DeliveryAddresses
                .Where(address => address.UserId == userId && address.DeliveryOption == deliveryOption)
                .Select(address => new AddressDto
                {
                    Id = address.Id,
                    City = address.City,
                    PostOfficeAddress = address.PostOfficeAddress,
                    DeliveryOption = address.DeliveryOption
                }).ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting delivery addresses in {nameof(AddressCrud)}.{nameof(GetAddressesByDeliveryOption)}");
            return Enumerable.Empty<AddressDto>().ToList();
        }
    }

    #endregion
}
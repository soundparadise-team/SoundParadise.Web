using System.Net;
using SoundParadise.Api.Controllers.Api;

namespace SoundParadise.Api.Dto.Requests;

/// <summary>
///     Creatte order results.
/// </summary>
public class CreateOrderResult : RequestResult
{
    /// <summary>
    ///     Constructor for CreateOrderResult.
    /// </summary>
    /// <param name="isSuccess">Success status</param>
    /// <param name="message">Message.</param>
    /// <param name="id">Id.</param>
    /// <param name="checkoutUrl">Checkout url.</param>
    /// <param name="httpStatus">HttpStatusCode object</param>
    private CreateOrderResult(bool isSuccess, string message, Guid id, string checkoutUrl, HttpStatusCode httpStatus) :
        base(isSuccess, message, httpStatus)
    {
        Id = id;
        CheckoutUrl = checkoutUrl;
    }

    /// <summary>
    ///     Create order result Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Checkout url.
    /// </summary>
    public string CheckoutUrl { get; set; }

    /// <summary>
    ///     Success.
    /// </summary>
    /// <param name="message">String message.</param>
    /// <param name="id">Id.</param>
    /// <param name="httpStatus">HttpStatusCode object.</param>
    /// <param name="checkoutUrl">CheckOut url.</param>
    /// <returns>CreateOrderResult object</returns>
    public static CreateOrderResult Success(string message = "", Guid id = default,
        HttpStatusCode httpStatus = HttpStatusCode.OK, string checkoutUrl = "")
    {
        return new CreateOrderResult(true, message, id, checkoutUrl, httpStatus);
    }

    /// <summary>
    ///     Error.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="httpStatus">HttpStatusCode object.</param>
    /// <param name="id">Id.</param>
    /// <param name="checkoutUrl">Checkout url.</param>
    /// <returns></returns>
    public static CreateOrderResult Error(string message, HttpStatusCode httpStatus = HttpStatusCode.BadRequest,
        Guid id = default, string checkoutUrl = "")
    {
        return new CreateOrderResult(false, message, id, checkoutUrl, httpStatus);
    }
}
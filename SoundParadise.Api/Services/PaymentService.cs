using CloudIpspSDK.Checkout;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Enums;

namespace SoundParadise.Api.Services;

/// <summary>
///     Payment service.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ILoggingService<PaymentService> _loggingService;

    /// <summary>
    ///     Constructor for ppayment service.
    /// </summary>
    /// <param name="loggingService">Service for logging errors.</param>
    public PaymentService(ILoggingService<PaymentService> loggingService)
    {
        _loggingService = loggingService;
    }

    /// <summary>
    ///     Checkout.
    /// </summary>
    /// <param name="orderId">Order Id.</param>
    /// <param name="description">Description.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="paymentProvider">Payment provider.</param>
    /// <returns>RequestResult object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public RequestResult Checkout(Guid orderId, string description, decimal amount, PaymentProvider paymentProvider)
    {
        try
        {
            switch (paymentProvider)
            {
                case PaymentProvider.Fondy:
                    var req = new CheckoutRequest
                    {
                        order_id = orderId.ToString("N"),
                        amount = (int)(amount * 100),
                        order_desc = description,
                        currency = "UAH"
                    };
                    var resp = new Url().Post(req);
                    return resp.response_status == "success"
                        ? RequestResult.Success(resp.checkout_url)
                        : RequestResult.Error("An error occurred while checking out");

                case PaymentProvider.LiqPay:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(paymentProvider), paymentProvider,
                        "Unknown payment provider");
            }
        }
        catch (Exception ex)
        {
            const string message = "An error occurred while checking out";
            _loggingService.LogException(ex, message);
            return RequestResult.Error(message);
        }
    }
}
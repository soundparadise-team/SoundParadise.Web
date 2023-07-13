using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Models.Enums;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     Payment Service implements that interface.
/// </summary>
public interface IPaymentService
{
    RequestResult Checkout(Guid orderId, string description, decimal amount, PaymentProvider paymentProvider);
}
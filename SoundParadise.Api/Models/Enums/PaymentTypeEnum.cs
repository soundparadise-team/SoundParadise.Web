using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Payment type enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentTypeEnum
{
    CardPayment = 0,
    CashOnDelivery = 1
}
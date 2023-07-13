using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Payment provider enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentProvider
{
    Fondy = 0,
    LiqPay = 1
}
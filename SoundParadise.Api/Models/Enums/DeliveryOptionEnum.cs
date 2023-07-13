using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Delivery Option enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DeliveryOptionEnum
{
    NovaPoshta = 0,
    UkrPoshta = 1
}
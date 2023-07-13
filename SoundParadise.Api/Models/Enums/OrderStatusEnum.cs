using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Order status enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum OrderStatusEnum
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Canceled
}
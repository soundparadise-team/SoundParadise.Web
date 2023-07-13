using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Delivery type enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DeliveryTypeEnum
{
    [Description("У відділення")] Department = 0,
    [Description("У поштомат")] ParcelLocker = 1,
    [Description("Кур'єром")] Courier = 2
}
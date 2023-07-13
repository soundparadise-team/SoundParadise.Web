using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     Sort option enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum SortOptionsEnum
{
    NameDesc = 0,
    NameAsc = 1,
    PriceAsc = 2,
    PriceDesc = 3
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoundParadise.Api.Models.Enums;

/// <summary>
///     User role enum
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum UserRoleEnum
{
    Admin = 0,
    Seller = 1,
    Customer = 2
}
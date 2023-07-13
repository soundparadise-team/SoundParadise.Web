using System.Globalization;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoundParadise.Api.Models.Enums;

namespace SoundParadise.Api.Dto.User;

/// <summary>
///     User  dto.
/// </summary>
public class UserDto
{
    private readonly string _email;
    private readonly string? _imageUrl;
    private readonly string _name;
    private readonly string _surname;
    private readonly string? _username;

    public Guid? Id { get; set; }

    /// <summary>
    ///     Name.
    /// </summary>
    public string Name
    {
        get => _name.Transform(To.LowerCase, To.TitleCase);
        init => _name = value;
    }

    /// <summary>
    ///     Surname.
    /// </summary>
    public string Surname
    {
        get => _surname.Transform(To.LowerCase, To.TitleCase);
        init => _surname = value;
    }

    /// <summary>
    ///     Username.
    /// </summary>
    public string Username
    {
        get => _username?.ToLower() ?? string.Empty;
        init => _username = value;
    }

    /// <summary>
    ///     Email.
    /// </summary>
    public string Email
    {
        get => CultureInfo.CurrentCulture.TextInfo.ToLower(_email);
        init => _email = value;
    }

    /// <summary>
    ///     Image Url.
    /// </summary>
    public string ImageUrl
    {
        get => _imageUrl?.ToLower() ?? string.Empty;
        init => _imageUrl = value;
    }

    /// <summary>
    ///     Phone Number.
    /// </summary>
    public string PhoneNumber { get; init; }

    /// <summary>
    ///     User role enum.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public UserRoleEnum Role { get; set; }
}
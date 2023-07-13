namespace SoundParadise.Api.Dto.User;

/// <summary>
///     User create dto.
/// </summary>
public class UserCreateDto : UserDto
{
    public string Password { get; set; }
}
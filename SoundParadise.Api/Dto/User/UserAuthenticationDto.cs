namespace SoundParadise.Api.Dto.User;

/// <summary>
///     User Authentication dto.
/// </summary>
public class UserAuthenticationDto
{
    /// <summary>
    ///     Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Password.
    /// </summary>
    public string Password { get; set; }
}
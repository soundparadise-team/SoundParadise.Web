using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Validators.User;

namespace SoundParadise.Api.Controllers.Api.v2;

/// <summary>
///     User API controller
/// </summary>
[ApiVersion(ApiVersions.V2)]
[BaseRoute(ApiRoutes.User.Base)]
public class UserController : BaseApiController
{
    private readonly UserAuthenticationDtoValidator _userAuthenticationDtoValidator;
    private readonly UserCreateDtoValidator _userCreateDtoValidator;
    private readonly UserCrud _userCrud;

    /// <summary>
    ///     Constructor for UserController
    /// </summary>
    /// <param name="userCrud"></param>
    /// <param name="userAuthenticationDtoValidator"></param>
    /// <param name="userCreateDtoValidator"></param>
    public UserController(UserCrud userCrud, UserAuthenticationDtoValidator userAuthenticationDtoValidator,
        UserCreateDtoValidator userCreateDtoValidator)
    {
        _userCrud = userCrud;
        _userAuthenticationDtoValidator = userAuthenticationDtoValidator;
        _userCreateDtoValidator = userCreateDtoValidator;
    }

    /// <summary>
    ///     Just a test method for checking if the API Versioning is working
    /// </summary>
    /// <param name="userCreateDto"></param>
    /// <returns>Returns a string</returns>
    [HttpPost("register-user")]
    public IActionResult RegisterUser([FromBody] UserCreateDto userCreateDto)
    {
        return Ok("Hello from v2");
    }
}
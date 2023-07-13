using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Extensions;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Options;
using SoundParadise.Api.Services;
using SoundParadise.Api.Validators.User;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     User API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.User.Base)]
public class UserController : BaseApiController
{
    private readonly ImageService _imageService;
    private readonly IOptions<UrlsOptions> _options;
    private readonly UserAuthenticationDtoValidator _userAuthenticationDtoValidator;
    private readonly UserCreateDtoValidator _userCreateDtoValidator;
    private readonly IUserCrud _userCrud;

    /// <summary>
    ///     Constructor for UserController v1.
    /// </summary>
    /// <param name="userCrud">The UserCrud instance.</param>
    /// <param name="userAuthenticationDtoValidator">The UserAuthenticationDtoValidator instance.</param>
    /// <param name="userCreateDtoValidator">The UserCreateDtoValidator instance.</param>
    /// <param name="cardCrud"></param>
    public UserController(IUserCrud userCrud, UserAuthenticationDtoValidator userAuthenticationDtoValidator,
        UserCreateDtoValidator userCreateDtoValidator, IOptions<UrlsOptions> options, ImageService imageService)
    {
        _userCrud = userCrud;
        _userAuthenticationDtoValidator = userAuthenticationDtoValidator;
        _userCreateDtoValidator = userCreateDtoValidator;
        _options = options;
        _imageService = imageService;
    }

    #region PUT

    [Authorize]
    [HttpPut(ApiRoutes.User.UpdateUser)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Updating user's data", typeof(string))]
    private IActionResult UpdateUser(UserDto userDto)
    {
        if (userDto == null!)
            return BadRequest("User data is null");

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = _userCrud.GetUserById(Guid.Parse(userId));

        if (user == null!)
            return BadRequest("User not found");

        if (userDto.Id != user.Id)
            return BadRequest("User ID does not match");

        var result = _userCrud.UpdateUser(userDto);

        return result.IsSuccess
            ? Ok(new { result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Deletes a user with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>The user is successfully deleted.</returns>
    [HttpDelete(ApiRoutes.Delete)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The user is successfully deleted.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid user ID. Returns an error message.", typeof(string))]
    public IActionResult DeleteUser([FromQuery] Guid userId)
    {
        var user = _userCrud.GetUserById(userId);
        if (user == null!) return NotFound("The user does not exist.");
        _userCrud.DeleteUser(userId);
        return Ok(new { message = "The user is successfully deleted" });
    }

    #endregion

    #region GET

    /// <summary>
    ///     Retrieves the details of the authenticated user.
    /// </summary>
    /// <returns>Details of the user.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.User.SecureEndpoint)]
    [Authorize(Policy = AuthPolicies.SellerPolicy)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the user information in the form of a UserDto object.",
        typeof(UserDto))]
    public IActionResult SecureEndpoint()
    {
        var userClaims = HttpContext.User;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null!;
        var user = _userCrud.GetUserById(Guid.Parse(userId));
        if (user.Name == null!) return null!;
        var userDto = new UserDto
        {
            Name = user.Name,
            Surname = user.Surname,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };
        return Ok(userDto);
    }

    /// <summary>
    ///     Confirms a user's registration using the provided user ID and token.
    /// </summary>
    /// <param name="userId">The ID of the user to confirm.</param>
    /// <param name="token">The confirmation token.</param>
    /// <returns>The user confirmation is successful. Returns an HTML page with a redirect to the login page.</returns>
    [HttpGet(ApiRoutes.User.ConfirmUser)]
    [SwaggerResponse((int)HttpStatusCode.OK,
        "The user confirmation is successful. Returns an HTML page with a redirect to the login page.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,
        "Invalid user ID or token. Returns a bad request with an error message.", typeof(string))]
    public IActionResult ConfirmUser(Guid userId, string token)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = "Invalid user id or token" });

        var result = _userCrud.ConfirmUser(userId, token);

        var contentResult = new ContentResult
        {
            Content =
                $"<html><head><meta http-equiv='refresh' content='0; url={_options.Value.Client + "/confirmed"}' /></head></html>",
            ContentType = "text/html"
        };

        return result.IsSuccess
            ? contentResult
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region POST

    /// <summary>
    ///     Registers a new user with the provided details.
    /// </summary>
    /// <param name="userCreateDto">The user registration data.</param>
    /// <returns>The user is successfully registered. Returns the registered user object.</returns>
    [HttpPost(ApiRoutes.User.RegisterUser)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The user is successfully registered. Returns the registered user object.",
        typeof(UserModel))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,
        "Invalid user registration data. Returns an error message with validation errors.",
        typeof(string))]
    public IActionResult RegisterUser([FromBody] UserCreateDto userCreateDto)
    {
        var validationResult = _userCreateDtoValidator.Validate(userCreateDto);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = validationErrors });
        }

        var cartItemDtos = HttpContext.Session.Get<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

        var user = _userCrud.RegisterUser(userCreateDto, cartItemDtos);
        return user == null!
            ? BadRequest(new { error = "Something went wrong while registering user" })
            : Ok(user);
    }

    /// <summary>
    ///     Authenticates a user by validating their credentials.
    /// </summary>
    /// <param name="authenticationDto">The user authentication data.</param>
    /// <returns>The user is successfully authenticated. Returns an authentication token.</returns>
    [HttpPost(ApiRoutes.User.LoginUser)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The user is successfully authenticated. Returns an authentication token.",
        typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid user credentials. Returns an error message.",
        typeof(string))]
    public IActionResult Login([FromBody] UserAuthenticationDto authenticationDto)
    {
        var validationResult = _userAuthenticationDtoValidator.Validate(authenticationDto);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = validationErrors });
        }

        var result = _userCrud.AuthenticateUser(authenticationDto);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { token = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    /// <summary>
    ///     Logs out the currently authenticated user.
    /// </summary>
    /// <returns>The user is successfully logged out.</returns>
    [Authorize]
    [HttpPost(ApiRoutes.User.LogoutUser)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The user is successfully logged out.", typeof(string))]
    public IActionResult Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = _userCrud.Logout(token);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    /// <summary>
    ///     Gets the currently authenticated user.
    /// </summary>
    /// <returns>UserDto</returns>
    [Authorize]
    [HttpPost(ApiRoutes.User.GetCurrentUser)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Getting the current user", typeof(UserDto))]
    public IActionResult GetCurrentUser()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = _userCrud.GetUserDtoById(Guid.Parse(userId));

        return Ok(user);
    }

    /// <summary>
    ///     Uploads a user's avatar.
    /// </summary>
    /// <param name="file"></param>
    /// <returns>Image url</returns>
    [Authorize]
    [HttpPost(ApiRoutes.User.UploadAvatar)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Uploading user's avatar", typeof(string))]
    public IActionResult UploadAvatar([FromForm] IFormFile file)
    {
        if (file is not { Length: > 0 })
            return BadRequest("No file has been selected");

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = _userCrud.GetUserById(Guid.Parse(userId));

        if (user == null!)
            return BadRequest("User not found");

        var result = _userCrud.UploadAvatar(file, user);

        return result.IsSuccess
            ? Ok(new { result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion
}
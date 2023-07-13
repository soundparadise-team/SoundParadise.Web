using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Cart;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Wishlist;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Models.User;

/// <summary>
///     User CRUD
/// </summary>
public class UserCrud : IUserCrud
{
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly SoundParadiseDbContext _context;
    private readonly HashingService _hashingService;
    private readonly ImageService _imageService;
    private readonly ILoggingService<UserCrud> _loggingService;
    private readonly SmtpService _smtpService;
    private readonly TokenService _tokenService;
    private readonly IUrlHelperFactory _urlHelperFactory;

    /// <summary>
    ///     User CRUD constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="hashingService"></param>
    /// <param name="tokenService"></param>
    /// <param name="smtpService"></param>
    /// <param name="loggingService"></param>
    /// <param name="hostingEnvironment"></param>
    public UserCrud(SoundParadiseDbContext context, HashingService hashingService, TokenService tokenService,
        SmtpService smtpService, ILoggingService<UserCrud> loggingService, IUrlHelperFactory urlHelperFactory,
        IActionContextAccessor actionContextAccessor, ImageService imageService)
    {
        _context = context;
        _hashingService = hashingService;
        _tokenService = tokenService;
        _smtpService = smtpService;
        _loggingService = loggingService;
        _urlHelperFactory = urlHelperFactory;
        _actionContextAccessor = actionContextAccessor;
        _imageService = imageService;
    }

    #region UPDATE

    /// <summary>
    ///     Update user in data base
    /// </summary>
    /// ///
    /// <param name="userDto">Сontains user data</param>
    /// ///
    /// <returns>True if Ok, false if error </returns>
    public RequestResult UpdateUser(UserDto userDto)
    {
        try
        {
            var user = _context.Users.Find(userDto.Id);
            if (user == null)
                return RequestResult.Error("User not found", HttpStatusCode.NotFound);


            user.Name = userDto.Name == null! ? user.Name : userDto.Name;
            user.Surname = userDto.Surname == null! ? user.Surname : userDto.Surname;
            user.Email = userDto.Email == null! ? user.Email : userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber == null! ? user.PhoneNumber : userDto.PhoneNumber;

            _context.SaveChanges();
            return RequestResult.Success("User updated successfully");
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while updating user in {nameof(UserCrud)}.{nameof(UpdateUser)}");
            return null!;
        }
    }

    #endregion

    #region DELETE

    public RequestResult DeleteUser(Guid id)
    {
        var user = _context.Users.Find(id);

        if (user == null) return RequestResult.Error("User not found", HttpStatusCode.NotFound);

        _context.Users.Remove(user);
        _context.SaveChanges();

        return _context.Subcategories.Any(u => u.Id == id)
            ? RequestResult.Success("User deleted successfully")
            : RequestResult.Error("An error occurred while deleting user", HttpStatusCode.InternalServerError);
    }

    #endregion

    #region CREATE

    /// <summary>
    ///     Add new user to data base
    /// </summary>
    /// ///
    /// <param name="userDto">Сontains user data</param>
    /// ///
    /// <returns>True if Ok, false if error </returns>
    public bool CreateUser(UserCreateDto userDto)
    {
        try
        {
            var user = new UserModel
            {
                Name = userDto.Name,
                Surname = userDto.Surname,
                Username = userDto.Username,
                Email = userDto.Email,
                Role = userDto.Role,
                PhoneNumber = userDto.PhoneNumber
            };

            _hashingService.CreatePasswordHash(userDto.Password, out var passwordSalt, out var passwordHash);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);

            _context.SaveChanges();
            return _context.Subcategories.Any(u => u.Id == user.Id);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while creating user in {nameof(UserCrud)}.{nameof(CreateUser)}");
            return false;
        }
    }

    public RequestResult UploadAvatar(IFormFile file, UserModel user)
    {
        var result = _imageService.UploadImage(file, user.Id);

        if (!result.IsSuccess)
            return RequestResult.Error(result.Message);

        user.Image = new ImageModel
        {
            Id = Guid.NewGuid(),
            Path = result.Message
        };

        _context.SaveChanges();

        return _context.Users.Any(u => u.Id == user.Id && u.Image == null)
            ? RequestResult.Error("Image not uploaded", HttpStatusCode.InternalServerError)
            : RequestResult.Success(result.Message);
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get list of all users
    /// </summary>
    /// ///
    /// <returns>List of users</returns>
    public List<UserModel> GetAllUsers()
    {
        try
        {
            var users = _context.Users.ToList();
            return !users.Any() ? Enumerable.Empty<UserModel>().ToList() : users;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all users in {nameof(UserCrud)}.{nameof(GetAllUsers)}");
            return Enumerable.Empty<UserModel>().ToList();
        }
    }

    /// <summary>
    ///     Get user by Id
    /// </summary>
    /// ///
    /// <param name="id">User Id</param>
    /// ///
    /// <returns>UserModel object</returns>
    public UserModel GetUserById(Guid id)
    {
        try
        {
            var user = _context.Users.Find(id);
            return user ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting user by id in {nameof(UserCrud)}.{nameof(GetUserById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get UserDto by Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public UserDto GetUserDtoById(Guid userId)
    {
        try
        {
            var user = _context.Users
                .Include(u => u.Image)
                .FirstOrDefault(u => u.Id == userId);
            return user == null
                ? null!
                : new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageUrl = _imageService.GetImageUrl(user.Image.Path)
                };
        }
        catch (Exception e)
        {
            _loggingService.LogException(e,
                $"An error occurred while getting user by id in {nameof(UserCrud)}.{nameof(GetUserById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get user by credentials
    /// </summary>
    /// ///
    /// <param name="authenticationDto">User credentials</param>
    /// ///
    /// <returns>UserModel object</returns>
    public UserModel? GetUserByCredentials(UserAuthenticationDto authenticationDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(authenticationDto.Email) ||
                string.IsNullOrWhiteSpace(authenticationDto.Password)) return null;
            var email = authenticationDto.Email;
            return _context.Users
                .Include(u => u.TokenJournals)
                .ThenInclude(tj => tj.Token)
                .AsSplitQuery()
                .FirstOrDefault(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting user by credentials in {nameof(UserCrud)}.{nameof(GetUserByCredentials)}");
            return null;
        }
    }

    /// <summary>
    ///     Get user by token
    /// </summary>
    /// ///
    /// <param name="token">User token</param>
    /// ///
    /// <returns>UserModel object</returns>
    public UserModel GetUserByToken(string token)
    {
        try
        {
            var principal = _tokenService.ValidateToken(token);
            if (principal == null)
                throw new Exception("Invalid token");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new Exception("Invalid user ID");

            var user = _context.Users.Find(userId);
            if (user == null)
                throw new Exception("User not found");

            return user;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting user by token in {nameof(UserCrud)}.{nameof(GetUserByToken)}");
            return null!;
        }
    }

    #endregion

    #region REGISTRATION

    /// <summary>
    ///     Registers a new user
    /// </summary>
    /// ///
    /// <param name="userDto">User data</param>
    /// ///
    /// <param name="cartItemDtos">User's products in cart</param>
    /// ///
    /// <returns>UserModel object</returns>
    public UserModel RegisterUser(UserCreateDto userDto, List<CartItemDto> cartItemDtos)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userDto.Password))
                throw new Exception("Password is required");

            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new Exception("Email is required");

            if (userDto.Role < 0)
                throw new Exception("Role is required");

            if (_context.Users.Any(u => u.Email == userDto.Email))
                throw new Exception("Email \"" + userDto.Email + "\" is already taken");

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Surname = userDto.Surname,
                Username = userDto.Username,
                Email = userDto.Email,
                Role = userDto.Role,
                PhoneNumber = userDto.PhoneNumber
            };

            var cart = new CartModel
            {
                UserId = user.Id
            };

            var wishlist = new WishlistModel
            {
                UserId = user.Id
            };

            user.Cart = cart;
            user.Wishlist = wishlist;
            user.CartId = cart.Id;
            user.WishlistId = wishlist.Id;

            if (cartItemDtos is { Count: > 0 })
                foreach (var cartItem in cartItemDtos.Select(cartItemDto => new CartItemModel
                         {
                             ProductId = cartItemDto.ProductId,
                             Quantity = cartItemDto.Quantity,
                             CartId = cart.Id
                         }))
                    cart.CartItems?.Add(cartItem);

            _hashingService.CreatePasswordHash(userDto.Password, out var passwordSalt, out var passwordHash);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add(user);

            var link = GenerateConfirmationLink(user);

            _ = _smtpService.SendConfirmationEmail(user.Email, link);

            _context.SaveChanges();

            return user;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while registering user in {nameof(UserCrud)}.{nameof(RegisterUser)}");
            return null!;
        }
    }

    /// <summary>
    ///     Confirm user
    /// </summary>
    /// ///
    /// <param name="userId">User's Id</param>
    /// ///
    /// <param name="token">User's token</param>
    /// ///
    /// <returns>RequestResult object</returns>
    public RequestResult ConfirmUser(Guid userId, string token)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.ConfirmationToken == token);

            if (user == null) return RequestResult.Error("User not found", HttpStatusCode.NotFound);
            user.Confirm();
            _context.SaveChanges();
            return RequestResult.Success("User confirmed successfully");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while confirming user";
            _loggingService.LogException(ex, $"{error} in {nameof(UserCrud)}.{nameof(ConfirmUser)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Generate link to confirm user
    /// </summary>
    /// ///
    /// <param name="user">UserModel object</param>
    /// ///
    /// <returns>String object</returns>
    public string GenerateConfirmationLink(UserModel user)
    {
        try
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var confirmationToken = GenerateUniqueToken();
            user.ConfirmationToken = confirmationToken;
            _context.SaveChanges();

            var confirmationLink = urlHelper.Action("ConfirmUser", "User",
                new { userId = user.Id, token = confirmationToken },
                urlHelper.ActionContext.HttpContext.Request.Scheme);

            return confirmationLink ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while generating confirmation link in {nameof(UserCrud)}.{nameof(GenerateConfirmationLink)}");
            return null!;
        }
    }

    /// <summary>
    ///     Generate unique token
    /// </summary>
    /// ///
    /// <returns>String object</returns>
    public string GenerateUniqueToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    #endregion

    #region LOGIN/LOGOUT

    /// <summary>
    ///     Logout user
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>String object</returns>
    public RequestResult Logout(string token)
    {
        try
        {
            var tokenJournal = _context.TokenJournals.FirstOrDefault(tj => tj.Token.Token == token);

            if (tokenJournal == null) return RequestResult.Error("Token not found", HttpStatusCode.NotFound);
            if (!tokenJournal.IsActive)
                return RequestResult.Error("Token is already inactive");
            tokenJournal.IsActive = false;
            tokenJournal.DeactivatedAt = DateTime.Now;
            _context.SaveChanges();
            return RequestResult.Success("Successfully logged out");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while logging out";
            _loggingService.LogException(ex, $"{error} in {nameof(UserCrud)}.{nameof(Logout)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     User authorizationм
    /// </summary>
    /// <param name="authenticationDto">User'datas</param>
    /// <returns>RequestResult object</returns>
    public RequestResult AuthenticateUser(UserAuthenticationDto authenticationDto)
    {
        try
        {
            var user = GetUserByCredentials(authenticationDto);
            if (user == null)
                return RequestResult.Error("User not found", HttpStatusCode.NotFound);

            if (!_hashingService.VerifyPasswordHash(authenticationDto.Password, user.PasswordHash, user.PasswordSalt))
                return RequestResult.Error("Invalid password");

            if (!user.IsConfirmed)
                return RequestResult.Error("User is not confirmed");

            var tokenJournal = user.TokenJournals?.FirstOrDefault(tj => tj.IsActive);
            var token = tokenJournal?.Token?.Token;

            if (token != null) return RequestResult.Success(token);
            var generatedToken = _tokenService.GenerateToken(user, out tokenJournal);
            _context.Tokens.Add(generatedToken);
            _context.TokenJournals.Add(tokenJournal);
            _context.SaveChanges();

            token = generatedToken.Token;

            return RequestResult.Success(token);
        }
        catch (Exception ex)
        {
            const string error = "Error while authenticating";
            _loggingService.LogException(ex, $"{error} in {nameof(UserCrud)}.{nameof(AuthenticateUser)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion
}
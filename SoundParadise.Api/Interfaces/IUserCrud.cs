using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     UserCrud implements that interface.
/// </summary>
public interface IUserCrud
{
    bool CreateUser(UserCreateDto userDto);
    RequestResult UpdateUser(UserDto userDto);
    RequestResult DeleteUser(Guid id);
    List<UserModel> GetAllUsers();
    UserModel GetUserById(Guid id);
    UserDto GetUserDtoById(Guid userId);

    UserModel? GetUserByCredentials(UserAuthenticationDto authenticationDto);
    UserModel GetUserByToken(string token);
    UserModel RegisterUser(UserCreateDto userDto, List<CartItemDto> cartItemDtos);
    RequestResult ConfirmUser(Guid userId, string token);
    RequestResult UploadAvatar(IFormFile file, UserModel userModel);
    string GenerateConfirmationLink(UserModel user);
    string GenerateUniqueToken();
    RequestResult Logout(string token);
    RequestResult AuthenticateUser(UserAuthenticationDto authenticationDto);
}
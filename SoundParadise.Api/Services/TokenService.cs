using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SoundParadise.Api.Models.Token;
using SoundParadise.Api.Models.TokenJournal;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Options;

namespace SoundParadise.Api.Services;

/// <summary>
///     Token service.
/// </summary>
public class TokenService
{
    private readonly string _secretKey;

    /// <summary>
    ///     TokenService constructor.
    /// </summary>
    /// <param name="options">Token option.</param>
    public TokenService(IOptions<TokenOptions> options)
    {
        options.Value.Validate();
        _secretKey = options.Value.SecretKey;
    }

    /// <summary>
    ///     Generate new token.
    /// </summary>
    /// <param name="user">UserModel object.</param>
    /// <param name="tokenJournal">TokenJournalModel object</param>
    /// <returns>TokenModel object.</returns>
    public TokenModel GenerateToken(UserModel user, out TokenJournalModel tokenJournal)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenModel = new TokenModel
        {
            Id = Guid.NewGuid(),
            Token = tokenHandler.WriteToken(token),
            ExpirationDate = tokenDescriptor.Expires ?? DateTime.UtcNow.AddDays(7)
        };

        tokenJournal = new TokenJournalModel
        {
            Id = Guid.NewGuid(),
            TokenId = tokenModel.Id,
            UserId = user.Id,
            IsActive = true,
            ActivatedAt = DateTime.Now
        };

        return tokenModel;
    }

    /// <summary>
    ///     Token validator.
    /// </summary>
    /// <param name="token">Token.</param>
    /// <returns>TokenModel object.</returns>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
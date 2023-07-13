using System.Security.Cryptography;
using System.Text;

namespace SoundParadise.Api.Services;

/// <summary>
///     Hashing service.
/// </summary>
public class HashingService
{
    /// <summary>
    ///     Create password hash.
    /// </summary>
    /// <param name="password">Password.</param>
    /// <param name="passwordSalt">Password salt.</param>
    /// <param name="passwordHash">Password hash.</param>
    public void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    ///     Verify password hesh.
    /// </summary>
    /// <param name="password">Password.</param>
    /// <param name="passwordHash">Password hash.</param>
    /// <param name="passwordSalt">PAssword salt.</param>
    /// <returns>True if ok, false if not.</returns>
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return !computedHash.Where((t, i) => t != passwordHash[i]).Any();
    }
}
using System.Text.RegularExpressions;
using SoundParadise.Api.Data;

namespace SoundParadise.Api.Helpers;

/// <summary>
///     User validator helper.
/// </summary>
public static class UserValidationHelper
{
    /// <summary>
    ///     Validate password.
    /// </summary>
    /// <param name="password">Password.</param>
    /// <returns>True if validate, false if not.</returns>
    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 8)
            return false;

        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        if (!passwordRegex.IsMatch(password))
            return false;

        return true;
    }

    /// <summary>
    ///     Validate email.
    /// </summary>
    /// <param name="email">String email.</param>
    /// <returns></returns>
    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        if (!emailRegex.IsMatch(email))
            return false;

        return true;
    }

    /// <summary>
    ///     Email is not taken.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <param name="context">Db context.</param>
    /// <returns>True if ok, false if not.</returns>
    public static bool EmailIsNotTaken(string email, SoundParadiseDbContext context)
    {
        return !context.Users.Any(u => u.Email == email);
    }

    /// <summary>
    ///     Validate phone number.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns>True if ok, false if not.</returns>
    public static bool ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var phoneNumberRegex = new Regex(@"^[0-9]*$");

        if (!phoneNumberRegex.IsMatch(phoneNumber))
            return false;

        return true;
    }

    /// <summary>
    ///     Phone number is not taken.
    /// </summary>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="context">Db context.</param>
    /// <returns>True if ok, false if not.</returns>
    public static bool PhoneNumberIsNotTaken(string phoneNumber, SoundParadiseDbContext context)
    {
        return !context.Users.Any(u => u.PhoneNumber == phoneNumber);
    }
}
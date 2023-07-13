using FluentValidation;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Helpers;

namespace SoundParadise.Api.Validators.User;

/// <summary>
///     UserAuthenticationDto validator.
/// </summary>
public class UserAuthenticationDtoValidator : AbstractValidator<UserAuthenticationDto>
{
    private readonly SoundParadiseDbContext _context;

    /// <summary>
    ///     Validator.
    /// </summary>
    /// <param name="context">Db context</param>
    public UserAuthenticationDtoValidator(SoundParadiseDbContext context)
    {
        _context = context;

        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(UserValidationHelper.ValidateEmail).WithMessage("Invalid email format");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required")
            .Must(UserValidationHelper.ValidatePassword).WithMessage(
                "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, and one digit");
    }
}
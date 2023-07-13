using FluentValidation;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Helpers;

namespace SoundParadise.Api.Validators.User;

public class UserCreateDtoValidator : UserDtoValidator<UserCreateDto>
{
    private readonly SoundParadiseDbContext _context;

    /// <summary>
    ///     Constructor for the UserCreateDtoValidator
    /// </summary>
    /// <param name="context">Db context.</param>
    public UserCreateDtoValidator(SoundParadiseDbContext context)
        : base(context)
    {
        _context = context;

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .Must(UserValidationHelper.ValidatePassword).WithMessage(
                "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, and one digit");
    }
}
using FluentValidation;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.User;
using SoundParadise.Api.Helpers;

namespace SoundParadise.Api.Validators.User;

/// <summary>
///     UserDto validator.
/// </summary>
/// <typeparam name="TUserDto">Parent class parameter.</typeparam>
public abstract class UserDtoValidator<TUserDto> : AbstractValidator<TUserDto>
    where TUserDto : UserDto
{
    private readonly SoundParadiseDbContext _context;

    /// <summary>
    ///     Constructor for UserDtoValidator.
    /// </summary>
    /// <param name="context">Db context.</param>
    protected UserDtoValidator(SoundParadiseDbContext context)
    {
        _context = context;

        RuleFor(u => u.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(u => u.Surname).NotEmpty().WithMessage("Surname is required");

        RuleFor(u => u.Username)
            .Custom((username, context) =>
            {
                if (_context.Users.Any(u => u.Username == username))
                    context.AddFailure("Username is already taken");
            })
            .NotEmpty().WithMessage("Username is required");

        RuleFor(u => u.Email)
            .Must(UserValidationHelper.ValidateEmail).WithMessage("Invalid email format")
            .Must(email => UserValidationHelper.EmailIsNotTaken(email, _context)).WithMessage("Email is already taken");

        RuleFor(u => u.PhoneNumber)
            .Must(UserValidationHelper.ValidatePhoneNumber)
            .WithMessage("Phone number must contain only numeric characters.")
            .Must(phoneNumber => UserValidationHelper.PhoneNumberIsNotTaken(phoneNumber, _context))
            .WithMessage("Phone number is already taken");
    }
}
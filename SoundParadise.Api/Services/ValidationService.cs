// using FluentValidation;
// using SoundParadise.Api.DataAccess;
//
// namespace SoundParadise.Api.Dto.User.Validation;
//
// public abstract class UserDtoValidator<TUserDto> : AbstractValidator<TUserDto>
//     where TUserDto : UserDto
// {
//     protected readonly SoundParadiseDbContext _context;
//
//     protected UserDtoValidator(SoundParadiseDbContext context)
//     {
//         _context = context;
//
//         RuleFor(u => u.Username).NotEmpty().WithMessage("Username is required");
//         RuleFor(u => u.Name).NotEmpty().WithMessage("Name is required");
//         RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone number is required").Matches(@"^[0-9]*$").WithMessage("Phone number must contain only numeric characters.");
//
//         RuleFor(u => u.Username).Custom((username, context) =>
//         {
//             if (_context.Users.Any(u => u.Username == username))
//                 context.AddFailure("Username is already taken");
//         });
//
//         RuleFor(u => u.PhoneNumber).Custom((phoneNumber, context) =>
//         {
//             if (_context.Users.Any(u => u.PhoneNumber == phoneNumber))
//                 context.AddFailure("Phone number is already taken");
//         });
//     }
// }
//
// public class UserAuthenticationDtoValidator : AbstractValidator<UserAuthenticationDto>
// {
//     private readonly SoundParadiseDbContext _context;
//
//     public UserAuthenticationDtoValidator(SoundParadiseDbContext context)
//     {
//         _context = context;
//
//         RuleFor(dto => dto.Email)
//             .NotEmpty().WithMessage("Email is required.")
//             .EmailAddress().WithMessage("Invalid email address");
//
//         RuleFor(dto => dto.Password)
//             .NotEmpty().WithMessage("Password is required");
//     }
// }
//
// public class UserCreateDtoValidator : UserDtoValidator<UserCreateDto>
// {
//     public UserCreateDtoValidator(SoundParadiseDbContext context)
//         : base(context)
//     {
//         RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required");
//         RuleFor(u => u.Password).Must(ValidatePassword).WithMessage("Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, and one digit");
//         RuleFor(u => u.Email).Must(ValidateEmail).WithMessage("Invalid email format");
//         RuleFor(u => u.Email).Must(EmailIsNotTaken).WithMessage("Email is already taken");
//         RuleFor(u => u.PhoneNumber).Must(ValidatePhoneNumber).WithMessage("Phone number must contain only numeric characters.");
//         RuleFor(u => u.PhoneNumber).Must(PhoneNumberIsNotTaken).WithMessage("Phone number is already taken");
//     }
//
//     private bool ValidatePassword(string password)
//     {
//         if (string.IsNullOrWhiteSpace(password))
//             return false;
//
//         if (password.Length < 8)
//             return false;
//
//         var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
//         if (!passwordRegex.IsMatch(password))
//             return false;
//
//         return true;
//     }
//
//     private bool ValidateEmail(string email)
//     {
//         if (string.IsNullOrWhiteSpace(email))
//             return false;
//
//         var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
//         if (!emailRegex.IsMatch(email))
//             return false;
//
//         return true;
//     }
//
//     private bool EmailIsNotTaken(string email)
//     {
//         return !_context.Users.Any(u => u.Email == email);
//     }
//
//     private bool ValidatePhoneNumber(string phoneNumber)
//     {
//         if (string.IsNullOrWhiteSpace(phoneNumber))
//             return false;
//
//         var phoneNumberRegex = new Regex(@"^[0-9]*$");
//
//         if (!phoneNumberRegex.IsMatch(phoneNumber))
//             return false;
//
//         return true;
//     }
//
//     private bool PhoneNumberIsNotTaken(string phoneNumber)
//     {
//         return !_context.Users.Any(u => u.PhoneNumber == phoneNumber);
//     }
// }


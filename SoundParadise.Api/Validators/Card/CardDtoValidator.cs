using FluentValidation;
using SoundParadise.Web.Dto.Card;

namespace SoundParadise.Api.Validators.Card;

/// <summary>
///     CartDto validator.
/// </summary>
public class CardDtoValidator : AbstractValidator<CardDto>
{
    /// <summary>
    ///     Validate.
    /// </summary>
    public CardDtoValidator()
    {
        RuleFor(dto => dto.CardNumber)
            .NotEmpty().WithMessage("Card number is required")
            .Matches(@"^\d{4}-\d{4}-\d{4}-\d{4}$")
            .WithMessage("Card number should be in the format XXXX-XXXX-XXXX-XXXX, with only numbers");

        RuleFor(dto => dto.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required")
            .Matches(@"^\d{2}/\d{2}$").WithMessage("Expiration date should be in the format xx/xx, with only numbers");

        RuleFor(dto => dto.CVV)
            .NotEmpty().WithMessage("CVV is required")
            .Matches(@"^\d{3}$").WithMessage("CVV should be a 3-digit number");
    }
}
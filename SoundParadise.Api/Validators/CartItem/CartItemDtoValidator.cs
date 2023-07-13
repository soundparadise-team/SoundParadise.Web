using FluentValidation;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Helpers;

namespace SoundParadise.Api.Validators.CartItem;

/// <summary>
///     CartItemDto validator.
/// </summary>
public class CartItemDtoValidator : AbstractValidator<CartItemDto>
{
    private readonly SoundParadiseDbContext _context;

    /// <summary>
    ///     Validator.
    /// </summary>
    /// <param name="context"></param>
    public CartItemDtoValidator(SoundParadiseDbContext context)
    {
        _context = context;

        RuleFor(dto => dto.ProductId)
            .Must(productId => CartItemValidationHelper.IsProductExists(productId, _context))
            .WithMessage("Invalid product ID");
    }
}
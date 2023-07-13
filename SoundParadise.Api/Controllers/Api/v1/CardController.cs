using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Validators.Card;
using SoundParadise.Web.Dto.Card;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Card API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Card.Base)]
public class CardController : BaseApiController
{
    private readonly ICardCrud _cardCrud;
    private readonly CardDtoValidator _cardValidator;

    /// <summary>
    ///     Constructor for CardController v1.
    /// </summary>
    /// <param name="cardCrud"></param>
    public CardController(ICardCrud cardCrud, CardDtoValidator cardValidator)
    {
        _cardCrud = cardCrud;
        _cardValidator = cardValidator;
    }

    #region POST

    /// <summary>
    ///     Adds a new card to the user's account.
    /// </summary>
    /// <returns>Invalid card data. Returns an error message.</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Card.AddCard)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The card is successfully added.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid card data. Returns an error message.", typeof(string))]
    public IActionResult AddCard(CardDto cardDto)
    {
        var validationResult = _cardValidator.Validate(cardDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors });
        }

        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);

        var result = _cardCrud.AddCard(cardDto, userId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region PUT

    /// <summary>
    ///     Updates the card information in the user's account.
    /// </summary>
    /// <param name="cardDto"></param>
    /// <returns>Card updated.</returns>
    [Authorize]
    [HttpPut(ApiRoutes.Card.UpdateCard)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The card is successfully updated.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid card data. Returns an error message.", typeof(string))]
    public IActionResult UpdateCard(CardDto cardDto)
    {
        var validationResult = _cardValidator.Validate(cardDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors });
        }

        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);
        var result = _cardCrud.UpdateCard(cardDto, userId);
        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Deletes the card from the user's account.
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns>Card deleted successfully.</returns>
    [Authorize]
    [HttpDelete(ApiRoutes.Card.DeleteCard)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The card is successfully deleted.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid card data. Returns an error message.", typeof(string))]
    public IActionResult DeleteCard(Guid cardId)
    {
        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);
        var result = _cardCrud.DeleteCard(cardId, userId);

        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region GET

    /// <summary>
    ///     Returns all cards for related user in the form of a CardDto object.
    /// </summary>
    /// <returns>Returns list of dtos.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.GetAll)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns all cards for related user in the form of a CardDto object.",
        typeof(CardDto))]
    public IActionResult GetCards()
    {
        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);
        var cards = _cardCrud.GetCardsByUserId(userId);
        return cards.Any()
            ? Ok(cards)
            : NotFound(new { error = "No cards found" });
    }

    /// <summary>
    ///     Returns the card information in the form of a CardDto object.
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns>Returns CardDto.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.Card.GetCard)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the card information in the form of a CardDto object.",
        typeof(CardDto))]
    public IActionResult GetCard(Guid cardId)
    {
        var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return BadRequest(new { error = "Invalid user ID" });
        var userId = Guid.Parse(id);
        var result = _cardCrud.CardExists(cardId, userId);
        return result.IsSuccess
            ? Ok(_cardCrud.GetCardById(cardId, userId))
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion
}
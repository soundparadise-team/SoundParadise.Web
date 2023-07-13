using System.Collections;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Services;
using SoundParadise.Web.Dto.Card;

namespace SoundParadise.Api.Models.Card;

/// <summary>
///     CardModel CRUD.
/// </summary>
public class CardCrud : ICardCrud
{
    private readonly CardEncryptionService _cardEncryptionService;
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<CardCrud> _loggingService;
    private readonly IUserCrud _userCrud;

    /// <summary>
    ///     CardCrud constructor.
    /// </summary>
    /// <param name="context">Db context</param>
    /// <param name="cardEncryptionService">Service for encryption card</param>
    /// <param name="loggingService">Service for logging errors</param>
    /// <param name="userCrud">UserCrud</param>
    public CardCrud(SoundParadiseDbContext context, ILoggingService<CardCrud> loggingService, IUserCrud userCrud,
        CardEncryptionService cardEncryptionService)
    {
        _context = context;
        _loggingService = loggingService;
        _cardEncryptionService = cardEncryptionService;
        _userCrud = userCrud;
    }

    #region CREATE

    /// <summary>
    ///     Add card in data base.
    /// </summary>
    /// <param name="cardDto">CardDto object</param>
    /// <param name="userId">User Id</param>
    /// <returns>RequestResult object</returns>
    public RequestResult AddCard(CardDto cardDto, Guid userId)
    {
        try
        {
            // var user = _userCrud.GetUserById(userId);
            // if (user == null)
            //     return RequestResult.Error("User does not exist", HttpStatusCode.NotFound);

            var card = _cardEncryptionService.EncryptCardData(cardDto);
            card.UserId = userId;

            if (CardExists(card.EncryptedCardNumber, card.UserId).IsSuccess)
                return RequestResult.Error("Card already exists", HttpStatusCode.Conflict);
            _context.Cards.Add(card);
            _context.SaveChanges();
            return RequestResult.Success("Card created");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while creating card";
            _loggingService.LogException(ex, $"{error} in {nameof(CardCrud)}.{nameof(AddCard)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region UPDATE

    /// <summary>
    ///     Update card in data base.
    /// </summary>
    /// <param name="cardDto">CardDto object</param>
    /// <param name="userId">User Id</param>
    /// <returns>RequestResult object</returns>
    public RequestResult UpdateCard(CardDto cardDto, Guid userId)
    {
        try
        {
            var card = _cardEncryptionService.EncryptCardData(cardDto);
            card.UserId = userId;

            if (!CardExists(card.Id, card.UserId).IsSuccess)
                return RequestResult.Error("Card does not exist", HttpStatusCode.NotFound);
            _context.Cards.Update(card);
            _context.SaveChanges();
            return RequestResult.Success("Card updated");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while updating card";
            _loggingService.LogException(ex, $"{error} in {nameof(CardCrud)}.{nameof(UpdateCard)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete card from data base.
    /// </summary>
    /// <param name="cardId">Card Id</param>
    /// <param name="userId">User Id</param>
    /// <returns>RequestResult object</returns>
    public RequestResult DeleteCard(Guid cardId, Guid userId)
    {
        try
        {
            _context.Cards.RemoveRange(_context.Cards.Where(c => c.Id == cardId && c.UserId == userId));
            _context.SaveChanges();
            return !CardExists(cardId, userId).IsSuccess
                ? RequestResult.Success("Card deleted")
                : RequestResult.Error("An error occurred while deleting card", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while deleting card";
            _loggingService.LogException(ex, $"{error} in {nameof(CardCrud)}.{nameof(DeleteCard)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get Cards by User Id.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <returns>List of CardDto object</returns>
    public List<CardDto> GetCardsByUserId(Guid userId)
    {
        try
        {
            var cards = _context.Cards
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .ToList();

            var cardDtos = cards.Select(card => _cardEncryptionService.DecryptCardData(card)).ToList();

            return cardDtos;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting cards for user with id {userId} in {nameof(CardCrud)}.{nameof(GetCardsByUserId)}");
            return Enumerable.Empty<CardDto>().ToList();
        }
    }

    /// <summary>
    ///     Get Cards by Id.
    /// </summary>
    /// <param name="cardId">Card Id.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>CardDto object</returns>
    public CardDto GetCardById(Guid cardId, Guid userId)
    {
        try
        {
            var card = _context.Cards
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == cardId && c.UserId == userId);

            return card == null! ? null! : _cardEncryptionService.DecryptCardData(card);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting card with id {cardId} in {nameof(CardCrud)}.{nameof(GetCardById)}");
            return null!;
        }
    }

    #endregion

    #region HELPERS

    /// <summary>
    ///     Check if card exists.
    /// </summary>
    /// <param name="cardId">Card Id.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>RequestResult object.</returns>
    public RequestResult CardExists(Guid cardId, Guid userId)
    {
        return _context.Cards.Any(c => c.Id == cardId && c.UserId == userId)
            ? RequestResult.Success("Card exists")
            : RequestResult.Error("Card does not exist", HttpStatusCode.NotFound);
    }

    /// <summary>
    ///     Check if card exists.
    /// </summary>
    /// <param name="cardNumber">Card number.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>RequestResult object.</returns>
    public RequestResult CardExists(IEnumerable cardNumber, Guid userId)
    {
        return _context.Cards.Any(c => c.EncryptedCardNumber == cardNumber && c.UserId == userId)
            ? RequestResult.Success("Card exists")
            : RequestResult.Error("Card does not exist", HttpStatusCode.NotFound);
    }

    #endregion
}
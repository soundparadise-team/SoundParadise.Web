using System.Collections;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Web.Dto.Card;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     CardCrud implements that interface.
/// </summary>
public interface ICardCrud
{
    RequestResult AddCard(CardDto cardDto, Guid userId);
    List<CardDto> GetCardsByUserId(Guid userId);
    CardDto GetCardById(Guid cardId, Guid userId);
    RequestResult UpdateCard(CardDto cardDto, Guid userId);
    RequestResult DeleteCard(Guid cardId, Guid userId);
    RequestResult CardExists(Guid cardId, Guid userId);
    RequestResult CardExists(IEnumerable cardNumber, Guid userId);
}
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SoundParadise.Api.Models.Card;
using SoundParadise.Api.Options;
using SoundParadise.Web.Dto.Card;

namespace SoundParadise.Api.Services;

/// <summary>
///     Card encryption service.
/// </summary>
public class CardEncryptionService : EncryptionService
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _iv;

    /// <summary>
    ///     Constructor for card encrypt service.
    /// </summary>
    /// <param name="options">Encrypt option.</param>
    public CardEncryptionService(IOptions<EncryptionOptions> options)
    {
        options.Value.Validate();
        _encryptionKey = Encoding.UTF8.GetBytes(options.Value.Key);
        _iv = ConvertHexStringToByteArray(options.Value.IV);
    }

    /// <summary>
    ///     Encrypt card data.
    /// </summary>
    /// <param name="cardDto">CardDto object.</param>
    /// <returns>CardModel object.</returns>
    public CardModel EncryptCardData(CardDto cardDto)
    {
        CardModel card = new();
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        card.EncryptedCardNumber = EncryptData(cardDto.CardNumber, encryptor);
        card.EncryptedExpiryDate = EncryptData(cardDto.ExpirationDate, encryptor);
        card.EncryptedCVV = EncryptData(cardDto.CVV, encryptor);

        return card;
    }

    /// <summary>
    ///     Decrypt card data.
    /// </summary>
    /// <param name="card">CardModel object.</param>
    /// <returns>CartDto object.</returns>
    public CardDto DecryptCardData(CardModel card)
    {
        CardDto cardDto = new();
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        cardDto.Id = card.Id;
        cardDto.CardNumber = DecryptData(card.EncryptedCardNumber, decryptor);
        cardDto.ExpirationDate = DecryptData(card.EncryptedExpiryDate, decryptor);
        cardDto.CVV = DecryptData(card.EncryptedCVV, decryptor);

        return cardDto;
    }

    /// <summary>
    ///     Convert hex string to byte array.
    /// </summary>
    /// <param name="hexString">Hex string.</param>
    /// <returns>Byte array.</returns>
    private byte[] ConvertHexStringToByteArray(string hexString)
    {
        var numberChars = hexString.Length;
        var bytes = new byte[numberChars / 2];
        for (var i = 0; i < numberChars; i += 2) bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        return bytes;
    }
}
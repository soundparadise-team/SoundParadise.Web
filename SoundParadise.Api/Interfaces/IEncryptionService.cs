using System.Security.Cryptography;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     Encryption Service implements that interface.
/// </summary>
public interface IEncryptionService
{
    byte[] EncryptData(string data, ICryptoTransform encryptor);
    string DecryptData(byte[] encryptedData, ICryptoTransform decryptor);
}
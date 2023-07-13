using System.Security.Cryptography;
using System.Text;
using SoundParadise.Api.Interfaces;

namespace SoundParadise.Api.Services;

/// <summary>
///     Encription service.
/// </summary>
public class EncryptionService : IEncryptionService
{
    /// <summary>
    ///     Encrypt data.
    /// </summary>
    /// <param name="data">Data to encrypt.</param>
    /// <param name="encryptor">Encryptor.</param>
    /// <returns>Encrypt data in byte array.</returns>
    public byte[] EncryptData(string data, ICryptoTransform encryptor)
    {
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            var plaintextBytes = Encoding.UTF8.GetBytes(data);
            cs.Write(plaintextBytes, 0, plaintextBytes.Length);
        }

        var encryptedData = ms.ToArray();

        return encryptedData;
    }

    /// <summary>
    ///     Decrypt data.
    /// </summary>
    /// <param name="encryptedData">Encrypted data.</param>
    /// <param name="decryptor">Decryptor data.</param>
    /// <returns>Decrypt data in string</returns>
    public string DecryptData(byte[] encryptedData, ICryptoTransform decryptor)
    {
        using var ms = new MemoryStream(encryptedData);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        var decryptedData = sr.ReadToEnd();

        return decryptedData;
    }
}
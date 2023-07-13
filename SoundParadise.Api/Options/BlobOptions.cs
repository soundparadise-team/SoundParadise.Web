namespace SoundParadise.Api.Options;

public class BlobOptions
{
    /// <summary>
    ///     Connection string to Blob.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    ///     Base url.
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    ///     Token Sas.
    /// </summary>
    public string TokenSas { get; set; }

    /// <summary>
    ///     Container name.
    /// </summary>
    public string ContainerName { get; set; }

    /// <summary>
    ///     Validate.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
            throw new ArgumentNullException(nameof(ConnectionString),
                "The connection string cannot be null or empty. Please provide a valid connection string in the AzureBlobOptions.");

        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new ArgumentNullException(nameof(BaseUrl),
                "The container url cannot be null or empty. Please provide a valid container url in the AzureBlobOptions.");

        if (string.IsNullOrWhiteSpace(TokenSas))
            throw new ArgumentNullException(nameof(TokenSas),
                "The container sas cannot be null or empty. Please provide a valid container sas in the AzureBlobOptions.");

        if (string.IsNullOrWhiteSpace(ContainerName))
            throw new ArgumentNullException(nameof(ContainerName),
                "The container name cannot be null or empty. Please provide a valid container name in the AzureBlobOptions.");
    }
}
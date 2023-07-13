using System.Drawing;
using System.Drawing.Imaging;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Options;

namespace SoundParadise.Api.Services;

/// <summary>
///     Image service.
/// </summary>
public class ImageService
{
    private readonly string _baseUrl;
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName;
    private readonly string _tokenSas;

    /// <summary>
    ///     Constructor for image service.
    /// </summary>
    /// <param name="options">BlopOption option.</param>
    public ImageService(IOptions<BlobOptions> options)
    {
        options.Value.Validate();
        var blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);

        _containerName = options.Value.ContainerName;
        _tokenSas = options.Value.TokenSas;
        _baseUrl = options.Value.BaseUrl;
    }

    /// <summary>
    ///     Get image url.
    /// </summary>
    /// <param name="fileName">Image file name.</param>
    /// <returns>Url string.</returns>
    public string GetImageUrl(string fileName)
    {
        return $"{_baseUrl}/{_containerName}/{fileName}";
    }

    /// <summary>
    ///     Upload image.
    /// </summary>
    /// <param name="imageStream">Image stream.</param>
    /// <param name="fileName">Image file name.</param>
    /// <returns>Task object.</returns>
    public async Task<string> UploadImage(Stream imageStream, string fileName)
    {
        await _containerClient.CreateIfNotExistsAsync();

        var blobClient = _containerClient.GetBlobClient(fileName);

        imageStream.Position = 0;

        await blobClient.UploadAsync(imageStream, true);

        return blobClient.Uri.ToString();
    }

    /// <summary>
    ///     Uploads an image file to the blob storage container.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <param name="user">The user associated with the image.</param>
    /// <returns>The uploaded image URL or an error message.</returns>
    public RequestResult UploadImage(IFormFile file, Guid userId)
    {
        if (file == null! || file.Length == 0)
            return RequestResult.Error("No file was selected.");

        if (!IsImage(file))
            return RequestResult.Error("The selected file is not an image.");

        var fileName = userId + Path.GetExtension(file.FileName.ToLower());
        var blobClient = _containerClient.GetBlobClient(fileName);

        using (var stream = file.OpenReadStream())
        {
            blobClient.Upload(stream, true);
        }

        return RequestResult.Success(GetImageUrl(fileName));
    }

    /// <summary>
    ///     Checks if the provided file is an image.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns>True if the file is an image; otherwise, false.</returns>
    private static bool IsImage(IFormFile file)
    {
        try
        {
            using var image = Image.FromStream(file.OpenReadStream());
            return image.RawFormat.Guid == ImageFormat.Jpeg.Guid ||
                   image.RawFormat.Guid == ImageFormat.Png.Guid ||
                   image.RawFormat.Guid == ImageFormat.Gif.Guid ||
                   image.RawFormat.Guid == ImageFormat.Bmp.Guid;
        }
        catch
        {
            return false;
        }
    }
}
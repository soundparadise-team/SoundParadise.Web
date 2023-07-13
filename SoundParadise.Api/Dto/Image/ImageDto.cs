namespace SoundParadise.Api.Dto.Image;

/// <summary>
///     Image dto.
/// </summary>
public class ImageDto
{
    private string _path = string.Empty;

    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Path.
    /// </summary>
    public string Path
    {
        get => _path.ToLower();
        set => _path = value;
    }
}
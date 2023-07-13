using Humanizer;

namespace SoundParadise.Api.Dto.Subcategory;

/// <summary>
///     Subcategory dto.
/// </summary>
public class SubcategoryDto
{
    private string _name;

    private string _path;
    public Guid Id { get; set; }

    /// <summary>
    ///     Path.
    /// </summary>
    public string Path
    {
        get => _path.ToLower();
        set => _path = value;
    }

    /// <summary>
    ///     Name.
    /// </summary>
    public string Name
    {
        get => _name.Transform(To.LowerCase, To.TitleCase);
        set => _name = value;
    }
}
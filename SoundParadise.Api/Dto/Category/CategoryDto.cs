using Humanizer;
using SoundParadise.Api.Dto.Image;
using SoundParadise.Api.Dto.Subcategory;

namespace SoundParadise.Api.Dto.Category;

/// <summary>
///     Category dto.
/// </summary>
public class CategoryDto
{
    private string _name = string.Empty;

    /// <summary>
    ///     Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Name.
    /// </summary>
    public string Name
    {
        get => _name.Transform(To.LowerCase, To.TitleCase);
        set => _name = value;
    }

    /// <summary>
    ///     Image.
    /// </summary>
    public ImageDto Image { get; set; }

    /// <summary>
    ///     List of subcategorydto objects.
    /// </summary>
    public List<SubcategoryDto> Subcategories { get; set; }
}
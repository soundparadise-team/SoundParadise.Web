using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Subcategory;

namespace SoundParadise.Api.Models.Category;

/// <summary>
///     Category data model.
/// </summary>
public class CategoryModel
{
    /// <summary>
    ///     Category Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("category_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Subcategories navigation property.
    /// </summary>
    [JsonIgnore]
    public List<SubcategoryModel>? Subcategories { get; set; }

    /// <summary>
    ///     Image Id.
    /// </summary>
    [Column("image_id")]
    public Guid? ImageId { get; set; }

    /// <summary>
    ///     Image navigation property.
    /// </summary>
    [JsonIgnore]
    public ImageModel? Image { get; set; }

    /// <summary>
    ///     Category name.
    /// </summary>
    [Column("name")]
    public string Name { get; set; }
}
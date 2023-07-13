using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Category;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Models.Subcategory;

/// <summary>
///     Subcategoty data model.
/// </summary>
public class SubcategoryModel
{
    /// <summary>
    ///     Subcategoty's Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("subcategory_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Category's Id.
    /// </summary>
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    /// <summary>
    ///     Category's navigation property.
    /// </summary>
    [JsonIgnore]
    public CategoryModel? Category { get; set; }

    /// <summary>
    ///     Products navigation property.
    /// </summary>
    [JsonIgnore]
    public List<ProductModel>? Products { get; set; }

    /// <summary>
    ///     Name of subcategory.
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Image Id.
    /// </summary>
    [Column("image_id")]
    public Guid ImageId { get; set; }

    /// <summary>
    ///     Image navigation property.
    /// </summary>
    public ImageModel? Image { get; set; }
}
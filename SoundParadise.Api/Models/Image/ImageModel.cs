using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Category;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Models.Subcategory;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Image;

/// <summary>
///     Image data model.
/// </summary>
public class ImageModel
{
    /// <summary>
    ///     Image Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("image_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Product Id.
    /// </summary>
    [Column("product_id")]
    public Guid? ProductId { get; set; }

    /// <summary>
    ///     Product navigation property.
    /// </summary>
    [JsonIgnore]
    public ProductModel? Product { get; set; }

    /// <summary>
    ///     Category Id.
    /// </summary>
    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    /// <summary>
    ///     Category navigation property.
    /// </summary>
    [JsonIgnore]
    public CategoryModel? Category { get; set; }

    /// <summary>
    ///     Subcategory Id.
    /// </summary>
    [Column("subcategory_id")]
    public Guid? SubcategoryId { get; set; }

    /// <summary>
    ///     User navigation property.
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }

    [Column("user_id")] public Guid? UserId { get; set; }

    /// <summary>
    ///     Subcategory navigation property.
    /// </summary>
    [JsonIgnore]
    public SubcategoryModel? Subcategory { get; set; }

    /// <summary>
    ///     Path.
    /// </summary>
    [Column("image_path")]
    public string Path { get; set; }
}
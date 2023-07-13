using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.CartItem;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.ProductParameter;
using SoundParadise.Api.Models.Subcategory;
using SoundParadise.Api.Models.User;
using SoundParadise.Api.Models.Wishlist;

namespace SoundParadise.Api.Models.Product;

/// <summary>
///     Product data model.
/// </summary>
public class ProductModel
{
    /// <summary>
    ///     Comments count.
    /// </summary>
    [NotMapped] public int CommentsCount;

    /// <summary>
    ///     Product Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("product_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Slug's Id.
    /// </summary>
    [Column("slug_id")]
    public string Slug { get; set; }

    /// <summary>
    ///     Product's name.
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Product description.
    /// </summary>
    [Column("description")]
    public string Description { get; set; }

    /// <summary>
    ///     Product quantity.
    /// </summary>
    [Column("quantity")]
    public short Quantity { get; set; }

    /// <summary>
    ///     Product price.
    /// </summary>
    [Column("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///     Product code.
    /// </summary>
    [Column("code")]
    public int Code { get; set; }

    /// <summary>
    ///     Discount status.
    /// </summary>
    [Column("is_discount")]
    public bool IsDiscount { get; set; }

    /// <summary>
    ///     Discount sum.
    /// </summary>
    [Column("discount")]
    public decimal Discount { get; set; }

    /// <summary>
    ///     Date of product create.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Date of product update.
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    ///     Date of product delete.
    /// </summary>
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    ///     Product manufacturer.
    /// </summary>
    [Column("manufacturer")]
    public string Manufacturer { get; set; }

    /// <summary>
    ///     Product weight.
    /// </summary>
    [Column("weight")]
    public decimal Weight { get; set; }

    /// <summary>
    ///     Product lenght.
    /// </summary>
    [Column("length")]
    public decimal Length { get; set; }

    /// <summary>
    ///     Product widtht.
    /// </summary>
    [Column("width")]
    public decimal Width { get; set; }

    /// <summary>
    ///     Product height.
    /// </summary>
    [Column("height")]
    public decimal Height { get; set; }

    /// <summary>
    ///     Product color.
    /// </summary>
    [Column("color")]
    public string Color { get; set; }

    /// <summary>
    ///     Is new status.
    /// </summary>
    [Column("is_new")]
    public bool IsNew { get; set; }

    /// <summary>
    ///     In stok status.
    /// </summary>
    [Column("in_stock")]
    public bool InStock { get; set; }

    /// <summary>
    ///     In cart status.
    /// </summary>
    [NotMapped]
    public bool InCart { get; set; }

    /// <summary>
    ///     In wishlist status.
    /// </summary>
    [NotMapped]
    public bool InWishlist { get; set; }

    /// <summary>
    ///     Product country.
    /// </summary>
    [Column("country")]
    public string Country { get; set; }

    /// <summary>
    ///     Seller Id.
    /// </summary>
    [Column("seller_id")]
    public Guid SellerId { get; set; }

    /// <summary>
    ///     Seller navigation property.
    /// </summary>
    [JsonIgnore]
    public UserModel? Seller { get; set; }

    /// <summary>
    ///     List of comments about product.
    /// </summary>
    [JsonIgnore]
    public List<CommentModel>? Comments { get; set; }

    /// <summary>
    ///     List of cart items, where is the product.
    /// </summary>
    [JsonIgnore]
    public List<CartItemModel>? CartItems { get; set; }

    /// <summary>
    ///     Subcategory Id.
    /// </summary>
    [Column("subcategory_id")]
    public Guid? SubcategoryId { get; set; }

    /// <summary>
    ///     Rating of product.
    /// </summary>
    [Column("rating")]
    public decimal Rating { get; set; }

    /// <summary>
    ///     Rating count of product.
    /// </summary>
    [Column("rating_count")]
    public int? RatingCount { get; set; }

    /// <summary>
    ///     Subcategory navigation property.
    /// </summary>
    [JsonIgnore]
    public SubcategoryModel? Subcategory { get; set; }

    /// <summary>
    ///     List of product images.
    /// </summary>
    public List<ImageModel>? Images { get; set; }

    /// <summary>
    ///     List of WishListProduct objects.
    /// </summary>
    [JsonIgnore]
    public List<WishlistProductsModel>? WishlistProducts { get; set; }

    /// <summary>
    ///     List of parameters.
    /// </summary>
    [JsonIgnore]
    public List<ProductParameterModel>? Parameters { get; set; }
}
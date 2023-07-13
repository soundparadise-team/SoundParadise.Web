using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Models.ProductParameter;

/// <summary>
///     ProductParameter data model.
/// </summary>
public class ProductParameterModel
{
    /// <summary>
    ///     ProductParameter Id.
    /// </summary>
    [Column("parameter_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Product's Id.
    /// </summary>

    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    ///     Product's navigation property.
    /// </summary>
    [JsonIgnore]
    public ProductModel? Product { get; set; }

    /// <summary>
    ///     Parameter's name.
    /// </summary>
    [Column("parameter_name")]
    public string ParameterName { get; set; }

    /// <summary>
    ///     Parameter's value.
    /// </summary>
    [Column("parameter_value")]
    public string ParameterValue { get; set; }
}
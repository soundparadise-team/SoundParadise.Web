using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundParadise.Api.Models.Accumulators.ProductSalesAccumulator;

/// <summary>
///     ProductSales data model.
/// </summary>
public class ProductSalesAccumulator
{
    /// <summary>
    ///     Date.
    /// </summary>
    [Key]
    [Column("date")]
    public DateTime Date { get; set; }

    /// <summary>
    ///     Product Name.
    /// </summary>
    [Column("product_name")]
    public string ProductName { get; set; }

    /// <summary>
    ///     Sales count.
    /// </summary>
    [Column("count")]
    public int AllCount { get; set; }
}
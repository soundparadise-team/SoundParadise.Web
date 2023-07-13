using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundParadise.Api.Models.Accumulators.AllSalesAccumulator;

/// <summary>
///     AllSalesAccumulator data model.
/// </summary>
public class AllSalesAccumulator
{
    /// <summary>
    ///     Date.
    /// </summary>
    [Key]
    [Column("date")]
    public DateTime Date { get; set; }

    /// <summary>
    ///     Sales ount.
    /// </summary>
    [Column("count")]
    public int AllCount { get; set; }
}
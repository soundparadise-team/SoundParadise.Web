using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundParadise.Api.Models.Accumulators.UserAccumulator;

/// <summary>
///     UserAccumulator data model.
/// </summary>
public class UserAccumulator
{
    /// <summary>
    ///     Date.
    /// </summary>
    [Key]
    [Column("date")]
    public DateTime Date { get; set; }

    /// <summary>
    ///     Registered users.
    /// </summary>
    [Column("registered")]
    public int Registered { get; set; }

    /// <summary>
    ///     Authorized users.
    /// </summary>
    [Column("authorized")]
    public int Authorized { get; set; }
}
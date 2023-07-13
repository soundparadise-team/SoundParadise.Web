using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundParadise.Api.Models.Log;

/// <summary>
///     Log data model.
/// </summary>
public class LogModel
{
    /// <summary>
    ///     Log Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("log_id")]
    public int Id { get; set; }

    /// <summary>
    ///     Log Application.
    /// </summary>
    [Column("application")]
    public string Application { get; set; }

    /// <summary>
    ///     Logged string.
    /// </summary>
    [Column("logged")]
    public string Logged { get; set; }

    /// <summary>
    ///     Error level.
    /// </summary>
    [Column("level")]
    public string Level { get; set; }

    /// <summary>
    ///     Message string.
    /// </summary>
    [Column("message")]
    public string Message { get; set; }

    /// <summary>
    ///     Exception.
    /// </summary>
    [Column("exception")]
    public string Exception { get; set; }
}
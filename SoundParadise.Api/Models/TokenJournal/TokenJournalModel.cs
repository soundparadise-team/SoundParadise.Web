using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Token;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.TokenJournal;

/// <summary>
///     TokenJournal data model.
/// </summary>
public class TokenJournalModel
{
    /// <summary>
    ///     TokenJournal's Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("token_journal_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Token's Id.
    /// </summary>
    [Column("token_id")]
    public Guid TokenId { get; set; }

    /// <summary>
    ///     Token's navigation property.
    /// </summary>
    [JsonIgnore]
    public TokenModel? Token { get; set; }

    /// <summary>
    ///     User's Id.
    /// </summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    ///     User's navigation property.
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }

    /// <summary>
    ///     Active status.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    ///     Activated date.
    /// </summary>
    [Column("activated_at")]
    public DateTime ActivatedAt { get; set; }

    /// <summary>
    ///     Deactivated date.
    /// </summary>
    [Column("deactivated_at")]
    public DateTime DeactivatedAt { get; set; }
}
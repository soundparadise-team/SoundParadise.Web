using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.TokenJournal;

namespace SoundParadise.Api.Models.Token;

/// <summary>
///     Token data model.
/// </summary>
public class TokenModel
{
    /// <summary>
    ///     Token Id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("token_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Token string.
    /// </summary>
    [Column("token")]
    public string Token { get; set; }

    /// <summary>
    ///     Expiration date.
    /// </summary>
    [Column("expiration_date")]
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    ///     TokenJournal navigation property.
    /// </summary>
    [JsonIgnore]
    public List<TokenJournalModel>? TokenJournals { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Models.User;

namespace SoundParadise.Api.Models.Comment;

/// <summary>
///     Comment data model
/// </summary>
public class CommentModel
{
    private string _content;

    /// <summary>
    ///     Comment Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("comment_id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     User Id
    /// </summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    ///     User navigation property
    /// </summary>
    [JsonIgnore]
    public UserModel? User { get; set; }

    /// <summary>
    ///     Product Id
    /// </summary>
    [Column("product_id")]
    public Guid ProductId { get; set; }

    /// <summary>
    ///     Product navigation property
    /// </summary>
    [JsonIgnore]
    public ProductModel? Product { get; set; }

    /// <summary>
    ///     Content
    /// </summary>
    [Column("content")]
    public string Content
    {
        get => _content;
        set
        {
            if (_content == value) return;
            _content = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    ///     Create date
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Update date
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    ///     Delete date
    /// </summary>
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    ///     Is delete status
    /// </summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Is edited status
    /// </summary>
    [Column("is_edited")]
    public bool IsEdited { get; set; }

    /// <summary>
    ///     Is approved status
    /// </summary>
    [Column("is_approved")]
    public bool IsApproved { get; set; }

    /// <summary>
    ///     Is blocked status
    /// </summary>
    [Column("is_blocked")]
    public bool IsBlocked { get; set; }
}
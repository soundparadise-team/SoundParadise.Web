using SoundParadise.Api.Models.Comment;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     CommentCrud implements that interface.
/// </summary>
public interface ICommentCrud
{
    bool CreateComment(CommentModel comment);
    List<CommentModel> GetAllComments();
    CommentModel GetCommentByUserId(Guid userId);
    CommentModel GetCommentByProductId(Guid productId);
    int GetCommentsCount(Guid productId);
    bool DeleteComment(Guid commentId);
}
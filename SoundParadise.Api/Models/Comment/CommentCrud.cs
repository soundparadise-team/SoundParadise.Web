using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;

namespace SoundParadise.Api.Models.Comment;

/// <summary>
///     CommentModel CRUD.
/// </summary>
public class CommentCrud : ICommentCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<CommentCrud> _loggingService;

    /// <summary>
    ///     CommentCrud constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    public CommentCrud(SoundParadiseDbContext context, ILoggingService<CommentCrud> loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region CREATE

    /// <summary>
    ///     Create comment in data base.
    /// </summary>
    /// <param name="comment">Comment bodel.</param>
    /// <returns>True if create, false if not.</returns>
    public bool CreateComment(CommentModel comment)
    {
        try
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService
                .LogException(ex,
                    $"An error occurred while creating comment in {nameof(CommentCrud)}.{nameof(CreateComment)}");
            return false;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete comment in data base.
    /// </summary>
    /// <param name="commentId">Comment Id.</param>
    /// <returns>True if create, false if not.</returns>
    public bool DeleteComment(Guid commentId)
    {
        try
        {
            var comment = _context.Comments.Find(commentId);
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);

            _context.SaveChanges();
            return !_context.CartItems.Any(u => u.Id == commentId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CommentCrud.DeleteComment");
            return false;
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all comments.
    /// </summary>
    /// <returns>List of comment model</returns>
    public List<CommentModel> GetAllComments()
    {
        try
        {
            return _context.Comments.ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while getting all comments in {nameof(CommentCrud)}.{nameof(GetAllComments)}");
            return Enumerable.Empty<CommentModel>().ToList();
        }
    }

    /// <summary>
    ///     Get comment by user Id.
    /// </summary>
    /// <returns>Comment model</returns>
    public CommentModel GetCommentByUserId(Guid userId)
    {
        try
        {
            var comment = _context.Comments.Include(p => p.User)
                .FirstOrDefault(p => p.User != null && p.User.Id == userId);
            return comment ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting comment by user id in {nameof(CommentCrud)}.{nameof(GetCommentByUserId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get comment by product Id.
    /// </summary>
    /// <param name="productId">Product Id</param>
    /// <returns>Comment model</returns>
    public CommentModel GetCommentByProductId(Guid productId)
    {
        try
        {
            var comment = _context.Comments.Include(c => c.Product).FirstOrDefault(u => u.Product.Id == productId);
            return comment ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting comment by product id in {nameof(CommentCrud)}.{nameof(GetCommentByProductId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get comment count.
    /// </summary>
    /// <param name="productId">Product Id</param>
    /// <returns>Comment model</returns>
    public int GetCommentsCount(Guid productId)
    {
        try
        {
            var comments = _context.Comments.Where(c => c.ProductId == productId);
            return comments == null! ? 0 : comments.Count();
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while getting comments count in {nameof(CommentCrud)}.{nameof(GetCommentsCount)}");
            return 0;
        }
    }

    #endregion
}
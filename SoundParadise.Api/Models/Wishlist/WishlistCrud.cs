using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Models.Wishlist;

/// <summary>
///     Whislist CRUD.
/// </summary>
public class WishlistCrud : IWishlistCrud
{
    private readonly ICommentCrud _commentCrud;
    private readonly SoundParadiseDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILoggingService<WishlistCrud> _loggingService;
    private readonly IProductCrud _productCrud;

    /// <summary>
    ///     Constructor for WishlistCrus.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    /// <param name="productCrud">Product crud.</param>
    /// <param name="commentCrud">Comment crud.</param>
    /// <param name="imageService">Image service.</param>
    public WishlistCrud(SoundParadiseDbContext context, ILoggingService<WishlistCrud> loggingService,
        IProductCrud productCrud, ICommentCrud commentCrud, ImageService imageService)
    {
        _context = context;
        _loggingService = loggingService;
        _productCrud = productCrud;
        _commentCrud = commentCrud;
        _imageService = imageService;
    }

    #region CREATE

    /// <summary>
    ///     Add new Whishlist to data base
    /// </summary>
    /// ///
    /// <param name="wishlist">Whishlist data model</param>
    /// <returns>RequestResult object</returns>
    public RequestResult CreateWishlist(WishlistModel wishlist)
    {
        try
        {
            _context.Wishlists.Add(wishlist);
            _context.SaveChanges();
            return _context.Wishlists.Any(u => u.Id == wishlist.Id)
                ? RequestResult.Success("Wishlist created")
                : RequestResult.Error("Wishlist was not created", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while creating wishlist";
            _loggingService.LogException(ex, $"{error} in {nameof(WishlistCrud)}.{nameof(CreateWishlist)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get Whishlist from data base by Id
    /// </summary>
    /// ///
    /// <param name="id">Whishkist Id</param>
    /// <returns>WhishlistModel object</returns>
    public WishlistModel GetWishlistById(Guid id)
    {
        try
        {
            var favorites = _context.Wishlists.Find(id);
            return favorites ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting wishlist in {nameof(WishlistCrud)}.{nameof(GetWishlistById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get Whishlist from data base by user Id
    /// </summary>
    /// ///
    /// <param name="userId">User's Id</param>
    /// <returns>List of ProductDto</returns>
    public List<ProductDto> GetWishlistByUserId(Guid userId)
    {
        try
        {
            var wishlist = _context.Wishlists
                .Include(w => w.WishlistProducts)
                .ThenInclude(wp => wp.Product)
                .ThenInclude(p => p.Images)
                .Include(c => c.User)
                .AsSplitQuery()
                .FirstOrDefault(p => p.UserId == userId);

            return wishlist?.WishlistProducts == null || !wishlist.WishlistProducts.Any()
                ? Enumerable.Empty<ProductDto>().ToList()
                : wishlist.WishlistProducts
                    .Select(wp => new ProductDto
                    {
                        Id = wp.Product.Id,
                        Name = wp.Product.Name,
                        Price = wp.Product.Price,
                        Description = wp.Product.Description,
                        Quantity = wp.Product.Quantity,
                        IsNew = wp.Product.IsNew,
                        InStock = wp.Product.InStock,
                        InWishlist = true,
                        Rating = wp.Product.Rating,
                        CommentsCount = _commentCrud.GetCommentsCount(wp.Product.Id),
                        InCart = _productCrud.IsProductInCart(userId, wp.Product.Id),
                        ImagePath = _imageService.GetImageUrl(
                            wp.Product?.Images?.FirstOrDefault()?.Path ?? string.Empty)
                    })
                    .ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting wishlist in {nameof(WishlistCrud)}.{nameof(GetWishlistByUserId)}");
            return Enumerable.Empty<ProductDto>().ToList();
        }
    }

    #endregion

    #region UPDATE

    /// <summary>
    ///     Add product to whishlist
    /// </summary>
    /// ///
    /// <param name="userId">User Id</param>
    /// ///
    /// <param name="productId">Product Id</param>
    /// <returns>RequestResult object</returns>
    public RequestResult AddToWishlist(Guid userId, Guid productId)
    {
        try
        {
            var wishlist = _context.Wishlists
                .Include(w => w.WishlistProducts)
                .FirstOrDefault(w => w.UserId == userId);

            if (wishlist == null)
                return RequestResult.Error("Wishlist not found", HttpStatusCode.NotFound);

            var product = _context.Products.Find(productId);
            if (product == null)
                return RequestResult.Error("Product not found", HttpStatusCode.NotFound);

            if (wishlist.WishlistProducts != null && wishlist.WishlistProducts.Any(wp => wp.ProductId == productId))
                return RequestResult.Error("Product already exists in the wishlist");

            var wishlistProduct = new WishlistProductsModel
            {
                WishlistId = wishlist.Id,
                ProductId = productId
            };

            wishlist.WishlistProducts?.Add(wishlistProduct);
            _context.SaveChanges();

            return RequestResult.Success("Product added to the wishlist");
        }
        catch (Exception ex)
        {
            const string error = "Error while adding product to wishlist";
            _loggingService.LogException(ex, $"{error} in {nameof(WishlistCrud)}.{nameof(AddToWishlist)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Remove product from whishlist
    /// </summary>
    /// ///
    /// <param name="userId">User Id</param>
    /// ///
    /// <param name="productId">Product Id</param>
    /// <returns>RequestResult object</returns>
    public RequestResult RemoveFromWishlist(Guid userId, Guid productId)
    {
        try
        {
            var wishlist = _context.Wishlists
                .Include(w => w.WishlistProducts)
                .FirstOrDefault(w => w.UserId == userId);

            if (wishlist == null)
                return RequestResult.Error("Wishlist is not found", HttpStatusCode.NotFound);

            var product = _context.Products.Find(productId);

            if (product == null)
                return RequestResult.Error("Product is not found", HttpStatusCode.NotFound);

            var wishlistProduct = wishlist.WishlistProducts?.FirstOrDefault(p => p.ProductId == productId);

            if (wishlistProduct == null)
                return RequestResult.Error("Product not found in wishlist.", HttpStatusCode.NotFound);

            wishlist.WishlistProducts?.Remove(wishlistProduct);
            _context.SaveChanges();

            return RequestResult.Success("Product removed from wishlist.");
        }
        catch (Exception ex)
        {
            const string error = "Error while removing product from wishlist";
            _loggingService.LogException(ex, $"{error} in {nameof(WishlistCrud)}.{nameof(RemoveFromWishlist)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion
}
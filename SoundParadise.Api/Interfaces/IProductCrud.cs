using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Dto.Requests;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Product;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     ProductCrud implements that interface.
/// </summary>
public interface IProductCrud
{
    bool UpdateProduct(ProductModel productChange);
    RequestResult UploadImage(Guid productId, string image);
    ProductModel CreateProduct(ProductModel product);
    CommentModel CreateComment(CommentModel comment);
    List<SuggestionProductDto> SearchSuggestions(string query, int limit);

    SearchProductDtoResult SearchProducts(SortOptionsEnum sort, string query, int page = 1, int pageSize = 20,
        Guid userId = default);

    List<ProductDto> GetSpecial0ffers(SortOptionsEnum sort, int page = 1, int limit = 3, Guid userId = default);
    List<ProductDto> GetPopularProducts(int page = 1, int limit = 3, Guid userId = default);
    List<ProductDto> GetRecentBuys(int page = 1, int limit = 3, Guid userId = default);
    SearchProductResult GetAllProducts(SortOptionsEnum sort, int page = 1, int pageSize = 3, Guid userId = default);
    SearchProductResult GetProductsByUserId(Guid id);
    ProductModel GetProductById(Guid productId, Guid userId = default);
    ProductModel GetProductBySlug(string slug, Guid userId = default);
    ProductCartDto GetProductCartDtoById(Guid productId);

    SearchProductDtoResult GetProductsBySubcategoryId(SortOptionsEnum sort, Guid subcategoryId, int page = 1,
        int pageSize = 20,
        Guid userId = default);

    SearchProductDtoResult GetProductsBySubcategoryName(SortOptionsEnum sort, string subcategoryName, int page = 1,
        int pageSize = 20,
        Guid userId = default);

    SearchProductDtoResult GetProductsByCategoryId(SortOptionsEnum sort, Guid categoryId, int page = 1,
        int pageSize = 20, Guid userId = default);

    SearchProductDtoResult GetProductsByCategoryName(SortOptionsEnum sort, string categoryName, int page = 1,
        int pageSize = 20,
        Guid userId = default);

    RequestResult DeleteImage(Guid imageId, Guid productId);
    RequestResult DeleteProduct(Guid productId);
    bool IsProductInCart(Guid userId, Guid productId);
    bool IsProductInWishlist(Guid userId, Guid productId);
    bool ProductExists(Guid productId);
    bool IsQuantityValid(Guid productId, short quantity);
}
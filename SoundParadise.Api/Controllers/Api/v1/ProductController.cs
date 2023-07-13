using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Product;
using SoundParadise.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Product API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Product.Base)]

// [SwaggerTag("SoundParadise API v1")]
public class ProductController : BaseApiController
{
    private readonly ImageService _imageService;
    private readonly IProductCrud _productCrud;
    private readonly IUserCrud _userCrud;

    /// <summary>
    ///     Constructor for ProductController v1.
    /// </summary>
    /// <param name="productCrud"></param>
    /// <param name="userCrud"></param>
    /// <param name="imageService"></param>
    public ProductController(IProductCrud productCrud, IUserCrud userCrud, ImageService imageService)
    {
        _productCrud = productCrud;
        _imageService = imageService;
        _userCrud = userCrud;
    }

    #region POST

    /// <summary>
    ///     Uploads an image for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to upload the image for.</param>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The URL of the uploaded image.</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Product.UploadImage)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the URL of the uploaded image.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid file or an error occurred while uploading.",
        typeof(string))]
    public async Task<IActionResult> UploadImage([FromQuery] Guid productId, IFormFile file)
    {
        if (file.ContentType is not ("image/jpeg" or "image/png") || file.Length == 0)
            return BadRequest(new { error = "Invalid file" });
        var product = _productCrud.GetProductById(productId);

        using var memoryStream = new MemoryStream();
        memoryStream.Seek(0, SeekOrigin.Begin);
        await file.CopyToAsync(memoryStream);

        var imageUrl = await _imageService.UploadImage(memoryStream, file.FileName);

        var result = _productCrud.UploadImage(product.Id, imageUrl);

        return result.IsSuccess
            ? Ok(_imageService.GetImageUrl(file.FileName))
            : StatusCode((int)result.HttpStatus, result.Message);
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Deletes an image from a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product to delete the image from.</param>
    /// <param name="imageId">The ID of the image to delete.</param>
    /// <returns>A response indicating the status of the operation.</returns>
    [Authorize]
    [HttpDelete(ApiRoutes.Product.DeleteImage)]
    [SwaggerResponse((int)HttpStatusCode.OK, "The image is successfully deleted.", typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized. The user is not the owner of the product.",
        typeof(string))]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Not found. The user or product is not found.", typeof(string))]
    public IActionResult DeleteImageFromProduct([FromQuery] Guid productId, Guid imageId)
    {
        var product = _productCrud.GetProductById(productId);
        var id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(id);
        var user = _userCrud.GetUserById(userId);

        if (product == null! || user == null!)
            return NotFound(new { error = "User or product not found" });

        if (product.SellerId != user.Id)
            return Unauthorized(new { error = "User is not the owner of the product" });

        _productCrud.DeleteImage(product.Id, imageId);
        return Ok(new { message = "Image successfully deleted" });
    }

    #endregion

    #region GET

    /// <summary>
    ///     Searches for products by name.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="limit">The maximum number of results to retrieve (default is 4).</param>
    /// <returns>The list of products matching the search query.</returns>
    [HttpGet(ApiRoutes.Product.SearchSuggestions)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products matching the search query.",
        typeof(List<SuggestionProductDto>))]
    public IActionResult SearchSuggestions([FromQuery] string query = "query-example", int limit = 4)
    {
        var products = _productCrud.SearchSuggestions(query, limit);
        return !products.Any()
            ? NotFound(new { error = $"No products found matching the search query: {query}" })
            : Ok(products);
    }

    /// <summary>
    ///     Searches for products by name.
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="query"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>List of ProductDtos</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.SearchProducts)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products matching the search query.",
        typeof(List<ProductDto>))]
    public IActionResult SearchProducts([FromQuery] SortOptionsEnum sort, string query = "query-example",
        int page = 1, int limit = 20)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.SearchProducts(sort, query, page, limit)
            : _productCrud.SearchProducts(sort, query, page, limit, Guid.Parse(userId));

        return result.Products != null && result.Products.Any()
            ? Ok(result)
            : NotFound(new { error = $"No products found matching the search query: {query}" });
    }

    /// <summary>
    ///     Gets the product by ID.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>The ProductModel.</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.GetById)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the product with the specified ID.", typeof(ProductModel))]
    public IActionResult GetProductById([FromQuery] Guid productId)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var product = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductById(productId)
            : _productCrud.GetProductById(productId, Guid.Parse(userId));

        return product == null!
            ? NotFound(new { error = $"No product found with the specified ID: {productId}" })
            : Ok(product);
    }

    /// <summary>
    ///     Gets the product by slug.
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>ProductModel</returns>
    [HttpGet(ApiRoutes.Product.GetBySlug)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the product with the specified slug.", typeof(ProductModel))]
    public IActionResult GetProductBySlug([FromQuery] string slug)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var product = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductBySlug(slug)
            : _productCrud.GetProductBySlug(slug, Guid.Parse(userId));

        return product == null!
            ? NotFound(new { error = $"No product found with the specified slug: {slug}" })
            : Ok(product);
    }

    /// <summary>
    ///     Gets the list of products by category name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>The list of products.</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.GetByCategoryName)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified category name.",
        typeof(List<ProductModel>))]
    public IActionResult GetProductsByCategoryName([FromQuery] SortOptionsEnum sort,
        string categoryName = "category-name", int page = 1,
        int limit = 20)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductsByCategoryName(sort, categoryName, page, limit)
            : _productCrud.GetProductsByCategoryName(sort, categoryName, page, limit, Guid.Parse(userId));

        return result.Products == null && !result.Products!.Any()
            ? NotFound(new { error = $"No products found with the specified category name: {categoryName}" })
            : Ok(result);
    }

    /// <summary>
    ///     Gets the list of products by category ID.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>The list of products.</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.GetByCategoryId)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified category ID.",
        typeof(List<ProductModel>))]
    public IActionResult GetProductsByCategoryId([FromQuery] SortOptionsEnum sort, Guid categoryId, int page = 1,
        int limit = 20)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductsByCategoryId(sort, categoryId, page, limit)
            : _productCrud.GetProductsByCategoryId(sort, categoryId, page, limit, Guid.Parse(userId));

        return result.Products == null && !result.Products!.Any()
            ? NotFound(new { error = $"No products found with the specified category ID: {categoryId}" })
            : Ok(result);
    }

    /// <summary>
    ///     Gets the list of products by subcategory name.
    /// </summary>
    /// <param name="subcategoryName"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>The list of products.</returns>
    [HttpGet(ApiRoutes.Product.GetBySubcategoryName)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified subcategory name.",
        typeof(List<ProductModel>))]
    public IActionResult GetProductsBySubcategoryName([FromQuery] SortOptionsEnum sort,
        string subcategoryName = "subcategory-name",
        int page = 1, int limit = 20)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductsBySubcategoryName(sort, subcategoryName)
            : _productCrud.GetProductsBySubcategoryName(sort, subcategoryName, page, limit, Guid.Parse(userId));

        return result.Products != null && !result.Products.Any()
            ? NotFound(new { error = $"No products found with the specified subcategory name: {subcategoryName}" })
            : Ok(result);
    }

    /// <summary>
    ///     Gets the list of products by subcategory ID.
    /// </summary>
    /// <param name="subcategoryId"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>The list of products.</returns>
    [HttpGet(ApiRoutes.Product.GetBySubcategoryId)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified subcategory ID.",
        typeof(List<ProductModel>))]
    public IActionResult GetProductsBySubcategoryId([FromQuery] SortOptionsEnum sort, Guid subcategoryId, int page = 1,
        int limit = 20)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetProductsBySubcategoryId(sort, subcategoryId, page, limit)
            : _productCrud.GetProductsBySubcategoryId(sort, subcategoryId, page, limit, Guid.Parse(userId));

        return result.Products != null && !result.Products.Any()
            ? NotFound(new { error = "No products found with the specified subcategory ID" })
            : Ok(result);
    }

    /// <summary>
    ///     Gets the list of products by subcategory ID.
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>List of ProductDtos</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.GetSpecialOffers)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified subcategory ID.",
        typeof(List<ProductDto>))]
    public IActionResult GetSpecialOffers([FromQuery] SortOptionsEnum sort, int page = 1, int limit = 3)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetSpecial0ffers(sort, page, limit)
            : _productCrud.GetSpecial0ffers(sort, page, limit, Guid.Parse(userId));

        return result == null! && !result!.Any()
            ? NotFound(new { error = "No products found with the specified subcategory ID" })
            : Ok(result);
    }

    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.GetPopular)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified subcategory ID.",
        typeof(List<ProductDto>))]
    public IActionResult GetPopular([FromQuery] int page = 1, int limit = 3)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetPopularProducts(page, limit)
            : _productCrud.GetPopularProducts(page, limit, Guid.Parse(userId));

        return result == null!
            ? NotFound(new { error = "No products found" })
            : Ok(result);
    }

    /// <summary>
    ///     Gets the list of products by subcategory ID.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>List of ProductDtos</returns>
    [AllowAnonymous]
    [HttpGet(ApiRoutes.Product.GetRecentBuys)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of products with the specified subcategory ID.",
        typeof(List<ProductDto>))]
    public IActionResult GetRecentBuys([FromQuery] int page = 1, int limit = 3)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = string.IsNullOrEmpty(userId)
            ? _productCrud.GetRecentBuys(page, limit)
            : _productCrud.GetRecentBuys(page, limit, Guid.Parse(userId));

        return result == null!
            ? NotFound(new { error = "No products found" })
            : Ok(result);
    }

    #endregion

    #region PUT

    #endregion
}
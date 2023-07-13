using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Dto.Requests;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Comment;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.Image;
using SoundParadise.Api.Models.Wishlist;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Models.Product;

/// <summary>
///     ProductModel CRUD.
/// </summary>
public class ProductCrud : IProductCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILoggingService<ProductCrud> _loggingService;

    /// <summary>
    ///     ProductCrud constructor.
    /// </summary>
    /// <param name="context">Db context</param>
    /// <param name="loggingService">Servuce for logging errors</param>
    /// <param name="imageService">Image service</param>
    public ProductCrud(SoundParadiseDbContext context, ILoggingService<ProductCrud> loggingService,
        ImageService imageService)
    {
        _context = context;
        _loggingService = loggingService;
        _imageService = imageService;
    }

    #region UPDATE

    /// <summary>
    ///     Update product in data base.
    /// </summary>
    /// <param name="productchange">Product model object</param>
    /// <returns>True if Ok, False if error</returns>
    public bool UpdateProduct(ProductModel productchange)
    {
        try
        {
            var product = _context.Products.Find(productchange.Id);

            if (product == null)
            {
                _loggingService.LogError(
                    $"Product not found by ID: {productchange} in {nameof(ProductCrud)}.{nameof(UpdateProduct)}");
                return false;
            }

            product.Price = productchange.Price;
            product.Description = productchange.Description;
            product.Name = productchange.Name;
            product.Quantity = productchange.Quantity;
            product.Slug = productchange.Slug;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
            (ex,
                $"An error occurred while updating product by ID: {productchange.Id} in {nameof(ProductCrud)}.{nameof(UpdateProduct)}");
            return false;
        }
    }

    #endregion

    #region CREATE

    /// <summary>
    ///     Add image to product.
    /// </summary>
    /// <param name="productId">Product model Id.</param>
    /// <param name="image">Path to image.</param>
    /// <returns>RequstResult object</returns>
    public RequestResult UploadImage(Guid productId, string image)
    {
        try
        {
            var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return RequestResult.Error("Product not found", HttpStatusCode.NotFound);

            product.Images ??= new List<ImageModel>();

            product.Images.Add(new ImageModel
            {
                Product = product,
                Path = image
            });

            _context.SaveChanges();
            return RequestResult.Success("Image uploaded successfully");
        }
        catch (Exception ex)
        {
            var error = ex.InnerException is SqlException { Number: 2627 }
                ? "Image already exists"
                : "An error occurred while uploading image";
            _loggingService.LogException(ex, $"{error} in {nameof(ProductCrud)}.{nameof(UploadImage)}");
            return RequestResult.Error($"{error}", HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Add new product in data base.
    /// </summary>
    /// <param name="product">Product model.</param>
    /// <returns>New product</returns>
    public ProductModel CreateProduct(ProductModel product)
    {
        try
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while creating product in {nameof(ProductCrud)}.{nameof(CreateProduct)}");
            return new ProductModel();
        }
    }

    /// <summary>
    ///     Add comment to product.
    /// </summary>
    /// <param name="comment">Comment model.</param>
    /// <returns>New comment</returns>
    public CommentModel CreateComment(CommentModel comment)
    {
        try
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return comment;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while creating comment in {nameof(CommentCrud)}.{nameof(CreateComment)}");
            return null!;
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Search suggestion.
    /// </summary>
    /// <param name="query">String query.</param>
    /// <param name="limit">Count results.</param>
    /// <returns>List SuggestionProductDto</returns>
    public List<SuggestionProductDto> SearchSuggestions(string query, int limit)
    {
        try
        {
            var products = _context.Products
                .Where(p => p.Name.StartsWith(query) || p.Name.Contains(query))
                .OrderByDescending(p => p.Name.StartsWith(query))
                .ThenBy(p => p.Name)
                .Take(limit)
                .Select(p => new SuggestionProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug
                })
                .ToList();

            return !products.Any() ? Enumerable.Empty<SuggestionProductDto>().ToList() : products;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while searching product by name in {nameof(ProductCrud)}.{nameof(SearchSuggestions)}");
            return Enumerable.Empty<SuggestionProductDto>().ToList();
        }
    }

    /// <summary>
    ///     Search ProductDtoResult.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="query">String query.</param>
    /// <param name="page">Result for page.Default 1 page.</param>
    /// <returns>List SuggestionProductDto</returns>
    public SearchProductDtoResult SearchProducts(SortOptionsEnum sort, string query, int page = 1, int pageSize = 20,
        Guid userId = default)
    {
        try
        {
            var productsQuery = _context.Products
                .Where(p => p.Name.StartsWith(query) || p.Name.Contains(query));

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };

            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Include(p => p.Comments)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = p.Images.FirstOrDefault().Path ?? string.Empty,
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                })
                .ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductDtoResult
            {
                Products = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while searching products by name in {nameof(ProductCrud)}.{nameof(SearchProducts)}");
            return new SearchProductDtoResult();
        }
    }

    /// <summary>
    ///     Get all products.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="page">Page count.</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="userId">User Id.Default value</param>
    /// <returns>SearchProductResult object</returns>
    public SearchProductResult GetAllProducts(SortOptionsEnum sort, int page, int pageSize, Guid userId = default)
    {
        try
        {
            var queryable = _context.Products
                .OrderByDescending(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var products = queryable.ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductResult
            {
                Products = products,
                TotalCount = queryable.Count()
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all products in {nameof(ProductCrud)}.{nameof(GetAllProducts)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get products by User Id.
    /// </summary>
    /// <param name="id">User Id.</param>
    /// <returns>SearchProductResult object</returns>
    public SearchProductResult GetProductsByUserId(Guid id)
    {
        try
        {
            var products = _context.Products.Where(j => j.SellerId == id).ToList();
            return new SearchProductResult
            {
                Products = products,
                TotalCount = products.Count
            };
        }
        catch (SqlException ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all products by user ID: {id} in {nameof(ProductCrud)}.{nameof(GetProductsByUserId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get special offers products.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="page">Page count.Default 1.</param>
    /// <param name="limit">Product limit.Default 3.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>List of ProductDto</returns>
    public List<ProductDto> GetSpecial0ffers(SortOptionsEnum sort, int page = 1, int limit = 3, Guid userId = default)
    {
        try
        {
            var productsQuery =
                _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Comments)
                    .Where(p => p.IsNew == true);

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };
            var products = productsQuery
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                })
                .ToList();


            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return products.Any() ? products : Enumerable.Empty<ProductDto>().ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting special offers in {nameof(ProductCrud)}.{nameof(GetSpecial0ffers)}");
            return Enumerable.Empty<ProductDto>().ToList();
        }
    }

    /// <summary>
    ///     Get popular products.
    /// </summary>
    /// <param name="page">Page count.Default 1.</param>
    /// <param name="limit">Product limit.Default 3.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>List of ProductDto</returns>
    public List<ProductDto> GetPopularProducts(int page = 1, int limit = 3, Guid userId = default)
    {
        try
        {
            var products =
                _context.Products
                    .Include(p => p.Images)
                    .Include(p => p.Comments)
                    .Include(p => p.WishlistProducts)
                    .OrderBy(p => p.WishlistProducts.Count)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Slug = p.Slug,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Description = p.Description,
                        ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                        InStock = p.InStock,
                        IsNew = p.IsNew,
                        Rating = p.Rating,
                        CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                    })
                    .ToList();


            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });
            return products.Any() ? products : Enumerable.Empty<ProductDto>().ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting special offers in {nameof(ProductCrud)}.{nameof(GetSpecial0ffers)}");
            return Enumerable.Empty<ProductDto>().ToList();
        }
    }

    /// <summary>
    ///     Get recent products.
    /// </summary>
    /// <param name="page">Page count.Default 1.</param>
    /// <param name="limit">Product limit.Default 3.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>List of ProductDto</returns>
    public List<ProductDto> GetRecentBuys(int page = 1, int limit = 3, Guid userId = default)
    {
        try
        {
            var products = _context.Orders.Include(o => o.CartItems)
                .ThenInclude(o => o.Product)
                .OrderBy(o => o.OrderDate)
                .SelectMany(o => o.CartItems.Select(c => new ProductDto
                {
                    Id = c.Product.Id,
                    Slug = c.Product.Slug,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Product.Quantity,
                    Description = c.Product.Description,
                    ImagePath = _imageService.GetImageUrl(c.Product.Images.FirstOrDefault().Path),
                    InStock = c.Product.InStock,
                    IsNew = c.Product.IsNew,
                    Rating = c.Product.Rating,
                    CommentsCount = c.Product.Comments == null ? 0 : c.Product.Comments.Count
                }));

            return products.Any() ? products.ToList() : Enumerable.Empty<ProductDto>().ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting special offers in {nameof(ProductCrud)}.{nameof(GetSpecial0ffers)}");
            return Enumerable.Empty<ProductDto>().ToList();
        }
    }

    /// <summary>
    ///     Get product by slug.
    /// </summary>
    /// <param name="slug">Slug string.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>ProductModel object</returns>
    public ProductModel GetProductBySlug(string slug, Guid userId = default)
    {
        try
        {
            var product =
                _context.Products
                    .Include(p => p.Images)
                    .Select(p => new ProductModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Description = p.Description,
                        InStock = p.InStock,
                        IsNew = p.IsNew,
                        Rating = p.Rating,
                        Weight = p.Weight,
                        Height = p.Height,
                        Color = p.Color,
                        Manufacturer = p.Manufacturer,
                        SellerId = p.SellerId,
                        Images = p.Images.Select(i => new ImageModel
                        {
                            Id = i.Id,
                            Path = _imageService.GetImageUrl(i.Path)
                        }).ToList()
                    }).FirstOrDefault(p => p.Slug == slug);

            if (userId != default) return product != null! ? product : null!;
            product.InCart = IsProductInCart(userId, product.Id);
            product.InWishlist = IsProductInWishlist(userId, product.Id);
            return product != null! ? product : null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting product by slug: {slug} in {nameof(ProductCrud)}.{nameof(GetProductBySlug)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get product by user Id.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>ProductModel object</returns>
    public ProductModel GetProductById(Guid productId, Guid userId = default)
    {
        try
        {
            var product = _context.Products
                .Include(p => p.Images)
                .Select(p => new ProductModel
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    Weight = p.Weight,
                    Height = p.Height,
                    Color = p.Color,
                    Manufacturer = p.Manufacturer,
                    SellerId = p.SellerId,
                    Images = p.Images.Select(i => new ImageModel
                    {
                        Id = i.Id,
                        Path = _imageService.GetImageUrl(i.Path)
                    }).ToList()
                })
                .FirstOrDefault(p => p.Id == productId);

            if (product == null) return null!;

            if (userId != default)
            {
                product.InCart = IsProductInCart(userId, productId);
                product.InWishlist = IsProductInWishlist(userId, productId);
            }

            product.Comments = _context.Comments
                .Where(c => c.ProductId == productId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToList();

            return product;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting product by ID: {productId} in {nameof(ProductCrud)}.{nameof(GetProductById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get product cart by Id.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <returns>ProductCartDto object</returns>
    public ProductCartDto GetProductCartDtoById(Guid productId)
    {
        try
        {
            var product = _context.Products.Include(p => p.Images)
                .Where(p => p.Id == productId)
                .Select(p => new ProductCartDto
                {
                    Id = productId,
                    Name = p.Name,
                    Price = p.Price,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path) ?? string.Empty
                }).FirstOrDefault();

            return product ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting product by ID: {productId} in {nameof(ProductCrud)}.{nameof(GetProductById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get product by subcategory Id.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="subcategoryId">Subcategory Id.</param>
    /// <param name="page">Page count.Default 1</param>
    /// <param name="pageSize">Page size.Default 20</param>
    /// <returns>SearchProductDtoResult object</returns>
    public SearchProductDtoResult GetProductsBySubcategoryId(SortOptionsEnum sort, Guid subcategoryId, int page = 1,
        int pageSize = 20,
        Guid userId = default)
    {
        try
        {
            var productsQuery = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .Where(p => p.SubcategoryId == subcategoryId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                });

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };

            var totalCount = productsQuery.Count();

            var products = productsQuery.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductDtoResult
            {
                Products = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting products by subcategory ID: {subcategoryId} in {nameof(ProductCrud)}.{nameof(GetProductsBySubcategoryId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get product by subcategory name.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="subcategoryName">Subcategory name.</param>
    /// <param name="page">Page count.Default 1</param>
    /// <param name="pageSize">Page count.Default 20</param>
    /// <returns>SearchProductDtoResult object</returns>
    public SearchProductDtoResult GetProductsBySubcategoryName(SortOptionsEnum sort, string subcategoryName,
        int page = 1, int pageSize = 20,
        Guid userId = default)
    {
        try
        {
            var productsQuery = _context.Products
                .Include(p => p.Subcategory)
                .Where(p => p.Subcategory != null && p.Subcategory.Name == subcategoryName)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                });

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };

            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductDtoResult
            {
                Products = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting products by subcategory name: {subcategoryName} in {nameof(ProductCrud)}.{nameof(GetProductsBySubcategoryName)})");
            return null!;
        }
    }

    /// <summary>
    ///     Get product by category Id.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="categoryId">Category Id.</param>
    /// <param name="page">Page count.Default 1</param>
    /// <param name="pageSize">Page count.Default 20</param>
    /// <param name="userId">User Id.Default value</param>
    /// <returns>SearchProductDtoResult object</returns>
    public SearchProductDtoResult GetProductsByCategoryId(SortOptionsEnum sort, Guid categoryId, int page = 1,
        int pageSize = 20,
        Guid userId = default)
    {
        try
        {
            var productsQuery = _context.Products
                .Include(p => p.Subcategory)
                .Include(p => p.Comments)
                .Include(p => p.Images)
                .AsSplitQuery()
                .Where(p => p.Subcategory != null && p.Subcategory.CategoryId == categoryId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                });

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };

            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductDtoResult
            {
                Products = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting products by category id: {categoryId} in {nameof(ProductCrud)}.{nameof(GetProductsByCategoryId)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get product by category name.
    /// </summary>
    /// <param name="sort">Enum of sort option.</param>
    /// <param name="categoryName">Category name.</param>
    /// <param name="page">Page count.Default 1</param>
    /// <param name="pageSize">Page count.Default 20</param>
    /// <param name="userId">User Id.Default value</param>
    /// <returns>SearchProductDtoResult object</returns>
    public SearchProductDtoResult GetProductsByCategoryName(SortOptionsEnum sort, string categoryName, int page = 1,
        int pageSize = 20,
        Guid userId = default)
    {
        try
        {
            var productsQuery = _context.Products
                .Include(p => p.Subcategory)
                .Include(p => p.Comments)
                .Include(p => p.Images)
                .AsSplitQuery()
                .Where(p => p.Subcategory != null && p.Subcategory.Category != null &&
                            p.Subcategory.Category.Name == categoryName)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    ImagePath = _imageService.GetImageUrl(p.Images.FirstOrDefault().Path),
                    InStock = p.InStock,
                    IsNew = p.IsNew,
                    Rating = p.Rating,
                    CommentsCount = p.Comments == null ? 0 : p.Comments.Count
                });

            productsQuery = sort switch
            {
                SortOptionsEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                SortOptionsEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                SortOptionsEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                SortOptionsEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };

            var totalCount = productsQuery.Count();

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (userId != default)
                products.ForEach(product =>
                {
                    product.InCart = IsProductInCart(userId, product.Id);
                    product.InWishlist = IsProductInWishlist(userId, product.Id);
                });

            return new SearchProductDtoResult
            {
                Products = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting products by category name: {categoryName} in {nameof(ProductCrud)}.{nameof(GetProductsByCategoryName)}");
            return null!;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete image by imageId and productId.
    /// </summary>
    /// <param name="imageId">Image Id.</param>
    /// <param name="productId">Product Id.</param>
    /// <returns>RequstResult object</returns>
    public RequestResult DeleteImage(Guid imageId, Guid productId)
    {
        try
        {
            var product = _context.Products.Find(productId);

            if (product == null)
                return RequestResult.Error("Product not found", HttpStatusCode.NotFound);

            var image = _context.Images.Include(i => i.Product)
                .FirstOrDefault(i => i.Id == imageId && i.Product!.Id == productId);

            if (image == null)
                return RequestResult.Error("Image not found", HttpStatusCode.NotFound);

            _context.Images.Remove(image);

            _context.SaveChanges();

            return _context.Images.Any(i => i.Id == imageId)
                ? RequestResult.Error("Image not deleted", HttpStatusCode.InternalServerError)
                : RequestResult.Success("Image deleted");
        }
        catch (Exception ex)
        {
            var error = $"An error occurred while deleting image by ID: {imageId}";
            _loggingService.LogException(ex, $"{error} in {nameof(ProductCrud)}.{nameof(DeleteImage)}");
            return RequestResult.Error($"{error}", HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Delete product by Id.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <returns>RequstResult object</returns>
    public RequestResult DeleteProduct(Guid productId)
    {
        try
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return RequestResult.Error("Product not found", HttpStatusCode.NotFound);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return _context.Products.Any(u => u.Id == productId)
                ? RequestResult.Error("Product not deleted", HttpStatusCode.InternalServerError)
                : RequestResult.Success("Product deleted");
        }
        catch (Exception ex)
        {
            var error = $"An error occurred while deleting product by ID: {productId}";
            _loggingService.LogException(ex, $"{error} in {nameof(ProductCrud)}.{nameof(DeleteProduct)}");
            return RequestResult.Error($"{error}", HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region HELPERS

    /// <summary>
    ///     Check is product in cart.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <param name="productId">Product Id.</param>
    /// <returns>True if in cart. False if not in cart</returns>
    public bool IsProductInCart(Guid userId, Guid productId)
    {
        return _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .AsSplitQuery()
            .Any(c => c.UserId == userId && c.CartItems.Any(ci => ci.ProductId == productId));
    }

    /// <summary>
    ///     Check is product in wishlist.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <param name="productId">Product Id.</param>
    /// <returns>True if in wishlist. False if not in wishlist</returns>
    public bool IsProductInWishlist(Guid userId, Guid productId)
    {
        try
        {
            return _context.WishlistProducts
                .Include(c => c.Wishlist)
                .Any(c => c.Wishlist.UserId == userId && c.ProductId == productId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting wishlist in {nameof(WishlistCrud)}.{nameof(IsProductInWishlist)}");
            return false;
        }
    }

    /// <summary>
    ///     Check is product exists.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <returns>True if product exists. False if not exists</returns>
    public bool ProductExists(Guid productId)
    {
        try
        {
            var product = _context.Products.Find(productId);
            return product != null;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while checking if product exists by ID: {productId} in {nameof(ProductCrud)}.{nameof(ProductExists)}");
            return false;
        }
    }

    /// <summary>
    ///     Check quantity valid.
    /// </summary>
    /// <param name="productId">Product Id.</param>
    /// <param name="quantity">Quantity.</param>
    /// <returns>True if valid. False if not valid</returns>
    public bool IsQuantityValid(Guid productId, short quantity)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            return product != null && product.Quantity >= quantity;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while checking if quantity is valid by ID: {productId} in {nameof(ProductCrud)}.{nameof(IsQuantityValid)}");
            return false;
        }
    }

    #endregion
}
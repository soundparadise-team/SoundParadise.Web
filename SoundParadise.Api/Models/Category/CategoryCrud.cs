using Humanizer;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.Category;
using SoundParadise.Api.Dto.Image;
using SoundParadise.Api.Dto.Subcategory;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Models.Category;

/// <summary>
///     CategoryModel CRUD.
/// </summary>
public class CategoryCrud : ICategoryCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILoggingService<CategoryCrud> _loggingService;

    /// <summary>
    ///     CategoryCrud constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    /// <param name="imageService">Service for images.</param>
    public CategoryCrud(SoundParadiseDbContext context, ILoggingService<CategoryCrud> loggingService,
        ImageService imageService)
    {
        _context = context;
        _loggingService = loggingService;
        _imageService = imageService;
    }

    #region CREATE

    /// <summary>
    ///     Add category in data base.
    /// </summary>
    /// <param name="category">Category model.</param>
    /// <returns>True if create, false if not</returns>
    public bool CreateCategory(CategoryModel category)
    {
        try
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, $"An error occurred while creating category in {nameof(CategoryCrud)}.{nameof(CreateCategory)}");
            return false;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete category from data base.
    /// </summary>
    /// <param name="categoryId">Category Id.</param>
    /// <returns>True if delete, false if not</returns>
    public bool DeleteCategory(Guid categoryId)
    {
        try
        {
            var category = _context.Categories.Find(categoryId);

            if (category == null)
                return false;

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return !_context.Categories.Any(u => u.Id == categoryId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CategoryCrud.DeleteCategory");
            return false;
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all categories from data base.
    /// </summary>
    /// <returns>List CategoryDto</returns>
    public List<CategoryDto> GetAllCategories()
    {
        try
        {
            var categoriesDto = _context.Categories
                .Include(c => c.Image)
                .Include(c => c.Subcategories)
                .ThenInclude(c => c.Image)
                .AsSplitQuery()
                .Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Image = category.Image != null
                        ? new ImageDto
                        {
                            Id = category.Image.Id,
                            Path = _imageService.GetImageUrl(category.Image.Path)
                        }
                        : null,
                    Subcategories = category.Subcategories.Select(subcategory => new SubcategoryDto
                    {
                        Id = subcategory.Id,
                        Path = _imageService.GetImageUrl(subcategory.Image!.Path),
                        Name = subcategory.Name
                    }).ToList()
                });

            return !categoriesDto.Any() ? Enumerable.Empty<CategoryDto>().ToList() : categoriesDto.ToList();
        }

        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CategoryCrud.GetAllCategories");
            return Enumerable.Empty<CategoryDto>().ToList();
        }
    }

    /// <summary>
    ///     Get subcategory of category.
    /// </summary>
    /// <param name="categoryId">CategoryId</param>
    /// <returns>List SubcategoryDto</returns>
    public List<SubcategoryDto> GetSubcategoriesOfCategory(Guid categoryId)
    {
        try
        {
            var subcategoriesDto = _context.Subcategories
                .Include(c => c.Image)
                .Where(c => c.CategoryId == categoryId)
                .Select(subcategory => new SubcategoryDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Path = _imageService.GetImageUrl(subcategory.Image!.Path)
                }).ToList();

            return !subcategoriesDto.Any() ? Enumerable.Empty<SubcategoryDto>().ToList() : subcategoriesDto;
        }

        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CategoryCrud.GetSubcategoriesOfCategory");
            return Enumerable.Empty<SubcategoryDto>().ToList();
        }
    }

    /// <summary>
    ///     Get category by Id.
    /// </summary>
    /// <param name="categoryId">Category Id</param>
    /// <returns>Category model</returns>
    public CategoryModel? GetCategoryById(Guid categoryId)
    {
        try
        {
            var category = _context.Categories.FirstOrDefault(p => p.Id == categoryId);

            if (category != null) return category;
            _loggingService.LogError($"Category not found with ID: {categoryId}");
            return null;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CategoryCrud.GetCategoryById");
            return null;
        }
    }

    /// <summary>
    ///     Get category by name.
    /// </summary>
    /// <param name="name">Category name.</param>
    /// <returns>Category model</returns>
    public CategoryModel? GetCategoryByName(string name)
    {
        try
        {
            var category = _context.Categories.FirstOrDefault(p => p.Name.Equals(name));

            if (category != null) return category;
            _loggingService.LogError($"Category not found with Name: {name}");
            return null;
        }
        catch (Exception ex)
        {
            _loggingService.LogException
                (ex, "CategoryCrud.GetCategoryByName");
            return null;
        }
    }

    /// <summary>
    ///     Get category name by Id.
    /// </summary>
    /// <param name="categoryId">Category Id.</param>
    /// <returns>Name string</returns>
    public string GetCategoryNameById(Guid categoryId)
    {
        try
        {
            return _context.Categories.Find(categoryId)?.Name.Transform(To.LowerCase, To.TitleCase) ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex, $"{nameof(CategoryCrud)}.{nameof(GetCategoryNameById)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get subcategory name by Id.
    /// </summary>
    /// <param name="subcategoryId">Subcategory Id.</param>
    /// <returns>name string</returns>
    public string GetSubcategoryNameById(Guid subcategoryId)
    {
        try
        {
            return _context.Subcategories.Find(subcategoryId)?.Name.Transform(To.LowerCase, To.TitleCase) ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex, $"{nameof(CategoryCrud)}.{nameof(GetSubcategoryNameById)}");
            return null!;
        }
    }

    #endregion
}
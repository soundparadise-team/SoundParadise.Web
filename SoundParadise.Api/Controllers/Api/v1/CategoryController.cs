using System.Net;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.Category;
using SoundParadise.Api.Dto.Subcategory;
using SoundParadise.Api.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Category API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Category.Base)]
public class CategoryController : BaseApiController
{
    private readonly ICategoryCrud _categoryCrud;

    /// <summary>
    ///     Constructor for CategoryController v1.
    /// </summary>
    /// <param name="categoryCrud"></param>
    public CategoryController(ICategoryCrud categoryCrud)
    {
        _categoryCrud = categoryCrud;
    }

    #region GET

    /// <summary>
    ///     Gets the list of all categories.
    /// </summary>
    /// <returns>List of CategoryModel.</returns>
    [HttpGet(ApiRoutes.GetAll)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of all categories.", typeof(List<CategoryDto>))]
    public IActionResult GetAllCategories()
    {
        var categories = _categoryCrud.GetAllCategories();
        return !categories.Any()
            ? NotFound(new { error = "The categories do not exist" })
            : Ok(categories);
    }

    [HttpGet(ApiRoutes.Category.GetSubcategoriesOfCategory)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of all subcategories of the category.",
        typeof(List<SubcategoryDto>))]
    public IActionResult GetSubcategoriesOfCategory([FromQuery] Guid categoryId)
    {
        var subcategories = _categoryCrud.GetSubcategoriesOfCategory(categoryId);
        return !subcategories.Any()
            ? NotFound(new { error = "The subcategories do not exist" })
            : Ok(subcategories);
    }

    /// <summary>
    ///     Return name of the category by ID
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns>Name of the Ccategory</returns>
    [HttpGet(ApiRoutes.Category.GetCategoryName)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the name of the category", typeof(string))]
    public IActionResult GetCategoryName([FromQuery] Guid categoryId)
    {
        var categoryName = _categoryCrud.GetCategoryNameById(categoryId);
        return string.IsNullOrEmpty(categoryName)
            ? NotFound(new { error = "The category name " + categoryName + " does not exist" })
            : Ok(new { categoryName });
    }

    /// <summary>
    ///     Returns name of the subcategory by ID
    /// </summary>
    /// <param name="subcategoryId"></param>
    /// <returns>The name of the Subcategory</returns>
    [HttpGet(ApiRoutes.Category.GetSubcategoryName)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the name of the subcategory", typeof(string))]
    public IActionResult GetSubcategoryName([FromQuery] Guid subcategoryId)
    {
        var subcategoryName = _categoryCrud.GetSubcategoryNameById(subcategoryId);

        return string.IsNullOrEmpty(subcategoryName)
            ? NotFound(new { error = "The category name " + subcategoryName + " does not exist" })
            : Ok(new { subcategoryName });
    }

    #endregion
}
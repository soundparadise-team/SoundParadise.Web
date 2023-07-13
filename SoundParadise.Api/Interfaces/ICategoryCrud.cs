using SoundParadise.Api.Dto.Category;
using SoundParadise.Api.Dto.Subcategory;
using SoundParadise.Api.Models.Category;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     CategoryCrud implements that interface.
/// </summary>
public interface ICategoryCrud
{
    bool CreateCategory(CategoryModel category);
    bool DeleteCategory(Guid categoryId);
    List<CategoryDto> GetAllCategories();
    List<SubcategoryDto> GetSubcategoriesOfCategory(Guid categoryId);
    CategoryModel? GetCategoryById(Guid categoryId);
    CategoryModel? GetCategoryByName(string name);
    string GetCategoryNameById(Guid categoryId);
    string GetSubcategoryNameById(Guid subcategoryId);
}
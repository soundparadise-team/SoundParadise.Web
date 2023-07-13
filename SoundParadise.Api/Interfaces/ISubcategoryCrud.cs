using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Models.Category;
using SoundParadise.Api.Models.Subcategory;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     SubcategoryCrud implements that interface.
/// </summary>
public interface ISubcategoryCrud
{
    bool CreateSubcategory(SubcategoryModel category);
    RequestResult DeleteSubcategory(Guid subcategoryId);
    List<SubcategoryModel> GetAllSubcategories();
    SubcategoryModel GetSubcategoryById(Guid id);
    SubcategoryModel GetSubcategoryByName(string name);
    List<SubcategoryModel> GetSubcategoryByCategory(CategoryModel category);
}
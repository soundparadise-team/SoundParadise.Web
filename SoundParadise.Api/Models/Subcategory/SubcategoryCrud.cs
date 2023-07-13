using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Category;

namespace SoundParadise.Api.Models.Subcategory;

/// <summary>
///     Subcategory CRUD.
/// </summary>
public class SubcategoryCrud : ISubcategoryCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<SubcategoryCrud> _loggingService;

    /// <summary>
    ///     Subcategory constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    public SubcategoryCrud(SoundParadiseDbContext context, ILoggingService<SubcategoryCrud> loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region CREATE

    public bool CreateSubcategory(SubcategoryModel category)
    {
        try
        {
            _context.Subcategories.Add(category);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while creating subcategory in {nameof(SubcategoryCrud)}.{nameof(CreateSubcategory)}");
            return false;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete Subcategory.
    /// </summary>
    /// <param name="subcategoryId">Subcategory's Id.</param>
    /// <returns>RequestResult object</returns>
    public RequestResult DeleteSubcategory(Guid subcategoryId)
    {
        try
        {
            var category = _context.Subcategories.Find(subcategoryId);

            if (category == null)
                return RequestResult.Error("Subcategory not found", HttpStatusCode.NotFound);

            _context.Subcategories.Remove(category);
            _context.SaveChanges();

            return _context.Subcategories.Any(u => u.Id == subcategoryId)
                ? RequestResult.Error("Subcategory was not deleted", HttpStatusCode.InternalServerError)
                : RequestResult.Success("Subcategory deleted");
        }
        catch (Exception ex)
        {
            var error = $"An error occured while deleting subcategory with ID: {subcategoryId}";
            _loggingService.LogException(ex, $"{error} in {nameof(SubcategoryCrud)}.{nameof(DeleteSubcategory)}");
            return RequestResult.Error($"{error}", HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all subcategory from data base.
    /// </summary>
    /// <returns>List of SubcategoryModel object.</returns>
    public List<SubcategoryModel> GetAllSubcategories()
    {
        try
        {
            var categories = _context.Subcategories.ToList();

            return !categories.Any() ? Enumerable.Empty<SubcategoryModel>().ToList() : categories;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all subcategories in {nameof(SubcategoryCrud)}.{nameof(GetAllSubcategories)}");
            return Enumerable.Empty<SubcategoryModel>().ToList();
        }
    }

    /// <summary>
    ///     Get Subcategory by Id.
    /// </summary>
    /// <param name="id">Subcategory Id.</param>
    /// <returns>SubcategoryModel object.</returns>
    public SubcategoryModel GetSubcategoryById(Guid id)
    {
        try
        {
            var subCategory = _context.Subcategories.Find(id);

            if (subCategory != null) return subCategory;
            _loggingService.LogError(
                $"Category not found with ID: {id} in {nameof(SubcategoryCrud)}.{nameof(GetSubcategoryById)}");
            return new SubcategoryModel();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting subcategory with ID: {id} in {nameof(SubcategoryCrud)}.{nameof(GetSubcategoryById)}");
            return new SubcategoryModel();
        }
    }

    /// <summary>
    ///     Get Subcategory by name.
    /// </summary>
    /// <param name="name">Subcategory name.</param>
    /// <returns>SubcategoryModel object.</returns>
    public SubcategoryModel GetSubcategoryByName(string name)
    {
        try
        {
            var category = _context.Subcategories.FirstOrDefault(p => p.Name.Equals(name));
            return category ?? null!;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting subcategory with name: {name} in {nameof(SubcategoryCrud)}.{nameof(GetSubcategoryByName)}");
            return null!;
        }
    }

    /// <summary>
    ///     Get Subcategory by category.
    /// </summary>
    /// <param name="category">Category model.</param>
    /// <returns>SubcategoryModel object.</returns>
    public List<SubcategoryModel> GetSubcategoryByCategory(CategoryModel category)
    {
        try
        {
            var subcategories = _context.Subcategories
                .Include(p => p.Category)
                .Where(j => j.Category!.Id == category.Id)
                .ToList();

            return !subcategories.Any() ? Enumerable.Empty<SubcategoryModel>().ToList() : subcategories;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting subcategories for category with ID: {category.Id} in {nameof(SubcategoryCrud)}.{nameof(GetSubcategoryByCategory)}");
            return Enumerable.Empty<SubcategoryModel>().ToList();
        }
    }

    #endregion
}
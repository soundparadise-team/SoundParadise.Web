using SoundParadise.Api.Data;
using SoundParadise.Api.Interfaces;

namespace SoundParadise.Api.Models.ProductParameter;

/// <summary>
///     ProductParameter CRUD.
/// </summary>
public class ProductParameterCrud : IProductParameterCrud
{
    private readonly SoundParadiseDbContext _context;
    private readonly ILoggingService<ProductParameterCrud> _loggingService;

    /// <summary>
    ///     ProductParameter constructor.
    /// </summary>
    /// <param name="context">Db context.</param>
    /// <param name="loggingService">Service for logging errors.</param>
    public ProductParameterCrud(SoundParadiseDbContext context, ILoggingService<ProductParameterCrud> loggingService)
    {
        _context = context;
        _loggingService = loggingService;
    }

    #region CREATE

    /// <summary>
    ///     Add ProductParameter to data base.
    /// </summary>
    /// <param name="parameter">ProductParameter model.</param>
    /// <returns>True if Ok. False if error.</returns>
    public bool CreateParameter(ProductParameterModel parameter)
    {
        try
        {
            _context.Parameters.Add(parameter);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while creating parameter in {nameof(ProductParameterCrud)}.{nameof(CreateParameter)}");
            return false;
        }
    }

    #endregion

    #region DELETE

    /// <summary>
    ///     Delete ProductParameter model from data base.
    /// </summary>
    /// <param name="parameterId">ProductParameter Id.</param>
    /// <returns>True if Ok. False if error.</returns>
    public bool DeleteParameter(Guid parameterId)
    {
        try
        {
            var parameter = _context.Parameters.Find(parameterId);
            if (parameter == null)
                return false;

            _context.Parameters.Remove(parameter);

            _context.SaveChanges();
            return !_context.Parameters.Any(u => u.Id == parameterId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while deleting parameter in {nameof(ProductParameterCrud)}.{nameof(DeleteParameter)}");
            return false;
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all ProductParameter models from data base.
    /// </summary>
    /// <returns>List of ProductParameter model.</returns>
    public List<ProductParameterModel> GetAllParameters()
    {
        try
        {
            var parameters = _context.Parameters.ToList();
            return !parameters.Any() ? Enumerable.Empty<ProductParameterModel>().ToList() : parameters;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all parameters in {nameof(ProductParameterCrud)}.{nameof(GetAllParameters)}");
            return Enumerable.Empty<ProductParameterModel>().ToList();
        }
    }

    /// <summary>
    ///     Get ProductParameter by Id.
    /// </summary>
    /// <returns>List of ProductParameter model.</returns>
    public List<ProductParameterModel> GetParameterByProductId(Guid productId)
    {
        try
        {
            var parameters = _context.Parameters.Where(p => p.ProductId == productId).ToList();

            return !parameters.Any() ? Enumerable.Empty<ProductParameterModel>().ToList() : parameters;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting parameters by product id in {nameof(ProductParameterCrud)}.{nameof(GetParameterByProductId)}");
            return Enumerable.Empty<ProductParameterModel>().ToList();
        }
    }

    #endregion
}
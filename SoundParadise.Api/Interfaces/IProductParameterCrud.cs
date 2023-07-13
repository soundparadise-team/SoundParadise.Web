using SoundParadise.Api.Models.ProductParameter;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     ProductParameterCrud implements that interface.
/// </summary>
public interface IProductParameterCrud
{
    bool CreateParameter(ProductParameterModel parameter);
    bool DeleteParameter(Guid parameterId);
    List<ProductParameterModel> GetAllParameters();
    List<ProductParameterModel> GetParameterByProductId(Guid productId);
}
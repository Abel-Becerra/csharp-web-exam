using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class GetGroupedProductsUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(GetGroupedProductsUseCase));
    private readonly IProductService _productService;

    public GetGroupedProductsUseCase(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<IEnumerable<ProductGroupDto>> ExecuteAsync()
    {
        _log.Info("Executing GetGroupedProductsUseCase");
        
        try
        {
            var groupedProducts = await _productService.GetProductsGroupedByCategoryAsync();
            _log.Info($"Retrieved {groupedProducts.Count()} product groups");
            return groupedProducts;
        }
        catch (Exception ex)
        {
            _log.Error("Error in GetGroupedProductsUseCase", ex);
            throw;
        }
    }
}

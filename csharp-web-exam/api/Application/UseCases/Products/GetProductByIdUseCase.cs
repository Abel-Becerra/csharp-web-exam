using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class GetProductByIdUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(GetProductByIdUseCase));
    private readonly IProductService _productService;

    public GetProductByIdUseCase(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<ProductDto?> ExecuteAsync(int id)
    {
        _log.Info($"Executing GetProductByIdUseCase for ID: {id}");
        
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                _log.Warn($"Product with ID {id} not found");
            }
            else
            {
                _log.Info($"Retrieved product: {product.Name}");
            }
            
            return product;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in GetProductByIdUseCase for ID {id}", ex);
            throw;
        }
    }
}

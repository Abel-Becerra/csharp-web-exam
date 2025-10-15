using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class DeleteProductUseCase(IProductService productService)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(DeleteProductUseCase));
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    public async Task<bool> ExecuteAsync(int id)
    {
        _log.Info($"Executing DeleteProductUseCase for ID: {id}");
        
        try
        {
            bool result = await _productService.DeleteProductAsync(id);
            
            if (result)
            {
                _log.Info($"Product {id} deleted successfully");
            }
            else
            {
                _log.Warn($"Product {id} not found for deletion");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in DeleteProductUseCase for ID {id}", ex);
            throw;
        }
    }
}

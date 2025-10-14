using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class UpdateProductUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(UpdateProductUseCase));
    private readonly IProductService _productService;

    public UpdateProductUseCase(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<bool> ExecuteAsync(int id, UpdateProductDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        _log.Info($"Executing UpdateProductUseCase for ID: {id}");
        
        try
        {
            var result = await _productService.UpdateProductAsync(id, dto);
            
            if (result)
            {
                _log.Info($"Product {id} updated successfully");
            }
            else
            {
                _log.Warn($"Product {id} not found for update");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in UpdateProductUseCase for ID {id}", ex);
            throw;
        }
    }
}

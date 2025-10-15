using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class CreateProductUseCase(IProductService productService)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CreateProductUseCase));
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    public async Task<ProductDto> ExecuteAsync(CreateProductDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        _log.Info($"Executing CreateProductUseCase for product: {dto.Name}");
        
        try
        {
            ProductDto product = await _productService.CreateProductAsync(dto);
            _log.Info($"Product created successfully with ID: {product.Id}");
            return product;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in CreateProductUseCase for product: {dto.Name}", ex);
            throw;
        }
    }
}

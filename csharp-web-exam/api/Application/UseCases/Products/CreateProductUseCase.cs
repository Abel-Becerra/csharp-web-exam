using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class CreateProductUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CreateProductUseCase));
    private readonly IProductService _productService;

    public CreateProductUseCase(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<ProductDto> ExecuteAsync(CreateProductDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        _log.Info($"Executing CreateProductUseCase for product: {dto.Name}");
        
        try
        {
            var product = await _productService.CreateProductAsync(dto);
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

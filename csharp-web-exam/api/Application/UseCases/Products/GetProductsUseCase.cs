using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Products;

public class GetProductsUseCase(IProductService productService)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(GetProductsUseCase));
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    public async Task<PaginatedResultDto<ProductDto>> ExecuteAsync(int page, int pageSize, string? searchTerm, int? categoryId, string? sortBy, bool sortDescending = false)
    {
        _log.Info($"Executing GetProductsUseCase - Page: {page}, PageSize: {pageSize}, Search: {searchTerm}, CategoryId: {categoryId}, SortBy: {sortBy}, SortDesc: {sortDescending}");
        
        try
        {
            PaginatedResultDto<ProductDto> products = await _productService.GetProductsAsync(page, pageSize, searchTerm, categoryId, sortBy, sortDescending);
            _log.Info($"Retrieved {products.TotalCount} products (Page {page} of {products.TotalPages})");
            return products;
        }
        catch (Exception ex)
        {
            _log.Error("Error in GetProductsUseCase", ex);
            throw;
        }
    }
}

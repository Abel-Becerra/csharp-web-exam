using api.Application.DTOs;

namespace api.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResultDto<ProductDto>> GetProductsAsync(
        int page = 1, 
        int pageSize = 10, 
        string? searchTerm = null, 
        int? categoryId = null,
        string? sortBy = null,
        bool sortDescending = false);
    
    Task<ProductDto?> GetProductByIdAsync(int id);
    
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
    
    Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto);
    
    Task<bool> DeleteProductAsync(int id);
    
    Task<IEnumerable<ProductGroupDto>> GetProductsGroupedByCategoryAsync();
}

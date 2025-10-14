using api.Domain.Entities;

namespace api.Application.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        int? categoryId = null,
        string? sortBy = null,
        bool sortDescending = false);
    
    Task<Product?> GetByIdAsync(int id);
    
    Task<int> CreateAsync(Product product);
    
    Task<bool> UpdateAsync(Product product);
    
    Task<bool> DeleteAsync(int id);
    
    Task<bool> ExistsAsync(int id);
    
    Task<IEnumerable<(int CategoryId, string CategoryName, int Count, decimal TotalValue, decimal AvgPrice, decimal MinPrice, decimal MaxPrice)>> GetGroupedByCategoryAsync();
}

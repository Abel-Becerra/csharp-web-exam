using api.Domain.Entities;

namespace api.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    
    Task<Category?> GetByIdAsync(int id);
    
    Task<int> CreateAsync(Category category);
    
    Task<bool> UpdateAsync(Category category);
    
    Task<bool> DeleteAsync(int id);
    
    Task<bool> ExistsAsync(int id);
}

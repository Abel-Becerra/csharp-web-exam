using api.Application.DTOs;

namespace api.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
    
    Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
    
    Task<bool> DeleteCategoryAsync(int id);
}

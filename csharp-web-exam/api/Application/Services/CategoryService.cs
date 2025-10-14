using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain.Entities;
using log4net;

namespace api.Application.Services;

public class CategoryService : ICategoryService
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CategoryService));
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        _log.Info("Retrieving all categories");
        
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var result = categories.Select(MapToDto).ToList();
            
            _log.Info($"Retrieved {result.Count} categories successfully");
            return result;
        }
        catch (Exception ex)
        {
            _log.Error("Error retrieving categories", ex);
            throw;
        }
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        _log.Info($"Retrieving category with ID: {id}");
        
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                _log.Warn($"Category with ID {id} not found");
                return null;
            }
            
            _log.Info($"Category with ID {id} retrieved successfully");
            return MapToDto(category);
        }
        catch (Exception ex)
        {
            _log.Error($"Error retrieving category with ID {id}", ex);
            throw;
        }
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
    {
        _log.Info($"Creating new category: {createDto.Name}");
        
        try
        {
            var category = new Category
            {
                Name = createDto.Name,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _categoryRepository.CreateAsync(category);
            category.Id = id;
            
            _log.Info($"Category created successfully with ID: {id}");
            return MapToDto(category);
        }
        catch (Exception ex)
        {
            _log.Error($"Error creating category: {createDto.Name}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
    {
        _log.Info($"Updating category with ID: {id}");
        
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            
            if (existingCategory == null)
            {
                _log.Warn($"Category with ID {id} not found for update");
                return false;
            }

            existingCategory.Name = updateDto.Name;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            var result = await _categoryRepository.UpdateAsync(existingCategory);
            
            if (result)
            {
                _log.Info($"Category with ID {id} updated successfully");
            }
            else
            {
                _log.Warn($"Failed to update category with ID {id}");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error updating category with ID {id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        _log.Info($"Deleting category with ID: {id}");
        
        try
        {
            var exists = await _categoryRepository.ExistsAsync(id);
            
            if (!exists)
            {
                _log.Warn($"Category with ID {id} not found for deletion");
                return false;
            }

            var result = await _categoryRepository.DeleteAsync(id);
            
            if (result)
            {
                _log.Info($"Category with ID {id} deleted successfully");
            }
            else
            {
                _log.Warn($"Failed to delete category with ID {id}");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error deleting category with ID {id}", ex);
            throw;
        }
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}

using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Categories;

public class GetCategoryByIdUseCase(ICategoryService categoryService)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(GetCategoryByIdUseCase));
    private readonly ICategoryService _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

    public async Task<CategoryDto?> ExecuteAsync(int id)
    {
        _log.Info($"Executing GetCategoryByIdUseCase for ID: {id}");
        
        try
        {
            CategoryDto? category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                _log.Warn($"Category with ID {id} not found");
            }
            else
            {
                _log.Info($"Retrieved category: {category.Name}");
            }
            
            return category;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in GetCategoryByIdUseCase for ID {id}", ex);
            throw;
        }
    }
}

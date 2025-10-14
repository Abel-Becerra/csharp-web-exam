using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Categories;

public class GetAllCategoriesUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(GetAllCategoriesUseCase));
    private readonly ICategoryService _categoryService;

    public GetAllCategoriesUseCase(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public async Task<IEnumerable<CategoryDto>> ExecuteAsync()
    {
        _log.Info("Executing GetAllCategoriesUseCase");
        
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            _log.Info($"Retrieved {categories.Count()} categories");
            return categories;
        }
        catch (Exception ex)
        {
            _log.Error("Error in GetAllCategoriesUseCase", ex);
            throw;
        }
    }
}

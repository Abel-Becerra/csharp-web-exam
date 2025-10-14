using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Categories;

public class CreateCategoryUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CreateCategoryUseCase));
    private readonly ICategoryService _categoryService;

    public CreateCategoryUseCase(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public async Task<CategoryDto> ExecuteAsync(CreateCategoryDto dto)
    {
        _log.Info($"Executing CreateCategoryUseCase for category: {dto.Name}");
        
        try
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            _log.Info($"Category created successfully with ID: {category.Id}");
            return category;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in CreateCategoryUseCase for category: {dto.Name}", ex);
            throw;
        }
    }
}

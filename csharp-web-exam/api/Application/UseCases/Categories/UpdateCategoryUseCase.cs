using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Categories;

public class UpdateCategoryUseCase
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(UpdateCategoryUseCase));
    private readonly ICategoryService _categoryService;

    public UpdateCategoryUseCase(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public async Task<bool> ExecuteAsync(int id, UpdateCategoryDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        _log.Info($"Executing UpdateCategoryUseCase for ID: {id}");
        
        try
        {
            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            
            if (result)
            {
                _log.Info($"Category {id} updated successfully");
            }
            else
            {
                _log.Warn($"Category {id} not found for update");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in UpdateCategoryUseCase for ID {id}", ex);
            throw;
        }
    }
}

using api.Application.Interfaces;
using log4net;

namespace api.Application.UseCases.Categories;

public class DeleteCategoryUseCase(ICategoryService categoryService)
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(DeleteCategoryUseCase));
    private readonly ICategoryService _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

    public async Task<bool> ExecuteAsync(int id)
    {
        _log.Info($"Executing DeleteCategoryUseCase for ID: {id}");
        
        try
        {
            bool result = await _categoryService.DeleteCategoryAsync(id);
            
            if (result)
            {
                _log.Info($"Category {id} deleted successfully");
            }
            else
            {
                _log.Warn($"Category {id} not found for deletion");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error in DeleteCategoryUseCase for ID {id}", ex);
            throw;
        }
    }
}

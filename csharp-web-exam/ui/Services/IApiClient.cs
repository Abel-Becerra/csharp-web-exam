using System.Collections.Generic;
using System.Threading.Tasks;
using csharp_web_exam.Models;

namespace csharp_web_exam.Services
{
    public interface IApiClient
    {
        // Authentication
        Task<LoginResponse> LoginAsync(string username, string password);

        // Categories
        Task<List<CategoryViewModel>> GetCategoriesAsync();
        Task<CategoryViewModel> GetCategoryByIdAsync(int id);
        Task<CategoryViewModel> CreateCategoryAsync(CategoryViewModel category);
        Task<bool> UpdateCategoryAsync(int id, CategoryViewModel category);
        Task<bool> DeleteCategoryAsync(int id);

        // Products
        Task<ProductListResult> GetProductsAsync(int page = 1, int pageSize = 10, string search = null, int? categoryId = null, string sortBy = null, bool sortDesc = false);
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task<ProductViewModel> CreateProductAsync(ProductViewModel product);
        Task<bool> UpdateProductAsync(int id, ProductViewModel product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<ProductGroupViewModel>> GetProductsGroupedAsync();
    }
}

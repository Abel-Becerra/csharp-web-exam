using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using csharp_web_exam.Models;
using log4net;
using Newtonsoft.Json;

namespace csharp_web_exam.Services
{
    public class ApiClient
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ApiClient));
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:5001/api";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Categories

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            try
            {
                _log.Info("Fetching all categories from API");
                var response = await _httpClient.GetAsync("/api/categories");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);
                _log.Info($"Successfully fetched {categories?.Count ?? 0} categories");
                return categories ?? new List<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                _log.Error("Error fetching categories from API", ex);
                throw;
            }
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(int id)
        {
            try
            {
                _log.Info($"Fetching category with ID {id} from API");
                var response = await _httpClient.GetAsync($"/api/categories/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryViewModel>(content);
                _log.Info($"Successfully fetched category with ID {id}");
                return category;
            }
            catch (Exception ex)
            {
                _log.Error($"Error fetching category with ID {id} from API", ex);
                throw;
            }
        }

        public async Task<CategoryViewModel> CreateCategoryAsync(CategoryViewModel category)
        {
            try
            {
                _log.Info($"Creating category: {category.Name}");
                var json = JsonConvert.SerializeObject(new { Name = category.Name });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/categories", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdCategory = JsonConvert.DeserializeObject<CategoryViewModel>(responseContent);
                _log.Info($"Successfully created category with ID {createdCategory?.Id}");
                return createdCategory;
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating category: {category.Name}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryViewModel category)
        {
            try
            {
                _log.Info($"Updating category with ID {id}");
                var json = JsonConvert.SerializeObject(new { Name = category.Name });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/categories/{id}", content);
                response.EnsureSuccessStatusCode();
                _log.Info($"Successfully updated category with ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating category with ID {id}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                _log.Info($"Deleting category with ID {id}");
                var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
                response.EnsureSuccessStatusCode();
                _log.Info($"Successfully deleted category with ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting category with ID {id}", ex);
                throw;
            }
        }

        #endregion

        #region Products

        public async Task<ProductListResult> GetProductsAsync(int page = 1, int pageSize = 10, string search = null, int? categoryId = null, string sortBy = null, bool sortDesc = false)
        {
            try
            {
                _log.Info($"Fetching products - Page: {page}, PageSize: {pageSize}");
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(search))
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");

                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId.Value}");

                if (!string.IsNullOrWhiteSpace(sortBy))
                    queryParams.Add($"sortBy={sortBy}");

                if (sortDesc)
                    queryParams.Add("sortDesc=true");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"/api/products?{queryString}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProductListResult>(content);
                _log.Info($"Successfully fetched {result?.Items?.Count ?? 0} products");
                return result ?? new ProductListResult();
            }
            catch (Exception ex)
            {
                _log.Error("Error fetching products from API", ex);
                throw;
            }
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int id)
        {
            try
            {
                _log.Info($"Fetching product with ID {id} from API");
                var response = await _httpClient.GetAsync($"/api/products/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
                _log.Info($"Successfully fetched product with ID {id}");
                return product;
            }
            catch (Exception ex)
            {
                _log.Error($"Error fetching product with ID {id} from API", ex);
                throw;
            }
        }

        public async Task<ProductViewModel> CreateProductAsync(ProductViewModel product)
        {
            try
            {
                _log.Info($"Creating product: {product.Name}");
                var json = JsonConvert.SerializeObject(new
                {
                    Name = product.Name,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/products", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdProduct = JsonConvert.DeserializeObject<ProductViewModel>(responseContent);
                _log.Info($"Successfully created product with ID {createdProduct?.Id}");
                return createdProduct;
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating product: {product.Name}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, ProductViewModel product)
        {
            try
            {
                _log.Info($"Updating product with ID {id}");
                var json = JsonConvert.SerializeObject(new
                {
                    Name = product.Name,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/products/{id}", content);
                response.EnsureSuccessStatusCode();
                _log.Info($"Successfully updated product with ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating product with ID {id}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _log.Info($"Deleting product with ID {id}");
                var response = await _httpClient.DeleteAsync($"/api/products/{id}");
                response.EnsureSuccessStatusCode();
                _log.Info($"Successfully deleted product with ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting product with ID {id}", ex);
                throw;
            }
        }

        public async Task<List<ProductGroupViewModel>> GetProductsGroupedAsync()
        {
            try
            {
                _log.Info("Fetching products grouped by category from API");
                var response = await _httpClient.GetAsync("/api/products/grouped");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var groups = JsonConvert.DeserializeObject<List<ProductGroupViewModel>>(content);
                _log.Info($"Successfully fetched {groups?.Count ?? 0} product groups");
                return groups ?? new List<ProductGroupViewModel>();
            }
            catch (Exception ex)
            {
                _log.Error("Error fetching grouped products from API", ex);
                throw;
            }
        }

        #endregion
    }

    public class ProductListResult
    {
        public List<ProductViewModel> Items { get; set; } = new List<ProductViewModel>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

using csharp_web_exam.Configuration;
using csharp_web_exam.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace csharp_web_exam.Services
{
    public class ApiClient : IApiClient
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ApiClient));
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient()
        {
            _baseUrl = AppSettings.ApiBaseUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            _log.Info($"ApiClient initialized with base URL: {_baseUrl}");
            
            // Agregar token JWT si existe
            SetAuthorizationHeader();
        }

        private void SetAuthorizationHeader()
        {
            try
            {
                string token = HttpContext.Current?.Request.Cookies["AuthToken"]?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _log.Debug("Authorization header set with JWT token");
                }
            }
            catch (Exception ex)
            {
                _log.Warn("Could not set authorization header", ex);
            }
        }

        #region Authentication

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            try
            {
                _log.Info($"Attempting login for user: {username}");
                var loginRequest = new { Username = username, Password = password };
                string json = JsonConvert.SerializeObject(loginRequest);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PostAsync("/api/auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    _log.Info($"Login successful for user: {username}");
                    return loginResponse;
                }
                else
                {
                    _log.Warn($"Login failed for user: {username}. Status: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error during login for user: {username}", ex);
                throw;
            }
        }

        #endregion

        #region Categories

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            try
            {
                _log.Info("Fetching all categories from API");
                HttpResponseMessage response = await _httpClient.GetAsync("/api/categories");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                List<CategoryViewModel> categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);
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
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/categories/{id}");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                CategoryViewModel category = JsonConvert.DeserializeObject<CategoryViewModel>(content);
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
                string json = JsonConvert.SerializeObject(new { Name = category.Name });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/categories", content);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                CategoryViewModel createdCategory = JsonConvert.DeserializeObject<CategoryViewModel>(responseContent);
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
                string json = JsonConvert.SerializeObject(new { Name = category.Name });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync($"/api/categories/{id}", content);
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
                HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/categories/{id}");
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
                List<string> queryParams = new List<string>
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

                string queryString = string.Join("&", queryParams);
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/products?{queryString}");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(content);
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
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/{id}");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(content);
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
                string json = JsonConvert.SerializeObject(new
                {
                    product.Name,
                    product.Price,
                    product.CategoryId
                });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/products", content);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                ProductViewModel createdProduct = JsonConvert.DeserializeObject<ProductViewModel>(responseContent);
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
                string json = JsonConvert.SerializeObject(new
                {
                    product.Name,
                    product.Price,
                    product.CategoryId
                });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync($"/api/products/{id}", content);
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
                HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/products/{id}");
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
                HttpResponseMessage response = await _httpClient.GetAsync("/api/products/grouped");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                List<ProductGroupViewModel> groups = JsonConvert.DeserializeObject<List<ProductGroupViewModel>>(content);
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

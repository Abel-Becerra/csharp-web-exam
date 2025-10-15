using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using csharp_web_exam.Filters;
using csharp_web_exam.Models;
using csharp_web_exam.Services;
using log4net;

namespace csharp_web_exam.Controllers
{
    [AuthorizeUser]
    public class ProductsController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ProductsController));
        private readonly ApiClient _apiClient;

        public ProductsController()
        {
            _apiClient = new ApiClient();
        }

        // GET: Products
        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string search = null, int? categoryId = null, string sortBy = null, bool sortDesc = false)
        {
            try
            {
                _log.Info($"Loading products index - Page: {page}, Search: {search}, CategoryId: {categoryId}");

                var productsResult = await _apiClient.GetProductsAsync(page, pageSize, search, categoryId, sortBy, sortDesc);
                var categories = await _apiClient.GetCategoriesAsync();

                var viewModel = new ProductListViewModel
                {
                    Products = productsResult.Items,
                    TotalCount = productsResult.TotalCount,
                    Page = productsResult.Page,
                    PageSize = productsResult.PageSize,
                    TotalPages = productsResult.TotalPages,
                    SearchTerm = search,
                    CategoryId = categoryId,
                    SortBy = sortBy,
                    SortDescending = sortDesc,
                    Categories = categories
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _log.Error("Error loading products index", ex);
                TempData["Error"] = "Error loading products. Please try again.";
                return View(new ProductListViewModel());
            }
        }

        // GET: Products/Grouped
        public async Task<ActionResult> Grouped()
        {
            try
            {
                _log.Info("Loading grouped products report");
                var groups = await _apiClient.GetProductsGroupedAsync();
                return View(groups);
            }
            catch (Exception ex)
            {
                _log.Error("Error loading grouped products", ex);
                TempData["Error"] = "Error loading grouped products report. Please try again.";
                return View(new System.Collections.Generic.List<ProductGroupViewModel>());
            }
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                _log.Info($"Loading product details for ID: {id}");
                var product = await _apiClient.GetProductByIdAsync(id);
                return View(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading product details for ID: {id}", ex);
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            try
            {
                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _log.Error("Error loading create product form", ex);
                TempData["Error"] = "Error loading form. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductViewModel product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log.Info($"Creating new product: {product.Name}");
                    await _apiClient.CreateProductAsync(product);
                    TempData["Success"] = "Product created successfully.";
                    return RedirectToAction("Index");
                }

                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating product: {product.Name}", ex);
                TempData["Error"] = "Error creating product. Please try again.";
                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                _log.Info($"Loading edit form for product ID: {id}");
                var product = await _apiClient.GetProductByIdAsync(id);
                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading edit form for product ID: {id}", ex);
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductViewModel product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log.Info($"Updating product ID: {id}");
                    await _apiClient.UpdateProductAsync(id, product);
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction("Index");
                }

                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating product ID: {id}", ex);
                TempData["Error"] = "Error updating product. Please try again.";
                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _log.Info($"Loading delete confirmation for product ID: {id}");
                var product = await _apiClient.GetProductByIdAsync(id);
                return View(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading delete confirmation for product ID: {id}", ex);
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _log.Info($"Deleting product ID: {id}");
                await _apiClient.DeleteProductAsync(id);
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting product ID: {id}", ex);
                TempData["Error"] = "Error deleting product. Please try again.";
                return RedirectToAction("Index");
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using csharp_web_exam.Models;
using csharp_web_exam.Services;
using log4net;

namespace csharp_web_exam.Controllers
{
    public class CategoriesController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CategoriesController));
        private readonly ApiClient _apiClient;

        public CategoriesController()
        {
            _apiClient = new ApiClient();
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            try
            {
                _log.Info("Loading categories index");
                var categories = await _apiClient.GetCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _log.Error("Error loading categories index", ex);
                TempData["Error"] = "Error loading categories. Please try again.";
                return View(new System.Collections.Generic.List<CategoryViewModel>());
            }
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                _log.Info($"Loading category details for ID: {id}");
                var category = await _apiClient.GetCategoryByIdAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading category details for ID: {id}", ex);
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Index");
            }
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryViewModel category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log.Info($"Creating new category: {category.Name}");
                    await _apiClient.CreateCategoryAsync(category);
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating category: {category.Name}", ex);
                TempData["Error"] = "Error creating category. Please try again.";
                return View(category);
            }
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                _log.Info($"Loading edit form for category ID: {id}");
                var category = await _apiClient.GetCategoryByIdAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading edit form for category ID: {id}", ex);
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Index");
            }
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, CategoryViewModel category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log.Info($"Updating category ID: {id}");
                    await _apiClient.UpdateCategoryAsync(id, category);
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating category ID: {id}", ex);
                TempData["Error"] = "Error updating category. Please try again.";
                return View(category);
            }
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _log.Info($"Loading delete confirmation for category ID: {id}");
                var category = await _apiClient.GetCategoryByIdAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error loading delete confirmation for category ID: {id}", ex);
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Index");
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _log.Info($"Deleting category ID: {id}");
                await _apiClient.DeleteCategoryAsync(id);
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting category ID: {id}", ex);
                TempData["Error"] = "Error deleting category. Please try again.";
                return RedirectToAction("Index");
            }
        }
    }
}

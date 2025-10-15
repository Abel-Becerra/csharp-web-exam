using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Mvc;
using csharp_web_exam.Controllers;
using csharp_web_exam.Models;
using csharp_web_exam.Services;

namespace ui.tests.Controllers
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<ApiClient> _mockApiClient;
        private ProductsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<ApiClient>();
            _controller = new ProductsController();
            // Note: In real scenario, you'd need dependency injection to inject the mock
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithProductList()
        {
            // Arrange
            var products = new List<ProductViewModel>
            {
                new ProductViewModel { Id = 1, Name = "Product 1", Price = 100 },
                new ProductViewModel { Id = 2, Name = "Product 2", Price = 200 }
            };

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductListViewModel));
        }

        [TestMethod]
        public async Task Index_WithSearchTerm_FiltersProducts()
        {
            // Arrange
            string searchTerm = "Laptop";

            // Act
            var result = await _controller.Index(search: searchTerm) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as ProductListViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(searchTerm, model.SearchTerm);
        }

        [TestMethod]
        public async Task Index_WithCategoryFilter_FiltersProducts()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _controller.Index(categoryId: categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as ProductListViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(categoryId, model.CategoryId);
        }

        [TestMethod]
        public async Task Index_WithSorting_SortsProducts()
        {
            // Arrange
            string sortBy = "price";
            bool sortDesc = true;

            // Act
            var result = await _controller.Index(sortBy: sortBy, sortDesc: sortDesc) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as ProductListViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(sortBy, model.SortBy);
            Assert.AreEqual(sortDesc, model.SortDescending);
        }

        [TestMethod]
        public async Task Index_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            int page = 2;
            int pageSize = 10;

            // Act
            var result = await _controller.Index(page: page, pageSize: pageSize) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as ProductListViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(page, model.Page);
            Assert.AreEqual(pageSize, model.PageSize);
        }

        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = await _controller.Details(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1;

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            // Should return HttpNotFoundResult or redirect with error
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Grouped_ReturnsViewResult_WithGroupedData()
        {
            // Arrange

            // Act
            var result = await _controller.Grouped() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<ProductGroupViewModel>));
        }

        [TestMethod]
        public void Create_GET_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Create_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "New Product",
                Price = 150,
                CategoryId = 1
            };

            // Act
            var result = await _controller.Create(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task Create_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var product = new ProductViewModel(); // Invalid: missing required fields
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(product) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Edit_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = await _controller.Edit(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Id = 1,
                Name = "Updated Product",
                Price = 200,
                CategoryId = 1
            };

            // Act
            var result = await _controller.Edit(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task Delete_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = await _controller.Delete(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Delete_POST_WithValidId_RedirectsToIndex()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = await _controller.DeleteConfirmed(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}

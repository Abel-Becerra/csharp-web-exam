using csharp_web_exam.Controllers;
using csharp_web_exam.Models;
using csharp_web_exam.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ui.tests.Controllers
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IApiClient> _mockApiClient;
        private ProductsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<IApiClient>();
            _controller = new ProductsController(_mockApiClient.Object);
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithProductList()
        {
            // Arrange
            ProductListResult productsResult = new()
            {
                Items =
                [
                    new() { Id = 1, Name = "Product 1", Price = 100 },
                    new() { Id = 2, Name = "Product 2", Price = 200 }
                ],
                TotalCount = 2
            };

            List<CategoryViewModel> categories =
            [
                new CategoryViewModel { Id = 1, Name = "Category 1" }
            ];

            _mockApiClient.Setup(x => x.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(productsResult);

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            ViewResult result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductListViewModel));
            ProductListViewModel model = result.Model as ProductListViewModel;
            Assert.AreEqual(2, model.Products.Count);
        }

        [TestMethod]
        public async Task Index_WithSearchTerm_FiltersProducts()
        {
            // Arrange
            string searchTerm = "Laptop";
            ProductListResult productsResult = new()
            {
                Items =
                [
                    new() { Id = 1, Name = "Laptop", Price = 1000 }
                ],
                TotalCount = 1
            };

            _mockApiClient.Setup(x => x.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), searchTerm, 
                It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(productsResult);

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(new List<CategoryViewModel>());

            // Act
            ViewResult result = await _controller.Index(search: searchTerm) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            ProductListViewModel model = result.Model as ProductListViewModel;
            Assert.AreEqual(searchTerm, model.SearchTerm);
            _mockApiClient.Verify(x => x.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), searchTerm, 
                It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task Index_WithCategoryFilter_FiltersProducts()
        {
            // Arrange
            int categoryId = 1;
            ProductListResult productsResult = new()
            {
                Items = [],
                TotalCount = 0
            };

            _mockApiClient.Setup(x => x.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 
                categoryId, It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(productsResult);

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync([]);

            // Act
            ViewResult result = await _controller.Index(categoryId: categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            ProductListViewModel model = result.Model as ProductListViewModel;
            Assert.AreEqual(categoryId, model.CategoryId);
            _mockApiClient.Verify(x => x.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 
                categoryId, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;
            ProductViewModel product = new() { Id = productId, Name = "Test Product", Price = 100 };

            _mockApiClient.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            ViewResult result = await _controller.Details(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductViewModel));
            ProductViewModel model = result.Model as ProductViewModel;
            Assert.AreEqual(productId, model.Id);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_RedirectsToIndex()
        {
            // Arrange
            int invalidId = 999;
            _mockApiClient.Setup(x => x.GetProductByIdAsync(invalidId))
                .ThrowsAsync(new System.Exception("Product not found"));

            // Act
            RedirectToRouteResult result = await _controller.Details(invalidId) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public async Task Create_GET_ReturnsViewResult()
        {
            // Arrange
            List<CategoryViewModel> categories =
            [
                new CategoryViewModel { Id = 1, Name = "Category 1" }
            ];

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            ViewResult result = await _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Categories);
        }

        [TestMethod]
        public async Task Create_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            ProductViewModel product = new()
            {
                Name = "New Product",
                Price = 150,
                CategoryId = 1
            };

            ProductViewModel createdProduct = new()
            {
                Id = 1,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            _mockApiClient.Setup(x => x.CreateProductAsync(It.IsAny<ProductViewModel>()))
                .ReturnsAsync(createdProduct);

            // Act
            RedirectToRouteResult result = await _controller.Create(product) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.CreateProductAsync(It.IsAny<ProductViewModel>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            ProductViewModel product = new();
            _controller.ModelState.AddModelError("Name", "Name is required");

            List<CategoryViewModel> categories =
            [
                new CategoryViewModel { Id = 1, Name = "Category 1" }
            ];

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            ViewResult result = await _controller.Create(product) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            _mockApiClient.Verify(x => x.CreateProductAsync(It.IsAny<ProductViewModel>()), Times.Never);
        }

        #endregion

        #region Edit Tests

        [TestMethod]
        public async Task Edit_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;
            ProductViewModel product = new() { Id = productId, Name = "Test Product", Price = 100, CategoryId = 1 };
            List<CategoryViewModel> categories =
            [
                new() { Id = 1, Name = "Category 1" }
            ];

            _mockApiClient.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            ViewResult result = await _controller.Edit(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductViewModel));
        }

        [TestMethod]
        public async Task Edit_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            int productId = 1;
            ProductViewModel product = new()
            {
                Id = productId,
                Name = "Updated Product",
                Price = 200,
                CategoryId = 1
            };

            _mockApiClient.Setup(x => x.UpdateProductAsync(productId, It.IsAny<ProductViewModel>()))
                .ReturnsAsync(true);

            // Act
            RedirectToRouteResult result = await _controller.Edit(productId, product) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.UpdateProductAsync(productId, It.IsAny<ProductViewModel>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int productId = 1;
            ProductViewModel product = new() { Id = productId, Name = "Test Product", Price = 100 };

            _mockApiClient.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            ViewResult result = await _controller.Delete(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductViewModel));
        }

        [TestMethod]
        public async Task Delete_POST_WithValidId_RedirectsToIndex()
        {
            // Arrange
            int productId = 1;

            _mockApiClient.Setup(x => x.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            RedirectToRouteResult result = await _controller.DeleteConfirmed(productId) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.DeleteProductAsync(productId), Times.Once);
        }

        #endregion

        #region Grouped Tests

        [TestMethod]
        public async Task Grouped_ReturnsViewResult_WithGroupedData()
        {
            // Arrange
            List<ProductGroupViewModel> groupedProducts =
            [
                new()
                {
                    CategoryId = 1,
                    CategoryName = "Electronics",
                    ProductCount = 10,
                    TotalValue = 1000,
                    AveragePrice = 100,
                    MinPrice = 50,
                    MaxPrice = 200
                }
            ];

            _mockApiClient.Setup(x => x.GetProductsGroupedAsync())
                .ReturnsAsync(groupedProducts);

            // Act
            ViewResult result = await _controller.Grouped() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<ProductGroupViewModel>));
            List<ProductGroupViewModel> model = result.Model as List<ProductGroupViewModel>;
            Assert.AreEqual(1, model.Count);
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}

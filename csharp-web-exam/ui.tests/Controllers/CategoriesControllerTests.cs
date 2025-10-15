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
    public class CategoriesControllerTests
    {
        private Mock<IApiClient> _mockApiClient;
        private CategoriesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<IApiClient>();
            _controller = new CategoriesController(_mockApiClient.Object);
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithCategoryList()
        {
            // Arrange
            List<CategoryViewModel> categories =
            [
                new() { Id = 1, Name = "Category 1" },
                new() { Id = 2, Name = "Category 2" }
            ];

            _mockApiClient.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            ViewResult result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<CategoryViewModel>));
            List<CategoryViewModel> model = result.Model as List<CategoryViewModel>;
            Assert.AreEqual(2, model.Count);
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;
            CategoryViewModel category = new() { Id = categoryId, Name = "Test Category" };

            _mockApiClient.Setup(x => x.GetCategoryByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            ViewResult result = await _controller.Details(categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(CategoryViewModel));
            CategoryViewModel model = result.Model as CategoryViewModel;
            Assert.AreEqual(categoryId, model.Id);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_RedirectsToIndex()
        {
            // Arrange
            int invalidId = 999;
            _mockApiClient.Setup(x => x.GetCategoryByIdAsync(invalidId))
                .ThrowsAsync(new System.Exception("Category not found"));

            // Act
            RedirectToRouteResult result = await _controller.Details(invalidId) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        public void Create_GET_ReturnsViewResult()
        {
            // Act
            ViewResult result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Create_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            CategoryViewModel category = new()
            {
                Name = "New Category"
            };

            CategoryViewModel createdCategory = new()
            {
                Id = 1,
                Name = category.Name
            };

            _mockApiClient.Setup(x => x.CreateCategoryAsync(It.IsAny<CategoryViewModel>()))
                .ReturnsAsync(createdCategory);

            // Act
            RedirectToRouteResult result = await _controller.Create(category) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.CreateCategoryAsync(It.IsAny<CategoryViewModel>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            CategoryViewModel category = new();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            ViewResult result = await _controller.Create(category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            _mockApiClient.Verify(x => x.CreateCategoryAsync(It.IsAny<CategoryViewModel>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_POST_WithEmptyName_ReturnsViewWithError()
        {
            // Arrange
            CategoryViewModel category = new() { Name = "" };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            ViewResult result = await _controller.Create(category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        #endregion

        #region Edit Tests

        [TestMethod]
        public async Task Edit_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;
            CategoryViewModel category = new() { Id = categoryId, Name = "Test Category" };

            _mockApiClient.Setup(x => x.GetCategoryByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            ViewResult result = await _controller.Edit(categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(CategoryViewModel));
        }

        [TestMethod]
        public async Task Edit_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            int categoryId = 1;
            CategoryViewModel category = new()
            {
                Id = categoryId,
                Name = "Updated Category"
            };

            _mockApiClient.Setup(x => x.UpdateCategoryAsync(categoryId, It.IsAny<CategoryViewModel>()))
                .ReturnsAsync(true);

            // Act
            RedirectToRouteResult result = await _controller.Edit(categoryId, category) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.UpdateCategoryAsync(categoryId, It.IsAny<CategoryViewModel>()), Times.Once);
        }

        [TestMethod]
        public async Task Edit_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            int categoryId = 1;
            CategoryViewModel category = new() { Id = categoryId };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            ViewResult result = await _controller.Edit(categoryId, category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;
            CategoryViewModel category = new() { Id = categoryId, Name = "Test Category" };

            _mockApiClient.Setup(x => x.GetCategoryByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            ViewResult result = await _controller.Delete(categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(CategoryViewModel));
        }

        [TestMethod]
        public async Task Delete_POST_WithValidId_RedirectsToIndex()
        {
            // Arrange
            int categoryId = 1;

            _mockApiClient.Setup(x => x.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(true);

            // Act
            RedirectToRouteResult result = await _controller.DeleteConfirmed(categoryId) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            _mockApiClient.Verify(x => x.DeleteCategoryAsync(categoryId), Times.Once);
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}

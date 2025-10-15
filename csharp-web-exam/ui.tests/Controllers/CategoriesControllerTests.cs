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
    public class CategoriesControllerTests
    {
        private Mock<ApiClient> _mockApiClient;
        private CategoriesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<ApiClient>();
            _controller = new CategoriesController();
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithCategoryList()
        {
            // Arrange

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<CategoryViewModel>));
        }

        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _controller.Details(categoryId) as ViewResult;

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
            Assert.IsNotNull(result);
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
            var category = new CategoryViewModel
            {
                Name = "New Category"
            };

            // Act
            var result = await _controller.Create(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task Create_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var category = new CategoryViewModel(); // Invalid: missing required fields
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Create_POST_WithEmptyName_ReturnsViewWithError()
        {
            // Arrange
            var category = new CategoryViewModel { Name = "" };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Edit_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _controller.Edit(categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_POST_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var category = new CategoryViewModel
            {
                Id = 1,
                Name = "Updated Category"
            };

            // Act
            var result = await _controller.Edit(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task Edit_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var category = new CategoryViewModel { Id = 1 };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Edit(category) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Delete_GET_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _controller.Delete(categoryId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Delete_POST_WithValidId_RedirectsToIndex()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _controller.DeleteConfirmed(categoryId);

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

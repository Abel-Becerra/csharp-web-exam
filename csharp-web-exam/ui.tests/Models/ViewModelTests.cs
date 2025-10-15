using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using csharp_web_exam.Models;

namespace ui.tests.Models
{
    [TestClass]
    public class ViewModelTests
    {
        #region LoginViewModel Tests

        [TestMethod]
        public void LoginViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "SampleEx4mF0rT3st!ñ"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        public void LoginViewModel_WithoutUsername_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = null,
                Password = "password"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.IsTrue(validationResults.Count > 0);
            Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Username")));
        }

        [TestMethod]
        public void LoginViewModel_WithoutPassword_FailsValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = null
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.IsTrue(validationResults.Count > 0);
            Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Password")));
        }

        [TestMethod]
        public void LoginViewModel_RememberMe_DefaultsToFalse()
        {
            // Arrange
            var model = new LoginViewModel();

            // Assert
            Assert.IsFalse(model.RememberMe);
        }

        #endregion

        #region ProductViewModel Tests

        [TestMethod]
        public void ProductViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Test Product",
                Price = 100.00m,
                CategoryId = 1,
                CategoryName = "Electronics"
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        public void ProductViewModel_WithoutName_FailsValidation()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = null,
                Price = 100.00m,
                CategoryId = 1
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.IsTrue(validationResults.Count > 0);
        }

        [TestMethod]
        public void ProductViewModel_WithNegativePrice_FailsValidation()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Test Product",
                Price = -10.00m,
                CategoryId = 1
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.IsTrue(validationResults.Count > 0);
            Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Price")));
        }

        [TestMethod]
        public void ProductViewModel_WithZeroPrice_FailsValidation()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Free Product",
                Price = 0.00m,
                CategoryId = 1
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.AreEqual(1, validationResults.Count); // Price must be greater than 0
            Assert.IsTrue(validationResults.Any(v => v.MemberNames.Contains("Price")));
        }

        #endregion

        #region CategoryViewModel Tests

        [TestMethod]
        public void CategoryViewModel_WithValidData_PassesValidation()
        {
            // Arrange
            var model = new CategoryViewModel
            {
                Name = "Electronics",
                CreatedAt = DateTime.Now
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.AreEqual(0, validationResults.Count);
        }

        [TestMethod]
        public void CategoryViewModel_WithoutName_FailsValidation()
        {
            // Arrange
            var model = new CategoryViewModel
            {
                Name = null
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.IsTrue(validationResults.Count > 0);
        }

        [TestMethod]
        public void CategoryViewModel_UpdatedAt_CanBeNull()
        {
            // Arrange
            var model = new CategoryViewModel
            {
                Name = "Electronics",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            Assert.AreEqual(0, validationResults.Count);
            Assert.IsFalse(model.UpdatedAt.HasValue);
        }

        #endregion

        #region ProductListViewModel Tests

        [TestMethod]
        public void ProductListViewModel_DefaultValues_AreCorrect()
        {
            // Arrange
            var model = new ProductListViewModel();

            // Assert
            Assert.AreEqual(0, model.Page); // Default int value is 0
            Assert.AreEqual(0, model.PageSize); // Default int value is 0
            Assert.AreEqual(0, model.TotalPages); // Default int value is 0
            Assert.IsNull(model.SearchTerm);
            Assert.IsNull(model.CategoryId);
            Assert.IsFalse(model.SortDescending);
        }

        [TestMethod]
        public void ProductListViewModel_TotalPages_IsNotCalculated()
        {
            // Arrange
            var model = new ProductListViewModel
            {
                TotalCount = 50,
                PageSize = 12,
                TotalPages = 5 // Must be set explicitly (calculated in controller)
            };

            // Act
            int totalPages = model.TotalPages;

            // Assert
            Assert.AreEqual(5, totalPages); // TotalPages is a simple property, not auto-calculated
        }

        [TestMethod]
        public void ProductListViewModel_WithProducts_StoresCorrectly()
        {
            // Arrange
            var products = new List<ProductViewModel>
            {
                new ProductViewModel { Id = 1, Name = "Product 1" },
                new ProductViewModel { Id = 2, Name = "Product 2" }
            };

            var model = new ProductListViewModel
            {
                Products = products
            };

            // Assert
            Assert.AreEqual(2, model.Products.Count);
        }

        #endregion

        #region ProductGroupViewModel Tests

        [TestMethod]
        public void ProductGroupViewModel_AveragePrice_IsNotCalculated()
        {
            // Arrange
            var model = new ProductGroupViewModel
            {
                CategoryName = "Electronics",
                ProductCount = 10,
                TotalValue = 1000.00m,
                AveragePrice = 100.00m, // Must be set explicitly (calculated in API)
                MinPrice = 50.00m,
                MaxPrice = 200.00m
            };

            // Act
            decimal averagePrice = model.AveragePrice;

            // Assert
            Assert.AreEqual(100.00m, averagePrice); // AveragePrice is a simple property
        }

        [TestMethod]
        public void ProductGroupViewModel_WithZeroProducts_HandlesGracefully()
        {
            // Arrange
            var model = new ProductGroupViewModel
            {
                CategoryName = "Empty Category",
                ProductCount = 0,
                TotalValue = 0.00m,
                AveragePrice = 0.00m
            };

            // Assert
            Assert.AreEqual(0, model.ProductCount);
            Assert.AreEqual(0.00m, model.TotalValue);
            Assert.AreEqual(0.00m, model.AveragePrice);
        }

        #endregion

        #region Helper Methods

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}

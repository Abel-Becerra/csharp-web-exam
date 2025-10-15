using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.Services;
using api.Domain.Entities;
using Moq;
using Xunit;

namespace api.tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        List<Category> categories =
        [
            new() { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Books", CreatedAt = DateTime.UtcNow }
        ];
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        IEnumerable<CategoryDto> result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ExistingId_ReturnsCategory()
    {
        // Arrange
        Category category = new() { Id = 1, Name = "Electronics", CreatedAt = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        CategoryDto? result = await _service.GetCategoryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        // Act
        CategoryDto? result = await _service.GetCategoryByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidData_ReturnsCreatedCategory()
    {
        // Arrange
        CreateCategoryDto createDto = new() { Name = "New Category" };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(1);

        // Act
        CategoryDto? result = await _service.CreateCategoryAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Category", result.Name);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        Category existingCategory = new() { Id = 1, Name = "Old Name", CreatedAt = DateTime.UtcNow };
        UpdateCategoryDto updateDto = new() { Name = "Updated Name" };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCategory);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateCategoryAsync(1, updateDto);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Category>(c => c.Name == "Updated Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_NonExistingCategory_ReturnsFalse()
    {
        // Arrange
        UpdateCategoryDto updateDto = new() { Name = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        // Act
        bool result = await _service.UpdateCategoryAsync(999, updateDto);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        bool result = await _service.DeleteCategoryAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_NonExistingCategory_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        bool result = await _service.DeleteCategoryAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}

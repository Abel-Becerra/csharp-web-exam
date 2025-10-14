using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Categories;
using Moq;
using Xunit;

namespace api.tests.Endpoints;

public class CategoryEndpointsTests
{
    private readonly Mock<ICategoryService> _mockService;

    public CategoryEndpointsTests()
    {
        _mockService = new Mock<ICategoryService>();
    }

    [Fact]
    public async Task GetAllCategories_ReturnsCategories()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Electronics" },
            new CategoryDto { Id = 2, Name = "Books" }
        };
        _mockService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);
        var useCase = new GetAllCategoriesUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCategoryById_ExistingId_ReturnsCategory()
    {
        // Arrange
        var category = new CategoryDto { Id = 1, Name = "Electronics" };
        _mockService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);
        var useCase = new GetCategoryByIdUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetCategoryById_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetCategoryByIdAsync(999)).ReturnsAsync((CategoryDto?)null);
        var useCase = new GetCategoryByIdUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategory_ValidDto_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Sports" };
        var createdDto = new CategoryDto { Id = 1, Name = "Sports" };
        _mockService.Setup(s => s.CreateCategoryAsync(createDto)).ReturnsAsync(createdDto);
        var useCase = new CreateCategoryUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Sports", result.Name);
    }

    [Fact]
    public async Task UpdateCategory_ValidDto_ReturnsTrue()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Name = "Updated" };
        _mockService.Setup(s => s.UpdateCategoryAsync(1, updateDto)).ReturnsAsync(true);
        var useCase = new UpdateCategoryUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1, updateDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCategory_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Name = "Updated" };
        _mockService.Setup(s => s.UpdateCategoryAsync(999, updateDto)).ReturnsAsync(false);
        var useCase = new UpdateCategoryUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999, updateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategory_ExistingId_ReturnsTrue()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(true);
        var useCase = new DeleteCategoryUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCategory_NonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(999)).ReturnsAsync(false);
        var useCase = new DeleteCategoryUseCase(_mockService.Object);

        // Act
        var result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.False(result);
    }
}

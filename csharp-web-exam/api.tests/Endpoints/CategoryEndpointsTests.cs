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
        List<CategoryDto> categories =
        [
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Books" }
        ];
        _mockService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);
        GetAllCategoriesUseCase useCase = new(_mockService.Object);

        // Act
        IEnumerable<CategoryDto> result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCategoryById_ExistingId_ReturnsCategory()
    {
        // Arrange
        CategoryDto category = new() { Id = 1, Name = "Electronics" };
        _mockService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);
        GetCategoryByIdUseCase useCase = new(_mockService.Object);

        // Act
        CategoryDto? result = await useCase.ExecuteAsync(1);

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
        GetCategoryByIdUseCase useCase = new(_mockService.Object);

        // Act
        CategoryDto? result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategory_ValidDto_ReturnsCreated()
    {
        // Arrange
        CreateCategoryDto createDto = new() { Name = "Sports" };
        CategoryDto createdDto = new() { Id = 1, Name = "Sports" };
        _mockService.Setup(s => s.CreateCategoryAsync(createDto)).ReturnsAsync(createdDto);
        CreateCategoryUseCase useCase = new(_mockService.Object);

        // Act
        CategoryDto? result = await useCase.ExecuteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Sports", result.Name);
    }

    [Fact]
    public async Task UpdateCategory_ValidDto_ReturnsTrue()
    {
        // Arrange
        UpdateCategoryDto updateDto = new() { Name = "Updated" };
        _mockService.Setup(s => s.UpdateCategoryAsync(1, updateDto)).ReturnsAsync(true);
        UpdateCategoryUseCase useCase = new(_mockService.Object);

        // Act
        bool result = await useCase.ExecuteAsync(1, updateDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCategory_NonExistingId_ReturnsFalse()
    {
        // Arrange
        UpdateCategoryDto updateDto = new() { Name = "Updated" };
        _mockService.Setup(s => s.UpdateCategoryAsync(999, updateDto)).ReturnsAsync(false);
        UpdateCategoryUseCase useCase = new(_mockService.Object);

        // Act
        bool result = await useCase.ExecuteAsync(999, updateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategory_ExistingId_ReturnsTrue()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(true);
        DeleteCategoryUseCase useCase = new(_mockService.Object);

        // Act
        bool result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCategory_NonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(999)).ReturnsAsync(false);
        DeleteCategoryUseCase useCase = new(_mockService.Object);

        // Act
        bool result = await useCase.ExecuteAsync(999);

        // Assert
        Assert.False(result);
    }
}

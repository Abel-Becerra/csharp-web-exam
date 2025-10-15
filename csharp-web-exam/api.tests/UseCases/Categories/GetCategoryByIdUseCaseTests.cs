using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Categories;
using Moq;
using Xunit;

namespace api.tests.UseCases.Categories;

public class GetCategoryByIdUseCaseTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly GetCategoryByIdUseCase _useCase;

    public GetCategoryByIdUseCaseTests()
    {
        _mockService = new Mock<ICategoryService>();
        _useCase = new GetCategoryByIdUseCase(_mockService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsCategory()
    {
        // Arrange
        CategoryDto category = new() { Id = 1, Name = "Electronics" };
        _mockService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);

        // Act
        CategoryDto? result = await _useCase.ExecuteAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Electronics", result.Name);
        _mockService.Verify(s => s.GetCategoryByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetCategoryByIdAsync(999)).ReturnsAsync((CategoryDto?)null);

        // Act
        CategoryDto? result = await _useCase.ExecuteAsync(999);

        // Assert
        Assert.Null(result);
    }
}

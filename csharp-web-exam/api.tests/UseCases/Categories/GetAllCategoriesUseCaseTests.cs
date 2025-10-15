using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Categories;
using Moq;
using Xunit;

namespace api.tests.UseCases.Categories;

public class GetAllCategoriesUseCaseTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly GetAllCategoriesUseCase _useCase;

    public GetAllCategoriesUseCaseTests()
    {
        _mockService = new Mock<ICategoryService>();
        _useCase = new GetAllCategoriesUseCase(_mockService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsAllCategories()
    {
        // Arrange
        List<CategoryDto> categories =
        [
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Books" }
        ];
        _mockService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);

        // Act
        IEnumerable<CategoryDto> result = await _useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ServiceReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(new List<CategoryDto>());

        // Act
        IEnumerable<CategoryDto> result = await _useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
